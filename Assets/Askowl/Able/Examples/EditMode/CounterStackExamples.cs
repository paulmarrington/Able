// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

#if AskowlAble
namespace Askowl.Examples {
  using NUnit.Framework;

  public class CounterStackExamples {
    /// Using <see cref="CounterFifo.Instance"/>
    [Test]
    public void Instance() {
      var counter = CounterFifo.Instance;
      Assert.IsNotNull(counter);
    }

    /// Using <see cref="CounterFifo.Start"/>
    [Test]
    public void Start() {
      using (var counter = CounterFifo.Instance) {
        Assert.AreEqual(0, counter.Start());
      }
    }

    /// Using <see cref="CounterFifo.Next"/>
    [Test]
    public void Next() {
      using (var counter = CounterFifo.Instance) {
        Assert.AreEqual(0, counter.Start());
        Assert.AreEqual(1, counter.Next());
      }
    }

    /// Using <see cref="CounterFifo.Reached"/>
    [Test]
    public void Reached() {
      using (var counter = CounterFifo.Instance) {
        var actual = "";

        for (var count = counter.Start(); !counter.Reached(10); count = counter.Next()) {
          actual += count.ToString();
        }

        Assert.AreEqual("0123456789", actual);
      }
    }

    /// Using <see cref="CounterFifo"/>
    [Test]
    public void Countdown() {
      using (var counter = CounterFifo.Instance) {
        var actual = "";

        for (var count = counter.Start(-10); !counter.Reached(0); count = counter.Next()) {
          actual += count;
        }

        Assert.AreEqual("10987654321", actual);
      }
    }
  }
}
#endif