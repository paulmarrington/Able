// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

using System;
using NUnit.Framework;

namespace Askowl.Examples {
  /// <remarks><a href="http://unitydoc.marrington.net/Able#disposable">Use iDisposable for Greater Good</a></remarks>
  public class DisposableExamples {
    /// <remarks><a href="http://unitydoc.marrington.net/Able#disposable">Using <see cref="Disposable"/></a></remarks>
    [Test]
    public void Disposable() {
      numberOfMonsters = 0;

      using (Ephemeral()) {
        numberOfMonsters += 2;
        Assert.AreEqual(expected: 2, actual: numberOfMonsters);
      }

      Assert.AreEqual(expected: 0, actual: numberOfMonsters);
    }

    private IDisposable Ephemeral() => new Disposable {Action = () => numberOfMonsters = 0};

    private int numberOfMonsters;

    /// <remarks><a href="http://unitydoc.marrington.net/Able#disposable-with-payload">Using <see cref="Disposable{T}"/></a></remarks>
    [Test]
    public void DisposableT() {
      var sealedClass = new SealedClass();

      using (new Disposable<SealedClass> {Value = sealedClass}) {
        Assert.AreEqual("g'day", sealedClass.howdie);
        sealedClass.howdie = "hi";
      }

      Assert.AreEqual("hi", sealedClass.howdie);
    }

    /// <remarks><a href="http://unitydoc.marrington.net/Able#disposable-with-payload">Using <see cref="Disposable{T}"/></a></remarks>
    [Test]
    public void DisposableTWithAction() {
      var sealedClass = new SealedClass();

      using (new Disposable<SealedClass> {Value = sealedClass, Action = Action}) {
        Assert.AreEqual("g'day", sealedClass.howdie);
        sealedClass.howdie = "hi";
        Assert.AreEqual("hi", sealedClass.howdie);
      }

      Assert.AreEqual("Bonjour", sealedClass.howdie);
    }

    private void Action(SealedClass sealedClass) { sealedClass.howdie = "Bonjour"; }

    /// <remarks><a href="http://unitydoc.marrington.net/Able#disposable-with-payload">Using <see cref="Disposable{T}"/></a></remarks>
    [Test]
    public void DisposableTWithDispose() {
      var sealedClass = new SealedClassWithDispose();

      using (new Disposable<SealedClassWithDispose> {Value = sealedClass}) {
        Assert.AreEqual("morning", sealedClass.howdie);
        sealedClass.howdie = "hi";
        Assert.AreEqual("hi", sealedClass.howdie);
      }

      Assert.AreEqual("hello", sealedClass.howdie);
    }

    /// <remarks><a href="http://unitydoc.marrington.net/Able#disposable-with-payload">Using <see cref="Disposable{T}"/></a></remarks>
    [Test]
    public void DisposableTWithDisposeAndAction() {
      var sealedClass = new SealedClassWithDispose();

      using (new Disposable<SealedClassWithDispose> {Value = sealedClass, Action = Action2}) {
        Assert.AreEqual("morning", sealedClass.howdie);
        sealedClass.howdie = "hi";
        Assert.AreEqual("hi", sealedClass.howdie);
      }

      Assert.AreEqual("hello", sealedClass.howdie);
    }

    private void Action2(SealedClassWithDispose sealedClass) { sealedClass.howdie += "I am invisible"; }
  }

  internal class SealedClass {
    public string howdie = "g'day";
  }

  internal class SealedClassWithDispose : IDisposable {
    public string howdie = "morning";
    public void   Dispose() { howdie = "hello"; }
  }
}