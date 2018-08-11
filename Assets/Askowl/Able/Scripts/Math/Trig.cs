using System;
using UnityEngine;

namespace Askowl {
  public class Trig : MonoBehaviour {
    public class Direction {
      public readonly int    Ord;
      public readonly int    X, Y, Z;
      public readonly string Name;
      public override string ToString() => $"{Name}-Axis";

      internal Direction(string name, int ord, int x = 0, int y = 0, int z = 0) {
        Ord  = ord;
        X    = x;
        Y    = y;
        Z    = z;
        Name = name;
      }
    }

    public static readonly Direction xAxis = new Direction("X", ord: 0, x: 1);
    public static readonly Direction yAxis = new Direction("Y", ord: 1, y: 1);
    public static readonly Direction zAxis = new Direction("Z", ord: 2, z: 1);

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