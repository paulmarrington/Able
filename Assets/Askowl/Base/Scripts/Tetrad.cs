using UnityEngine;

namespace Askowl {
  public class Tetrad {
    private double x, y, z, w;

    public static readonly Tetrad Identity = (new Tetrad()).Set(x: 0, y: 0, z: 0, w: 1);

    public Tetrad Set(float x, float y, float z, float w) {
      this.x = x;
      this.y = y;
      this.z = z;
      this.w = w;
      return this;
    }

    public Tetrad Set(double x, double y, double z, double w) {
      this.x = x;
      this.y = y;
      this.z = z;
      this.w = w;
      return this;
    }

    public Tetrad Set(Quaternion to) {
      Set(to.x, to.y, to.z, to.w);
      return this;
    }

    public Tetrad Set(Tetrad to) {
      Set(to.x, to.y, to.z, to.w);
      return this;
    }

    public Tetrad Rotate(params Quaternion[] attitudes) {
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

    public Tetrad Rotate(params Tetrad[] attitudes) {
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

    public Tetrad Multiply(float scalar) {
      x *= scalar;
      y *= scalar;
      z *= scalar;
      w *= scalar;
      return this;
    }

    public Tetrad Negate() {
      x = -x;
      y = -y;
      z = -z;
      w = -w;
      return this;
    }

    private Tetrad Slerp(Tetrad a, Tetrad b, float t) {
      // if either input is zero, return the other.
      bool startToSmall = (a.LengthSquared < 0.000001f);
      bool endToSmall   = (b.LengthSquared < 0.000001f);
      if (startToSmall) return endToSmall ? Set(Identity) : Set(b);
      if (endToSmall) return Set(a);

      float cosHalfAngle = a.Dot(b);

      if ((cosHalfAngle >= 1.0f) || (cosHalfAngle <= -1.0f)) {
        return Set(a); // angle = 0.0f, so just return one input.
      } else if (cosHalfAngle < 0.0f) {
        b.Negate();
        cosHalfAngle = -cosHalfAngle;
      }

      float blendA;
      float blendB;

      if (cosHalfAngle < 0.99f) { // do proper slerp for big angles
        float halfAngle           = Mathf.Acos(cosHalfAngle);
        float sinHalfAngle        = Mathf.Sin(halfAngle);
        float oneOverSinHalfAngle = 1.0f / sinHalfAngle;
        blendA = Mathf.Sin(halfAngle * (1.0f - t)) * oneOverSinHalfAngle;
        blendB = Mathf.Sin(halfAngle * t)          * oneOverSinHalfAngle;
      } else { // do lerp if angle is really small.
        blendA = 1 - t;
        blendB = t;
      }

      x = blendA * a.x + blendB * b.x;
      y = blendA * a.y + blendB * b.y;
      z = blendA * a.z + blendB * b.z;
      w = blendA * a.w + blendB * b.w;

      return (LengthSquared > 0.0f) ? Normalize() : Set(Identity);
    }

    public Tetrad Normalize() { return Multiply(scalar: 1f / Length); }

    public float Length { get { return Mathf.Sqrt(LengthSquared); } }

    public float LengthSquared { get { return (float) (x * x + y * y + z * z + w * w); } }

    public float Dot(Tetrad other) {
      return (float) (x * other.x + y * other.y + z * other.z + w * other.w);
    }

    /// Gyro is right-handed while Unity is left-handed.
    public Tetrad RightToLeftHanded() {
      z = -z;
      w = -w;
      return this;
    }
  }
}