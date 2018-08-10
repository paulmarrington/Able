using System;
using NUnit.Framework;
using UnityEngine;
using Random = UnityEngine.Random;

#if UNITY_EDITOR && Able
namespace Askowl.Examples {
  public class QuaternionsTests {
    [Test]
    public void Normalise() {
      Reset("Normalise", 1, 100000, 0);

      Walker(() => {
        DenormaliseSeed();
        AreEqual(seed.normalized, seed.Normalise());
      });
    }

    private void Reset(string test, int setsWithNewSeed, int nearRepeats, float maxDegrees) {
      testName     = test;
      sets         = setsWithNewSeed;
      repetitions  = nearRepeats;
      degreesApart = maxDegrees;
      testCount    = slowNormaliseCount = 0;
    }

    private void DenormaliseSeed() {
      for (int i = 0; i < 4; i++) {
        seed[i] = Random.Range(-0.5f, 0.5f);
      }
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
          if (seed.LengthSquared() >= 0.8) slowNormaliseCount++;
          action();
          NextSeed();
        }
      }

      var elapsed = Time.realtimeSinceStartup - startTime;
      Debug.Log($"{testCount} tests run in {elapsed} seconds, {slowNormaliseCount} using slow normalise");
    }

    private Quaternion seed;
    private float      degreesApart = 180;
    private int        sets         = 2, repetitions = 10, testCount, slowNormaliseCount;
    private string     testName;

    private void NextSeed() {
      var     angle  = Random.Range(-degreesApart, +degreesApart);
      var     axis   = Random.Range(1,             3);
      Vector3 vector = new Vector3 {[axis] = angle};
      seed *= Quaternion.Euler(vector);
    }

    private void Reseed() =>
      seed = Quaternion.Euler(Random.Range(-180, 360), Random.Range(-180, 360), Random.Range(-180, 360));
  }
}
#endif