#if UNITY_EDITOR && AskowlBase

using NUnit.Framework;
using UnityEngine;

namespace Askowl {
  public class GeodeticTests {
    [Test]
    public void TestDistances() {
      double[,] coords = new double[,] {
        {-27.46850, 151.94379, 5}, {-27.46855, 151.94379, 5},
        {-27.46851, 151.94377, 34}, {-27.46820, 151.94377, 34},
        {-27.46851, 151.94377, 7}, {-27.46844, 151.94377, 7},
        {-27.46850, 151.94379, 43}, {-27.46811, 151.94379, 43}
      };

      for (int i = 0; i < coords.GetLength(0); i += 2) {
        var from = Geodetic.Coords(latitude: coords[i, 0],     longitude: coords[i, 1]);
        var to   = Geodetic.Coords(latitude: coords[i + 1, 0], longitude: coords[i + 1, 1]);

        Debug.LogFormat("{0} to {1} is {2} - should be {3} m",
                        from, to, Geodetic.DistanceBetween(from, to), coords[i, 2]);

        Assert.AreEqual(Geodetic.Kilometres(from, to) * 1000, coords[i, 2], 0.99);
      }

      for (int i = 0; i < 10; i++) {
        var from = Geodetic.Coords(latitude: coords[0, 0], longitude: coords[0, 1]);
        var to   = Geodetic.Coords(latitude: coords[0, 0], longitude: coords[0, 1]);
        to.Latitude  += Random.Range(min: -0.0005f, max: 0.0005f);
        to.Longitude += Random.Range(min: -0.0005f, max: 0.0005f);
        Debug.LogFormat("{0} to {1} is {2}", from, to, Geodetic.DistanceBetween(from, to));
      }
    }
  }
}
#endif