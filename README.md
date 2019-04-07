# CustomWorldBootstrap

## Default File
Instead of inheriting from `ICustomBootStrap` create a class that extends CustomWorldBootStrap.

```csharp
public class MyBootstrap : CustomWorldBootstrap
{
}
```

### Options
Currently the only option is to provide a list of worlds to create by default. To set the options create a parameterless contructor and call `SetOptions` with a list of `WorldOptions`.
As worlds are created automatically from the detection of `CreateOnlyInWorld` attribute, you only need to set options if you want a world with no systems.

```csharp
public class MyBootstrap : CustomWorldBootstrap
{
    public MyBootstrap()
    {
        SetOptions(new List<WorldOptions> { new WorldOptions("Custom World Name") });
    }
}
```

## System Groups

Add custom group (Optional, systems can also`UpdateInGroup(typeof(InitializationSystemGroup))`):

```csharp
[UpdateInGroup(typeof(InitializationSystemGroup))]
[CreateOnlyInWorld("SettingsWorld")]
public class MyCustomGroup : ComponentSystemGroup)
{

}
```

## Systems
Tag systems with `[CreateOnlyInWorld("Custom World Name")]` this will create the world if not specified in *WorldOptions*.
Systems can contain *UpdateInGroup*, *UpdateBefore* and *UpdateAfter*.
If *UpdateInGroup* is omitted then it is created in the default `SimulationSystemGroup`.

```csharp
[UpdateInGroup(typeof(MyCustomGroup))]
[CreateOnlyInWorld("SettingsWorld")]
public class TestSettingsSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        // do stuff
    }
}

[UpdateInGroup(typeof(MyCustomGroup))]
[UpdateBefore(typeof(TestSettingsSystem))]
[CreateOnlyInWorld("SettingsWorld")]
public class TestSettingsSystem2 : ComponentSystem
{
    protected override void OnUpdate()
    {
        // do stuff
    }
}

```






