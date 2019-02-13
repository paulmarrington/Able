using System;
using NUnit.Framework;
#if AskowlTests
namespace Askowl.Able.Examples {
  public class ClockExamples {
    [Test] public void EpochTime() {
      DateTime dateTimeNow  = DateTime.Now;
      double   epochTimeNow = Clock.EpochTimeNow;
      double   epochTime    = Clock.EpochTimeAt(dateTimeNow);
      Assert.AreEqual(epochTime, epochTimeNow, 1e3f);

      DateTime later          = dateTimeNow.AddDays(1);
      double   epochTimeLater = Clock.EpochTimeAt(later);
      Assert.AreEqual(24 * 60 * 60, epochTimeLater - epochTimeNow, 1e3f);

      var diff = later.Subtract(Clock.FromEpochTime(epochTimeLater));
      Assert.AreEqual(diff.TotalSeconds, 0, 1e3f);
    }
  }
}
#endif