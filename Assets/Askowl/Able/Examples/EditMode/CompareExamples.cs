#if UNITY_EDITOR && Able

using NUnit.Framework;

namespace Askowl.Examples { //#TBD#
  /// <remarks><a href="http://unitydoc.marrington.net/Able#comparecs-equality-and-almost-equality">check if two numbers are almost the same</a></remarks>
  public class CompareExamples {
    /// <remarks><a href="http://unitydoc.marrington.net/Able#almostequal-for-floating-point">check if two numbers are almost the same</a></remarks>
    [Test]
    public void AlmostEqualFloatingPoint() {
      Assert.IsFalse(Compare.AlmostEqual(a: 1.1f, b: 1.2f, minimumChange: 0.1f));
      Assert.IsTrue(Compare.AlmostEqual(a: 1.1f,  b: 1.2f, minimumChange: 0.11f));

      Assert.IsFalse(Compare.AlmostEqual(a: 1.1f, b: 1.11f));
      Assert.IsTrue(Compare.AlmostEqual(a: 1.1f,  b: 1.0999f));

      Assert.IsFalse(Compare.AlmostEqual(a: 103.11, b: 104, minimumChange: 0.5));
      Assert.IsTrue(Compare.AlmostEqual(a: 103.11,  b: 104, minimumChange: 0.9));

      Assert.IsFalse(Compare.AlmostEqual(a: 123.45678, b: 123.45679));
      Assert.IsTrue(Compare.AlmostEqual(a: 123.456789, b: 123.45679));
    }

    /// <remarks><a href="http://unitydoc.marrington.net/Able#almostequal-for-integers">check if two numbers are almost the same</a></remarks>
    [Test]
    public void AlmostEqualInteger() {
      Assert.IsFalse(Compare.AlmostEqual(a: 123L, b: 133L, minimumChange: 10L));
      Assert.IsTrue(Compare.AlmostEqual(a: 123L,  b: 133L, minimumChange: 11L));

      Assert.IsFalse(Compare.AlmostEqual(a: 123L, b: 125L));
      Assert.IsFalse(Compare.AlmostEqual(a: 123L, b: 121L));
      Assert.IsTrue(Compare.AlmostEqual(a: 123L,  b: 124L));
      Assert.IsTrue(Compare.AlmostEqual(a: 123L,  b: 122L));

      Assert.IsFalse(Compare.AlmostEqual(a: 1, b: 4, minimumChange: 2));
      Assert.IsTrue(Compare.AlmostEqual(a: 1,  b: 3, minimumChange: 4));

      Assert.IsFalse(Compare.AlmostEqual(a: 1, b: 4));
      Assert.IsTrue(Compare.AlmostEqual(a: 1,  b: 2));
    }
  }
}
#endif