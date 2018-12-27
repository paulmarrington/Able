// With thanks to Jason Weimann  -- jason@unity3d.college

using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Askowl {
  /// <a href="http://bit.ly/2OuGzAF">Simple class to represent the high and low bounds for a float. It includes a picker to randomly choose a number within that range</a> <inheritdoc />
  [Serializable] public class Range : Pick<float> {
    [SerializeField] private float min;
    [SerializeField] private float max;

    /// <a href="http://bit.ly/2OuGzAF">Lowest value a number can have in this range</a>
    public float Min { get => min; set => min = value; }

    /// <a href="http://bit.ly/2OuGzAF">Highest value a number can have in this range</a>
    public float Max { get => max; set => max = value; }

    /// <a href="http://bit.ly/2OuGzAF">Default constructor used when the range is set in a MonoBehaviour in the Unity Editor</a>
    public Range() { }

    /// <a href="http://bit.ly/2OuGzAF">Constructor used to set the range directly or as initialisation for MonoBehaviour data</a>
    public Range(float min, float max) {
      Min = min;
      Max = max;
    }

    /// <a href="http://bit.ly/2OuGzAF">Choose a random number within the inclusive range</a> <inheritdoc />
    public float Pick() => Random.Range(min, max);
  }

  /// <a href="http://bit.ly/2NTC1o1">Used for `Range` variable in the Unity Editor to set the maximum bounds they can be set to</a>
  public class RangeBoundsAttribute : Attribute {
    /// <a href="http://bit.ly/2NTC1o1">[SerializeField, RangeBounds(0, 999)] private Range distance = new Range(1, 999);</a>
    public RangeBoundsAttribute(float min, float max) {
      Min = min;
      Max = max;
    }

    /// <a href="http://bit.ly/2NTC1o1">Used by RangeDrawer exclusively</a>
    public float Max { get; }

    /// <a href="http://bit.ly/2NTC1o1">Used by RangeDrawer exclusively</a>
    public float Min { get; }
  }
}