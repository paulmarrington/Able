using UnityEngine;

public class ExponentialMovingAverage {
  private float alpha;
  private float lastValue = float.NaN;

  public ExponentialMovingAverage(int lookback = 8) { alpha = 2.0f / (lookback + 1); }

  public float Average(float value) {
    return lastValue = float.IsNaN(lastValue) ? value : (value - lastValue) * alpha + lastValue;
  }

  public float AverageAngle(float angle) {
    float difference                 = Mathf.Repeat(angle - lastValue, 360);
    if (difference > 180) difference -= 360;
    return lastValue = float.IsNaN(lastValue) ? angle : difference * alpha + lastValue;
  }
}