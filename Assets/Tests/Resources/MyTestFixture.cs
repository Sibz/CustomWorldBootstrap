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
        protected static bool hasIntialised = false;
        protected List<Type> m_DefaultSystems;
        protected FakeCustomWorldBootstrap m_FakeCWB;
        protected int m_IntitialNumberOfWorlds = 0;
        protected List<World> m_InitialWorlds = new List<World>();
        protected List<Type> m_InitialSystems = new List<Type>();

        [OneTimeSetUp]
        protected virtual void OneTimeSetup()
        {
            if (!hasIntialised)
            {
                DefaultWorldInitialization.Initialize("Default World", false);
                hasIntialised = true;
            }
            foreach (var s in World.Active.Systems)
            {
                m_InitialSystems.Add(s.GetType());
            }
            foreach (var w in World.AllWorlds)
            {
                m_InitialWorlds.Add(w);
            }
            m_IntitialNumberOfWorlds = World.AllWorlds.Count;
            m_DefaultSystems = new List<Type>();
            m_DefaultSystems.AddRange(DefaultSystems);

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
            m_FakeCWB = null;
            for (int i = World.AllWorlds.Count - 1; i >= 0; i--)
            {
                var item = World.AllWorlds[i];
                if (!m_InitialWorlds.Contains(item))
                {
                    while (item.Systems.Any(x => !m_InitialSystems.Contains(x.GetType())))
                    {
                        var system = item.Systems.Where(x => !m_InitialSystems.Contains(x.GetType())).First();
                        item.DestroySystem(system);
                    }

                    //new EntityManager.EntityManagerDebug(item.EntityManager).CheckInternalConsistency();
                    //if (item.IsCreated)
                    //{
                    //    item.Dispose();
                    //    item.
                    //}
                    //item = null;
                }
            }
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