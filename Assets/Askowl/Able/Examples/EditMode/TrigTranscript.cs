//- It is time for a simple one - trigonometry functions to make some of our Unity scripting a bit easier. Trigonometry is about triangles. For us that translates into distance and direction.

#if AskowlTests
namespace Askowl.Able.Transcript {
  using NUnit.Framework;
  using UnityEngine;

  public class TrigTranscript {
    [Test] public void Example() {
      //- To use trig for spacial calculations, we anchor once side of the triangle to an axis starting at the origin with the direction indicated by the sign. X, -X, Y, -Y, Z, -Z.
      Trig.Direction xRight = Trig.XAxis;
      Trig.Direction xLeft = -Trig.XAxis;
      Trig.Direction yUp = Trig.YAxis;
      Trig.Direction yDown = -Trig.YAxis;
      Trig.Direction zTowards = Trig.ZAxis;
      Trig.Direction zAway = -Trig.ZAxis;
      //- The Trip.Direction class contains data translations for the different perspectives we have on directions and angles.
      Trig.Direction aDirection = Trig.XAxis;
      Vector3        unityVector = new Vector3(x: 12, y: 72, z: 34);
      //- We can write code that shows which axis we are working.
      float xComponent = unityVector[Trig.XAxis.Ord];
      //- Direction instances have normalised X, Y and Z members where only one is non-zero. It will be either 1 or -1. Use it to zero out unwanted vector components.
      unityVector.Set(unityVector.x * aDirection.X, unityVector.y * aDirection.Y, unityVector.z * aDirection.Z);
      //- If the ordinal for direction fits with your code, then a more compact solution could be used.
      for (var i = 0; i <= 2; i++) unityVector[i] *= aDirection[i];
    }
  }
}
#endif