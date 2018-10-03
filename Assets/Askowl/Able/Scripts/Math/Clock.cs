// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

namespace Askowl {
  using System;

  /// <a href="http://bit.ly/2Rj0RMi">Static helper class to move between time systems.</a>
  public static class Clock {
    private static DateTime EpochStart =
      new DateTime(year: 1970, month: 1, day: 1, hour: 0, minute: 0, second: 0, kind: DateTimeKind.Utc);

    /// <a href="http://bit.ly/2NWvKYI">Retrieve epoch time at the time of this call.</a>
    public static double EpochTimeNow => EpochTimeAt(DateTime.UtcNow);

    /// <a href="http://bit.ly/2NYOFSy">Convert from C# native time representation to epoch time. Epoch time is the number of seconds since the start of 1970 in Greenwich.</a>
    public static double EpochTimeAt(DateTime when) => (when.ToUniversalTime() - EpochStart).TotalSeconds;

    /// <a href="http://bit.ly/2OpmeNj">Given a value in epoch time UTC, retrieve the local time</a>
    public static DateTime FromEpochTime(double epochTime) => EpochStart.AddSeconds(epochTime).ToLocalTime();
  }
}