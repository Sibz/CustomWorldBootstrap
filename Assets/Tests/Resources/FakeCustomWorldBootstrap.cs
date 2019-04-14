using System.Collections.Generic;
using System;

namespace Tests
{
    public class FakeCustomWorldBootstrap : ICustomWorldBootstrap
    {
        public List<Type> Alterations = new List<Type>();
        public bool PostInitializeRan = false;
        public List<Type> PostInitialize(List<Type> systems)
        {
            PostInitializeRan = true;
            systems.AddRange(Alterations);
            return systems;
        }
    }
}