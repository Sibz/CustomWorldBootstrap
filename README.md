.

<a href="http://tcci.3utilities.com/viewType.html?buildTypeId=myID&guest=1"> 
<img src="http://tcci.3utilities.com/app/rest/builds/buildType(id:myID)/statusIcon"/>
</a>

# CustomWorldBootstrap

## Default File
Instead of inheriting from `ICustomBootStrap` create a class that extends CustomWorldBootStrap.

```csharp
public class MyBootstrap : CustomWorldBootstrap
{
}
```
Once you have that set up you can create new worlds just by adding the `CreateInWorld` attribute

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

This takes and returns a list of systems that defines what will be in the default world. If `CreateDefaultWorld` is false it will be null.

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

* CreateDefaultWorld - Default true, set false to disable default world creation.
* WorldOptionList - List of world options. This can be left empty. Worlds will be created with default options upon detection of the use of CreateInWorld Attribute. If you need a world with no systems, it needs to be specified here.


```csharp
public class MyBootstrap : CustomWorldBootstrap
{
    public MyBootstrap()
    {
        CreateDefaultWorld = false; // Disable default world creation
        WorldOptions.Add(new WorldOption("My No System World"));
    }
}
```

#### World Options
* OnInitialize - default null, Action<World> callback for post world initialisation.

```csharp
public class MyBootstrap : CustomWorldBootstrap
{
    public MyBootstrap()
    {
        WorldOptions.Add(new WorldOption("My world with initialise callback") { OnInitialize = OnMyWorldInitialise});
    }

    public void OnMyWorldInitialise(World world)
    {
        // Do stuff once wiht this world when all systems have been created.
    } 
}

```


## Update In Group
`UpdateInGroup` attribute can be used with any group, providing it exists (and does not have `[DisableAutoCreation]`). 

Omitting `UpdateInGroup` causes system/group to be updated in the `SimulationSystemGroup`. 

*Note that is you omit CreateInWorld attribute on a ComponentSystemGroup, and a ComponentSystem uses that group in UpdateInGroup, then the group will be created in both the default world and any worlds that use that group with the UpdateInGroup attribute.*

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






































