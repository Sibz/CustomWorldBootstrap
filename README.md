# CustomWorldBootstrap

Put in your own bootstrap file:
```csharp
public class MyBootstrap : CustomWorldBootstrap
{
    public MyBootstrap()
    {
        // At the moment, only need to do this if you want an empty world with no systems
        SetOptions(new List<WorldOptions> { new WorldOptions("Custom World Name") });
    }
}
```

Add custom group (Optional, systems can also `UpdateInGroup(typeof(InitializationSystemGroup))`):
```csharp
[UpdateInGroup(typeof(InitializationSystemGroup))]
[CreateOnlyInWorld("SettingsWorld")]
public class MyCustomGroup : ComponentSystemGroup)
{

}
```

Create systems:
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
