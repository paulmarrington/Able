using System;

namespace Askowl {
  /// <summary>
  /// Comparators not available elsewhere
  /// </summary>
  public static class Compare {
    /// <summary>
    /// Check two floating point numbers to be within rounding tolerance.
    /// </summary>
    public static bool AlmostEqual(float a, float b) { return Math.Abs(a - b) < 1e-5; }

    /// <summary>
    /// Check two double floating point numbers to be within rounding tolerance.
    /// </summary>
    public static bool AlmostEqual(double a, double b) { return Math.Abs(a - b) < 1e-5; }
  }
}