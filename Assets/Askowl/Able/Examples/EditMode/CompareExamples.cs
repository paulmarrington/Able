using NUnit.Framework;
#if !ExcludeAskowlTests

namespace Askowl.Able.Examples {
  public class CompareExamples {
    [Test] public void AlmostEqualFloatingPoint() {
      Assert.IsFalse(Compare.AlmostEqual(a: 1.1f, b: 1.2f, minimumChange: 0.1f));
      Assert.IsTrue(Compare.AlmostEqual(a: 1.1f,  b: 1.2f, minimumChange: 0.11f));

      Assert.IsFalse(Compare.AlmostEqual(a: 1.1f, b: 1.11f));
      Assert.IsTrue(Compare.AlmostEqual(a: 1.1f,  b: 1.0999f));

      Assert.IsFalse(Compare.AlmostEqual(a: 103.11, b: 104, minimumChange: 0.5));
      Assert.IsTrue(Compare.AlmostEqual(a: 103.11,  b: 104, minimumChange: 0.9));

      Assert.IsFalse(Compare.AlmostEqual(a: 123.45678, b: 123.45679));
      Assert.IsTrue(Compare.AlmostEqual(a: 123.456789, b: 123.45679));
    }

    [Test] public void AlmostEqualInteger() {
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

    public void IsDigitsOnly() {
      Assert.IsTrue(Compare.IsDigitsOnly("123987654"));

      Assert.IsFalse(Compare.IsDigitsOnly("12no shoe"));
      Assert.IsFalse(Compare.IsDigitsOnly("1.4"));
      Assert.IsFalse(Compare.IsDigitsOnly("-66"));
      Assert.IsFalse(Compare.IsDigitsOnly(""));
    }
  }
}
#endif