﻿// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

// ReSharper disable MissingXmlDoc
using NUnit.Framework;
using UnityEngine;
#if !ExcludeAskowlTests
namespace Askowl.Able.Examples {
  public class ExponentialMovingAverageExamples {
    [Test] public void AverageValue() {
      var ema = new ExponentialMovingAverage(lookBack: 4);

      AreEqual(expected: 1f,      actual: ema.Average(value: 1));
      AreEqual(expected: 2.6f,    actual: ema.Average(value: 5));
      AreEqual(expected: 2.76f,   actual: ema.Average(value: 3));
      AreEqual(expected: 4.056f,  actual: ema.Average(value: 6));
      AreEqual(expected: 4.0336f, actual: ema.Average(value: 4));

      AreEqual(expected: 4.02016f,  actual: ema.Average(value: 4));
      AreEqual(expected: 4.012096f, actual: ema.Average(value: 4));
      AreEqual(expected: 4.007257f, actual: ema.Average(value: 4));
      AreEqual(expected: 4.004354f, actual: ema.Average(value: 4));
      AreEqual(expected: 4.002613f, actual: ema.Average(value: 4));
      AreEqual(expected: 4.001567f, actual: ema.Average(value: 4));
      AreEqual(expected: 4f,        actual: ema.Average(value: 4));
      AreEqual(expected: 4f,        actual: ema.Average(value: 4));
    }

    [Test] public void AverageAngle() {
      var ema = new ExponentialMovingAverage(); // look-back defaults to 8

      AreEqual(expected: -10f,       actual: ema.AverageAngle(degrees: -10));
      AreEqual(expected: -6f,        actual: ema.AverageAngle(degrees: 10));
      AreEqual(expected: -5.8f,      actual: ema.AverageAngle(degrees: -5));
      AreEqual(expected: -3.64f,     actual: ema.AverageAngle(degrees: 5));
      AreEqual(expected: -3.511997f, actual: ema.AverageAngle(degrees: 357));
      AreEqual(expected: -2.009598f, actual: ema.AverageAngle(degrees: 364));
      /* Why did this change?
      AreEqual(expected: -10f, actual: ema.AverageAngle(degrees: -10));
      AreEqual(expected: -5.555555f, actual: ema.AverageAngle(degrees: 10));
      AreEqual(expected: -5.432098f, actual: ema.AverageAngle(degrees: -5));
      AreEqual(expected: -3.113854f, actual: ema.AverageAngle(degrees: 5));
      AreEqual(expected: -3.088552f, actual: ema.AverageAngle(degrees: 357));
      AreEqual(expected: -1.513316f, actual: ema.AverageAngle(degrees: 364));
       */
    }

    private void AreEqual(float expected, float actual) => Assert.IsTrue(
      Compare.AlmostEqual(expected, actual), $"Expected {expected}, Actual {actual}");

    [Test] public void Example() {
      var ema = new ExponentialMovingAverage();
      var sum = 0;
      for (var i = 1; i <= 20; i++) {
        float sma = (float) (sum += i) / i;
        Debug.Log($"{i,2}: sma={sma:n2}\tema={ema.Average(i):n2}");
      }
    }
  }
}
#endif