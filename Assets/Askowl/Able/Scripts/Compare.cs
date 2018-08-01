using System;

namespace Askowl {
  /// <summary>
  /// Comparators not available elsewhere
  /// </summary>
  public static class Compare {
    /// <summary>
    /// Check two floating point numbers to be within rounding tolerance.
    /// </summary>
    public static bool AlmostEqual(float a, float b, float minimumChange) {
      return Math.Abs(a - b) < minimumChange;
    }

    /// <summary>
    /// Check two double floating point numbers to be within rounding tolerance.
    /// </summary>
    public static bool AlmostEqual(double a, double b, double minimumChange) {
      return Math.Abs(a - b) < minimumChange;
    }

    /// <summary>
    /// Check two floating point numbers to be within rounding tolerance.
    /// </summary>
    public static bool AlmostEqual(float a, float b) { return AlmostEqual(a, b, 1e-5f); }

    /// <summary>
    /// Check two double floating point numbers to be within rounding tolerance.
    /// </summary>
    public static bool AlmostEqual(double a, double b) { return AlmostEqual(a, b, 1e-5); }
  }
}