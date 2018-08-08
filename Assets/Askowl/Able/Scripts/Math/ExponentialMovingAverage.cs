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
    ///
    /// </summary>
    /// <param name="lookback"></param>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#ema-initialisation">EMA Initialisation</a></remarks>
    public ExponentialMovingAverage(int lookback = 8) { alpha = 2.0f / (lookback + 1); }

    /// <summary>
    ///
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#ema-average-value">EMA Average Value</a></remarks>
    public float Average(float value) {
      return lastValue = float.IsNaN(lastValue) ? value : (value - lastValue) * alpha + lastValue;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="angle"></param>
    /// <returns></returns>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#ema-average-angle">EMA Average Angle</a></remarks>
    public float AverageAngle(float angle) {
      float difference                 = Mathf.Repeat(angle - lastValue, 360);
      if (difference > 180) difference -= 360;
      return lastValue = float.IsNaN(lastValue) ? angle : difference * alpha + lastValue;
    }
  }
}