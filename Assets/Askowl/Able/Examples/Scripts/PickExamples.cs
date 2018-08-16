// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

using NUnit.Framework;
using UnityEngine;

namespace Askowl.Examples {
  /// <remarks><a href="http://unitydoc.marrington.net/Able#pickcs-interface-to-choose-from-options">Pick Interface</a></remarks>
  public class PickExamples {
    /// <remarks><a href="http://unitydoc.marrington.net/Able#pickcs-interface-to-choose-from-options">Pick Interface</a></remarks>
    [Test]
    internal void PickExample() {
      PickImplementation nose = new PickImplementation();

      Assert.AreEqual("1", nose.Pick());
      Assert.AreEqual("2", nose.Pick());
      Assert.AreEqual("3", nose.Pick());
    }

    private class PickImplementation : Pick<string> {
      private int    count;
      public  string Pick() => (++count).ToString();
    }
  }
}