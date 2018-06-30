using System;

public class ExponentialMovingAverage {
  /// Decay parameter (0 &lt; alpha &lt; 1);
  public double alpha = 0.2;

  private bool   hasLastValue;
  private double lastValue;

  public double Average(double value) {
    if (!hasLastValue) {
      hasLastValue = true;
      return lastValue = value;
    }

    double emaValue = lastValue + alpha * (value - lastValue);
    lastValue = emaValue;
    return emaValue;
  }
}