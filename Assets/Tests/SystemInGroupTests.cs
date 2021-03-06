﻿using NUnit.Framework;
using Unity.Entities;
using CustomWorldBoostrapInternal;

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

        [Test]
        public void Creates_Custom_World()
        {
            Assert.IsTrue(WorldExists(WORLDNAME));
        }

        [Test]
        public void Group_Is_In_Custom_World()
        {
            Assert.IsTrue(SystemExistsInWorld(WORLDNAME, typeof(Test4_Group)));
        }


        [Test]
        public void System_Is_In_Custom_World()
        {
            Assert.IsTrue(SystemExistsInWorld(WORLDNAME, typeof(Test4_System)));
        }

        [Test]
        public void Systems_Updates_In_UpdateGroup()
        {
            var world = GetWorld(WORLDNAME);

            World.Active.GetExistingSystem<SimulationSystemGroup>().Update();
            Assert.IsTrue(world.GetExistingSystem<Test4_System>().Updated);
        }
    }
}