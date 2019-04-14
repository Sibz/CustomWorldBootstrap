using NUnit.Framework;
using Unity.Entities;
using CustomWorldBoostrapInternal;

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

        [Test]
        public void Creates_Custom_World()
        {
            Assert.IsTrue(WorldExists(WORLDNAME));
        }

        [Test]
        public void System_Is_In_Custom_World()
        {
            Assert.IsTrue(SystemExistsInWorld(WORLDNAME, typeof(Test2)));
        }

        [Test]
        public void System_Updates_In_Default_UpdateGroup()
        {
            var world =GetWorld(WORLDNAME);
            world.GetExistingSystem<SimulationSystemGroup>().Update();
            Assert.IsTrue(world.GetExistingSystem<Test2>().Updated);
        }
    }
}