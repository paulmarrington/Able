// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

using System;
using UnityEngine;

namespace Askowl {
  /// <inheritdoc />
  /// <summary>
  /// Trigonometry Support
  /// </summary>
  /// <remarks><a href="http://unitydoc.marrington.net/Able#trigcs">Trigonometry Rocks</a></remarks>
  public class Trig : MonoBehaviour {
    /// <remarks><a href="http://unitydoc.marrington.net/Able#direction">Define axes</a></remarks>
    public class Direction {
      /// <summary>
      /// Ordinal or order where x=0, y=1 and z=2. Useful to match to Vector2/3
      /// </summary>
      public readonly int Ord;

      /// <summary>
      /// One of the three will be set to 1 for that axis or -1 for reverse
      /// </summary>
      public readonly int X, Y, Z;

      /// <summary>
      /// Name as a character - great for switch statements.
      /// </summary>
      public readonly char Name;

      /// <summary>
      /// True if negative direction along the axis
      /// </summary>
      public readonly bool Negative;

      /// <summary>
      /// Vector3 equivalent of this direction
      /// </summary>
      public readonly Vector3 Vector;

      /// <summary>
      /// Up, down, left, right, forward, back
      /// </summary>
      public readonly string VectorName;

      /// <summary>
      /// Array of the two other axes not set
      /// </summary>
      public Direction[] OtherAxes => otherAxes[negOrd, Ord];

      /// <inheritdoc />
      public override string ToString() => toString;

      internal Direction(char name, bool neg, int ord, int x = 0, int y = 0, int z = 0) {
        Ord      = ord;
        X        = x;
        Y        = y;
        Z        = z;
        list     = new[] {X, Y, Z};
        Name     = name;
        Negative = neg;
        negOrd   = neg ? 0 : 1;
        string minus = neg ? "-" : "";
        toString    = $"{minus}{Name} Axis";
        Vector      = Vector3.zero;
        Vector[ord] = neg ? -1f : 1f;
        VectorName  = vectorNames[negOrd, ord];
      }

      private readonly string toString;
      private readonly int    negOrd;
      private          int[]  list;

      private static readonly string[,] vectorNames = {{"left", "down", "back"}, {"right", "up", "forward"}};

      /// <summary>
      /// For negative axis directions
      /// </summary>
      /// <param name="d">xAxis, yAxis or zAxis</param>
      public static Direction operator -(Direction d) => directions[d.negOrd, d.Ord];

      /// <summary>
      /// Access XYZ by index (or ordinal)
      /// </summary>
      /// <param name="i">index</param>
      public int this[int i] => list[i];
    }

    private static readonly Direction[,] directions = {
      {
        new Direction('X', neg: false, ord: 0, x: 1),
        new Direction('Y', neg: false, ord: 1, y: 1),
        new Direction('Z', neg: false, ord: 2, z: 1)
      }, {
        new Direction('X', neg: true, ord: 0, x: -1),
        new Direction('Y', neg: true, ord: 1, y: -1),
        new Direction('Z', neg: true, ord: 2, z: -1)
      }
    };

    /// <remarks><a href="http://unitydoc.marrington.net/Able#direction">Define axes</a></remarks>
    public static readonly Direction xAxis = directions[0, 0];

    /// <remarks><a href="http://unitydoc.marrington.net/Able#direction">Define axes</a></remarks>
    public static readonly Direction yAxis = directions[0, 1];

    /// <remarks><a href="http://unitydoc.marrington.net/Able#direction">Define axes</a></remarks>
    public static readonly Direction zAxis = directions[0, 2];

    private static Direction[,][] otherAxes = {
      {new[] {-yAxis, -zAxis}, new[] {-xAxis, -zAxis}, new[] {-xAxis, -yAxis}},
      {new[] {yAxis, zAxis}, new[] {xAxis, zAxis}, new[] {xAxis, yAxis}}
    };

    private const double RadiansToDegrees = (180.0   / Math.PI);
    private const double DegreesToRadians = (Math.PI / 180.0);

    /// <summary>
    /// Convert degrees to radians
    /// </summary>
    /// <param name="degrees">decimal degrees</param>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#toradians">Degrees to Radians</a></remarks>
    public static double ToRadians(double degrees) => degrees * DegreesToRadians;

    /// <summary>
    /// Convert radians to degrees
    /// </summary>
    /// <param name="radians"></param>
    /// <returns>decimal degrees</returns>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#todegrees">Radians to Degrees</a></remarks>
    public static double ToDegrees(double radians) => radians * RadiansToDegrees;

    /// <summary>
    /// Find the relative coordinate given distance and bearing.
    /// </summary>
    /// <param name="distanceApart">in whatever units you need</param>
    /// <param name="bearingInRadians">Where 0 degrees is north (+Y), 90 degrees is East (+X), clockwise</param>
    /// <returns>Vector2</returns>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#relative-position-given-distance-and-angle-or-bearing">Relative Position given Distance and Angle or Bearing</a></remarks>
    public static Vector2 RelativePositionFromBearing(float distanceApart, float bearingInRadians) {
      var x = distanceApart * Mathf.Sin(bearingInRadians);
      var y = distanceApart * Mathf.Cos(bearingInRadians);
      return new Vector2(x, y);
    }

    /// <summary>
    /// Find the relative coordinate given distance and angle from X axis.
    /// </summary>
    /// <param name="distanceApart">in whatever units you need</param>
    /// <param name="bearingInRadians">Where 0 degrees is +X, 90 degrees is +Y, counterclockwise</param>
    /// <returns>Vector2</returns>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#relative-position-given-distance-and-angle-or-bearing">Relative Position given Distance and Angle or Bearing</a></remarks>
    public static Vector2 RelativePositionFromAngle(float distanceApart, float bearingInRadians) {
      var x = distanceApart * Mathf.Cos(bearingInRadians);
      var y = distanceApart * Mathf.Sin(bearingInRadians);
      return new Vector2(x, y);
    }
  }
}