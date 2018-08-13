#if UNITY_EDITOR && Able

using NUnit.Framework;
using UnityEngine;

namespace Askowl.Examples { //#TBD#
  /// <remarks><a href="http://unitydoc.marrington.net/Able#trigcs">Trigonometry Rocks</a></remarks>
  public class TrigExamples {
    /// <remarks><a href="http://unitydoc.marrington.net/Able#direction">Define axes</a></remarks>
    [Test]
    public void PositiveDirection() {
      var xAxis = Trig.xAxis;
      var yAxis = Trig.yAxis;
      var zAxis = Trig.zAxis;

      Assert.AreEqual(1,             xAxis.X);
      Assert.AreEqual(0,             xAxis.Y);
      Assert.AreEqual(0,             xAxis.Z);
      Assert.AreEqual(xAxis[0],      xAxis.X);
      Assert.AreEqual(xAxis[1],      xAxis.Y);
      Assert.AreEqual(xAxis[2],      xAxis.Z);
      Assert.AreEqual('X',           xAxis.Name);
      Assert.AreEqual("X Axis",      xAxis.ToString());
      Assert.AreEqual(0,             xAxis.Ord);
      Assert.AreEqual(Vector3.right, xAxis.Vector);
      Assert.AreEqual("right",       xAxis.VectorName);
      var otherAxes = xAxis.OtherAxes;
      Assert.AreEqual(yAxis, otherAxes[0]);
      Assert.AreEqual(zAxis, otherAxes[1]);

      Assert.AreEqual(0,          yAxis.X);
      Assert.AreEqual(1,          yAxis.Y);
      Assert.AreEqual(0,          yAxis.Z);
      Assert.AreEqual(yAxis[0],   yAxis.X);
      Assert.AreEqual(yAxis[1],   yAxis.Y);
      Assert.AreEqual(yAxis[2],   yAxis.Z);
      Assert.AreEqual('Y',        yAxis.Name);
      Assert.AreEqual("Y Axis",   yAxis.ToString());
      Assert.AreEqual(1,          yAxis.Ord);
      Assert.AreEqual(Vector3.up, yAxis.Vector);
      Assert.AreEqual("up",       yAxis.VectorName);
      otherAxes = yAxis.OtherAxes;
      Assert.AreEqual(xAxis, otherAxes[0]);
      Assert.AreEqual(zAxis, otherAxes[1]);

      Assert.AreEqual(0,               zAxis.X);
      Assert.AreEqual(0,               zAxis.Y);
      Assert.AreEqual(1,               zAxis.Z);
      Assert.AreEqual(zAxis[0],        zAxis.X);
      Assert.AreEqual(zAxis[1],        zAxis.Y);
      Assert.AreEqual(zAxis[2],        zAxis.Z);
      Assert.AreEqual('Z',             zAxis.Name);
      Assert.AreEqual("Z Axis",        zAxis.ToString());
      Assert.AreEqual(2,               zAxis.Ord);
      Assert.AreEqual(Vector3.forward, zAxis.Vector);
      Assert.AreEqual("forward",       zAxis.VectorName);
      otherAxes = zAxis.OtherAxes;
      Assert.AreEqual(xAxis, otherAxes[0]);
      Assert.AreEqual(yAxis, otherAxes[1]);
    }

    /// <remarks><a href="http://unitydoc.marrington.net/Able#direction">Define axes</a></remarks>
    [Test]
    public void NegativeDirection() {
      var minusX = -Trig.xAxis;
      var minusY = -Trig.yAxis;
      var minusZ = -Trig.zAxis;

      var xAxis = Trig.xAxis;
      var yAxis = Trig.yAxis;
      var zAxis = Trig.zAxis;

      Assert.IsFalse(xAxis.Negative);
      Assert.IsTrue(minusX.Negative);
      Assert.AreEqual(-1,           minusX.X);
      Assert.AreEqual(0,            minusX.Y);
      Assert.AreEqual(0,            minusX.Z);
      Assert.AreEqual(minusX[0],    minusX.X);
      Assert.AreEqual(minusX[1],    minusX.Y);
      Assert.AreEqual(minusX[2],    minusX.Z);
      Assert.AreEqual('X',          minusX.Name);
      Assert.AreEqual("-X Axis",    minusX.ToString());
      Assert.AreEqual(0,            minusX.Ord);
      Assert.AreEqual(Vector3.left, minusX.Vector);
      Assert.AreEqual("left",       minusX.VectorName);
      var otherAxes = minusX.OtherAxes;
      Assert.AreEqual(minusY, otherAxes[0]);
      Assert.AreEqual(minusZ, otherAxes[1]);

      Assert.IsFalse(yAxis.Negative);
      Assert.IsTrue(minusY.Negative);
      Assert.AreEqual(0,            minusY.X);
      Assert.AreEqual(-1,           minusY.Y);
      Assert.AreEqual(0,            minusY.Z);
      Assert.AreEqual(minusY[0],    minusY.X);
      Assert.AreEqual(minusY[1],    minusY.Y);
      Assert.AreEqual(minusY[2],    minusY.Z);
      Assert.AreEqual('Y',          minusY.Name);
      Assert.AreEqual("-Y Axis",    minusY.ToString());
      Assert.AreEqual(1,            minusY.Ord);
      Assert.AreEqual(Vector3.down, minusY.Vector);
      Assert.AreEqual("down",       minusY.VectorName);
      otherAxes = minusY.OtherAxes;
      Assert.AreEqual(minusX, otherAxes[0]);
      Assert.AreEqual(minusZ, otherAxes[1]);

      Assert.IsFalse(zAxis.Negative);
      Assert.IsTrue(minusZ.Negative);
      Assert.AreEqual(0,            minusZ.X);
      Assert.AreEqual(0,            minusZ.Y);
      Assert.AreEqual(-1,           minusZ.Z);
      Assert.AreEqual(minusZ[0],    minusZ.X);
      Assert.AreEqual(minusZ[1],    minusZ.Y);
      Assert.AreEqual(minusZ[2],    minusZ.Z);
      Assert.AreEqual('Z',          minusZ.Name);
      Assert.AreEqual("-Z Axis",    minusZ.ToString());
      Assert.AreEqual(2,            minusZ.Ord);
      Assert.AreEqual(Vector3.back, minusZ.Vector);
      Assert.AreEqual("back",       minusZ.VectorName);
      otherAxes = minusZ.OtherAxes;
      Assert.AreEqual(minusX, otherAxes[0]);
      Assert.AreEqual(minusY, otherAxes[1]);
    }

    /// <remarks><a href="http://unitydoc.marrington.net/Able#toradians">Degrees to Radians</a></remarks>
    [Test]
    public void ToRadians() {
      Assert.IsTrue(Compare.AlmostEqual(0.0174533, Trig.ToRadians(1)));
      Assert.IsTrue(Compare.AlmostEqual(0.575959,  Trig.ToRadians(33)));
      Assert.IsTrue(Compare.AlmostEqual(1.29154,   Trig.ToRadians(74)));
      Assert.IsTrue(Compare.AlmostEqual(1.55334,   Trig.ToRadians(89)));
      Assert.IsTrue(Compare.AlmostEqual(1.5708,    Trig.ToRadians(90)));
      Assert.IsTrue(Compare.AlmostEqual(1.58825,   Trig.ToRadians(91)));
      Assert.IsTrue(Compare.AlmostEqual(2.14675,   Trig.ToRadians(123)));
      Assert.IsTrue(Compare.AlmostEqual(3.45575,   Trig.ToRadians(198)));
      Assert.IsTrue(Compare.AlmostEqual(4.45059,   Trig.ToRadians(255)));
      Assert.IsTrue(Compare.AlmostEqual(5.46288,   Trig.ToRadians(313)));
    }

    /// <remarks><a href="http://unitydoc.marrington.net/Able#todegrees">Radians to Degrees</a></remarks>
    [Test]
    public void ToDegrees() {
      Assert.IsTrue(Compare.AlmostEqual(1f,   Trig.ToDegrees(0.0174533), 1e5));
      Assert.IsTrue(Compare.AlmostEqual(33f,  Trig.ToDegrees(0.575959),  1e5));
      Assert.IsTrue(Compare.AlmostEqual(74f,  Trig.ToDegrees(1.29154),   1e5));
      Assert.IsTrue(Compare.AlmostEqual(89f,  Trig.ToDegrees(1.55334),   1e5));
      Assert.IsTrue(Compare.AlmostEqual(90f,  Trig.ToDegrees(1.5708),    1e5));
      Assert.IsTrue(Compare.AlmostEqual(91f,  Trig.ToDegrees(1.58825),   1e5));
      Assert.IsTrue(Compare.AlmostEqual(123f, Trig.ToDegrees(2.14675),   1e5));
      Assert.IsTrue(Compare.AlmostEqual(198f, Trig.ToDegrees(3.45575),   1e5));
      Assert.IsTrue(Compare.AlmostEqual(255f, Trig.ToDegrees(4.45059),   1e5));
      Assert.IsTrue(Compare.AlmostEqual(313f, Trig.ToDegrees(5.46288),   1e5));
    }

    /// <remarks><a href="http://unitydoc.marrington.net/Able#relative-position-given-distance-and-angle-or-bearing">Relative Position given Distance and Angle or Bearing</a></remarks>
    [Test]
    public void RelativePositionFromBearing() {
      Vector2 expected = Vector2.zero;
      expected.Set(0, 1);
      Vector2 actual = Trig.RelativePositionFromBearing(1, (float) Trig.ToRadians(0));
      AreEqual(expected, actual);

      expected.Set(3.3f, 0);
      actual = Trig.RelativePositionFromBearing(3.3f, (float) Trig.ToRadians(90));
      AreEqual(expected, actual);

      expected.Set(-3.3f, 0);
      actual = Trig.RelativePositionFromBearing(3.3f, (float) Trig.ToRadians(-90));
      AreEqual(expected, actual);

      expected.Set(11.7258f, -4.5011f);
      actual = Trig.RelativePositionFromBearing(12.56f, (float) Trig.ToRadians(111));
      AreEqual(expected, actual);

      expected.Set(-3.1322f, 1.4606f);
      actual = Trig.RelativePositionFromBearing(3.456f, (float) Trig.ToRadians(295));
      AreEqual(expected, actual);
    }

    /// <remarks><a href="http://unitydoc.marrington.net/Able#relative-position-given-distance-and-angle-or-bearing">Relative Position given Distance and Angle or Bearing</a></remarks>
    [Test]
    public void RelativePositionFromAngle() {
      Vector2 expected = Vector2.zero;
      expected.Set(newX: 1, newY: 0);
      Vector2 actual = Trig.RelativePositionFromAngle(1, (float) Trig.ToRadians(0));
      AreEqual(expected, actual);

      expected.Set(newX: 0, newY: 3.3f);
      actual = Trig.RelativePositionFromAngle(3.3f, (float) Trig.ToRadians(90));
      AreEqual(expected, actual);

      expected.Set(newX: 0, newY: -3.3f);
      actual = Trig.RelativePositionFromAngle(3.3f, (float) Trig.ToRadians(-90));
      AreEqual(expected, actual);

      expected.Set(-4.5011f, 11.7258f);
      actual = Trig.RelativePositionFromAngle(12.56f, (float) Trig.ToRadians(111));
      AreEqual(expected, actual);

      expected.Set(1.4606f, -3.1322f);
      actual = Trig.RelativePositionFromAngle(3.456f, (float) Trig.ToRadians(295));
      AreEqual(expected, actual);
    }

    private void AreEqual(Vector2 expected, Vector2 actual) {
      Assert.IsTrue(Compare.AlmostEqual(expected.x, actual.x) & Compare.AlmostEqual(expected.y, actual.y),
                    $"expected: {expected.x:F4},{expected.y:F4}, actual: {actual.x:F4},{actual.y:F4}");
    }
  }
}
#endif