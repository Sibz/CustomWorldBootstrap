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
        private const string WORLDNAME = "Test Empty World";
        protected List<Type> m_InitialSystems = new List<Type>();


        protected override void OneTimeSetup()
        {
            base.OneTimeSetup();

            // List of initial systems we can test against to make sure our empty system does not contain more than this.
            foreach (var s in World.Active.Systems)
            {
                m_InitialSystems.Add(s.GetType());
            }
        }

        [Test]
        public void World_Gets_Created_And_Is_Empty()
        {
            new Initialiser(m_FakeCWB, true, new List<WorldOption>() { new WorldOption(WORLDNAME) }).Initialise(DefaultSystems);
            Assert.IsTrue(WorldExists(WORLDNAME));
            Assert.IsEmpty(GetWorld(WORLDNAME).Systems.Where(x=>!m_InitialSystems.Contains(x.GetType())));
        }
    }
}