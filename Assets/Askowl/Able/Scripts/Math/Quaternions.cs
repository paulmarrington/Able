using System.Collections.Generic;
using UnityEngine;

namespace Askowl {
  /// Added needed features to Quaternion.
  /// <remarks><a href="http://unitydoc.marrington.net/Able#quaternionscs-another-perspective-on-quaternions">Tetrads</a></remarks>
  public static class Quaternions {
    public static Vector3 Axis(this Quaternion q, Trig.Direction axis) => directions[axis];

    private static Dictionary<Trig.Direction, Vector3> directions
      = new Dictionary<Trig.Direction, Vector3> {
        {Trig.xAxis, Vector3.right}, {Trig.yAxis, Vector3.up}, {Trig.zAxis, Vector3.forward}
      };

    /// <summary>
    /// Rotate quaternion around an axis by a specified number of degrees then normalise
    /// </summary>
    /// <param name="q">this</param>
    /// <param name="axis">Trig.xAxis, yAxis or zAxis</param>
    /// <param name="degrees">Amount to rotate by</param>
    /// <returns>the quaternion for command chains</returns>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#quaternionsaroundaxis">Rotate round an XYZ axis</a></remarks>
    public static Quaternion AroundAxis(this Quaternion q, Trig.Direction axis, float degrees) {
      float      theta    = (float) (Trig.ToRadians(degrees) / 2);
      var        sin      = Mathf.Sin(theta);
      Quaternion rotation = new Quaternion(axis.x * sin, axis.y * sin, axis.z * sin, Mathf.Cos(theta));
      return rotation.normalized * q;
    }

    /// <summary>
    /// The inverse used when the Quaternion is a rotation - and is the reverse rotation.
    /// In other words apply rotation then apply inverse and nothing will have changed.
    /// </summary>
    /// <returns>the quaternion for command chains</returns>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#inverse">Invert the Quaternion Rotation</a></remarks>
    public static Quaternion Inverse(this Quaternion q) => Quaternion.Inverse(q);

    /// <summary>
    ///
    /// </summary>
    /// <param name="q">this</param>
    public static float LengthSquared(this Quaternion q) => q.x * q.x + q.y * q.y + q.z * q.z + q.w * q.w;

    /// <summary>
    /// Gyro is right-handed while Unity is left-handed - so change chiralty
    /// </summary>
    /// <param name="q"></param>
    /// <param name="axis">Axis that represents forward and hence the spindle to rotate around</param>
    /// <returns>the quaternion for command chains</returns>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#righttolefthanded">Change chiralty</a></remarks>
    public static Quaternion RightToLeftHanded(this Quaternion q, Trig.Direction axis) {
      return new Quaternion(q.x * -axis.x, q.y * -axis.y, q.z * -axis.z, -q.w);
    }

    /// <summary>
    /// Apply a rotation - same as nultiply (*) but can be chained
    /// </summary>
    /// <param name="attitude"></param>
    /// <returns>the quaternion for command chains</returns>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#rotateby">Quaternion Rotate By</a></remarks>
    public static Quaternion RotateBy(this Quaternion q, Quaternion attitude) => attitude * q;

    /// <summary>
    /// One person's left is another person's up. More importantly the gyroscope
    /// in the phone sees forward as the Z axis while Unity likes to use Y for that.
    /// </summary>
    /// <param name="q">this</param>
    /// <param name="pivot">Axis that does not change</param>
    /// <returns>the quaternion for command chains</returns>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#switchaxis">Switch two axes</a></remarks>
    public static Quaternion SwitchAxis(this Quaternion q, Trig.Direction pivot) {
      if (pivot.x != 0) return new Quaternion(-q.x, -q.z, -q.y, q.w);
      if (pivot.y != 0) return new Quaternion(-q.z, -q.y, -q.x, q.w);

      return new Quaternion(-q.y, -q.x, -q.z, q.w);
    }
  }
}