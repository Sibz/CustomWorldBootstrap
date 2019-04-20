using Unity.Entities;

namespace Tests
{
    /// <summary>
    /// All systems have [DisableAutoCreation] as we want to control
    /// what is in the list of system types for testing
    /// </summary>
    public abstract class UpdateableSystem : ComponentSystem
    {
        public bool Updated = false;
        protected override void OnUpdate()
        {
            Updated = true;
        }
    }

    [DisableAutoCreation]
    public class Test1 : UpdateableSystem
    {

    }

    [DisableAutoCreation]
    [CreateInWorld("Test2 World")]
    public class Test2 : UpdateableSystem
    {

    }

    [DisableAutoCreation]
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [CreateInWorld("Test3 World")]
    public class Test3_InitializationSystem : UpdateableSystem
    {

    }

    [DisableAutoCreation]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [CreateInWorld("Test3 World")]
    public class Test3_SimulationSystem : UpdateableSystem
    {

    }

    [DisableAutoCreation]
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    [CreateInWorld("Test3 World")]
    public class Test3_PresentationSystem : UpdateableSystem
    {

    }

    [DisableAutoCreation]
    [CreateInWorld("Test4 World")]
    public class Test4_Group : ComponentSystemGroup
    {

    }

    [DisableAutoCreation]
    [UpdateInGroup(typeof(Test4_Group))]
    [CreateInWorld("Test4 World")]
    public class Test4_System : UpdateableSystem
    {

    }

    [DisableAutoCreation]
    [CreateInWorld("Test5 World")]
    public class Test5_Group1 : ComponentSystemGroup
    {

    }

    [DisableAutoCreation]
    [UpdateInGroup(typeof(Test5_Group1))]
    [CreateInWorld("Test5 World")]
    public class Test5_Group2 : ComponentSystemGroup
    {

    }

    [DisableAutoCreation]
    [UpdateInGroup(typeof(Test5_Group2))]
    [CreateInWorld("Test5 World")]
    public class Test5_System : UpdateableSystem
    {

    }

    [DisableAutoCreation]
    [CreateInWorld("Test6 World")]
    public class Test6_Group: ComponentSystemGroup
    {
        public string UpdateOrder = "";
    }

    [DisableAutoCreation]
    [CreateInWorld("Test6 World")]
    [UpdateInGroup(typeof(Test6_Group))]
    [UpdateAfter(typeof(Test6_System2))]
    public class Test6_System1 : UpdateableSystem
    {
        protected override void OnUpdate()
        {
            base.OnUpdate();
            World.GetExistingSystem<Test6_Group>().UpdateOrder += "1";
        }
    }

    [DisableAutoCreation]
    [UpdateInGroup(typeof(Test6_Group))]
    [CreateInWorld("Test6 World")]
    public class Test6_System2 : UpdateableSystem
    {
        protected override void OnUpdate()
        {
            base.OnUpdate();
            World.GetExistingSystem<Test6_Group>().UpdateOrder += "2";
        }
    }

    [DisableAutoCreation]
    [UpdateInGroup(typeof(Test6_Group))]
    [CreateInWorld("Test6 World")]
    [UpdateBefore(typeof(Test6_System2))]
    public class Test6_System3 : UpdateableSystem
    {
        protected override void OnUpdate()
        {
            base.OnUpdate();
            World.GetExistingSystem<Test6_Group>().UpdateOrder += "3";
        }
    }

    [DisableAutoCreation]
    public class Test7_System : UpdateableSystem, ITest7
    {

    }
    public interface ITest7 { }
}