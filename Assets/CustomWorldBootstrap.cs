﻿/*
 * v1.0.16
 * */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;
using UnityEngine;

public class Initialiser
{
    public List<WorldOption> WorldOptions { get; }
    public Dictionary<string, World> CustomWorlds { get; }
    public bool CreateDefaultWorld = true;
    private ICustomWorldBootstrap m_CustomWorldBootstrap;

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

        SystemInfo info = new SystemInfo(CustomWorlds, systems, WorldOptions, CreateDefaultWorld);

        PerWorldPostInitialization();

        return m_CustomWorldBootstrap.PostInitialize(info.DefaultWorldSystems);
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
}

public interface ICustomWorldBootstrap
{
    List<Type> PostInitialize(List<Type> systems);
}

public abstract class CustomWorldBootstrap : ICustomBootstrap, ICustomWorldBootstrap
{
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


    public List<WorldOption> WorldOptions { get; set; }

    //private Dictionary<string, World> m_CustomWorlds => m_Initialiser.CustomWorlds;
    public IReadOnlyDictionary<string, World> Worlds => Initialiser.CustomWorlds;
    public bool CreateDefaultWorld = true;

    public static World DefaultWorld;


    public static Exception InitializeException;


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

public class SystemInfo
{
    public List<Type> SystemTypes;
    public List<Type> DefaultWorldSystems = null;
    public Dictionary<string, List<Type>> WorldSystems = new Dictionary<string, List<Type>>();
    public List<WorldOption> CustomWorldOptions;
    public Dictionary<string, World> CustomWorlds = new Dictionary<string, World>();
    private bool m_CreateDefaultWorld = true;

    public SystemInfo(Dictionary<string, World> customWorlds, List<Type> systemTypes, List<WorldOption> customWorldOptions, bool createDefaultWorld)
    {
        SystemTypes = systemTypes;
        CustomWorlds = customWorlds;
        m_CreateDefaultWorld = createDefaultWorld;

        CustomWorldOptions = customWorldOptions ?? new List<WorldOption>();

        var customWorldNames = SystemTypes
            .Where(x => x.CustomAttributes.Any(n => n.AttributeType.Name == nameof(CreateInWorldAttribute)))
            .Select(x => x.CustomAttributes
                .Where(n => n.AttributeType.Name == nameof(CreateInWorldAttribute))
                .FirstOrDefault().ConstructorArguments[0].ToString().Trim('"'))
            .Distinct();

        foreach (var worldname in customWorldNames)
        {
            if (!CustomWorldOptions.Any(x => x.Name == worldname))
            {
                CustomWorldOptions.Add(new WorldOption(worldname));
            }
        }

        if (!CustomWorldOptions.Any(x => x.Name == "Default World"))
        {
            CustomWorldOptions.Add(WorldOption.DefaultWorld());
        }

        var defaultSystemTypes = SystemTypes.Where(x => !x.CustomAttributes.Any(n => n.AttributeType.Name == nameof(CreateInWorldAttribute)));

        foreach (var worldOptions in CustomWorldOptions)
        {
            if (!CustomWorlds.ContainsKey(worldOptions.Name))
            {
                CustomWorlds.Add(worldOptions.Name, new World(worldOptions.Name));
                WorldSystems.Add(worldOptions.Name, new List<Type>());
            }
            if (worldOptions.Name == "Default World")
            {
                if (m_CreateDefaultWorld)
                {
                    DefaultWorldSystems = defaultSystemTypes.ToList();
                }
                continue;
            }

            var worldSystemTypes = GetSystemTypesIncludingUpdateInGroupAncestors(worldOptions.Name);

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
    }

    private IEnumerable<Type> GetSystemTypesIncludingUpdateInGroupAncestors(string worldName)
    {
        var worldSystemTypes = SystemTypes.Where(x => x.CustomAttributes.Any(n => n.AttributeType.Name == nameof(CreateInWorldAttribute) && n.ConstructorArguments[0].ToString().Trim('"') == worldName)).ToList();
        List<Type> results = new List<Type>();
        foreach (var type in worldSystemTypes)
        {
            GetAncestorTypes(results, type, worldName);
        }
        worldSystemTypes.AddRange(results);
        return worldSystemTypes.Distinct().ToList(); ;
    }

    private void GetAncestorTypes(List<Type> listOfTypes, Type type, string worldName)
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
                else if (SystemTypes.Contains(updateInGroupType))
                {
                    listOfTypes.Add(updateInGroupType);
                    GetAncestorTypes(listOfTypes, updateInGroupType, worldName);
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