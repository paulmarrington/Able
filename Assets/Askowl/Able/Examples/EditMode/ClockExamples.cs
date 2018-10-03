#if UNITY_EDITOR && Able
using System;
using NUnit.Framework;

namespace Askowl.Examples {
  public class ClockExamples {
    [Test] public void EpochTime() {
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