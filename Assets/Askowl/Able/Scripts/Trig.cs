using System;
using UnityEngine;

namespace Askowl {
  public class Trig : MonoBehaviour {
    public class Direction {
      internal int x, y, z;
    }

    public static readonly Direction xAxis = new Direction {x = 1};
    public static readonly Direction yAxis = new Direction {y = 1};
    public static readonly Direction zAxis = new Direction {z = 1};

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