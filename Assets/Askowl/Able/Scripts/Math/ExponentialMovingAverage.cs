// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

using UnityEngine;

namespace Askowl {
  /// <a href="http://bit.ly/2NTA3Ed">An **exponential moving average** is a way to calculate an average where older values have less impact on the average than more recent ones.</a>
  public class ExponentialMovingAverage {
    private readonly float alpha;
    private          float average = float.NaN;

    /// <a href="http://bit.ly/2OpRz2k">Initialise with a look-back parameter - being how many points will effect the current value.</a>
    public ExponentialMovingAverage(int lookBack = 9) => alpha = 2.0f / (lookBack + 1);

    /// <a href="http://bit.ly/2Rj0UHY">Method with side-effects. Provide the next sample in the stream and get give back the exponential moving average.</a>
    public float Average(float value) => average = float.IsNaN(average) ? value : ((value - average) * alpha) + average;

    /// <a href="http://bit.ly/2OxNV6I">Method with side-effects. Provide the next sample in the stream of angles and get give back the exponential moving average in degrees. Result will be between -180 and 180.</a>
    public float AverageAngle(float degrees) {
      float difference                 = Mathf.Repeat(degrees - average, 360);
      if (difference > 180) difference -= 360;
      return average = float.IsNaN(average) ? degrees : (difference * alpha) + average;
    }
  }
}