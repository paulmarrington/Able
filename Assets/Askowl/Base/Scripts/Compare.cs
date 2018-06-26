using System;

namespace Askowl {
  /// <summary>
  /// Comparators not available elsewhere
  /// </summary>
  public static class Compare {
    /// <summary>
    /// Check two floating point numbers to be within rounding tolerance.
    /// </summary>
    public static bool AlmostEqual(float a, float b, float minimumChange = 1e-5f) {
      return Math.Abs(a - b) < minimumChange;
    }

    /// <summary>
    /// Check two double floating point numbers to be within rounding tolerance.
    /// </summary>
    public static bool AlmostEqual(double a, double b, double minimumChange = 1e-5) {
      return Math.Abs(a - b) < minimumChange;
    }
  }
}