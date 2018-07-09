using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Askowl {
  /// Geodesy: The branch of mathematics dealing with the shape and area of the earth or large portions of it.
  /// Origin: late 16th century: from modern Latin geodaesia, from Greek geōdaisia, from gē ‘earth’ + daiein ‘divide’.
  /// https://en.wikipedia.org/wiki/Geodesy  https://www.movable-type.co.uk/scripts/latlong.html
  public static class Geodetic {
    public static float EarthMeanRadiusMetres = 6371e3f;

    public struct Coordinates {
      public double Latitude, Longitude;
      public bool   radians;

      public void ToRadians() {
        if (radians) return;

        Latitude  = Trig.ToRadians(Latitude);
        Longitude = Trig.ToRadians(Longitude);
        radians   = true;
      }

      public void ToDegrees() {
        if (!radians) return;

        Latitude  = Trig.ToDegrees(Latitude);
        Longitude = Trig.ToDegrees(Longitude);
        radians   = false;
      }

      public override string ToString() {
        return String.Format("({0:n5}, {1:n5})", Latitude, Longitude);
      }
    }

    public static Coordinates Coords(double latitude, double longitude, bool radians = false) {
      Coordinates coordinates;
      coordinates.Latitude  = latitude;
      coordinates.Longitude = longitude;
      coordinates.radians   = radians;
      coordinates.ToDegrees();
      return coordinates;
    }

    public static double Kilometres(Coordinates first, Coordinates second) {
      return Haversine(first, second);
    }

    public static string DistanceBetween(Coordinates first, Coordinates second) {
      double km = Kilometres(first, second);
      if (km < 1) return string.Format("{0} m", (int) (km * 1000));

      return string.Format((km > 10) ? "{0:n0} km" : "{0:n1} km", km);
    }

    public static double Haversine(Coordinates first, Coordinates second) {
      var deltaLatitude  = Trig.ToRadians(second.Latitude  - first.Latitude);
      var deltaLongitude = Trig.ToRadians(second.Longitude - first.Longitude);

      var sinDeltaLatitude  = Math.Sin(deltaLatitude  / 2);
      var sinDeltaLongitude = Math.Sin(deltaLongitude / 2);

      var a = (sinDeltaLatitude * sinDeltaLatitude) +
              Math.Cos(Trig.ToRadians(first.Latitude))  *
              Math.Cos(Trig.ToRadians(second.Latitude)) *
              (sinDeltaLongitude * sinDeltaLongitude);

      return EarthMeanRadiusMetres * (2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a)));
    }

    public static double SphericalLawOfCosines(Coordinates first, Coordinates second) {
      var firstRadians  = Trig.ToRadians(first.Latitude);
      var secondRadians = Trig.ToRadians(second.Latitude);

      return Math.Acos(Math.Sin(firstRadians) * Math.Sin(secondRadians) +
                       Math.Cos(firstRadians) * Math.Cos(secondRadians) *
                       Trig.ToRadians(second.Longitude - second.Latitude)) * EarthMeanRadiusMetres;
    }

    public static double BearingDegrees(Coordinates from, Coordinates to) { // forward azimuth
      from.ToRadians();
      to.ToRadians();
      var y = Math.Sin(to.Longitude - from.Longitude) * Math.Cos(to.Latitude);

      var x = Math.Cos(from.Latitude) * Math.Sin(to.Latitude) -
              Math.Sin(from.Latitude) * Math.Cos(to.Latitude) *
              Math.Cos(to.Longitude - from.Longitude);

      return Trig.ToDegrees(Math.Atan2(y, x));
    }

    public static Coordinates Destination(Coordinates start, double distanceKm,
                                          double      bearingDegrees) {
      start.ToRadians();
      var deltaDistance    = distanceKm / EarthMeanRadiusMetres;
      var sinStartLatitude = Math.Sin(start.Latitude);
      var cosStartLatitude = Math.Cos(start.Latitude);
      var sinDeltaDistance = Math.Sin(deltaDistance);
      var cosDeltaDistance = Math.Cos(deltaDistance);

      var latitude = Math.Asin((sinStartLatitude * cosDeltaDistance) +
                               cosStartLatitude * sinDeltaDistance * Math.Cos(bearingDegrees));

      var longitude = start.Longitude + Math.Atan2(
                        Math.Sin(bearingDegrees) * sinDeltaDistance * cosStartLatitude,
                        cosDeltaDistance - sinStartLatitude * Math.Sin(latitude));

      return Coords(latitude, longitude, radians: true);
    }
  }
}