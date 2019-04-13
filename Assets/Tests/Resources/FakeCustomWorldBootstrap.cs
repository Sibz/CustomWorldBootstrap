using System.Collections.Generic;
using System;

namespace Tests
{
    public class FakeCustomWorldBootstrap : ICustomWorldBootstrap
    {
        public List<Type> PostInitialize(List<Type> systems)
        {
            return systems;
        }
    }
}