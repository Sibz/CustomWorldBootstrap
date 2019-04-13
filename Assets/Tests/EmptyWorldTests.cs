using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class EmptyWorldTests : MyTestFixture
    {
        [Test]
        public void World_Gets_Created_And_Is_Empty()
        {
            string worldName = "Test Empty World";
            new Initialiser(m_FakeCWB, true, new List<WorldOption>() { new WorldOption(worldName) }).Initialise(DefaultSystems);
            Assert.IsTrue(World.AllWorlds.Any(x => x.Name.Equals(worldName)));
            Assert.IsEmpty(World.AllWorlds.Where(x => x.Name.Equals(worldName)).First().Systems.Where(x=>!m_InitialSystems.Contains(x.GetType())));
        }
    }
}