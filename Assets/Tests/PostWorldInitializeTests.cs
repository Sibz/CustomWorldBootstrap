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
    public class PostWorldInitializeTests : MyTestFixture
    {
        private const string WorldAName = "Test World A";
        private const string WorldBName = "Test World B";

        private string PostInitWorldAName = "";
        private string PostInitWorldBName = "";

        protected override void OneTimeSetup()
        {
            base.OneTimeSetup();
            new Initialiser(m_FakeCWB, true, new List<WorldOption>()
            {
                new WorldOption(WorldAName) { OnInitialize = PostWorldAIntialize },
                new WorldOption(WorldBName) { OnInitialize = PostWorldBIntialize }
            }).Initialise(m_DefaultSystems);
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