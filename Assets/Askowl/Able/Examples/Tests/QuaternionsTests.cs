using System;
using NUnit.Framework;
using UnityEngine;
using Random = UnityEngine.Random;

#if UNITY_EDITOR && Able
namespace Askowl.Examples {
  public class QuaternionsTests {
    [Test]
    public void AroundAxis() {
      Reset(test: "RotateBy", setsWithNewSeed: 100, nearRepeats: 1000, maxDegrees: 10);

      Walker(() => {
        var angle = randomAngle;
        var axis  = randomAxis;

        Quaternion rotateBy = Quaternion.Euler(new Vector3 {[axis] = angle});
        rotateBy = Quaternion.AngleAxis(angle, seed.Axis(directions[axis]));
        Quaternion actual = rotateBy * seed;

        Quaternion expected = seed.AroundAxis(directions[axis], angle);

        AreEqual(expected, actual);
      });
    }

    [Test]
    public void RotateBy() {
      Reset(test: "RotateBy", setsWithNewSeed: 100, nearRepeats: 1000, maxDegrees: 10);

      Walker(() => {
        var start    = seed;
        var rotateBy = NextSeed();

        var actual = start * rotateBy;

        var expected = start.RotateBy(rotateBy);

        AreEqual(actual, expected);
      });
    }

    private Trig.Direction[] directions = {Trig.xAxis, Trig.yAxis, Trig.zAxis};

    private void Reset(string test, int setsWithNewSeed, int nearRepeats, float maxDegrees) {
      testName     = test;
      sets         = setsWithNewSeed;
      repetitions  = nearRepeats;
      degreesApart = maxDegrees;
      testCount    = 0;
    }

    private void AreEqual(Quaternion expected, Quaternion actual) {
      var dot = Mathf.Abs(Quaternion.Dot(expected, actual));
      if (dot > 0.9995) return;

      Debug.Log($"seed: {seed.eulerAngles} / {seed}");

      Assert.Fail(
        $"{testName} failed after {testCount} repititions with actual: {q2s(actual)}, expected: {q2s(expected)}");
    }

    private string q2s(Quaternion q) {
      Vector3 e = q.eulerAngles;
      return $"({q.x:F4}, {q.y:F4}, {q.z:F4}, {q.w:F4})/({e.x:F4}, {e.y:F4}, {e.z:F4})";
    }

    private void Walker(Action action) {
      var startTime = Time.realtimeSinceStartup;

      for (int i = 0; i < sets; i++) {
        Reseed();

        for (int j = 0; j < repetitions; j++) {
          testCount++;
          action();
          NextSeed();
        }
      }

      var elapsed = Time.realtimeSinceStartup - startTime;
      Debug.Log($"{testCount} tests run in {elapsed} seconds");
    }

    private Quaternion seed;
    private float      degreesApart = 180;
    private int        sets         = 2, repetitions = 10, testCount;
    private string     testName;
    private float      randomAngle => Random.Range(-degreesApart, +degreesApart);
    private int        randomAxis  => Random.Range(1,             3);

    private Quaternion NextSeed() {
      Vector3 vector = new Vector3 {[randomAxis] = randomAngle};
      return seed *= Quaternion.Euler(vector);
    }

    private void Reseed() =>
      seed = Quaternion.Euler(Random.Range(-180, 360), Random.Range(-180, 360), Random.Range(-180, 360));
  }
}
#endif