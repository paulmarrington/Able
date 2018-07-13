﻿using System;
using UnityEngine;

namespace Askowl {
  public class Tetrad {
    private double     x, y, z, w;
    private Quaternion quaternion;

    public static readonly Tetrad Identity = new Tetrad().Set(xx: 0, yy: 0, zz: 0, ww: 1);
    private static         Tetrad workingCopy;

    public class Direction {
      internal int x, y, z;
    }

    public static readonly Direction xAxis = new Direction {x = 1};
    public static readonly Direction yAxis = new Direction {y = 1};
    public static readonly Direction zAxis = new Direction {z = 1};

    public Quaternion Quaternion {
      get {
        quaternion.x = (float) x;
        quaternion.y = (float) y;
        quaternion.z = (float) z;
        quaternion.w = (float) w;
        return quaternion;
      }
    }

    public static Tetrad Do(double x, double y, double z, double w) {
      if (workingCopy == null) workingCopy = new Tetrad();
      return workingCopy.Set(x, y, z, w);
    }

    public static Tetrad Do(Quaternion quaternion) =>
      Do(quaternion.x, quaternion.y, quaternion.z, quaternion.w);

    public static Tetrad Do(Tetrad tetrad) => Do(tetrad.x, tetrad.y, tetrad.z, tetrad.w);

    public Tetrad Set(double xx, double yy, double zz, double ww) {
      x = xx;
      y = yy;
      z = zz;
      w = ww;
      return this;
    }

    public Tetrad Set(float xx, float yy, float zz, float ww) => Set((double) xx, yy, zz, ww);

    public Tetrad Set(Quaternion to) => Set(to.x, to.y, to.z, to.w);

    public Tetrad Set(Tetrad to) => Set(to.x, to.y, to.z, to.w);

    public Tetrad Reset() => Set(Identity);

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

    public Tetrad RotateBy(Direction axis, double degrees) {
      var theta = Trig.ToRadians(degrees) / 2;
      var sin   = Math.Sin(theta);
      return RotateBy(Do(axis.x * sin, axis.y * sin, axis.z * sin, Math.Cos(theta)));
    }

    public double BearingInDegreesFor(Direction axis) {
      var radians = Math.Atan2((axis.x * x) + (axis.y * y) + (axis.z * z), w);
      return Trig.ToDegrees(BearingInRadiansFor(axis));
    }

    public double BearingInRadiansFor(Direction axis) {
      return Math.Atan2((axis.x * x) + (axis.y * y) + (axis.z * z), w);
    }

    public Tetrad AngleAxis(Direction axis, double degrees) {
      var theta = Trig.ToRadians(degrees) / 2;
      var sin   = Math.Sin(theta);

      x = axis.x * sin;
      y = axis.y * sin;
      z = axis.z * sin;
      w = Math.Cos(theta);

      return Normalize();
    }

    /// remove rotation from the axis indicated
    public Tetrad ZeroAxis(Direction axis) {
      var radians = Math.Atan2((axis.x * x) + (axis.y * y) + (axis.z * z), w) / 2;
      Debug.Log($"**** Tetrad:114 theta={Trig.ToDegrees(radians)}"); //#DM#//
      var sin = Math.Sin(radians);
      return RotateBy(Do(axis.x * sin, axis.y * sin, axis.z * sin, Math.Cos(radians)));
    }

    // ReSharper disable once UnusedMethodReturnValue.Global
    public Tetrad Inverse() => Conjugate().Multiply(scalar: 1.0 / LengthSquared);

    public Tetrad Multiply(double scalar) =>
      Set(x * scalar, y * scalar, z * scalar, w * scalar).Normalize();

    // ReSharper disable once UnusedMethodReturnValue.Global
    public Tetrad Negate() => Set(xx: -x, yy: -y, zz: -z, ww: -w);

    public Tetrad Conjugate() => Set(xx: -x, yy: -y, zz: -z, ww: w);

    // ReSharper disable once UnusedMethodReturnValue.Global
    public Tetrad Slerp(Tetrad start, Tetrad end, float delta) {
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
        fromStart = Math.Sin(halfAngle * (1.0f - delta)) * oneOverSinHalfAngle;
        fromEnd   = Math.Sin(halfAngle * delta)          * oneOverSinHalfAngle;
      } else { // do lerp if angle is really small.
        fromStart = 1 - delta;
        fromEnd   = delta;
      }

      x = fromStart * start.x + fromEnd * end.x;
      y = fromStart * start.y + fromEnd * end.y;
      z = fromStart * start.z + fromEnd * end.z;
      w = fromStart * start.w + fromEnd * end.w;

      return (LengthSquared > 0.0f) ? Normalize() : Set(Identity);
    }

    public Tetrad Normalize() {
      var length = Length;
      if (length <= 0) return Set(Identity);

      var scalar = 1f / length;

      return Set(x * scalar, y * scalar, z * scalar, w * scalar);
    }

    public double Length => Math.Sqrt(LengthSquared);

    public double LengthSquared => (x * x + y * y + z * z + w * w);

    public double Dot(Tetrad other) => x * other.x + y * other.y + z * other.z + w * other.w;

    // Have to conjugate when we switch axes
    public Tetrad SwitchAxis(Direction to) {
      if (to.x == 0) return Set(-x, -z, -y, w);
      if (to.y == 0) return Set(-z, -y, -x, w);

      return Set(-y, -x, -z, w);
    }

    /// Gyro is right-handed while Unity is left-handed - so change Chirilty
    // ReSharper disable once UnusedMethodReturnValue.Global
    public Tetrad RightToLeftHanded() => Set(x, y, -z, -w);

    public override string ToString() => $"({x:n1}, {y:n1}, {z:n1}, {w:n1})";
  }
}