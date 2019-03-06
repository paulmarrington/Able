// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

using NUnit.Framework;
#if !ExcludeAskowlTests
namespace Askowl.Able.Examples {
  public class CounterStackExamples {
    [Test] public void Instance() {
      var counter = CounterFifo.Instance;
      Assert.IsNotNull(counter);
    }

    [Test] public void Start() {
      using (var counter = CounterFifo.Instance) { Assert.AreEqual(0, counter.Start()); }
    }

    [Test] public void Next() {
      using (var counter = CounterFifo.Instance) {
        Assert.AreEqual(0, counter.Start());
        Assert.AreEqual(1, counter.Next());
      }
    }

    [Test] public void Reached() {
      using (var counter = CounterFifo.Instance) {
        var actual = "";

        for (int count = counter.Start(); !counter.Reached(10); count = counter.Next()) { actual += count.ToString(); }

        Assert.AreEqual("0123456789", actual);
      }
    }

    [Test] public void Countdown() {
      using (var counter = CounterFifo.Instance) {
        var actual = "";

        for (int count = counter.Start(-10); !counter.Reached(0); count = counter.Next()) { actual += count; }

        Assert.AreEqual("10987654321", actual);
      }
    }
  }
}
#endif