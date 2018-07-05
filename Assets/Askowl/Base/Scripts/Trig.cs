using System;
using UnityEngine;

namespace Askowl {
  public class Trig : MonoBehaviour {
    public const double RadiansToDegrees = (180.0   / Math.PI);
    public const double DegreesToRadians = (Math.PI / 180.0);

    public static double ToRadians(double degrees) { return degrees * DegreesToRadians; }

    public static double ToDegrees(double radians) { return radians * RadiansToDegrees; }
  }
}