using System;

namespace Askowl {
  /// <summary>
  /// Comparators not available elsewhere
  /// </summary> //#TBD#
  /// <remarks><a href="http://unitydoc.marrington.net/Able#comparecs-equality-and-almost-equality">check if two numbers are almost the same</a></remarks>
  public static class Compare {
    /// <summary>
    /// Check two floating point numbers to be within rounding tolerance.
    /// </summary>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#almostequal-for-floating-point">check if two numbers are almost the same</a></remarks>
    public static bool AlmostEqual(float a, float b, float minimumChange) => Math.Abs(a - b) < minimumChange;

    /// <summary>
    /// Check two floating point numbers to be within rounding tolerance (0.00001)
    /// </summary>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#almostequal-for-floating-point">check if two numbers are almost the same</a></remarks>
    public static bool AlmostEqual(float a, float b) => AlmostEqual(a, b, minimumChange: 1e-3f);

    /// <summary>
    /// Check two double floating point numbers to be within rounding tolerance.
    /// </summary>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#almostequal-for-floating-point">check if two numbers are almost the same</a></remarks>
    public static bool AlmostEqual(double a, double b, double minimumChange) => Math.Abs(a - b) < minimumChange;

    /// <summary>
    /// Check two double floating point numbers to be within rounding tolerance (0.00001)
    /// </summary>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#almostequal-for-floating-point">check if two numbers are almost the same</a></remarks>
    public static bool AlmostEqual(double a, double b) => AlmostEqual(a, b, minimumChange: 1e-5);

    /// <summary>
    /// Check two long integers to be within rounding tolerance.
    /// </summary>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#almostequal-for-integers">check if two numbers are almost the same</a></remarks>
    public static bool AlmostEqual(long a, long b, long minimumChange) => Math.Abs(a - b) < minimumChange;

    /// <summary>
    /// Check two long integers to be within rounding tolerance (1 each way)
    /// </summary>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#almostequal-for-integers">check if two numbers are almost the same</a></remarks>
    public static bool AlmostEqual(long a, long b) => AlmostEqual(a, b, minimumChange: 2);

    /// <summary>
    /// Check two integers to be within rounding tolerance.
    /// </summary>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#almostequal-for-integers">check if two numbers are almost the same</a></remarks>
    public static bool AlmostEqual(int a, int b, int minimumChange) => Math.Abs(a - b) < minimumChange;

    /// <summary>
    /// Check two integers to be within rounding tolerance (1 each way)
    /// </summary>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#almostequal-for-integers">check if two numbers are almost the same</a></remarks>
    public static bool AlmostEqual(int a, int b) => AlmostEqual(a, b, minimumChange: 2);
  }
}