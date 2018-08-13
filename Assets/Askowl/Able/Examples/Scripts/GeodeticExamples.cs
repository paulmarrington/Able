#if UNITY_EDITOR && Able

using System;
using NUnit.Framework;

namespace Askowl.Examples {
  /// <remarks><a href="http://unitydoc.marrington.net/Able#geodeticcs-distance-and-bearings">Geodetics</a></remarks>
  public class GeodeticExamples {
    double[,] coords = {
      {-27.46850, 151.94379, 5, 180}, {-27.46855, 151.94379, 5, 180},
      {-27.46851, 151.94377, 34, 0}, {-27.46820, 151.94377, 34, 0},
      {-27.46851, 151.94377, 7, 0}, {-27.46844, 151.94377, 7, 0},
      {-27.46850, 151.94379, 43, 0}, {-27.46811, 151.94379, 43, 0},
      {36.12, -86.67, 2886444, -85.4}, {33.94, -118.4, 2886444, -85.4},
      {-27.49210, 151.94379, 3318, -36.49}, {-27.46811, 151.92379, 3318, -36.49},
      {-27.49210, 151.94379, 202926.01, 90.4748}, {-27.49210, 154.00109, 202926.01, 90.4748}
    };

    string[] distances = {"5 m", "34 m", "7 m", "43 m", "2,886 km", "3.3 km", "203 km"};

    /// <remarks><a href="http://unitydoc.marrington.net/Able#distance-between-two-points">Distance Between Two Points</a></remarks>
    [Test]
    public void GeodeticDistances() {
      TestCoords((idx, from, to) => {
        var metresApart   = Geodetic.Kilometres(from, to) * 1000;
        var distanceApart = Geodetic.DistanceBetween(from, to);

        Assert.AreEqual(expected: coords[idx, 2], actual: metresApart, delta: 0.99);

        Assert.AreEqual(expected: distances[idx / 2], actual: distanceApart);

        Assert.AreEqual(metresApart, Geodetic.Haversine(from, to) * 1000);
      });
    }

    /// <remarks><a href="http://unitydoc.marrington.net/Able#bearing-from-one-point-to-the-next">Distance Between Two Points</a></remarks>
    [Test]
    public void GeodeticBearings() {
      TestCoords((idx, from, to) => {
        var bearing = Geodetic.BearingDegrees(from, to);
        Assert.AreEqual(coords[idx, 3], bearing, 0.01, $"degrees for index {idx}");
        var radians = Geodetic.BearingRadians(from, to);
        Assert.AreEqual(coords[idx, 3], Trig.ToDegrees(radians), 0.01, $"radians for index {idx}");
      });
    }

    /// <remarks><a href="http://unitydoc.marrington.net/Able#find-one-point-from-another">Distance Between Two Points</a></remarks>
    [Test]
    public void GeodeticDestination() {
      TestCoords((idx, from, to) => {
        var kmApart = Geodetic.Kilometres(from, to);
        var bearing = Geodetic.BearingDegrees(from, to);

        var destination = Geodetic.Destination(start: from, distanceKm: kmApart, bearingDegrees: bearing);

        Assert.IsTrue(to.Equals(destination));
      });
    }

    private void TestCoords(Action<int, Geodetic.Coordinates, Geodetic.Coordinates> action) {
      for (int i = 0; i < coords.GetLength(0); i += 2) {
        var from = Geodetic.Coords(latitude: coords[i, 0],     longitude: coords[i, 1]);
        var to   = Geodetic.Coords(latitude: coords[i + 1, 0], longitude: coords[i + 1, 1]);
        action(i, from, to);
      }
    }
  }
}
#endif