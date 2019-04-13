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
    }
}