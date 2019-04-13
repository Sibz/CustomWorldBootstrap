using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{

    public class SingleSystemTests : MyTestFixture
    {
        private const string WORLDNAME = "Test2 World";

        [OneTimeSetUp]
        protected override void OneTimeSetup()
        {
            base.OneTimeSetup();
            m_DefaultSystems.Add(typeof(Test2));
            var newSystems = new Initialiser(m_FakeCWB).Initialise(m_DefaultSystems);
        }

        [TearDown]
        protected override void TearDown()
        {

        }

        [Test]
        public void Creates_Custom_World()
        {
            Assert.Greater(World.AllWorlds.Count, m_IntitialNumberOfWorlds);
            Assert.IsTrue(World.AllWorlds.Any(x => x.Name.Equals(WORLDNAME)));

        }

        [Test]
        public void System_Is_In_Custom_World()
        {
            Assert.IsTrue(World.AllWorlds.Where(x => x.Name.Equals(WORLDNAME)).First().Systems.Any(x => x.GetType() == typeof(Test2)));
        }

        [Test]
        public void System_Updates_In_Default_UpdateGroup()
        {
            var world = World.AllWorlds.Where(x => x.Name.Equals(WORLDNAME)).First();
            world.GetExistingSystem<SimulationSystemGroup>().Update();
            Assert.IsTrue(world.GetExistingSystem<Test2>().Updated);
        }
    }
}