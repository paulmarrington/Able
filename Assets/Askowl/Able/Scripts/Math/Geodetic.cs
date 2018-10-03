// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

using System;

namespace Askowl {
  /// <a href="http://bit.ly/2NZT6Nd">Geodesy: The branch of mathematics dealing with the shape and area of the earth or large portions of it.</a>
  /// <br/>
  /// Origin: late 16th century: from modern Latin geodaesia, from Greek geōdaisia, from gē ‘earth’ + daiein ‘divide’.
  /// <br/>
  /// https://en.wikipedia.org/wiki/Geodesy  https://www.movable-type.co.uk/scripts/latlong.html
  public static class Geodetic {
    /// <a href="http://bit.ly/2NZT6Nd">Yes, all the calculations herein are based on a guess of the earth's radius between the two points in question.</a>
    public const double EarthMeanRadiusKm = 6371.01;

    /// <a href="http://bit.ly/2Oq9qpQ">Yet another way to specify coordinates. Consider it as another view on truth. It is a struct, so it will be passed around by value.</a>
    public struct Coordinates {
      /// <a href="http://bit.ly/2Oq9qpQ">Decimal latitude and longitude held in a 64 bit floating point number</a>
      public readonly double Latitude;

      /// <a href="http://bit.ly/2Oq9qpQ">Decimal latitude and longitude held in a 64 bit floating point number/</a>
      public readonly double Longitude;

      /// <a href="http://bit.ly/2Oq9qpQ">Whether we will be working in radians or degrees</a>
      public readonly bool Radians;

      /// <a href="http://bit.ly/2Oq9qpQ">Create a coordinates data structure.</a>
      public Coordinates(double latitude, double longitude, bool radians = false) {
        Latitude  = latitude;
        Longitude = longitude;
        Radians   = radians;
      }

      /// <a href="http://bit.ly/2Oq9qpQ">Convert to radians if needed</a>
      public Coordinates ToRadians() =>
        Radians ? this : Coords(Trig.ToRadians(Latitude), Trig.ToRadians(Longitude), true);

      /// <a href="http://bit.ly/2Oq9qpQ">Convert to degrees if needed.</a>
      public Coordinates ToDegrees() =>
        Radians ? Coords(Trig.ToDegrees(Latitude), Trig.ToDegrees(Longitude), true) : this;

      /// <a href="http://bit.ly/2Oq9qpQ">Coordinates Data Structure</a>
      public override string ToString() => $"({Latitude:n5}, {Longitude:n5})";

      /// <a href="http://bit.ly/2NZT6Nd">Account for inexact equality</a>
      public bool Equals(Coordinates other) {
        var my = ToRadians();
        other = other.ToRadians();

        return Compare.AlmostEqual(my.Latitude,  other.Latitude,  0.001) &&
               Compare.AlmostEqual(my.Longitude, other.Longitude, 0.001);
      }

      /// <a href="http://bit.ly/2NZT6Nd"></a>
      public override bool Equals(object obj) => base.Equals(obj);

      /// <a href="http://bit.ly/2NZT6Nd"></a>
      public override int GetHashCode() {
        unchecked { return (Latitude.GetHashCode() * 397) ^ Longitude.GetHashCode(); }
      }
    }

    /// <a href="">Create a coordinates data structure.</a>
    public static Coordinates Coords(double latitude, double longitude, bool radians = false) =>
      new Coordinates(latitude, longitude, radians);

    /// <a href="http://bit.ly/2NTfGXA">Calculate the distance between two points accounting for the curvature of the earth.</a>
    public static double Kilometres(Coordinates from, Coordinates to) => Haversine(from, to);

    /// <a href="http://bit.ly/2NTfGXA">Return a string representation of the distance that is realistic based in the distance. If it is less than one kilometer, return as metres without any decimal place. Otherwise for less than ten kilometres use one decimal, otherwise none.</a>
    public static string DistanceBetween(Coordinates from, Coordinates to) {
      double km = Kilometres(from, to);
      if (km < 1) return $"{(int) (km * 1000)} m";

      return (km > 10) ? $"{km:n0} km" : $"{km:n1} km";
    }

    /// <a href="http://bit.ly/2NTfGXA">Haversine approximation of the distance between two coordinates</a>
    public static double Haversine(Coordinates first, Coordinates second) {
      first  = first.ToRadians();
      second = second.ToRadians();
      var sinDeltaLatitude  = Math.Sin((second.Latitude  - first.Latitude)  / 2);
      var sinDeltaLongitude = Math.Sin((second.Longitude - first.Longitude) / 2);

      var a = (sinDeltaLatitude * sinDeltaLatitude) +
              Math.Cos(first.Latitude) * Math.Cos(second.Latitude) *
              (sinDeltaLongitude * sinDeltaLongitude);

      // return EarthMeanRadiusKm * 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
      return EarthMeanRadiusKm * 2 * Math.Asin(Math.Sqrt(a));
    }

    /// <a href="http://bit.ly/2Oq9oOK">Return the bearing in radians to get from one point to another.</a>
    public static double BearingRadians(Coordinates from, Coordinates to) { // forward azimuth
      from = from.ToRadians();
      to   = to.ToRadians();
      var y = Math.Sin(to.Longitude - from.Longitude) * Math.Cos(to.Latitude);

      var x = Math.Cos(from.Latitude) * Math.Sin(to.Latitude) -
              Math.Sin(from.Latitude) * Math.Cos(to.Latitude) *
              Math.Cos(to.Longitude - from.Longitude);

      return Math.Atan2(y, x);
    }

    /// <a href="http://bit.ly/2Oq9oOK">Return the bearing in degrees to get from one point to another.</a>
    public static double BearingDegrees(Coordinates from, Coordinates to) => Trig.ToDegrees(BearingRadians(from, to));

    /// <a href="http://bit.ly/2Oq9xlg">Give a starting coordinate, a distance and a direction, calculate and return a destination coordinate.</a>
    public static Coordinates Destination(Coordinates start, double distanceKm, double bearingDegrees) {
      start = start.ToRadians();
      var bearingRadians   = Trig.ToRadians(bearingDegrees);
      var deltaDistance    = distanceKm / EarthMeanRadiusKm;
      var sinDeltaDistance = Math.Sin(deltaDistance);
      var cosDeltaDistance = Math.Cos(deltaDistance);
      var sinStartLatitude = Math.Sin(start.Latitude);
      var cosStartLatitude = Math.Cos(start.Latitude);

      var latitude = Math.Asin(
        (sinStartLatitude * cosDeltaDistance) +
        cosStartLatitude * sinDeltaDistance * Math.Cos(bearingRadians));

      var longitude = start.Longitude + Math.Atan2(
                        Math.Sin(bearingRadians) * sinDeltaDistance * cosStartLatitude,
                        cosDeltaDistance - sinStartLatitude * Math.Sin(latitude));

      return Coords(latitude, longitude, radians: true);
    }
  }
}