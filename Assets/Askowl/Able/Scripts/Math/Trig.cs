using System;
using UnityEngine;

namespace Askowl {
  public class Trig : MonoBehaviour {
    /// <remarks><a href="http://unitydoc.marrington.net/Able#direction">Define axes</a></remarks>
    public class Direction {
      public readonly int    Ord;
      public readonly int    X, Y, Z;
      public readonly char   Name;
      public readonly bool   Negative;
      public readonly Vector2 vector2;
      public readonly Vector3 vector3;
      public override string ToString() => $"{minus}{Name}-Axis";

      internal Direction(char name, bool neg, int ord, int x = 0, int y = 0, int z = 0) {
        Ord      = ord;
        X        = x;
        Y        = y;
        Z        = z;
        Name     = name;
        Negative = neg;
        minus    = neg ? "-" : "";
        vector2 = Vector2.zero;
        vector3 = Vector3.zero;
        vector2[ord] = vector3[ord] = 1f;
      }

      private readonly string minus;

      /// <summary>
      /// For negative axis directions
      /// </summary>
      /// <param name="d">xAxis, yAxis or zAxis</param>
      /// <returns></returns>
      public static Direction operator -(Direction d) => d.Negative ? positives[d.Ord] : negatives[d.Ord];
    }

    private static Direction[] positives = {
      new Direction('X', neg: false, ord: 0, x: 1),
      new Direction('Y', neg: false, ord: 1, y: 1),
      new Direction('Z', neg: false, ord: 2, z: 1)
    };

    private static Direction[] negatives = {
      new Direction('X', neg: true, ord: 0, x: -1),
      new Direction('Y', neg: true, ord: 1, y: -1),
      new Direction('Z', neg: true, ord: 2, z: -1)
    };

    /// <remarks><a href="http://unitydoc.marrington.net/Able#direction">Define axes</a></remarks>
    public static readonly Direction xAxis = positives[0];

    /// <remarks><a href="http://unitydoc.marrington.net/Able#direction">Define axes</a></remarks>
    public static readonly Direction yAxis = positives[1];

    /// <remarks><a href="http://unitydoc.marrington.net/Able#direction">Define axes</a></remarks>
    public static readonly Direction zAxis = positives[2];

    private const double RadiansToDegrees = (180.0   / Math.PI);
    private const double DegreesToRadians = (Math.PI / 180.0);

    public static double ToRadians(double degrees) => degrees * DegreesToRadians;

    public static double ToDegrees(double radians) => radians * RadiansToDegrees;

    public static Vector2 Relative(float distanceApart, float bearing) {
      var x = distanceApart * Mathf.Sin(bearing);

      var y = distanceApart * Mathf.Cos(bearing);
      return new Vector2(x, y);
    }
  }
}