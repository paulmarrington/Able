// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

#if AskowlTests
using System;
using NUnit.Framework;

namespace Askowl.Able.Examples {
  public class SelectorExamples {
    private readonly int[] ints   = { 0, 1, 2, 3, 4 };
    private          int[] counts = { 0, 0, 0, 0, 0 };

    [Test] public void Random() {
      // default is random
      var selector = new Selector<int> { Choices = ints };

      for (var i = 0; i < 10; i++) {
        int pick = selector.Pick();
        counts[pick]++;
      }

      var failed = true;

      for (var i = 0; failed && (i < counts.Length); i++) {
        if (counts[i] != 2) failed = false;
      }

      if (failed) {
        Assert.Fail($"Random selector too even: {string.Join(", ", Array.ConvertAll(counts, i => i.ToString()))}");
      }
    }

    [Test] public void Sequential() {
      var selector = new Selector<int> { Choices = ints, IsRandom = false };

      for (var i = 0; i < 10; i++) {
        int pick = selector.Pick();

        if (pick != i % 5) Assert.Fail($"Sequential failed with {pick} on iteration {i}");
      }
    }

    [Test] public void ExhaustiveRandom() {
      // or we can be random, but exhaust all possibilities before going round again
      var selector = new Selector<int> { Choices = ints, ExhaustiveBelow = 100 };

      counts = new[] { 0, 0, 0, 0, 0 };

      for (var i = 0; i < 10; i++) {
        int pick = selector.Pick();
        counts[pick]++;
      }

      for (var i = 0; i < counts.Length; i++) Assert.AreEqual(2, counts[i]);
    }

    [Test] public void ExhaustiveWatermark() {
      // Unless our number of choices are below a watermark value
      var selector = new Selector<int> { Choices = ints, ExhaustiveBelow = 4 };

      for (var i = 0; i < 10; i++) {
        int pick = selector.Pick();
        counts[pick]++;
      }

      var failed = true;

      for (var i = 0; failed && (i < counts.Length); i++) {
        if (counts[i] != 2) failed = false;
      }

      Assert.IsFalse(failed);
    }
  }
}
#endif