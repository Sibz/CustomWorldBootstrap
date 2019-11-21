When I wrote this Custom Worlds were in a bit of a mess and changed during the development of this project.

Since then Knightmore has created a clean solution that I can recommend:

https://github.com/Knightmore/MultiWorldBootstrap

I'll leave this repo here for prosperity.


# CustomWorldBootstrap

#### [Change Log](ChangeLog.md)

## Files
Copy the following files into your unity project

* `Assets\CustomWorldBootstrap.cs`
* `Assets\CreateInWorldAttribute.cs`
* `Assets\CustomWorldBootstrapAssembly.asmdef` (optional)

## Bootstrap class
Instead of inheriting from `ICustomBootstrap` create a class that extends `CustomWorldBootStrap`

```csharp
public class MyBootstrap : CustomWorldBootstrap
{
}
```
Then you can create new worlds by decorating your class with the `CreateInWorld` attribute

```csharp
[CreateInWorld("My World Name")]
public class MyComponentSystem : ComponentSystem
```

### Accessing worlds
Worlds are exposed via `Worlds` property which is a dictionary using world name as key.

```csharp
Worlds["Custom World Name"]
```
Default world has it's own static property for easy access:

```csharp
MyBootstrap.DefaultWorld
```

### Post Initialize
To run your own code after all worlds are initialized, just override the `PostInitialize` function.

This takes and returns a list of systems that defines what will be in the default world.
*If the option `CreateDefaultWorld` is set to false or `DefaultWorldName` is not 'Default World', `systems` will be null.*

```csharp
public class MyBootStrap : CustomWorldBootstrap
{
    public override List<Type> PostInitialize(List<Type> systems) 
    {
        //Do stuff here after all custom worlds and their systems are created
        return systems;
    }
}
```

### Per world post initialize
Callback can be defined in options. see below

### Options
The follow properties can be set:

* **CreateDefaultWorld** - Default true, set false to disable default world creation. In doing so automatic updates will not work. If you need automatic updates, you need a default world that effectively shares the base system groups.
* **DefaultWorldName** - Default "", set to the name of another world to make it the new default world. The new default world will only have base system groups and buffer systems. The standard "Default World" will not be created. 
* **WorldOptionList** - List of world options. This can be left empty. Worlds will be created with default options upon detection of the use of CreateInWorld Attribute. If you need a world with no systems, it needs to be specified here.


```csharp
public class MyBootstrap : CustomWorldBootstrap
{
    public MyBootstrap()
    {
        CreateDefaultWorld = false; // Disable default world creation
        WorldOptions.Add(new CustomWorldBootstrap.WorldOption("My No System World"));
    }
}
```

#### World Options

* **OnInitialize** - default null, Action<World> callback for post world initialisation.
* **CustomIncludeQuery** - Set to a function that takes `List<Type>` and returns `List<Type>` in order to include systems based on a custom query. The systems returned will be included in the world and not the default world. 
```csharp
public class MyBootstrap : CustomWorldBootstrap
{
    public MyBootstrap()
    {
        var myWorldOption = new CustomWorldBootstrap.WorldOption("My world with initialise callback")
        {
            OnInitialize = OnMyWorldInitialise,
            CustomIncludeQuery = MyCustomQuery
        };
        WorldOptions.Add(myWorldOption);
    }

    public void OnMyWorldInitialise(World world)
    {
        // Do stuff once with this world when all systems have been created.
    }
    
    public List<Type> MyCustomQuery(List<Type> systems)
    {
        // Return only systems you want in the world.
        // Example below uses linq to filter systems 
        // that implement IMyInterface
        return systems
          .Where(type=>
            type.GetInterfaces()
              .Any(iface=>iface.Name==nameof(IMyInterface)));
    }
}

```


## Update In Group
`UpdateInGroup` attribute can be used with any group, providing it exists (and does not have `[DisableAutoCreation]`). 

Omitting `UpdateInGroup` causes system/group to be updated in the `SimulationSystemGroup`. 

*Note that if you omit CreateInWorld attribute on a ComponentSystemGroup, and a ComponentSystem uses that group in UpdateInGroup, then the group will be created in both the default world and any worlds that use that group with the UpdateInGroup attribute.*

## Update Before/After
This works as per default implementation.

## Example

System Group

```csharp
[CreateInWorld("My Custom World")]
public class MyCustomGroup : ComponentSystemGroup)
{

}
```

System in custom group
```csharp
[UpdateInGroup(typeof(MyCustomGroup))]
[CreateInWorld("My Custom World")]
public class TestSettingsSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        // do stuff
    }
}
```

System in custom group using UpdateBefore
```csharp
[UpdateInGroup(typeof(MyCustomGroup))]
[UpdateBefore(typeof(TestSettingsSystem))]
[CreateInWorld("My Custom World")]
public class TestSettingsSystem2 : ComponentSystem
{
    protected override void OnUpdate()
    {
        // do stuff
    }
}

```
