using System;
using UnityEngine;

namespace Askowl {
  public class Trig : MonoBehaviour {
    private const double RadiansToDegrees = (180.0   / Math.PI);
    private const double DegreesToRadians = (Math.PI / 180.0);

    public static double ToRadians(double degrees) => degrees * DegreesToRadians;

    public static double ToDegrees(double radians) => radians * RadiansToDegrees;
  }
}