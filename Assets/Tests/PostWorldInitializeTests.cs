using CustomWorldBoostrapInternal;
using NUnit.Framework;
using System.Collections.Generic;
using Unity.Entities;


namespace Tests
{
    public class PostWorldInitializeTests : MyTestFixture
    {
        private const string WorldAName = "Test World A";
        private const string WorldBName = "Test World B";

        private string PostInitWorldAName = "";
        private string PostInitWorldBName = "";

        protected override void OneTimeSetup()
        {
            base.OneTimeSetup();
            new Initialiser(new FakeCustomWorldBootstrap(new List<CustomWorldBootstrap.WorldOption>()
            {
                new CustomWorldBootstrap.WorldOption(WorldAName) { OnInitialize = PostWorldAIntialize },
                new CustomWorldBootstrap.WorldOption(WorldBName) { OnInitialize = PostWorldBIntialize }
            })).Initialise(m_DefaultSystems);
        }

        [Test]
        public void PostWorldAInitialize_Runs_With_Correct_World()
        {
            Assert.AreEqual(PostInitWorldAName, WorldAName);
        }

        [Test]
        public void PostWorldBInitialize_Runs_With_Correct_World()
        {
            Assert.AreEqual(PostInitWorldBName, WorldBName);
        }

        public void PostWorldAIntialize(World world)
        {
            PostInitWorldAName = world.Name;
        }
        public void PostWorldBIntialize(World world)
        {
            PostInitWorldBName = world.Name;
        }
    }
}