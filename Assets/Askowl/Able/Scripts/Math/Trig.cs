// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

using System;
using UnityEngine;

namespace Askowl {
  /// <a href="http://bit.ly/2Oq9tC2">Trigonometry is about the lengths and angles of triangles.</a>
  public static class Trig {
    /// <a href="http://bit.ly/2Rinqkc">For our spatial work we also need to know which side and the direction of travel</a>
    public class Direction {
      /// <a href="http://bit.ly/2OpRewA">Ordinal or order where x=0, y=1 and z=2. Useful to match to Vector2/3</a>
      public readonly int Ord;

      /// <a href="http://bit.ly/2Rh3aPQ">One of the three will be set to 1 for that axis or -1 for reverse</a>
      public readonly int X, Y, Z;

      /// <a href="http://bit.ly/2NUH7Ag">Name as a character - great for switch statements.</a>
      public readonly char Name;

      /// <a href="http://bit.ly/2Oq9sOu">True if negative direction along the axis</a>
      public readonly bool Negative;

      /// <a href="http://bit.ly/2Rj0QIe">Vector3 equivalent of this direction</a>
      public readonly Vector3 Vector;

      /// <a href="http://bit.ly/2NUH6wc">Up, down, left, right, forward, back</a>
      public readonly string VectorName;

      /// <a href="http://bit.ly/2NYnwPC">Array of the two other axes not set</a>
      public Direction[] OtherAxes => otherAxes[negOrd, Ord];

      /// <inheritdoc />
      public override string ToString() => toString;

      internal Direction(char name, bool neg, int ord, int x = 0, int y = 0, int z = 0) {
        Ord      = ord;
        X        = x;
        Y        = y;
        Z        = z;
        list     = new[] { X, Y, Z };
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

      private static readonly string[,] vectorNames = { { "left", "down", "back" }, { "right", "up", "forward" } };

      /// <a href="http://bit.ly/2Rinqkc">For negative axis directions</a>
      public static Direction operator -(Direction d) => directions[d.negOrd, d.Ord];

      /// <a href="http://bit.ly/2Oq9tC2">Access XYZ by index (or ordinal)</a>
      public int this[int i] => list[i];
    }

    private static readonly Direction[,] directions = {
      {
        new Direction('X', neg: false, ord: 0, x: 1), new Direction('Y', neg: false, ord: 1, y: 1)
      , new Direction('Z', neg: false, ord: 2, z: 1)
      }
    , {
        new Direction('X', neg: true, ord: 0, x: -1), new Direction('Y', neg: true, ord: 1, y: -1)
      , new Direction('Z', neg: true, ord: 2, z: -1)
      }
    };

    /// <a href="http://bit.ly/2OpRf3C">Define axes</a>
    public static readonly Direction XAxis = directions[0, 0];

    /// <a href="http://bit.ly/2OpRf3C">Define axes</a>
    public static readonly Direction YAxis = directions[0, 1];

    /// <a href="http://bit.ly/2OpRf3C">Define axes</a>
    public static readonly Direction ZAxis = directions[0, 2];

    private static Direction[,][] otherAxes = {
      { new[] { -YAxis, -ZAxis }, new[] { -XAxis, -ZAxis }, new[] { -XAxis, -YAxis } }
    , { new[] { YAxis, ZAxis }, new[] { XAxis, ZAxis }, new[] { XAxis, YAxis } }
    };

    private const double radiansToDegrees = 180.0   / Math.PI;
    private const double degreesToRadians = Math.PI / 180.0;

    /// <a href="http://bit.ly/2Rj0RMi">Convert degrees to radians</a>
    public static double ToRadians(double degrees) => degrees * degreesToRadians;

    /// <a href="http://bit.ly/2Orh5nJ">Convert radians to degrees</a>
    public static double ToDegrees(double radians) => radians * radiansToDegrees;

    /// <a href="http://bit.ly/2OpFtGy">Find the relative coordinate given distance and bearing.</a>
    public static Vector2 RelativePositionFromBearing(float distanceApart, float bearingInRadians) {
      float x = distanceApart * Mathf.Sin(bearingInRadians);
      float y = distanceApart * Mathf.Cos(bearingInRadians);
      return new Vector2(x, y);
    }

    /// <a href="http://bit.ly/2Rj0RMi">Find the relative coordinate given distance and angle from X axis.</a>
    public static Vector2 RelativePositionFromAngle(float distanceApart, float bearingInRadians) {
      float x = distanceApart * Mathf.Cos(bearingInRadians);
      float y = distanceApart * Mathf.Sin(bearingInRadians);
      return new Vector2(x, y);
    }
  }
}