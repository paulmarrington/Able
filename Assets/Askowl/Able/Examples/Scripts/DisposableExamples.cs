// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

using System;
using NUnit.Framework;

namespace Askowl.Examples {
  /// <remarks><a href="http://unitydoc.marrington.net/Able#disposable">Use iDisposable for Greater Good</a></remarks>
  public class DisposableExamples {
    /// <remarks><a href="http://unitydoc.marrington.net/Able#disposable">Use iDisposable for Greater Good</a></remarks>
    [Test]
    public void Disposable() {
      Assert.AreEqual(expected: 0, actual: numberOfMonsters);

      using (Ephemeral()) {
        numberOfMonsters += 2;
        Assert.AreEqual(expected: 2, actual: numberOfMonsters);
      }

      Assert.AreEqual(expected: 0, actual: numberOfMonsters);
    }

    private IDisposable Ephemeral() => new Disposable {Action = () => numberOfMonsters = 0};

    private int numberOfMonsters;
  }
}