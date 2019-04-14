using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using UnityEngine;
using UnityEngine.TestTools;
using CustomWorldBoostrapInternal;


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

        [Test]
        public void Creates_Custom_World()
        {
            Assert.IsTrue(WorldExists(WORLDNAME));
        }

        [Test]
        public void TestInitializationSystem_Is_In_Custom_World()
        {
            Assert.IsTrue(SystemExistsInWorld(WORLDNAME, typeof(Test3_InitializationSystem)));
        }

        [Test]
        public void TestSimulationSystem_Is_In_Custom_World()
        {
            Assert.IsTrue(SystemExistsInWorld(WORLDNAME, typeof(Test3_SimulationSystem)));
        }

        [Test]
        public void TestPresentationSystem_Is_In_Custom_World()
        {
            Assert.IsTrue(SystemExistsInWorld(WORLDNAME, typeof(Test3_PresentationSystem)));
        }

        [Test]
        public void TestInitializationSystem_Updates_In_InitializationSystemGroup()
        {
            var world = GetWorld(WORLDNAME);
            // Ensure updated values are false;
            var initSys = world.GetExistingSystem<Test3_InitializationSystem>();
            var simSys = world.GetExistingSystem<Test3_SimulationSystem>();
            var presSys = world.GetExistingSystem<Test3_PresentationSystem>();
            initSys.Updated = false;
            simSys.Updated = false;
            presSys.Updated = false;

            world.GetExistingSystem<InitializationSystemGroup>().Update();

            Assert.IsTrue(world.GetExistingSystem<Test3_InitializationSystem>().Updated);
            Assert.IsFalse(world.GetExistingSystem<Test3_SimulationSystem>().Updated);
            Assert.IsFalse(world.GetExistingSystem<Test3_PresentationSystem>().Updated);
        }

        [Test]
        public void TestSimulationSystem_Updates_In_SimulationSystemGroup()
        {
            var world = GetWorld(WORLDNAME);
            // Ensure updated values are false;
            var initSys = world.GetExistingSystem<Test3_InitializationSystem>();
            var simSys = world.GetExistingSystem<Test3_SimulationSystem>();
            var presSys = world.GetExistingSystem<Test3_PresentationSystem>();
            initSys.Updated = false;
            simSys.Updated = false;
            presSys.Updated = false;

            world.GetExistingSystem<SimulationSystemGroup>().Update();

            Assert.IsFalse(world.GetExistingSystem<Test3_InitializationSystem>().Updated);
            Assert.IsTrue(world.GetExistingSystem<Test3_SimulationSystem>().Updated);
            Assert.IsFalse(world.GetExistingSystem<Test3_PresentationSystem>().Updated);
        }

        [Test]
        public void TestPresentationSystem_Updates_In_PresentationSystemGroup()
        {
            var world = GetWorld(WORLDNAME);
            // Ensure updated values are false;
            var initSys = world.GetExistingSystem<Test3_InitializationSystem>();
            var simSys = world.GetExistingSystem<Test3_SimulationSystem>();
            var presSys = world.GetExistingSystem<Test3_PresentationSystem>();
            initSys.Updated = false;
            simSys.Updated = false;
            presSys.Updated = false;

            world.GetExistingSystem<PresentationSystemGroup>().Update();

            Assert.IsFalse(world.GetExistingSystem<Test3_InitializationSystem>().Updated);
            Assert.IsFalse(world.GetExistingSystem<Test3_SimulationSystem>().Updated);
            Assert.IsTrue(world.GetExistingSystem<Test3_PresentationSystem>().Updated);
        }
    }
}