#if UNITY_EDITOR && Able

using System;
using NUnit.Framework;
using UnityEngine;

namespace Askowl.Examples {
  public class GeodeticTests {
    double[,] coords = {
      {-27.46850, 151.94379, 5, 180}, {-27.46855, 151.94379, 5, 180},
      {-27.46851, 151.94377, 34, 0}, {-27.46820, 151.94377, 34, 0},
      {-27.46851, 151.94377, 7, 0}, {-27.46844, 151.94377, 7, 0},
      {-27.46850, 151.94379, 43, 0}, {-27.46811, 151.94379, 43, 0},
      {-27.49210, 151.94379, 2667, -36.49}, {-27.46811, 151.92379, 2667, -36.49},
      {-27.49210, 151.94379, 228753, 90.4748}, {-27.49210, 154.00109, 228753, 90.4748}
    };

    string[] distances = {"5 m", "34 m", "7 m", "43 m", "2.7 km", "229 km"};

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

    [Test]
    public void GeodeticBearings() {
      TestCoords((idx, from, to) => {
        var bearing = Geodetic.BearingDegrees(from, to);
        Assert.AreEqual(coords[idx, 3], bearing, 0.01, $"degrees for index {idx}");
        var radians = Geodetic.BearingRadians(from, to);
        Assert.AreEqual(coords[idx, 3], Trig.ToDegrees(radians), 0.01, $"radians for index {idx}");
      });
    }

    public void TestCoords(Action<int, Geodetic.Coordinates, Geodetic.Coordinates> action) {
      for (int i = 0; i < coords.GetLength(0); i += 2) {
        var from = Geodetic.Coords(latitude: coords[i, 0],     longitude: coords[i, 1]);
        var to   = Geodetic.Coords(latitude: coords[i + 1, 0], longitude: coords[i + 1, 1]);
        action(i, from, to);
      }
    }
  }
}
#endif