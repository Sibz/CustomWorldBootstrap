/*
 * v1.0.16
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;
using UnityEngine;
using CustomWorldBoostrapInternal;



public abstract class CustomWorldBootstrap : ICustomBootstrap, ICustomWorldBootstrap
{
    /// <summary>
    /// Per world options
    /// </summary>
    public List<WorldOption> WorldOptions { get; set; }
    /// <summary>
    /// Set false to disable the default world creation
    /// </summary>
    public bool CreateDefaultWorld = true;

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
                m_Initialiser = new Initialiser(this, CreateDefaultWorld, WorldOptions);
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
        DefaultWorld = World.Active;

        return Initialiser.Initialise(systems);
    }

}


namespace CustomWorldBoostrapInternal
{
    public class Initialiser
    {
        public List<WorldOption> WorldOptions { get; }
        public Dictionary<string, World> CustomWorlds { get; }
        public bool CreateDefaultWorld = true;
        private ICustomWorldBootstrap m_CustomWorldBootstrap;
        public Dictionary<string, List<Type>> WorldSystems = new Dictionary<string, List<Type>>();

        public Initialiser(ICustomWorldBootstrap customWorldBootstrap, bool createDefaultWorld = true, List<WorldOption> worldOptions = null)
        {
            WorldOptions = worldOptions ?? new List<WorldOption>();
            CustomWorlds = new Dictionary<string, World>();
            m_CustomWorldBootstrap = customWorldBootstrap;
            CreateDefaultWorld = createDefaultWorld;
        }

        public List<Type> Initialise(List<Type> systems)
        {
            // Add the default world, if it exists
            if (World.Active != null)
            {
                CustomWorlds.Add(World.Active.Name, World.Active);
            }

            var customWorldNames = systems
                .Where(x => x.CustomAttributes.Any(n => n.AttributeType.Name == nameof(CreateInWorldAttribute)))
                .Select(x => x.CustomAttributes
                    .Where(n => n.AttributeType.Name == nameof(CreateInWorldAttribute))
                    .FirstOrDefault().ConstructorArguments[0].ToString().Trim('"'))
                .Distinct();

            foreach (var worldname in customWorldNames)
            {
                if (!WorldOptions.Any(x => x.Name == worldname))
                {
                    WorldOptions.Add(new WorldOption(worldname));
                }
            }

            if (!WorldOptions.Any(x => x.Name == "Default World"))
            {
                WorldOptions.Add(WorldOption.DefaultWorld());
            }

            var defaultSystemTypes = systems.Where(x => !x.CustomAttributes.Any(n => n.AttributeType.Name == nameof(CreateInWorldAttribute)));

            List<Type> returnDefaultSystemTypes = null;

            foreach (var worldOptions in WorldOptions)
            {
                if (!CustomWorlds.ContainsKey(worldOptions.Name))
                {
                    CustomWorlds.Add(worldOptions.Name, new World(worldOptions.Name));
                    WorldSystems.Add(worldOptions.Name, new List<Type>());
                }
                if (worldOptions.Name == "Default World")
                {
                    if (CreateDefaultWorld)
                    {
                        returnDefaultSystemTypes = defaultSystemTypes.ToList();
                    }
                    continue;
                }

                var worldSystemTypes = GetSystemTypesIncludingUpdateInGroupAncestors(worldOptions.Name, systems);

                foreach (var worldSystemType in worldSystemTypes)
                {
                    CreateSystemInWorld(worldOptions.Name, worldSystemType);
                }

                foreach (var createdSystemType in WorldSystems[worldOptions.Name])
                {
                    if (createdSystemType.CustomAttributes.Any(x => x.AttributeType.Name == nameof(UpdateInGroupAttribute)))
                    {
                        var updateInGroupType = GetCustomAttributeFirstArg<Type>(createdSystemType, nameof(UpdateInGroupAttribute));
                        if (updateInGroupType == typeof(InitializationSystemGroup) ||
                            updateInGroupType == typeof(SimulationSystemGroup) ||
                            updateInGroupType == typeof(PresentationSystemGroup))
                        {
                            (CustomWorlds[worldOptions.Name].GetOrCreateSystem(updateInGroupType) as ComponentSystemGroup).AddSystemToUpdateList(CustomWorlds[worldOptions.Name].GetExistingSystem(createdSystemType));
                        }
                        else
                        {
                            if (!IsComponentSystemGroup(updateInGroupType))
                            {
                                throw new Exception(string.Format("System {0} is trying to update in a non ComponentSystemGroup class", createdSystemType.Name));
                            }
                            if (WorldSystems[worldOptions.Name].Contains(updateInGroupType))
                            {
                                (CustomWorlds[worldOptions.Name].GetOrCreateSystem(updateInGroupType) as ComponentSystemGroup).AddSystemToUpdateList(CustomWorlds[worldOptions.Name].GetOrCreateSystem(createdSystemType));
                            }
                            else
                            {
                                Debug.LogWarning(string.Format("Tried to create system {0} in {1} that doesn't exist. Updating in simulation group", createdSystemType.Name, updateInGroupType.Name));
                                CustomWorlds[worldOptions.Name].GetOrCreateSystem<SimulationSystemGroup>().AddSystemToUpdateList(CustomWorlds[worldOptions.Name].GetOrCreateSystem(createdSystemType));
                            }
                        }
                    }
                    else
                    {
                        CustomWorlds[worldOptions.Name].GetOrCreateSystem<SimulationSystemGroup>().AddSystemToUpdateList(CustomWorlds[worldOptions.Name].GetExistingSystem(createdSystemType));
                    }
                }
            }

            PerWorldPostInitialization();

            return m_CustomWorldBootstrap.PostInitialize(returnDefaultSystemTypes);
        }

        public void PerWorldPostInitialization()
        {
            foreach (World w in CustomWorlds.Values)
            {
                ScriptBehaviourUpdateOrder.UpdatePlayerLoop(w);
                if (WorldOptions.Where(x => x.Name == w.Name).FirstOrDefault().OnInitialize != null)
                {
                    WorldOptions.Where(x => x.Name == w.Name).FirstOrDefault().OnInitialize.Invoke(w);
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
        private void CreateSystemInWorld(string name, Type systemType)
        {
            CustomWorlds[name].CreateSystem(systemType);
            WorldSystems[name].Add(systemType);
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
        List<Type> PostInitialize(List<Type> systems);
    }

    public class WorldOption
    {
        public string Name;
        /// <summary>
        /// This is not yet implemented
        /// </summary>
        public List<Type> FilterTypes = new List<Type>();

        public Action<World> OnInitialize;

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
