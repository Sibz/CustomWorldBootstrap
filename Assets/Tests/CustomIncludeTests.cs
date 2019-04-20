using CustomWorldBoostrapInternal;
using NUnit.Framework;
using Unity.Burst;
using Unity.Entities;
using UnityEditor;
using UnityEngine;
using System.Linq;

namespace Tests
{
    public class CustomIncludeTests : MyTestFixture
    {
        private const string WORLDNAME = "Test7 World";

        [OneTimeSetUp]
        protected override void OneTimeSetup()
        {
            base.OneTimeSetup();
            m_DefaultSystems.Add(typeof(Test7_System));
            new Initialiser(
                m_FakeCWB,
                true,
                new System.Collections.Generic.List<CustomWorldBootstrap.WorldOption>() {
                    new CustomWorldBootstrap.WorldOption(WORLDNAME) {
                        CustomIncludeQuery = systems=>systems
                        .Where(system=>system.GetInterfaces().Any(iface=>iface.Name== nameof(ITest7))).ToList()
                    }
                }).Initialise(m_DefaultSystems);
        }

        [Test]
        public void Should_Have_Test7_World()
        {
            Assert.IsTrue(World.AllWorlds.Any(w=>w.Name==WORLDNAME));
        }
        [Test]
        public void Should_Have_Test7_System_In_Test7_World()
        {
            Assert.IsTrue(World.AllWorlds.Where(w => w.Name == WORLDNAME).First().Systems.Any(sys=>sys.GetType().Name==nameof(Test7_System)));
        }
    }
}
