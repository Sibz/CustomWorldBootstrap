using System;
using System.Collections.Generic;
using CustomWorldBoostrapInternal;

namespace Tests
{
    public class FakeCustomWorldBootstrap : ICustomWorldBootstrap
    {
        public List<Type> Alterations = new List<Type>();
        public bool PostInitializeRan = false;
        public List<Type> PostInitialize(List<Type> systems)
        {
            PostInitializeRan = true;
            if (systems != null)
            {
                systems.AddRange(Alterations);
            }

            return systems;
        }
    }
}