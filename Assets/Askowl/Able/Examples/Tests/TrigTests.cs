#if UNITY_EDITOR && Able

using NUnit.Framework;
using UnityEngine;

namespace Askowl.Examples { //#TBD#
  /// <remarks><a href="http://unitydoc.marrington.net/Able#trigcs">Trigonometry Rocks</a></remarks>
  public class TrigTests {
    /// <remarks><a href="http://unitydoc.marrington.net/Able#direction">Define axes</a></remarks>
    [Test]
    public void Direction() {
      Assert.AreEqual(1,        Trig.xAxis.X);
      Assert.AreEqual(0,        Trig.xAxis.Y);
      Assert.AreEqual(0,        Trig.xAxis.Z);
      Assert.AreEqual('X',      Trig.xAxis.Name);
      Assert.AreEqual("X-Axis", Trig.xAxis.ToString());
      Assert.AreEqual(0,        Trig.xAxis.Ord);
      var right = Vector3.zero;
      right[Trig.xAxis.Ord] = 1f;
      Assert.AreEqual(Vector3.right, right);

      Assert.AreEqual(0,        Trig.yAxis.X);
      Assert.AreEqual(1,        Trig.yAxis.Y);
      Assert.AreEqual(0,        Trig.yAxis.Z);
      Assert.AreEqual('Y',      Trig.yAxis.Name);
      Assert.AreEqual("Y-Axis", Trig.yAxis.ToString());
      Assert.AreEqual(1,        Trig.yAxis.Ord);
      var up = Vector3.zero;
      up[Trig.yAxis.Ord] = 1f;
      Assert.AreEqual(Vector3.up, up);

      Assert.AreEqual(0,        Trig.zAxis.X);
      Assert.AreEqual(0,        Trig.zAxis.Y);
      Assert.AreEqual(1,        Trig.zAxis.Z);
      Assert.AreEqual('Z',      Trig.zAxis.Name);
      Assert.AreEqual("Z-Axis", Trig.zAxis.ToString());
      Assert.AreEqual(2,        Trig.zAxis.Ord);
      var forward = Vector3.zero;
      forward[Trig.zAxis.Ord] = 1f;
      Assert.AreEqual(Vector3.forward, forward);

      // Tests for negative (same for other axes)
      Assert.IsFalse(Trig.xAxis.Negative);
      var minusX = -Trig.xAxis;
      Assert.IsTrue(minusX.Negative);
      Assert.AreEqual(-1, minusX.X);
      Assert.AreEqual(0,  minusX.Y);
      Assert.AreEqual(0,  minusX.Z);
    }
  }
}
#endif