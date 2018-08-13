// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

using UnityEngine;

namespace Askowl {
  /// <summary>
  /// An **exponential moving average** is a way to calculate an average where
  /// older values have less impact on the average than more recent ones.
  /// </summary> //#TBD#
  /// <remarks><a href="http://unitydoc.marrington.net/Able#exponentialmovingaveragecs">exponential moving average</a></remarks>
  public class ExponentialMovingAverage {
    private float alpha;
    private float lastValue = float.NaN;

    /// <summary>
    /// Initialise with a lookback parameter - being how many points will effect
    /// the current value.
    /// </summary>
    /// <param name="lookback">lookback count - defaults to 8</param>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#ema-initialisation">EMA Initialisation</a></remarks>
    public ExponentialMovingAverage(int lookback = 8) { alpha = 2.0f / (lookback + 1); }

    /// <summary>
    /// Method with side-effects. Provide the next sample in the stream and get
    /// give back the exponential moving average.
    /// </summary>
    /// <param name="value"> Next value in the sequence</param>
    /// <returns>current exponential moving average</returns>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#ema-average-value">EMA Average Value</a></remarks>
    public float Average(float value) {
      return lastValue = float.IsNaN(lastValue) ? value : (value - lastValue) * alpha + lastValue;
    }

    /// <summary>
    /// Method with side-effects. Provide the next sample in the stream of angles and get
    /// give back the exponential moving average in degrees. Result will be between
    /// -180 and 180.
    /// </summary>
    /// <param name="degrees">angle to add as the next value in the sequence</param>
    /// <returns>current exponential moving average adjusted to be -180 to 180</returns>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#ema-average-angle">EMA Average Angle</a></remarks>
    public float AverageAngle(float degrees) {
      float difference                 = Mathf.Repeat(degrees - lastValue, 360);
      if (difference > 180) difference -= 360;
      return lastValue = float.IsNaN(lastValue) ? degrees : difference * alpha + lastValue;
    }
  }
}