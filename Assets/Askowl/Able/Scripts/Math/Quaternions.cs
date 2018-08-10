using UnityEngine;

namespace Askowl {
  /// Added needed features to Quaternion.
  /// <remarks><a href="http://unitydoc.marrington.net/Able#quaternionscs-another-perspective-on-quaternions">Tetrads</a></remarks>
  public static class Quaternions {
    /// <summary>
    ///  Linearly interpolates between two Tetrads, with spherical adjustments.
    /// </summary>
    /// <param name="start">this</param>
    /// <param name="end">The second source Tetrad.</param>
    /// <param name="proportion">The relative weight of the second source Tetrad in the interpolation.</param>
    /// <returns>the quaternion for command chains</returns>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#quaternionsalerp">Quaternion Adjusted Lerp</a></remarks>
    public static Quaternion ALerp(this Quaternion start, Quaternion end, float proportion) {
      float left = proportion, right = 1f - proportion;

      if (Quaternion.Dot(start, end) < 0.0f) left = -left;

      return new Quaternion(
        x: right * start.x + left * end.x,
        y: right * start.y + left * end.y,
        z: right * start.z + left * end.z,
        w: right * start.w + left * end.w).normalized;
    }

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
    /// Gyro is right-handed while Unity is left-handed - so change Chirilty
    /// </summary>
    /// <param name="q"></param>
    /// <returns>the quaternion for command chains</returns>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#righttolefthanded">Change chirilty</a></remarks>
    public static Quaternion RightToLeftHanded(this Quaternion q) =>
      new Quaternion(q.x, q.y, -q.z, -q.w);

    /// <summary>
    /// Apply one or more rotations in sequence to this tetrad.
    /// </summary>
    /// <param name="q">this</param>
    /// <param name="attitudes"></param>
    /// <returns>the quaternion for command chains</returns>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#rotateby">Rotate By Angle in Degrees</a></remarks>
    public static Quaternion RotateBy(this Quaternion q, params Quaternion[] attitudes) {
      for (int i = 0; i < attitudes.Length; i++) {
        float x = q.w * attitudes[i].x + q.x * attitudes[i].w + q.y * attitudes[i].z - q.z * attitudes[i].y;
        float y = q.w * attitudes[i].y + q.y * attitudes[i].w + q.z * attitudes[i].x - q.x * attitudes[i].z;
        float z = q.w * attitudes[i].z + q.z * attitudes[i].w + q.x * attitudes[i].y - q.y * attitudes[i].x;
        float w = q.w * attitudes[i].w - q.x * attitudes[i].x - q.y * attitudes[i].y - q.z * attitudes[i].z;
        q.x = x;
        q.y = y;
        q.z = z;
        q.w = w;
      }

      return q;
    }

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

    /// <summary>
    ///
    /// </summary>
    /// <param name="q">this</param>
    public static float LengthSquared(this Quaternion q) => q.x * q.x + q.y * q.y + q.z * q.z + q.w * q.w;

    /// <summary>
    /// A much faster normalise, accurate to a dot product of 0.9995 or better.
    /// </summary>
    /// <param name="q"></param>
    /// <returns></returns>
    public static Quaternion Normalise(this Quaternion q) {
      float lengthSquared = q.LengthSquared();
      if (lengthSquared >= 0.8) return q.normalized; // do it the long way

      float il    = additiveConstant + factor * (lengthSquared - Neighborhood);
      int   count = 0;

      do {
        il *= additiveConstant + factor * (lengthSquared - Neighborhood);
      } while (((lengthSquared * il * il) < Limit) && (count++ < 8));

      if (count > 3) Debug.LogWarning($"Normalise for {q}, sql {lengthSquared} had {count} iterations");

      return new Quaternion(q.x * il, q.y * il, q.z * il, q.w * il);
    }

    private const           float Neighborhood     = 0.959066f;
    private const           float Scale            = 1.000311f;
    private const           float Limit            = 0.9995f * 0.9995f;
    private static readonly float additiveConstant = Scale / Mathf.Sqrt(Neighborhood);
    private static readonly float factor           = Scale * (-0.5f / (Neighborhood * Mathf.Sqrt(Neighborhood)));
  }
}