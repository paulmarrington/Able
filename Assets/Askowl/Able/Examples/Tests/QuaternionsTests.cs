using System;
using System.Diagnostics;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Analytics;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

#if UNITY_EDITOR && Able
namespace Askowl.Examples {
  public class QuaternionsTests {
    /// <remarks><a href="http://unitydoc.marrington.net/Able#axis">Trig Axis to Vector3</a></remarks>
    [Test]
    public void Axis() {
      Assert.AreEqual(Vector3.right,   seed.Axis(Trig.xAxis));
      Assert.AreEqual(Vector3.up,      seed.Axis(Trig.yAxis));
      Assert.AreEqual(Vector3.forward, seed.Axis(Trig.zAxis));
    }

    /// <remarks><a href="http://unitydoc.marrington.net/Able#aroundaxis">Rotate round an XYZ axis</a></remarks>
    [Test]
    public void AroundAxis() {
      Reset(test: "RotateBy", setsWithNewSeed: 100, nearRepeats: 1000, maxDegrees: 10);

      Walker(() => {
        var angle = randomAngle;
        var axis  = randomAxis;

        Quaternion rotateBy = Quaternion.AngleAxis(angle, seed.Axis(directions[axis]));
        Quaternion actual   = rotateBy * seed;

        Quaternion expected = seed.AroundAxis(directions[axis], angle);

        AreEqual(expected, actual);
      });
    }

    /// <remarks><a href="http://unitydoc.marrington.net/Able#inverse">Invert the Quaternion Rotation</a></remarks>
    [Test]
    public void Inverse() {
      Reset(test: "Inverse", setsWithNewSeed: 100, nearRepeats: 1000, maxDegrees: 180);

      Walker(() => {
        var expected = Quaternion.Inverse(seed);

        var actual = seed.Inverse();

        AreEqual(actual, expected);
      });
    }

    /// <remarks><a href="http://unitydoc.marrington.net/Able#lengthsquared">Magnitude of a Quaternion**2</a></remarks>
    [Test]
    public void LengthSquared() {
      Reset(test: "LengthSquared", setsWithNewSeed: 100, nearRepeats: 1000, maxDegrees: 180);

      Walker(() => {
        var expected = 1;

        var actual = seed.normalized.LengthSquared();

        Assert.IsTrue(Compare.AlmostEqual(expected, actual));
      });
    }

    [Test]
    public void Normalise() {
      Reset("Normalise", 1, 100000, 0);

      Walker(() => {
        DenormaliseSeed();
        var expected = seed.normalized;

        var actual = seed.Normalise();

        AreEqual(actual, expected);
      });
    }

    [Test]
    public void NormaliseSpeed() {
      Quaternion a = seed;
      DenormaliseSeed();

      var expected = SpeedTest("normalized", 10, 1000000, () => a = seed.normalized);

      var actual = SpeedTest("Normalise", 10, 1000000, () => a = seed.Normalise());

      seed = a;
      int improvement = (int) ((1 - (float) actual / expected) * 100);
      Debug.Log($"A {improvement}% improvement");
      Assert.Greater(improvement, 10); // better than 10%
    }

    /// <remarks><a href="http://unitydoc.marrington.net/Able#righttolefthanded">Change chirality</a></remarks>
    [Test]
    public void RightToLeftHanded() {
      Reset(test: "RightToLeftHanded", setsWithNewSeed: 100, nearRepeats: 1000, maxDegrees: 180);

      Walker(() => {
        var expected = seed;

        var actual = seed.RightToLeftHanded(Trig.zAxis).RightToLeftHanded(Trig.zAxis);

        AreEqual(expected, actual);
      });
    }

    /// <remarks><a href="http://unitydoc.marrington.net/Able#rotateby">Quaternion Rotate By another</a></remarks>
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

    private long SpeedTest(string name, int passes, long loops, Action action) {
      var    watch = new Stopwatch();
      string ms    = "";
      long   total = 0;

      for (int i = 0; i < passes; i++) {
        watch.Restart();

        for (int j = 0; j < loops; j++) action();

        watch.Stop();
        ms    += $"{watch.ElapsedMilliseconds}  ";
        total += watch.ElapsedMilliseconds;
      }

      var average = total / passes;
      Debug.Log($"{name}: {ms} = {average}");
      return average;
    }

    private void DenormaliseSeed() {
      for (int i = 0; i < 4; i++) {
        seed[i] = Random.Range(-0.5f, 0.5f);
      }
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