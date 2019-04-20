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
            m_DefaultSystems.Add(typeof(Test6_System1b));
            m_DefaultSystems.Add(typeof(Test6_System2b));
            m_DefaultSystems.Add(typeof(Test6_System3b));
            var newSystems = new Initialiser(m_FakeCWB).Initialise(m_DefaultSystems);

            World.Active.GetExistingSystem<SimulationSystemGroup>().Update();
        }
       
        [Test]
        public void Creates_Custom_World()
        {
            Assert.IsTrue(WorldExists(WORLDNAME));
        }

        [Test]
        public void System_Update_In_Order()
        {
            var world = GetWorld(WORLDNAME);
            Assert.AreEqual("321", world.GetExistingSystem<Test6_Group>().UpdateOrder);
        }

        [Test]
        public void System_In_Root_SimGroup_Update_In_Order()
        {
            var world = GetWorld(WORLDNAME);
            Assert.AreEqual("321", world.GetExistingSystem<Test6_Group>().UpdateOrderB);
        }
    }
}