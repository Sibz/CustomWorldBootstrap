using System;
using System.Collections.Generic;
using Unity.Entities;

namespace Tests
{
    public class MyBootStrap : ICustomBootstrap
    {
        
        public List<Type> Initialize(List<Type> systems)
        {
            MyTestFixture.DefaultSystems = systems;

            return systems;
        }
    }
}