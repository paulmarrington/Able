// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

using UnityEngine;

namespace Askowl {
  /// <a href="http://bit.ly/2Oq9ohI">Added needed features to Quaternion.</a>
  public static class Quaternions {
    /// <a href="http://bit.ly/2RhdaZl">Rotate quaternion around an axis by a specified number of degrees then normalise</a>
    public static Quaternion AroundAxis(this Quaternion q, Trig.Direction axis, float degrees) {
      var rotateBy = Quaternion.AngleAxis(degrees, axis.Vector);
      return rotateBy * q;
    }

    /// <a href="http://bit.ly/2RhLC63">The inverse used when the Quaternion is a rotation - and is the reverse rotation. In other words apply rotation then apply inverse and nothing will have changed.</a>
    public static Quaternion Inverse(this Quaternion q) => Quaternion.Inverse(q);

    /// <a href="http://bit.ly/2RezLWq">The concept of length or magnitude for a quaternion has no visual representation when dealing with attitude or rotation. The catch is that most algorithms require unit quaternions - where the length squared will approach one.</a>
    public static float LengthSquared(this Quaternion q) => q.x * q.x + q.y * q.y + q.z * q.z + q.w * q.w;

    /// <a href="http://bit.ly/2NUH4V6"> A faster normalise, accurate to a dot product of 0.9995 or better.</a>
    public static Quaternion Normalise(this Quaternion q) {
      float lengthSquared = q.LengthSquared();
      if ((lengthSquared < 0.02) || (lengthSquared >= 0.8)) return q.normalized; // do it the long way

      float il    = additiveConstant + factor * (lengthSquared - neighborhood);
      var   count = 0;

      while (((lengthSquared * il * il) < limit) && (count++ < 8)) {
        il *= additiveConstant + factor * (lengthSquared - neighborhood);
      }

      if (count > 3) Debug.LogWarning($"Normalise for {q}, sql {lengthSquared} had {count} iterations");

      return new Quaternion(q.x * il, q.y * il, q.z * il, q.w * il);
    }

    private const           float neighborhood     = 0.959066f;
    private const           float scale            = 1.000311f;
    private const           float limit            = 0.9995f * 0.9995f;
    private static readonly float additiveConstant = scale / Mathf.Sqrt(neighborhood);
    private static readonly float factor           = scale * (-0.5f / (neighborhood * Mathf.Sqrt(neighborhood)));

    /// <a href="http://bit.ly/2RezNxw">Gyro is right-handed while Unity is left-handed - so change chirality</a>
    public static Quaternion RightToLeftHanded(this Quaternion q, Trig.Direction axis) {
      q[axis.Ord] = -q[axis.Ord];
      q.w         = -q.w;
      return q;
    }

    /// <a href="http://bit.ly/2Oq9rtU">Gyro is right-handed while Unity is left-handed - so change chirality</a>
    public static Quaternion RotateBy(this Quaternion q, Quaternion attitude) => attitude * q;

    /// <a href="http://bit.ly/2Os3WLf">One person's left is another person's up. More importantly the gyroscope in the phone sees forward as the Z axis while Unity likes to use Y for that. This function also changes the chirality (handedness).</a>
    public static Quaternion SwitchAxis(this Quaternion q, Trig.Direction pivot) {
      q.Set(-q.x, -q.y, -q.z, -q.w); // change chirality
      int   left = otherAxes[pivot.Ord, 0], right = otherAxes[pivot.Ord, 1];
      float swap = q[left];
      q[left]  = q[right];
      q[right] = swap;
      return q.Normalise();
    }

    static int[,] otherAxes = { { 1, 2 }, { 0, 2 }, { 0, 1 } };
  }
}