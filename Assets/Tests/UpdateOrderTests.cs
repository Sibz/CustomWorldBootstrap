using NUnit.Framework;
using Unity.Entities;
using CustomWorldBoostrapInternal;


namespace Tests
{
    public class UpdateOrderTests : MyTestFixture
    {
        private const string WORLDNAME = "Test6 World";

        [OneTimeSetUp]
        protected override void OneTimeSetup()
        {
            base.OneTimeSetup();
            m_DefaultSystems.Add(typeof(Test6_Group));
            m_DefaultSystems.Add(typeof(Test6_System1));
            m_DefaultSystems.Add(typeof(Test6_System2));
            m_DefaultSystems.Add(typeof(Test6_System3));
            var newSystems = new Initialiser(m_FakeCWB).Initialise(m_DefaultSystems);
        }

        [Test]
        public void Creates_Custom_World()
        {
            Assert.IsTrue(WorldExists(WORLDNAME));
        }

        [Test]
        public void System_Updates_In_Order()
        {
            var world = GetWorld(WORLDNAME);
            World.Active.GetExistingSystem<SimulationSystemGroup>().Update();
            Assert.AreEqual("321", world.GetExistingSystem<Test6_Group>().UpdateOrder);
        }
    }
}