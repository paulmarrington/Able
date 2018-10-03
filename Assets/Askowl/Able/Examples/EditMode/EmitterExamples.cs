// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

#if UNITY_EDITOR && Able

using System;
using NUnit.Framework;

namespace Askowl.Examples { //#TBD#
  public class EmitterExamples {
    [Test] public void ObserverPatternUsing() {
      counter = 0;

      var emitter = new Emitter();

      using (emitter.Subscribe(new Observer1())) {
        using (emitter.Subscribe(new Observer2())) {
          Assert.AreEqual(expected: 0, actual: counter);
          emitter.Fire();
          Assert.AreEqual(expected: 3, actual: counter);
          emitter.Fire();
          Assert.AreEqual(expected: 6, actual: counter);
        }
      }

      // Each has run OnCompleted
      Assert.AreEqual(expected: 4, actual: counter);
    }

    [Test] public void ObserverT() {
      counter = 0;

      var emitter = new Emitter<int>();

      using (emitter.Subscribe(new Observer3())) {
        Assert.AreEqual(expected: 0, actual: counter);
        emitter.Fire(10);
        Assert.AreEqual(expected: 10, actual: counter);
        emitter.Fire(9);
        Assert.AreEqual(expected: 9, actual: counter);
      }

      // implicit Dispose()
      Assert.AreEqual(expected: 8, actual: counter);
      // Last value broadcast
      Assert.AreEqual(expected: 9, actual: emitter.LastValue);
    }

    [Test] public void ObserverPatternAbort() {
      counter = 0;

      var emitter = new Emitter();

      using (var subscription = emitter.Subscribe(new Observer1())) {
        // we now have one subscription
        Assert.AreEqual(expected: 0, actual: counter);
        // Tell observers we have something for them
        emitter.Fire();
        // The observer changes the value
        Assert.AreEqual(expected: 1, actual: counter);
        // A manual call to Dispose will stop the observer listening ...
        subscription.Dispose();
        // ... and calls OnComplete that in this case sets count to zero
        Assert.AreEqual(expected: 0, actual: counter);
        // Now if we fire...
        emitter.Fire(); // not listening any more
        // ... the counter doesn't change because we have no observers
        Assert.AreEqual(expected: 0, actual: counter);
      }

      // Outside the using - and Dispose was called implicitly but OnComplete was not called again
      Assert.AreEqual(expected: 0, actual: counter);
    }

    private struct Observer1 : IObserver {
      public void OnNext()      { ++counter; }
      public void OnCompleted() { counter--; }
    }

    private struct Observer2 : IObserver {
      public void OnNext()      { counter += 2; }
      public void OnCompleted() { counter--; }
    }

    private struct Observer3 : IObserver<int> {
      public void OnCompleted()            { counter--; }
      public void OnError(Exception error) { throw new NotImplementedException(); }
      public void OnNext(int        value) { counter = value; }
    }

    private static int counter;
  }
}
#endif