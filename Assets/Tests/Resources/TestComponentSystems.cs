using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;


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
    [UpdateAfter(typeof(Test6_System2))]
    public class Test6_System1 : UpdateableSystem
    {

    }
    [DisableAutoCreation]
    [CreateInWorld("Test6 World")]
    public class Test6_System2 : UpdateableSystem
    {

    }
    [DisableAutoCreation]
    [CreateInWorld("Test6 World")]
    [UpdateBefore(typeof(Test6_System2))]
    public class Test6_System3 : UpdateableSystem
    {

    }
}