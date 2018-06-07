// With thanks to Jason Weimann  -- jason@unity3d.college

using System;
using JetBrains.Annotations;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Askowl {
  /// <inheritdoc />
  /// <summary>
  /// Simple class to represent the high and low bounds for a float. It includes a picker to randomly choose a number within that range
  /// </summary>
  /// <remarks><a href="http://customassets.marrington.net#range">More...</a></remarks>
  [Serializable]
  public class Range : Pick<float> {
    [SerializeField, UsedImplicitly] private float min;
    [SerializeField, UsedImplicitly] private float max;

    /// <summary>
    /// Lowest value a number can have in this range
    /// </summary>
    /// <remarks><a href="http://customassets.marrington.net#range">More...</a></remarks>
    public float Min { get { return min; } private set { min = value; } }

    /// <summary>
    /// Highest value a number can have in this range
    /// </summary>
    /// <remarks><a href="http://customassets.marrington.net#range">More...</a></remarks>
    public float Max { get { return max; } private set { max = value; } }

    /// <summary>
    /// Default constructor used when the range is set in a MonoBehaviour in the Unity Editor
    /// </summary>
    /// <remarks><a href="http://customassets.marrington.net#range">More...</a></remarks>
    public Range() { }

    /// <summary>
    /// Constructor used to set the range directly or as initialisation for MonoBehaviour data
    /// </summary>
    /// <param name="min"><see cref="Min"/></param>
    /// <param name="max"><see cref="Max"/></param>
    /// <remarks><a href="http://customassets.marrington.net#range">More...</a></remarks>
    public Range(float min, float max) {
      Min = min;
      Max = max;
    }

    /// <inheritdoc />
    /// <summary>
    /// Choose a random number within the inclusive range
    /// </summary>
    /// <returns>A pseudo-random number</returns>
    /// <remarks><a href="http://customassets.marrington.net#range">More...</a></remarks>
    public float Pick() { return Random.Range(min, max); }
  }

  /// <summary>
  /// Used for <see cref="Range"/> variable in the Unity Editor to set the maximum bounds they can be set to
  /// </summary>
  /// <remarks><a href="http://customassets.marrington.net#rangebounds-attribute">More...</a></remarks>
  public class RangeBoundsAttribute : Attribute {
    /// <summary>
    /// Setting the bounds
    /// </summary>
    /// <remarks><a href="http://customassets.marrington.net#rangebounds-attribute">More...</a></remarks>
    /// <code>[SerializeField, RangeBounds(0, 999)] private Range distance = new Range(1, 999);</code>
    /// <param name="min">Minimum value the range slider will move down to</param>
    /// <param name="max">Maximum value the range slider will move up to</param>
    public RangeBoundsAttribute(float min, float max) {
      Min = min;
      Max = max;
    }

    /// <summary>
    /// Used by RangeDrawer exclusively
    /// </summary>
    public float Max { get; private set; }

    /// <summary>
    /// Used by RangeDrawer exclusively
    /// </summary>
    public float Min { get; private set; }
  }
}