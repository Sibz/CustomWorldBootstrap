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

    public class SystemInGroupTests : MyTestFixture
    {
        private const string WORLDNAME = "Test4 World";

        [OneTimeSetUp]
        protected override void OneTimeSetup()
        {
            base.OneTimeSetup();
            m_DefaultSystems.Add(typeof(Test4_Group));
            m_DefaultSystems.Add(typeof(Test4_System));

            var newSystems = new Initialiser(m_FakeCWB).Initialise(m_DefaultSystems);
        }

        [TearDown]
        protected override void TearDown()
        {

        }

        [Test]
        public void Creates_Custom_World()
        {
            Assert.IsTrue(World.AllWorlds.Any(x => x.Name.Equals(WORLDNAME)));
        }

        [Test]
        public void Systems_Are_In_Custom_World()
        {
            Assert.IsTrue(World.AllWorlds.Where(x => x.Name.Equals(WORLDNAME)).First().Systems.Any(x => x.GetType() == typeof(Test4_Group)));
            Assert.IsTrue(World.AllWorlds.Where(x => x.Name.Equals(WORLDNAME)).First().Systems.Any(x => x.GetType() == typeof(Test4_System)));
        }

        [Test]
        public void Systems_Updates_In_UpdateGroup()
        {
            var world = World.AllWorlds.Where(x => x.Name.Equals(WORLDNAME)).First();

            world.GetExistingSystem<SimulationSystemGroup>().Update();
            Assert.IsTrue(world.GetExistingSystem<Test4_System>().Updated);
        }
    }
}