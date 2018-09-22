// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

#if AskowlAble
namespace Askowl.Examples {
  using System;
  using System.Diagnostics;
  using NUnit.Framework;
  using UnityEngine;
  using Debug = UnityEngine.Debug;
  using Random = UnityEngine.Random;

  public class QuaternionsExamples {
    [Test]
    public void AroundAxis() {
      Reset(test: "RotateBy", setsWithNewSeed: 100, nearRepeats: 1000, maxDegrees: 10);

      Walker(
        () => {
          var angle = randomAngle;
          var axis  = randomAxis;

          var rotateBy = Quaternion.AngleAxis(angle, directions[axis].Vector);
          var actual   = rotateBy * seed;

          var expected = seed.AroundAxis(directions[axis], angle);

          AreEqual(expected, actual);
        });
    }

    [Test]
    public void Inverse() {
      Reset(test: "Inverse", setsWithNewSeed: 100, nearRepeats: 1000, maxDegrees: 180);

      Walker(
        () => {
          var expected = Quaternion.Inverse(seed);

          var actual = seed.Inverse();

          AreEqual(actual, expected);
        });
    }

    [Test]
    public void LengthSquared() {
      Reset(test: "LengthSquared", setsWithNewSeed: 100, nearRepeats: 1000, maxDegrees: 180);

      Walker(
        () => {
          var expected = 1;

          var actual = seed.normalized.LengthSquared();

          Assert.IsTrue(Compare.AlmostEqual(expected, actual));
        });
    }

    [Test]
    public void Normalise() {
      Reset("Normalise", 1, 100000, 180);

      Walker(
        () => {
          DenormaliseSeed();
          var expected = seed.normalized;

          var actual = seed.Normalise();

          AreEqual(actual, expected);
        });
    }

    [Test]
    public void NormaliseSpeed() {
      var a = Reseed();
      DenormaliseSeed();

      var expected = SpeedTest("normalized", 10, 1000000, () => a = seed.normalized);

      var actual = SpeedTest("Normalise", 10, 1000000, () => a = seed.Normalise());

      seed = a;
      var improvement = (int) ((1 - (float) actual / expected) * 100);
      Debug.Log($"A {improvement}% improvement");
      Assert.Greater(improvement, 10); // better than 10%
    }

    [Test]
    public void RightToLeftHanded() {
      Reset(test: "RightToLeftHanded", setsWithNewSeed: 100, nearRepeats: 1000, maxDegrees: 180);

      Walker(
        () => {
          var expected = seed;

          var actual = seed.RightToLeftHanded(Trig.zAxis).RightToLeftHanded(Trig.zAxis);

          AreEqual(expected, actual);
        });
    }

    [Test]
    public void RotateBy() {
      Reset(test: "RotateBy", setsWithNewSeed: 100, nearRepeats: 1000, maxDegrees: 180);

      Walker(
        () => {
          var start    = seed;
          var rotateBy = NextSeed();

          var expected = rotateBy * start;

          var actual = start.RotateBy(rotateBy);

          AreEqual(actual, expected);
        });
    }

    [Test]
    public void SwitchAxis() {
      Reset(test: "SwitchAxis", setsWithNewSeed: 100, nearRepeats: 1000, maxDegrees: 180);

      Walker(
        () => {
          var axis = Trig.xAxis;

          var euler    = seed.eulerAngles;
          var expected = Quaternion.Euler(euler.x, euler.z, euler.y);

          var actual = seed.SwitchAxis(axis);

          AreEqual(actual, expected);
        });
    }

    private long SpeedTest(string name, int passes, long loops, Action action) {
      var  watch = new Stopwatch();
      var  ms    = "";
      long total = 0;

      for (var i = 0; i < passes; i++) {
        watch.Restart();

        for (var j = 0; j < loops; j++) action();

        watch.Stop();
        ms    += $"{watch.ElapsedMilliseconds}  ";
        total += watch.ElapsedMilliseconds;
      }

      var average = total / passes;
      Debug.Log($"{name}: {ms} = {average}");
      return average;
    }

    private void DenormaliseSeed() {
      for (var i = 0; i < 4; i++) seed[i] = Random.Range(-0.5f, 0.5f);
    }

    private Trig.Direction[] directions = { Trig.xAxis, Trig.yAxis, Trig.zAxis };

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

      Assert.Fail(
        $"{testName} failed after {testCount} repetitions with actual: {q2s(actual)}, expected: {q2s(expected)}");
    }

    private string q2s(Quaternion q) {
      var e = q.eulerAngles;
      return $"({q.x:F4}, {q.y:F4}, {q.z:F4}, {q.w:F4})/({e.x:F4}, {e.y:F4}, {e.z:F4})";
    }

    private void Walker(Action action) {
      var startTime = Time.realtimeSinceStartup;

      for (var i = 0; i < sets; i++) {
        Reseed();

        for (var j = 0; j < repetitions; j++) {
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
      var vector = new Vector3 { [randomAxis] = randomAngle };
      return seed *= Quaternion.Euler(vector);
    }

    private Quaternion Reseed() => seed = Quaternion.Euler(
      Random.Range(-180, 360), Random.Range(-180, 360), Random.Range(-180, 360));
  }
}
#endif