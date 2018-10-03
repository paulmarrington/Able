// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

namespace Askowl {
  using System;

  /// <a href="http://bit.ly/2NTBLW9">Comparators not available elsewhere</a>
  public static class Compare {
    /// <a href="http://bit.ly/2Ozz4st">Check two floating point numbers to be within rounding tolerance.</a>
    public static bool AlmostEqual(float a, float b, float minimumChange) => Math.Abs(a - b) < minimumChange;

    /// <a href="http://bit.ly/2Ozz4st">Check two floating point numbers to be within rounding tolerance (0.00001)</a>
    public static bool AlmostEqual(float a, float b) => AlmostEqual(a, b, minimumChange: 1e-3f);

    /// <a href="http://bit.ly/2Ozz4st">Check two double floating point numbers to be within rounding tolerance.</a>
    public static bool AlmostEqual(double a, double b, double minimumChange) => Math.Abs(a - b) < minimumChange;

    /// <a href="http://bit.ly/2Ozz4st">Check two double floating point numbers to be within rounding tolerance (0.00001)</a>
    public static bool AlmostEqual(double a, double b) => AlmostEqual(a, b, minimumChange: 1e-5);

    /// <a href="http://bit.ly/2Rj0Xn8">Check two long integers to be within rounding tolerance.</a>
    public static bool AlmostEqual(long a, long b, long minimumChange) => Math.Abs(a - b) < minimumChange;

    /// <a href="http://bit.ly/2Rj0Xn8">Check two long integers to be within rounding tolerance (1 each way)</a>
    public static bool AlmostEqual(long a, long b) => AlmostEqual(a, b, minimumChange: 2);

    /// <a href="http://bit.ly/2Rj0Xn8">Check two integers to be within rounding tolerance.</a>
    public static bool AlmostEqual(int a, int b, int minimumChange) => Math.Abs(a - b) < minimumChange;

    /// <a href="http://bit.ly/2Rj0Xn8">Check two integers to be within rounding tolerance (1 each way)</a>
    public static bool AlmostEqual(int a, int b) => AlmostEqual(a, b, minimumChange: 2);

    /// <a href="http://bit.ly/2RfHPpT">Check a string to see if it only contains digits</a>
    public static bool IsDigitsOnly(string text) {
      if (text.Length == 0) return false;

      for (var i = 0; i < text.Length; i++) {
        if ((text[i] < '0') || (text[i] > '9')) return false;
      }

      return true;
    }
  }
}