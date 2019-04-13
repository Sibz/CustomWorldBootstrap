using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Unity.Entities;
using System;

namespace Tests
{
    public class Tests
    {
        public static List<Type> DefaultSystems;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            DefaultWorldInitialization.Initialize("Default World", false);
            World.DisposeAllWorlds();
        }

        [SetUp]
        public void Setup()
        {
            World.DisposeAllWorlds();
        }

        [TearDown]
        public void TearDown()
        {
            World.DisposeAllWorlds();
        }

        [Test]
        public void Default_Produces_Unchanged_System_List()
        {

        }

        public class MyBootStrap : CustomWorldBootstrap
        {
            public override List<Type> PostInitialize(List<Type> systems)
            {
                DefaultSystems = systems;

                return base.PostInitialize(systems);
            }
        }
    }
}
