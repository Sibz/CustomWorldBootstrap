﻿/*
 * v1.2.2
 * */

using CustomWorldBoostrapInternal;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using UnityEngine;

public abstract class CustomWorldBootstrap : ICustomBootstrap, ICustomWorldBootstrap
{
    /// <summary>
    /// Per world options
    /// </summary>
    public List<WorldOption> WorldOptions { get; } = new List<WorldOption>();

    /// <summary>
    /// Set false to disable the default world creation
    /// If false then the Initialization/Simulation/Presentation SystemGroups
    /// will need updating manually, and there will be no default buffer systems - creating them will
    /// not mean they update in the expected order.
    /// </summary>
    public bool CreateDefaultWorld { get; set; } = true;

    /// <summary>
    /// Name of world to assign as default world
    /// Setting this will change the default world
    /// The new default world will not have any additional systems
    /// such as the ones for hybrid and sub scenes
    /// </summary>
    public string DefaultWorldName { get; set; } = "";

    /// <summary>
    /// Accessor for the default world, null if CreateDefaultWorld = false
    /// </summary>
    public static World DefaultWorld;

    /// <summary>
    /// Dictionary containing each world by world name
    /// </summary>
    public IReadOnlyDictionary<string, World> Worlds => Initialiser.CustomWorlds;

    private Initialiser m_Initialiser;
    private Initialiser Initialiser
    {
        get
        {
            if (m_Initialiser == null)
            {
                m_Initialiser = new Initialiser(this);
            }
            return m_Initialiser;
        }
    }

    /// <summary>
    /// Override this function to customise the list of systems to be creating in the default world
    /// </summary>
    /// <param name="systems">List of system that would be created in default world</param>
    /// <returns>Modified list of systems</returns>
    public virtual List<Type> PostInitialize(List<Type> systems)
    {
        return systems;
    }

    public List<Type> Initialize(List<Type> systems)
    {
        systems = Initialiser.Initialise(systems);

        DefaultWorld = World.Active;

        return systems;
    }

    public class WorldOption
    {
        /// <summary>
        /// Name of world
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Function to run when world initializes
        /// </summary>
        public Action<World> OnInitialize;

        /// <summary>
        /// Use this function to select custom systems to include in world
        /// The systems returned will be created in this world
        ///  and will not be created in default world
        /// </summary>
        public Func<List<Type>, List<Type>> CustomIncludeQuery;

        public WorldOption(string name)
        {
            Name = name;
        }

        public static WorldOption DefaultWorld()
        {
            return new WorldOption("Default World");
        }
    }
}

[Obsolete("Use CustomWorldBootstrap.WorldOption", false)]
public class WorldOption : CustomWorldBootstrap.WorldOption
{
    public WorldOption(string name) : base(name) { }
}

namespace CustomWorldBoostrapInternal
{
    public class Initialiser
    {
        internal Dictionary<string, World> CustomWorlds { get; }

        private ICustomWorldBootstrap m_CustomWorldBootstrap;
        private readonly bool m_CreateDefaultWorld = true;
        private Dictionary<string, WorldInfo> WorldData { get; }
        private string m_DefaultWorldName = "Default World";

        private class WorldInfo
        {
            public World World;
            public List<Type> WorldSystems = new List<Type>();
            public CustomWorldBootstrap.WorldOption Options;
        }

        public Initialiser(ICustomWorldBootstrap customWorldBootstrap)
        {
            WorldData = new Dictionary<string, WorldInfo>();

            foreach (var wo in customWorldBootstrap.WorldOptions)
            {
                WorldData.Add(wo.Name, new WorldInfo() { Options = wo });
            }

            CustomWorlds = new Dictionary<string, World>();
            m_CustomWorldBootstrap = customWorldBootstrap;
            m_CreateDefaultWorld = customWorldBootstrap.CreateDefaultWorld;
            m_DefaultWorldName = customWorldBootstrap.DefaultWorldName == "" ? m_DefaultWorldName : customWorldBootstrap.DefaultWorldName;
        }

        public List<Type> Initialise(List<Type> systems)
        {
            if (m_CreateDefaultWorld && m_DefaultWorldName != "Default World")
            {
                AddDefaultWorldToWorldData();
            }

            PopulateWorldOptions(systems);

            InitialiseEachWorld(systems);

            PerWorldPostInitialization();

            if (m_CreateDefaultWorld && m_DefaultWorldName != "Default World")
            {
                World.Active = CustomWorlds[m_DefaultWorldName];
            }

            UpdateSortOrder();

            return m_CustomWorldBootstrap.PostInitialize(!m_CreateDefaultWorld || (m_CreateDefaultWorld && m_DefaultWorldName != "Default World") ? null : GetDefaultSystemTypes(systems).ToList());
        }

        private void AddDefaultWorldToWorldData()
        {
            if (!WorldData.Keys.Contains(m_DefaultWorldName))
            {
                WorldData.Add(m_DefaultWorldName, new WorldInfo() { Options = CustomWorldBootstrap.WorldOption.DefaultWorld() });
            }
        }

        private void PopulateWorldOptions(List<Type> systems)
        {
            var customWorldNames = systems
                .Where(x =>
                x.CustomAttributes.Any(n => n.AttributeType.Name == nameof(CreateInWorldAttribute)) &&
                x.CustomAttributes.Where(n => n.AttributeType.Name == nameof(CreateInWorldAttribute)).First().ConstructorArguments.Count > 0)
                .Select(x => x.CustomAttributes
                    .Where(n => n.AttributeType.Name == nameof(CreateInWorldAttribute))
                    .FirstOrDefault().ConstructorArguments[0].ToString().Trim('"'))
                .Distinct();

            foreach (var worldname in customWorldNames)
            {
                if (!WorldData.Keys.Contains(worldname))
                {
                    WorldData.Add(worldname, new WorldInfo() { Options = new CustomWorldBootstrap.WorldOption(worldname) });
                }
            }

            if (m_CreateDefaultWorld && m_DefaultWorldName != "Default World" && !WorldData.Keys.Contains(m_DefaultWorldName))
            {
                throw new UnityException(
                    string.Format("Unable to change default world to world with name {0} as world is not defined in WorldOptions nor is it used in a CreateInWorld attribute", m_DefaultWorldName));
            }
        }

        private IEnumerable<Type> GetDefaultSystemTypes(List<Type> systems)
        {
            var excludeSystems = new List<Type>();
            foreach (var worldSystems in WorldData.Select(x => x.Value.WorldSystems))
            {
                excludeSystems.AddRange(worldSystems);
            }

            return systems.Where(type => !excludeSystems.Contains(type)); ;
        }

        private void InitialiseEachWorld(List<Type> systems)
        {

            foreach (var data in WorldData.Values)
            {
                /*
                 * Create the world
                 */
                if (data.Options.Name == "Default World")
                {
                    data.World = World.Active;
                    CustomWorlds.Add(data.Options.Name, data.World);
                    continue;
                }
                else
                {
                    data.World = new World(data.Options.Name);
                    CustomWorlds.Add(data.Options.Name, data.World);

                    if (m_CreateDefaultWorld && m_DefaultWorldName != "Default World" && data.Options.Name == m_DefaultWorldName)
                    {
                        ScriptBehaviourUpdateOrder.UpdatePlayerLoop(data.World);
                    }
                }

                /*
                 * Create systems in the world
                 */
                data.WorldSystems = GetSystemTypesIncludingUpdateInGroupAncestors(data.Options.Name, systems).ToList();
                if (data.Options.CustomIncludeQuery != null)
                {
                    data.WorldSystems.AddRange(data.Options.CustomIncludeQuery(systems));
                    data.WorldSystems = data.WorldSystems.Distinct().ToList();
                }
                foreach (var worldSystemType in data.WorldSystems)
                {
                    data.World.CreateSystem(worldSystemType);
                }

                /*
                 * Build the UpdateInGroup Hierarchy
                 */
                BuildHierarchy(data);
            }
        }

        /// <summary>
        /// Iterate the groups and if they have a UpdateInGroup attribute
        ///  add them to that groups updatelist
        ///  otherwise add them to default updatelist
        /// </summary>
        /// <param name="data"></param>
        private void BuildHierarchy(WorldInfo data)
        {
            foreach (var createdSystemType in data.WorldSystems)
            {
                if (createdSystemType.CustomAttributes.Any(x => x.AttributeType.Name == nameof(UpdateInGroupAttribute)))
                {
                    var updateInGroupType = GetCustomAttributeFirstArg<Type>(createdSystemType, nameof(UpdateInGroupAttribute));

                    if (!IsComponentSystemGroup(updateInGroupType))
                    {
                        throw new Exception(string.Format("System {0} is trying to update in a non ComponentSystemGroup class", createdSystemType.Name));
                    }
                    ComponentSystemGroup updateGroup;
                    if (updateInGroupType == typeof(InitializationSystemGroup)
                        || updateInGroupType == typeof(SimulationSystemGroup)
                        || updateInGroupType == typeof(PresentationSystemGroup))
                    {
                        updateGroup = (ComponentSystemGroup)World.Active.GetOrCreateSystem(updateInGroupType);
                    }
                    else if (data.WorldSystems.Contains(updateInGroupType))
                    {
                        updateGroup = (ComponentSystemGroup)data.World.GetOrCreateSystem(updateInGroupType);
                    }
                    else
                    {
                        Debug.LogWarning(string.Format("Tried to create system {0} in {1} that doesn't exist. Updating in simulation group", createdSystemType.Name, updateInGroupType.Name));
                        updateGroup = data.World.GetOrCreateSystem<SimulationSystemGroup>();
                    }
                    updateGroup.AddSystemToUpdateList(data.World.GetOrCreateSystem(createdSystemType));
                }
                else
                {
                    World.Active.GetOrCreateSystem<SimulationSystemGroup>().AddSystemToUpdateList(data.World.GetExistingSystem(createdSystemType));
                }
            }
        }

        private void UpdateSortOrder()
        {
            foreach (var data in WorldData.Values)
            {
                foreach (var type in data.WorldSystems)
                {
                    if (IsComponentSystemGroup(type))
                    {
                        ((ComponentSystemGroup)data.World.GetExistingSystem(type)).SortSystemUpdateList();
                    }
                }
            }
            if (m_CreateDefaultWorld)
            {
                foreach (var t in new Type[] { typeof(InitializationSystemGroup), typeof(SimulationSystemGroup), typeof(PresentationSystemGroup) })
                {
                    (World.Active.GetOrCreateSystem(t) as ComponentSystemGroup).SortSystemUpdateList();
                }
            }
        }

        private void PerWorldPostInitialization()
        {
            foreach (var w in WorldData.Values)
            {
                if (w.Options.OnInitialize != null)
                {
                    w.Options.OnInitialize.Invoke(w.World);
                }
            }
        }

        private IEnumerable<Type> GetSystemTypesIncludingUpdateInGroupAncestors(string worldName, List<Type> systems)
        {
            var worldSystemTypes = systems.Where(x => x.CustomAttributes.Any(n => n.AttributeType.Name == nameof(CreateInWorldAttribute) && n.ConstructorArguments[0].ToString().Trim('"') == worldName)).ToList();
            List<Type> results = new List<Type>();

            foreach (var type in worldSystemTypes)
            {
                GetAncestorTypes(results, type, worldName, systems);
            }

            worldSystemTypes.AddRange(results);

            return worldSystemTypes.Distinct().ToList(); ;
        }

        private void GetAncestorTypes(List<Type> listOfTypes, Type type, string worldName, List<Type> systems)
        {
            if (type.CustomAttributes.Any(x => x.AttributeType.Name == nameof(UpdateInGroupAttribute)))
            {
                var updateInGroupType = GetCustomAttributeFirstArg<Type>(type, nameof(UpdateInGroupAttribute));
                if (updateInGroupType == typeof(InitializationSystemGroup) ||
                    updateInGroupType == typeof(SimulationSystemGroup) ||
                    updateInGroupType == typeof(PresentationSystemGroup))
                {
                    return;
                }
                else
                {
                    if (!IsComponentSystemGroup(updateInGroupType))
                    {
                        throw new Exception(string.Format("System {0} is trying to update in a non ComponentSystemGroup class", type.Name));
                    }
                    else if (systems.Contains(updateInGroupType))
                    {
                        listOfTypes.Add(updateInGroupType);
                        GetAncestorTypes(listOfTypes, updateInGroupType, worldName, systems);
                    }
                }
            }
        }

        private T GetCustomAttributeFirstArg<T>(Type type, string name)
        {
            var firstAttributeByName = type.CustomAttributes
                .Where(x => x.AttributeType.Name == name).FirstOrDefault();
            if (firstAttributeByName == null)
            {
                throw new Exception(string.Format("Class {0} does not have attribute {1}", type.Name, name));
            }
            if (firstAttributeByName.ConstructorArguments.Count == 0)
            {
                throw new Exception(string.Format("Class {0} has Attribute {1} but no argument", type.Name, name));
            }
            return (T)firstAttributeByName
                .ConstructorArguments[0].Value;
        }

        private bool IsComponentSystemGroup(Type type)
        {
            Type baseType = type;
            while (baseType != null)
            {
                if (baseType.Name == nameof(ComponentSystemGroup))
                {
                    return true;
                }

                baseType = baseType.BaseType;
            }
            return false;
        }
    }

    public interface ICustomWorldBootstrap
    {
        bool CreateDefaultWorld { get; set; }
        string DefaultWorldName { get; set; }
        List<CustomWorldBootstrap.WorldOption> WorldOptions { get; }
        List<Type> PostInitialize(List<Type> systems);
    }

    [Obsolete("Use CustomWorldBootstrap.WorldOption", false)]
    public class WorldOption : CustomWorldBootstrap.WorldOption
    {
        public WorldOption(string name) : base(name) { }
    }
}
