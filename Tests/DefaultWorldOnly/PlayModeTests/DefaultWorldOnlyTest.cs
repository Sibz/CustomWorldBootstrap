using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Unity.Entities;
using System.Linq;


namespace Tests
{
    public class DefaultWorldOnlyTest : MyECSTestFixtureBase
    {

        [Test]
        public void Initialize_Should_Not_Throw_Exception()
        {
            Assert.IsNull(CustomWorldBootstrap.InitializeException);
        }

        [Test]
        public void Should_Be_Able_To_Create_Entity()
        {
            var ent = m_Manager.CreateEntity();
            Assert.IsTrue(m_Manager.Exists(ent));
        }

        [Test]
        public void DefaultWorldShouldExist()
        {
            Assert.IsTrue(World.AllWorlds.Any(x => x.Name == "Default World"));
        }
    }
}
