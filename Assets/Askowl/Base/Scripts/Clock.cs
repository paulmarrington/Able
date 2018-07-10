using System;

namespace Askowl {
  public class Clock {
    public static DateTime EpochStart = new DateTime(year: 1970, month: 1, day: 1);

    public static double EpochTimeNow { get { return EpochTimeAt(DateTime.UtcNow); } }

    public static double EpochTimeAt(DateTime when) { return (when - EpochStart).TotalSeconds; }
  }
}