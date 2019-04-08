using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Unity.Entities;
using System.Linq;

namespace Tests
{
    public class DefaultWorldOnlyTest
    {
        [Test]
        public void DefaultWorldShouldExist()
        {
            Assert.IsTrue(World.AllWorlds.Any(x => x.Name == "Default World"));
        }
    }
}
