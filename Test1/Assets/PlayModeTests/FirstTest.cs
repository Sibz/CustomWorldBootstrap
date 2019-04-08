using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class FirstTest
    {
        // A Test behaves as an ordinary method
        [Test]
        [UnityPlatform(RuntimePlatform.WindowsPlayer)]
        public void NewTestScriptSimplePasses()
        {
            // Use the Assert class to test conditions
            Assert.IsTrue(true);
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        [UnityPlatform(RuntimePlatform.WindowsPlayer)]
        public IEnumerator NewTestScriptWithEnumeratorPasses()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            Assert.IsTrue(true);

            yield return null;
        }
    }
}
