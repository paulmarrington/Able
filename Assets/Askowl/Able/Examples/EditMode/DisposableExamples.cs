// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

#if AskowlTests
namespace Askowl.Able.Examples {
  using System;
  using NUnit.Framework;

  public class DisposableExamples {
    [Test] public void Disposable() {
      numberOfMonsters = 0;

      using (Ephemeral()) {
        numberOfMonsters += 2;
        Assert.AreEqual(expected: 2, actual: numberOfMonsters);
      }

      Assert.AreEqual(expected: 0, actual: numberOfMonsters);
    }

    private IDisposable Ephemeral() => new Disposable { Action = () => numberOfMonsters = 0 };

    private int numberOfMonsters;

    [Test] public void DisposableT() {
      var sealedClass = new SealedClass();

      using (new Disposable<SealedClass> { Value = sealedClass }) {
        Assert.AreEqual("g'day", sealedClass.Howdie);
        sealedClass.Howdie = "hi";
      }

      Assert.AreEqual("hi", sealedClass.Howdie);
    }

    [Test] public void DisposableTWithAction() {
      var sealedClass = new SealedClass();

      using (new Disposable<SealedClass> { Value = sealedClass, Action = Action }) {
        Assert.AreEqual("g'day", sealedClass.Howdie);
        sealedClass.Howdie = "hi";
        Assert.AreEqual("hi", sealedClass.Howdie);
      }

      Assert.AreEqual("Bonjour", sealedClass.Howdie);
    }

    private void Action(SealedClass sealedClass) { sealedClass.Howdie = "Bonjour"; }

    [Test] public void DisposableTWithDispose() {
      var sealedClass = new SealedClassWithDispose();

      using (new Disposable<SealedClassWithDispose> { Value = sealedClass }) {
        Assert.AreEqual("morning", sealedClass.Howdie);
        sealedClass.Howdie = "hi";
        Assert.AreEqual("hi", sealedClass.Howdie);
      }

      Assert.AreEqual("hello", sealedClass.Howdie);
    }

    [Test] public void DisposableTWithDisposeAndAction() {
      var sealedClass = new SealedClassWithDispose();

      using (new Disposable<SealedClassWithDispose> { Value = sealedClass, Action = Action2 }) {
        Assert.AreEqual("morning", sealedClass.Howdie);
        sealedClass.Howdie = "hi";
        Assert.AreEqual("hi", sealedClass.Howdie);
      }

      Assert.AreEqual("hello", sealedClass.Howdie);
    }

    private void Action2(SealedClassWithDispose sealedClass) { sealedClass.Howdie += "I am invisible"; }
  }

  internal class SealedClass {
    public string Howdie = "g'day";
  }

  internal class SealedClassWithDispose : IDisposable {
    public string Howdie = "morning";
    public void   Dispose() { Howdie = "hello"; }
  }
}
#endif