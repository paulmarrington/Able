public class ExponentialMovingAverage {
  private float alpha;
  private float lastValue = float.NaN;

  public ExponentialMovingAverage(int lookback = 8) { alpha = 2.0f / (lookback + 1); }

  public float Average(float value) {
    return lastValue = float.IsNaN(lastValue) ? value : (value - lastValue) * alpha + lastValue;
  }
}