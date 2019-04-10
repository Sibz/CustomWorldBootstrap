using NUnit.Framework;
using System.Linq;
using Unity.Entities;
using UnityEditor;
using UnityEngine;

[TestFixture]
public class MyECSTestFixtureBase
{

    protected World m_PreviousWorld;
    protected World World;
    protected EntityManager m_Manager;
    protected EntityManager.EntityManagerDebug m_ManagerDebug;

    protected int StressTestEntityCount = 1000;

    [SetUp]
    public virtual void Setup()
    {
        m_PreviousWorld = World.Active;
#if !UNITY_ZEROPLAYER
        World = World.Active = new World("Test World");
#else
            World = DefaultTinyWorldInitialization.Initialize("Test World");
#endif

        m_Manager = World.EntityManager;
        m_ManagerDebug = new EntityManager.EntityManagerDebug(m_Manager);

#if !UNITY_ZEROPLAYER
#if !UNITY_2019_2_OR_NEWER
            // Not raising exceptions can easily bring unity down with massive logging when tests fail.
            // From Unity 2019.2 on this field is always implicitly true and therefore removed.

            UnityEngine.Assertions.Assert.raiseExceptions = true;
#endif  // #if !UNITY_2019_2_OR_NEWER
#endif  // #if !UNITY_ZEROPLAYER
    }

    [TearDown]
    public virtual void TearDown()
    {
        if (m_Manager != null)
        {
            // Clean up systems before calling CheckInternalConsistency because we might have filters etc
            // holding on SharedComponentData making checks fail
            while (World.Systems.Any())
            {
                World.DestroySystem(World.Systems.First());
            }

            m_ManagerDebug.CheckInternalConsistency();

            World.Dispose();
            World = null;

            World.Active = m_PreviousWorld;
            m_PreviousWorld = null;
            m_Manager = null;
        }
    }

}