# CustomWorldBootstrap

```csharp
[UpdateInGroup(typeof(InitializationSystemGroup))]
public class MyCustomGroup : ComponentSystemGroup
{

}


[UpdateInGroup(typeof(SimulationSystemGroup))]
public class MyCustomGroup2 : ComponentSystemGroup
{

}



[UpdateInGroup(typeof(PresentationSystemGroup))]
public class MyCustomGroup3 : ComponentSystemGroup
{

}

[UpdateInGroup(typeof(MyCustomGroup3))]
public class MyCustomGroup4 : ComponentSystemGroup
{

}



[UpdateInGroup(typeof(MyCustomGroup4))]
[CreateOnlyInWorld("SettingsWorld")]
public class TestSettingsSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        // Debug.Log("Running");
    }
}

[UpdateInGroup(typeof(MyCustomGroup2))]
[CreateOnlyInWorld("SettingsWorld")]
public class TestSettingsSystem2 : ComponentSystem
{
    protected override void OnUpdate()
    {
        // Debug.Log("Running");
    }
}
[UpdateInGroup(typeof(MyCustomGroup))]
[CreateOnlyInWorld("SettingsWorld")]
public class TestSettingsSystem3 : ComponentSystem
{
    protected override void OnUpdate()
    {
        // Debug.Log("Running");
    }
}

[UpdateInGroup(typeof(MyCustomGroup))]
[UpdateAfter(typeof(TestSettingsSystem3))]
[CreateOnlyInWorld("SettingsWorld")]
public class TestSettingsSystem4 : ComponentSystem
{
    protected override void OnUpdate()
    {
        // Debug.Log("Running");
    }
}

[UpdateInGroup(typeof(MyCustomGroup))]
[UpdateBefore(typeof(TestSettingsSystem3))]
[CreateOnlyInWorld("SettingsWorld")]
public class TestSettingsSystem5 : ComponentSystem
{
    protected override void OnUpdate()
    {
        // Debug.Log("Running");
    }
}
```
