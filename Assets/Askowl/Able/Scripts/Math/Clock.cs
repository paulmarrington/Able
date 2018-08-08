using System;

namespace Askowl {
  /// <summary>
  /// Static helper class to move between time systems.
  /// </summary>
  /// <a href="http://unitydoc.marrington.net/Able#clockcs-">Time manipulation</a> //#TBD#
  public static class Clock {
    private static DateTime EpochStart =
      new DateTime(year: 1970, month: 1, day: 1, hour: 0, minute: 0, second: 0, kind: DateTimeKind.Utc);

    /// <summary>
    /// Retrieve epoch time at the time of this call.
    /// </summary>
    /// <a href="http://unitydoc.marrington.net/Able#epoch-time">Convert to epoch time</a>
    public static double EpochTimeNow => EpochTimeAt(DateTime.UtcNow);

    /// <summary>
    /// Convert from C# native time representation to epoch time. Epoch time is
    /// the number of seconds since the start of 1970 in Greenwich.
    /// </summary>
    /// <param name="when">Time object including location</param>
    /// <returns>Epoch time equivalent</returns>
    /// <a href="http://unitydoc.marrington.net/Able#epoch-time">Convert to epoch time</a>
    public static double EpochTimeAt(DateTime when) => (when.ToUniversalTime() - EpochStart).TotalSeconds;

    /// <summary>
    /// Given a value in epoch time UTC, retrieve the local time
    /// </summary>
    /// <param name="epochTime">Number of seconds since 1/1/1970 UTC</param>
    /// <returns>Local time</returns>
    /// <a href="http://unitydoc.marrington.net/Able#epoch-time">Convert from epoch time</a>
    public static DateTime FromEpochTime(double epochTime) => EpochStart.AddSeconds(epochTime).ToLocalTime();
  }
}