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
    [TestFixture]
    public abstract class MyTestFixture
    {
        public static List<Type> DefaultSystems;
        private static bool m_HasInitialised = false;
        protected List<Type> m_DefaultSystems;
        protected FakeCustomWorldBootstrap m_FakeCWB;
        //protected List<World> m_InitialWorlds = new List<World>();

        [OneTimeSetUp]
        protected virtual void OneTimeSetup()
        {
            // We need to intitialise the default world to get a list
            // of default systems, DefaultSystems is set in TestBootStrap
            if (!m_HasInitialised)
            {
                DefaultWorldInitialization.Initialize("Default World", false);
                m_HasInitialised = true;
            }

            // This is so we can preserve the initial worlds
            // This is redundant in Entities.30 as we can't remove
            // worlds or systems without breaking stuff
            //foreach (var w in World.AllWorlds)
            //{
            //    m_InitialWorlds.Add(w);
            //}

            // Set up our default list of systems to test with
            // Inherited classes can add to this list
            m_DefaultSystems = new List<Type>();
            m_DefaultSystems.AddRange(DefaultSystems);

            // Just to fake the PostInitialize function;
            m_FakeCWB = new FakeCustomWorldBootstrap();
        }

        [SetUp]
        protected virtual void Setup()
        {

        }

        [TearDown]
        protected virtual void TearDown()
        {

        }

        [OneTimeTearDown]
        protected virtual void OneTimeTearDown()
        {

        }

        protected bool WorldExists(string worldName)
        {
            return World.AllWorlds.Any(x => x.Name.Equals(worldName));
        }

        protected World GetWorld(string worldName)
        {
            return World.AllWorlds.Where(x => x.Name.Equals(worldName)).First();
        }

        protected bool SystemExistsInWorld(string worldName, Type sytemType)
        {
            return GetWorld(worldName).Systems.Any(x => x.GetType() == sytemType);
        }

        protected string GetListHash(List<Type> systems)
        {
            string hash = "";
            foreach (var item in systems)
            {
                hash += item.GetHashCode().ToString();
            }
            return hash;
        }
    }
}