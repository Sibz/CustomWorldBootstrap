using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using CustomWorldBoostrapInternal;

namespace Tests
{
    public class EmptyWorldTests : MyTestFixture
    {
        private const string WORLDNAME = "Test Empty World";
        private List<Type> m_InitialSystems = new List<Type>();

        protected override void OneTimeSetup()
        {
            base.OneTimeSetup();

            // List of initial systems we can test against to make sure our empty system does not contain more than this.
            foreach (var s in World.Active.Systems)
            {
                m_InitialSystems.Add(s.GetType());
            }

            new Initialiser(m_FakeCWB, true, new List<CustomWorldBootstrap.WorldOption>() { new CustomWorldBootstrap.WorldOption(WORLDNAME) }).Initialise(DefaultSystems);
        }

        [Test]
        public void World_Gets_Created()
        {
            Assert.IsTrue(WorldExists(WORLDNAME));
        }

        [Test]
        public void World_Is_Has_No_NonDefault_Systems()
        {
            Assert.IsEmpty(GetWorld(WORLDNAME).Systems);
        }
    }
}