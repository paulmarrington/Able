using System;
using UnityEngine;

namespace Askowl.Old {
  /// <summary>
  /// An alternate implementation of Quaternions that uses memory differently
  /// and provide some otherwise missing functionality.<br/>
  /// A Quaternion is a struct. This means it is passed by value 32 bytes per instance.
  /// On the plus side a struct does not use heap unless it is referenced in a class instance.<br/>
  /// A Tetrad works differently. It is a class, so is passed by reference (8 bytes) and kept on the heap.<br/>
  /// To be effective they need to be used differently. Quaternions work best when you need to create
  /// a lot of them for dynamic orientation tasks. Tetrads work best if they live a long time to
  /// store information about a 'thing'. The thing could be a camera or a gyroscope.
  /// Conversion between them is low-cost.
  /// </summary> //#TBD#
  /// <remarks><a href="http://unitydoc.marrington.net/Able#tetradcs-another-perspective-on-quaternions">Tetrads</a></remarks>
  public class Tetrad {
    private double     x, y, z, w;
    private Quaternion quaternion;

    /// <summary>
    /// The identity rotation (Read Only).<br/>
    /// This quaternion corresponds to "no rotation" - the object is perfectly aligned with the world or parent axes.
    /// </summary>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#tetradidentity">Tetrad Identity</a></remarks>
    public static readonly Tetrad Identity = new Tetrad().Set(xx: 0, yy: 0, zz: 0, ww: 1);

    private static Tetrad workingCopy;

    /// <summary>
    /// Since Quaternion is a struct, we can keep and update a single copy per Tetrad.
    /// </summary>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#tetradquaternion">Tetrad Quaternion</a></remarks>
    public Quaternion Quaternion {
      get {
        quaternion.x = (float) x;
        quaternion.y = (float) y;
        quaternion.z = (float) z;
        quaternion.w = (float) w;
        return quaternion;
      }
    }

    /// <summary>
    /// There is one static Tetrad that is used as a temporary register. It is
    /// really intended for use in a single statement, but it is safe until
    /// control is yielded - by the yield statement or the end of Update et al.
    /// </summary>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#tetraddo">Tetrad Do</a></remarks>
    public static Tetrad Do(double x, double y, double z, double w) {
      if (workingCopy == null) workingCopy = new Tetrad();
      return workingCopy.Set(x, y, z, w);
    }

    /// <see cref="Do(double,double,double,double)"/>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#tetraddo">Tetrad Do</a></remarks>
    public static Tetrad Do(Quaternion quaternion) => Do(quaternion.x, quaternion.y, quaternion.z, quaternion.w);

    /// <see cref="Do(double,double,double,double)"/>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#tetraddo">Tetrad Do</a></remarks>
    public static Tetrad Do(Tetrad tetrad) => Do(tetrad.x, tetrad.y, tetrad.z, tetrad.w);

    /// <summary>
    /// Because a Tetrad is used in-place rather than copied there are plenty
    /// of opportunities to change the contents.
    /// </summary>
    /// <returns>the tetrad for command chains</returns>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#tetradset">Tetrad Set</a></remarks>
    public Tetrad Set(double xx, double yy, double zz, double ww) {
      x = xx;
      y = yy;
      z = zz;
      w = ww;
      return this;
    }

    /// <see cref="Set(double,double,double,double)"/>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#tetradset">Tetrad Set</a></remarks>
    public Tetrad Set(float xx, float yy, float zz, float ww) => Set((double) xx, yy, zz, ww);

    /// <see cref="Set(double,double,double,double)"/>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#tetradset">Tetrad Set</a></remarks>
    public Tetrad Set(Quaternion to) => Set(to.x, to.y, to.z, to.w);

    /// <see cref="Set(double,double,double,double)"/>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#tetradset">Tetrad Set</a></remarks>
    public Tetrad Set(Tetrad to) => Set(to.x, to.y, to.z, to.w);

    /// <summary>
    /// Restore the tetrad to identity - no rotation in any direction
    /// </summary>
    /// <returns>the tetrad for command chains</returns>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#tetradreset">Tetrad Set</a></remarks>
    public Tetrad Reset() => Set(to: Identity);

    /// <summary>
    /// Apply one or more rotations in sequence to this tetrad.
    /// </summary>
    /// <param name="attitudes"></param>
    /// <returns>the tetrad for command chains</returns>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#tetradrotateby">Tetrad Rotate By</a></remarks>
    public Tetrad RotateBy(params Quaternion[] attitudes) {
      foreach (var rhs in attitudes) {
        double xx = w * rhs.x + x * rhs.w + y * rhs.z - z * rhs.y;
        double yy = w * rhs.y + y * rhs.w + z * rhs.x - x * rhs.z;
        double zz = w * rhs.z + z * rhs.w + x * rhs.y - y * rhs.x;
        double ww = w * rhs.w - x * rhs.x - y * rhs.y - z * rhs.z;
        x = xx;
        y = yy;
        z = zz;
        w = ww;
      }

      return this;
    }

    /// <see cref="RotateBy(UnityEngine.Quaternion[])"/>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#tetradrotateby">Tetrad Rotate By</a></remarks>
    public Tetrad RotateBy(params Tetrad[] attitudes) {
      foreach (var rhs in attitudes) {
        double xx = w * rhs.x + x * rhs.w + y * rhs.z - z * rhs.y;
        double yy = w * rhs.y + y * rhs.w + z * rhs.x - x * rhs.z;
        double zz = w * rhs.z + z * rhs.w + x * rhs.y - y * rhs.x;
        double ww = w * rhs.w - x * rhs.x - y * rhs.y - z * rhs.z;
        x = xx;
        y = yy;
        z = zz;
        w = ww;
      }

      return this;
    }

    /// <see cref="RotateBy(UnityEngine.Quaternion[])"/>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#tetradrotateby">Tetrad Rotate By</a></remarks>
    public Tetrad RotateBy(Trig.Direction axis, double degrees) {
      var theta = Trig.ToRadians(degrees) / 2;
      var sin   = Math.Sin(theta);
      return RotateBy(Do(axis.x * sin, axis.y * sin, axis.z * sin, Math.Cos(theta)));
    }

    /// <summary>
    /// Retrieve a bearing with respect to a specific axis (X, Y or Z).
    /// </summary>
    /// <param name="axis">Trig.xAxis, yAxis or xzAxis</param>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#tetradbearing">Bearing against an Axis</a></remarks>
    public double BearingInDegreesFor(Trig.Direction axis) =>
      Trig.ToDegrees(BearingInRadiansFor(axis));

    /// <see cref="BearingInDegreesFor"/>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#tetradbearing">Bearing against an Axis</a></remarks>
    public double BearingInRadiansFor(Trig.Direction axis) =>
      Math.Atan2((axis.x * x) + (axis.y * y) + (axis.z * z), w);

    /// <summary>
    /// Rotate tetrad around an axis by a specified number of degrees then normalise
    /// </summary>
    /// <param name="axis">Trig.xAxis, yAxis or zAxis</param>
    /// <param name="degrees">Amount to rotate by</param>
    /// <returns>the tetrad for command chains</returns>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#tetradbearing">Bearing against an Axis</a></remarks>
    public Tetrad AngleAxis(Trig.Direction axis, double degrees) {
      var theta = Trig.ToRadians(degrees) / 2;
      var sin   = Math.Sin(theta);

      if (axis.x != 0) x = sin;
      if (axis.y != 0) y = sin;
      if (axis.z != 0) z = sin;
      w = Math.Cos(theta);

      return Normalize();
    }

    /// <summary>
    /// The inverse used when the Tetrad is a rotation - and is the opposite.
    /// In other words apply rotation then apply inverse and nothing will have changed.
    /// </summary>
    /// <returns>the tetrad for command chains</returns>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#tetradinverse">Invert the Rotation Tetrad</a></remarks>
    public Tetrad Inverse() => Conjugate().Multiply(scalar: 1.0 / LengthSquared).Normalize();

    /// <summary>
    /// Multiplying a quaternion by a real number scales its norm by the absolute value of the number.<br/>
    /// Multiply any vector by any scalar a, is in general to change its length in a known ratio, and to preserve or reverse its direction
    /// </summary>
    /// <returns>the tetrad for command chains</returns>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#tetradmultiply">Multiply a Tetrad by a Scalar</a></remarks>
    public Tetrad Multiply(double scalar) => Set(x * scalar, y * scalar, z * scalar, w * scalar);

    /// <summary>
    /// The negative of a quaternion represents the same rotation, just that the axis and angle have both been reversed.
    /// </summary>
    /// <returns>the tetrad for command chains</returns>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#tetradnegate">Negate a Tetrad</a></remarks>
    public Tetrad Negate() => Set(xx: -x, yy: -y, zz: -z, ww: -w);

    /// <summary>
    /// Multiplying a quaternion by its conjugate gives a real number.
    /// This makes the conjugate useful for finding the multiplicative inverse.
    /// For instance, if we are using a quaternion q to represent a rotation then
    /// conj(q) represents the same rotation in the reverse direction.
    /// </summary>
    /// <returns>the tetrad for command chains</returns>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#tetradconjugate">Conjugate a Tetrad</a></remarks>
    public Tetrad Conjugate() => Set(xx: -x, yy: -y, zz: -z, ww: w);

    /// <summary>
    /// When Slerp is applied to unit quaternions, the quaternion path maps to a
    /// path through 3D rotations in a standard way. The effect is a rotation
    /// with uniform angular velocity around a fixed rotation axis. (Wikipedia)
    /// </summary>
    /// <remarks>Note that this Slerp calculates everything every time. if you
    /// are going to do a lot of slerping where you are just changing the delta,
    /// then we need to create a routine/struct that keeps and uses state.</remarks>
    /// <remarks>slerp that walks along the unit sphere in 4-dimensional space from
    /// one quaternion to the other. Because it's navigating a sphere, it involves
    /// a fair amount of trigonometry, and is correspondingly slow.</remarks>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="percentage"></param>
    /// <returns>the tetrad for command chains</returns>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#tetradeslerp">Tetrad Expensive Slerp</a></remarks>
    public Tetrad ESlerp(Tetrad start, Tetrad end, float percentage) {
      // Only unit quaternions are valid rotations.
      // Normalize to avoid undefined behavior.
      start.Normalize();
      end.Normalize();
      // if either input is zero, return the other.
      bool startToSmall = (start.LengthSquared < 0.000001f);
      bool endToSmall   = (end.LengthSquared   < 0.000001f);
      if (startToSmall) return endToSmall ? Set(Identity) : Set(end);
      if (endToSmall) return Set(start);

      double cosHalfAngle = start.Dot(end);

      if ((cosHalfAngle >= 1.0f) || (cosHalfAngle <= -1.0f)) {
        return Set(start); // angle = 0.0f, so just return one input.
      }

      // If the dot product is negative, slerp won't take the shorter path.
      // Note that start and -start are equivalent when the negation is applied
      // to all four components. Fix by  reversing one quaternion.
      if (cosHalfAngle < 0.0f) { // Warning: modifies end
        end.Negate();
        cosHalfAngle = -cosHalfAngle;
      }

      double fromStart, fromEnd;

      if (cosHalfAngle < 0.99f) { // do proper slerp for big angles (below a threshold)
//        double halfAngleStartToEnd = Math.Acos(cosHalfAngle);
//        double halfAngleStartToResult = halfAngleStartToEnd * delta;
//        double sinHalfStartAngle = Math.Sin(halfAngleStartToEnd);
//        double sinHalfResultAngle = Math.Sin(halfAngleStartToResult);
//        double cosHalfResultAngle = Math.Cos(halfAngleStartToResult);
//
//        fromStart = cosHalfResultAngle - cosHalfAngle * sinHalfResultAngle / sinHalfStartAngle;
//        fromEnd = sinHalfResultAngle / sinHalfStartAngle;

        double halfAngle           = Math.Acos(cosHalfAngle); // start to end
        double sinHalfAngle        = Math.Sin(halfAngle);
        double oneOverSinHalfAngle = 1.0f / sinHalfAngle;
        fromStart = Math.Sin(halfAngle * (1.0f - percentage)) * oneOverSinHalfAngle;
        fromEnd   = Math.Sin(halfAngle * percentage)          * oneOverSinHalfAngle;
      } else { // do lerp if angle is really small.
        fromStart = 1 - percentage;
        fromEnd   = percentage;
      }

      x = fromStart * start.x + fromEnd * end.x;
      y = fromStart * start.y + fromEnd * end.y;
      z = fromStart * start.z + fromEnd * end.z;
      w = fromStart * start.w + fromEnd * end.w;

      return (LengthSquared > 0.0f) ? Normalize() : Set(Identity);
    }

    /// <summary>
    ///  Linearly interpolates between two Tetrads, with spherical adjustments.
    /// </summary>
    /// <param name="start">The first source Tetrad.</param>
    /// <param name="end">The second source Tetrad.</param>
    /// <param name="percentage">The relative weight of the second source Tetrad in the interpolation.</param>
    /// <returns>the tetrad for command chains</returns>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#tetradlerp">Tetrad Lerp</a></remarks>
//    public Tetrad ALerp(Tetrad start, Tetrad end, float percentage) {
//      float left = percentage, right = 1f - percentage;
//
//      Quaternion r = new Quaternion();
//
//      float dot = start.X * end.X + start.Y * end.Y +
//                  start.Z * end.Z + start.W * end.W;
//
//      if (dot >= 0.0f) {
//        r.X = right * start.X + left * end.X;
//        r.Y = right * start.Y + left * end.Y;
//        r.Z = right * start.Z + left * end.Z;
//        r.W = right * start.W + left * end.W;
//      } else {
//        r.X = right * start.X - left * end.X;
//        r.Y = right * start.Y - left * end.Y;
//        r.Z = right * start.Z - left * end.Z;
//        r.W = right * start.W - left * end.W;
//      }
//
//      // Normalize it.
//      float ls      = r.X * r.X + r.Y * r.Y + r.Z * r.Z + r.W * r.W;
//      float invNorm = 1.0f / (float) Math.Sqrt((double) ls);
//
//      r.X *= invNorm;
//      r.Y *= invNorm;
//      r.Z *= invNorm;
//      r.W *= invNorm;
//
//      return r;
//    }

    /// <summary>
    /// When normalized, a quaternion keeps the same orientation but its magnitude is 1.0<br/>
    /// Using quaternions for rotation and attitude requires unit length for the math to work.
    /// Hoever, the more often you normalise, the more floating point rounding occurs.
    /// </summary>
    /// <returns>the tetrad for command chains</returns>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#tetradnormalize">Normalise a Tetrad</a></remarks>
    public Tetrad Normalize() {
      var length = Length;
      return (length <= 0) ? Set(Identity) : Multiply(1 / length);
    }

    /// <summary>
    /// This is not the length of anything in particular. It is only a length in
    /// that if you divide all four entries in a quaternion by this amount then
    /// you will have a unit-length quaternion.
    /// </summary>
    public double Length => Math.Sqrt(LengthSquared);

    /// <summary>
    /// If you only want to see if a tetrad is normalised, use this as it
    /// requires considerably less calculation.
    /// </summary>
    public double LengthSquared => (x * x + y * y + z * z + w * w);

    /// <summary>
    /// Return the cosine of the angle between two tetrads.
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public double Dot(Tetrad other) => x * other.x + y * other.y + z * other.z + w * other.w;

    // Have to conjugate when we switch axes
    public Tetrad SwitchAxis(Trig.Direction pivot) {
      if (pivot.x != 0) return Set(-x, -z, -y, w);
      if (pivot.y != 0) return Set(-z, -y, -x, w);

      return Set(-y, -x, -z, w);
    }

    /// Gyro is right-handed while Unity is left-handed - so change Chirilty
    public Tetrad RightToLeftHanded() => Set(x, y, -z, -w);

    public override string ToString() => $"({x:n1}, {y:n1}, {z:n1}, {w:n1})";

    /// <summary>
    /// A much faster normalise, accurate to a dot product of 0.9995 or better.
    /// </summary>
    /// <param name="q"></param>
    /// <returns></returns>
    public static Quaternion Normalise(Quaternion q) {
      float lengthSquared = 1;                                                   //q.LengthSquared();
      if ((lengthSquared < 0.02) || (lengthSquared >= 0.8)) return q.normalized; // do it the long way

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