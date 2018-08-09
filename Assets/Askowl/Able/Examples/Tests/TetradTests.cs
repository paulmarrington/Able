#if UNITY_EDITOR && Able
using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using Askowl;

public class TetradTests {
  [Test]
  public void TetradIdentity() {
    var identity = Tetrad.Identity;
    Assert.AreEqual(identity.Quaternion, Quaternion.identity);
  }
}
#endif