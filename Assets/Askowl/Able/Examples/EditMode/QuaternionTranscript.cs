//- Quaternions are truly mind-bending. Fortunately, Unity has done most of the work for us. As always, need always seems to exceed supply. The Askowl.Quaternion class adds a few extension methods with useful additions.

namespace Askowl.Examples {
  using NUnit.Framework;
  using UnityEngine;

  public class QuaternionTranscript {
    [Test] public void Example() {
      Quaternion quaternion  = Quaternion.Euler(x: 123, y: -55,  z: 271);
      Quaternion quaternion2 = Quaternion.Euler(x: -88, y: -122, z: -33);
      //- Rotate quaternion around an axis by a specified number of degrees then normalise. This is the same as the built-in AngleAxis, but considerable more readable and so easier to read.
      quaternion.AroundAxis(Trig.YAxis, 90.0f);
      //- Inverse reversed the rotation component of a quaternion. It uses the built-in Inverse but allows chaining. This example returns the original quaternion.
      var doubleInverse = quaternion.Inverse().Inverse();
      //- The concept of length or magnitude for a quaternion has no visual representation when dealing with attitude or rotation. The catch is that most algorithms require unit quaternions - where the length squared will approach one.
      float lengthSquared = quaternion.LengthSquared();
      //- This normalise is faster while maintaining an accuracy to a dot product of 0.9995 or better.
      var normalised = quaternion.Normalise();
      //- The iOS gyro is right-handed while Unity is left-handed - so change chirality around the selected axis. Repeating it twice will return the original value.
      var doubleHanded = quaternion.RightToLeftHanded(Trig.ZAxis).RightToLeftHanded(Trig.ZAxis);
      //- Multiplying quaternions may make sense to mathematicians, but to the rest of is RotateBy is clearer
      var rotated = quaternion.RotateBy(attitude: quaternion2);
      //- ne person's left is another person's up. More importantly the gyroscope in the phone sees forward as the Z axis while Unity likes to use Y for that. This function also changes the chirality (handedness).
      var switched = quaternion.SwitchAxis(pivot: Trig.XAxis);
    }
  }
}