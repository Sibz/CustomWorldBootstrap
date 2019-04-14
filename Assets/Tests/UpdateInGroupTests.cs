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

    public class UpdateInGroupTests : MyTestFixture
    {
        private const string WORLDNAME = "Test3 World";

        [OneTimeSetUp]
        protected override void OneTimeSetup()
        {
            base.OneTimeSetup();
            m_DefaultSystems.Add(typeof(Test3_InitializationSystem));
            m_DefaultSystems.Add(typeof(Test3_SimulationSystem));
            m_DefaultSystems.Add(typeof(Test3_PresentationSystem));
            var newSystems = new Initialiser(m_FakeCWB).Initialise(m_DefaultSystems);
        }

        [TearDown]
        protected override void TearDown()
        {

        }

        [Test]
        public void Creates_Custom_World()
        {
            Assert.IsTrue(WorldExists(WORLDNAME));
        }

        [Test]
        public void Systems_Are_In_Custom_World()
        {
            Assert.IsTrue(SystemExistsInWorld(WORLDNAME, typeof(Test3_InitializationSystem)));
            Assert.IsTrue(SystemExistsInWorld(WORLDNAME, typeof(Test3_SimulationSystem)));
            Assert.IsTrue(SystemExistsInWorld(WORLDNAME, typeof(Test3_PresentationSystem)));
        }

        [Test]
        public void Systems_Updates_In_UpdateGroup()
        {
            var world = GetWorld(WORLDNAME);
            world.GetExistingSystem<InitializationSystemGroup>().Update();
            Assert.IsTrue(world.GetExistingSystem<Test3_InitializationSystem>().Updated);
            Assert.IsFalse(world.GetExistingSystem<Test3_SimulationSystem>().Updated);
            Assert.IsFalse(world.GetExistingSystem<Test3_PresentationSystem>().Updated);
            world.GetExistingSystem<SimulationSystemGroup>().Update();
            Assert.IsTrue(world.GetExistingSystem<Test3_InitializationSystem>().Updated);
            Assert.IsTrue(world.GetExistingSystem<Test3_SimulationSystem>().Updated);
            Assert.IsFalse(world.GetExistingSystem<Test3_PresentationSystem>().Updated);
            world.GetExistingSystem<PresentationSystemGroup>().Update();
            Assert.IsTrue(world.GetExistingSystem<Test3_InitializationSystem>().Updated);
            Assert.IsTrue(world.GetExistingSystem<Test3_SimulationSystem>().Updated);
            Assert.IsTrue(world.GetExistingSystem<Test3_PresentationSystem>().Updated);
        }
    }
}