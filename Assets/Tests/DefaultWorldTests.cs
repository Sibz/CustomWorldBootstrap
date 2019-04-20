using CustomWorldBoostrapInternal;
using NUnit.Framework;
using Unity.Entities;
using System.Linq;

namespace Tests
{
    public class DefaultWorldTests : MyTestFixture
    {
        [Test]
        public void Default_Produces_Unchanged_System_List()
        {
            var init = new Initialiser(m_FakeCWB);
            var hash = GetListHash(DefaultSystems);
            Assert.AreEqual(hash, GetListHash(init.Initialise(DefaultSystems)));
        }

        [Test]
        public void Setting_CreateDefaultWorld_False_Produces_Null()
        {
            var init = new Initialiser(m_FakeCWB, false);
            Assert.IsNull(init.Initialise(DefaultSystems));
        }

        [Test]
        public void Untagged_Systems_Stay_In_Default_List()
        {
            m_DefaultSystems.Add(typeof(Test1));
            var newSystems = new Initialiser(m_FakeCWB).Initialise(m_DefaultSystems);
            Assert.Contains(typeof(Test1), newSystems);
        }

        [Test]
        public void Should_Create_A_New_Default_World_With_10_Default_Systems()
        {
            new Initialiser(
                m_FakeCWB,
                true,
                new System.Collections.Generic.List<CustomWorldBootstrap.WorldOption> {
                    new CustomWorldBootstrap.WorldOption("Test A")
                },
                "Test A")
                .Initialise(m_DefaultSystems);
            Assert.IsTrue(World.AllWorlds.Any(x=>x.Name=="Test A"));
            Assert.AreSame("Test A", World.Active.Name);
            Assert.AreEqual(10, World.Active.Systems.Count());
        }

        [Test]
        public void Should_Return_Null_When_Creating_A_New_Default_World()
        {
            Assert.IsNull(new Initialiser(
                m_FakeCWB,
                true,
                new System.Collections.Generic.List<CustomWorldBootstrap.WorldOption> {
                    new CustomWorldBootstrap.WorldOption("Test A")
                },
                "Test A")
                .Initialise(m_DefaultSystems));
        }
    }
}