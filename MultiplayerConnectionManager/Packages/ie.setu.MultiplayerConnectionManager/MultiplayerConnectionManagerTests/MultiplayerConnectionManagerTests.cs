using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using MultiplayerConnectionManagerPackage;

public class MultiplayerConnectionManagerTests
{
    [Test]
    public void MultiplayerConnectionManagerTestsSimplePasses()
    {
        MultiplayerConnectionManager mp = new MultiplayerConnectionManager();
        mp.Start();
    }
}
