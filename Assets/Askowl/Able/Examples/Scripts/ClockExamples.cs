#if UNITY_EDITOR && Able

using System;
using NUnit.Framework;

namespace Askowl.Examples {
  /// <a href="http://unitydoc.marrington.net/Able#clock">Static time helpers</a>
  public class ClockExamples {
    /// <a href="http://unitydoc.marrington.net/Able#epoch-time">Convert to and from epoch time</a>
    [Test]
    public void EpochTime() {
      DateTime now          = DateTime.Now;
      double   epochTimeNow = Clock.EpochTimeNow;
      double   epochTime    = Clock.EpochTimeAt(now);
      AssertAlmostEqual(epochTime, epochTimeNow);

      DateTime later          = now.AddDays(1);
      double   epochTimeLater = Clock.EpochTimeAt(later);
      AssertAlmostEqual(24 * 60 * 60, epochTimeLater - epochTimeNow);

      var diff = later.Subtract(Clock.FromEpochTime(epochTimeLater));
      AssertAlmostEqual(diff.TotalSeconds, 0);
    }

    private void AssertAlmostEqual(double a, double b) { Assert.IsTrue(Compare.AlmostEqual(a, b, 0.1)); }
  }
}
#endif