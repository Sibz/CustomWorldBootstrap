using NUnit.Framework;
using Unity.Entities;
using CustomWorldBoostrapInternal;


namespace Tests
{
    public class UpdateInTeir2GroupTests : MyTestFixture
    {
        private const string WORLDNAME = "Test5 World";

        [OneTimeSetUp]
        protected override void OneTimeSetup()
        {
            base.OneTimeSetup();
            m_DefaultSystems.Add(typeof(Test5_Group1));
            m_DefaultSystems.Add(typeof(Test5_Group2));
            m_DefaultSystems.Add(typeof(Test5_System));
            var newSystems = new Initialiser(m_FakeCWB).Initialise(m_DefaultSystems);
        }

        [Test]
        public void Creates_Custom_World()
        {
            Assert.IsTrue(WorldExists(WORLDNAME));
        }

        [Test]
        public void Group1_Is_In_Custom_World()
        {
            Assert.IsTrue(SystemExistsInWorld(WORLDNAME, typeof(Test5_Group1)));
        }
        [Test]
        public void Group2_Is_In_Custom_World()
        {
            Assert.IsTrue(SystemExistsInWorld(WORLDNAME, typeof(Test5_Group2)));
        }
        [Test]
        public void System_Is_In_Custom_World()
        {
            Assert.IsTrue(SystemExistsInWorld(WORLDNAME, typeof(Test5_System)));
        }

        [Test]
        public void System_Updates_In_UpdateGroup()
        {
            var world = GetWorld(WORLDNAME);
            World.Active.GetExistingSystem<SimulationSystemGroup>().Update();
            Assert.IsTrue(world.GetExistingSystem<Test5_System>().Updated);
        }
    }
}