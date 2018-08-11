using System.Collections.Generic;
using UnityEngine;

namespace Askowl {
  /// Added needed features to Quaternion.
  /// <remarks><a href="http://unitydoc.marrington.net/Able#quaternionscs-another-perspective-on-quaternions">Tetrads</a></remarks>
  public static class Quaternions {
    /// <summary>
    /// I hate using right, up and forward for X Y and Z.
    /// </summary>
    /// <param name="q">this</param>
    /// <param name="axis">Vector3.right, up or forward</param>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#axis">Trig Axis to Vector3</a></remarks>
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
    /// <remarks><a href="http://unitydoc.marrington.net/Able#aroundaxis">Rotate round an XYZ axis</a></remarks>
    public static Quaternion AroundAxis(this Quaternion q, Trig.Direction axis, float degrees) {
      var rotateBy = Quaternion.AngleAxis(degrees, q.Axis(axis));
      return rotateBy * q;
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
    /// <remarks><a href="http://unitydoc.marrington.net/Able#lengthsquared">Magnitude of a Quaternion**2</a></remarks>
    public static float LengthSquared(this Quaternion q) => q.x * q.x + q.y * q.y + q.z * q.z + q.w * q.w;

    /// <summary>
    /// A much faster normalise, accurate to a dot product of 0.9995 or better.
    /// </summary>
    /// <param name="q"></param>
    /// <returns></returns>
    public static Quaternion Normalise(this Quaternion q) {
      float lengthSquared = q.LengthSquared();
      if ((lengthSquared < 0.02) || (lengthSquared >= 0.8)) return q.normalized; // do it the long way

      float il    = additiveConstant + factor * (lengthSquared - Neighborhood);
      int   count = 0;

      while (((lengthSquared * il * il) < Limit) && (count++ < 8)) {
        il *= additiveConstant + factor * (lengthSquared - Neighborhood);
      }

      if (count > 3) Debug.LogWarning($"Normalise for {q}, sql {lengthSquared} had {count} iterations");

      return new Quaternion(q.x * il, q.y * il, q.z * il, q.w * il);
    }

    private const           float Neighborhood     = 0.959066f;
    private const           float Scale            = 1.000311f;
    private const           float Limit            = 0.9995f * 0.9995f;
    private static readonly float additiveConstant = Scale / Mathf.Sqrt(Neighborhood);
    private static readonly float factor           = Scale * (-0.5f / (Neighborhood * Mathf.Sqrt(Neighborhood)));

    /// <summary>
    /// Gyro is right-handed while Unity is left-handed - so change chirality
    /// </summary>
    /// <param name="q"></param>
    /// <param name="axis">Axis that represents forward and hence the spindle to rotate around</param>
    /// <returns>the quaternion for command chains</returns>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#righttolefthanded">Change chirality</a></remarks>
    public static Quaternion RightToLeftHanded(this Quaternion q, Trig.Direction axis) {
      return new Quaternion(q.x * -axis.x, q.y * -axis.y, q.z * -axis.z, -q.w);

      q.Normalize();
    }

    /// <summary>
    /// Apply a rotation - same as nultiply (*) but can be chained
    /// </summary>
    /// <param name="attitude"></param>
    /// <returns>the quaternion for command chains</returns>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#rotateby">Quaternion Rotate By another</a></remarks>
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