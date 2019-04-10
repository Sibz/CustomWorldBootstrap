using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Unity.Entities;
using System.Linq;


namespace Tests
{
    public class EmptyTestWorldExists : MyECSTestFixtureBase
    {
        [Test]
        public void Test_World_1_Should_Exist()
        {
            Assert.IsTrue(World.AllWorlds.Any(x => x.Name == "Test World 1"));
        }
    }
}
