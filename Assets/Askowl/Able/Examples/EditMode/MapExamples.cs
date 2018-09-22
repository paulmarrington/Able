// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

#if AskowlAble
namespace Askowl.Examples {
  using System;
  using System.Collections.Generic;
  using System.Diagnostics;
  using NUnit.Framework;
  using Debug = UnityEngine.Debug;

  public class MapExamples {
    [Test]
    public void NewMap() {
      var map = new Map();
      Assert.AreEqual(expected: 0, actual: map.Count);
    }

    [Test]
    public void Count() {
      var map = new Map("One", 1, "Two", 2, "Three", 3);

      Assert.AreEqual(expected: 3, actual: map.Count);
    }

    [Test]
    public void Set() {
      var set = new Map().Set(1, 3, 5, 7, 9);

      Assert.AreEqual(expected: 5, actual: set.Count);

      set.Set(11, 13, 15);

      Assert.AreEqual(expected: 8, actual: set.Count);
    }

    [Test]
    public void Add() {
      var map  = new Map("One", 1, "Two", 2, "Three", 3);
      var more = new object[] { "Six", 6, "Seven", 7 };

      map.Add("Four", 4, "Five", 5).Add(more);

      Assert.AreEqual(expected: 7, actual: map.Count);
    }

    [Test]
    public void Remove() {
      var map = new Map("One", 1, "Two", 2, "Three", 3);

      map.Remove("Two", "One");
      Assert.AreEqual(1, map.Count);
    }

    [Test]
    public void Found() {
      var map = new Map("One", 111, "Two", "2", "Three", new Map());

      Assert.IsTrue(map["Two"].Found);

      Assert.IsFalse(map["TryMe"].Found);
    }

    [Test]
    public void Key() {
      var map = new Map("One", 111, "Two", "2", "Three", new Map());

      Assert.AreEqual(expected: "Two", actual: map["Two"].Key);
    }

    [Test]
    public void Value() {
      var map = new Map("One", 111, "Two", "2", "Three", new Map());

      Assert.AreEqual(expected: "2", actual: map["Two"].Value);

      Assert.IsNull(map["TryMe"].Value);
    }

    [Test]
    public void StringKeyIteration() {
      var map  = new Map("One", 111, "Two", "2", "Three", new Map());
      var keys = "";

      for (var key = map.First; key != null; key = map.Next) keys += key;

      Assert.AreEqual("OneTwoThree", keys);
    }

    [Test]
    public void ObjectKeyIteration() {
      var map  = new Map("One", 111, "Two", "2", "Three", new Map());
      var keys = "";

      for (var i = 0; i < map.Count; i++) keys += map[i];

      Assert.AreEqual("OneTwoThree", keys);
    }

    [Test]
    public void Sort() {
      var map    = new Map("One", 1, "Four", 4, "Three", 3, "Five", 5, "Two", 2);
      var actual = "";

      for (var key = map.Sort().First; key != null; key = map.Next) actual += key;

      Assert.AreEqual("FiveFourOneThreeTwo", actual);
    }

    [Test]
    public void SortT() {
      var map    = new Map("One", 1, "Four", 4, "Three", 3, "Five", 5, "Two", 2);
      var actual = "";

      map.Sort((x, y) => ((int) map[x].Value).CompareTo((int) map[y].Value));
      for (var key = map.First; key != null; key = map.Next) actual += key;

      Assert.AreEqual("OneTwoThreeFourFive", actual);
    }

    [Test]
    public void ArrayGet() {
      var map = new Map("One", 1, "Four", 4, "Three", 3, "Five", 5, "Two", 2);

      Assert.IsTrue(map["One"].Found);
      Assert.IsTrue(map["Three"].Found);
      Assert.IsTrue(map["Five"].Found);
      Assert.IsFalse(map["Seven"].Found);
    }

    private static int disposals;

    private struct Disposable : IDisposable {
      public void Dispose() { disposals += 1; }
    }

    [Test]
    public void Dispose() {
      var values = new Disposable[3];
      var map    = new Map("One", values[0], "Two", values[1], "Three", values[2]);
      disposals = 0;
      map.Dispose();
      Assert.AreEqual(3, disposals);
      Assert.AreEqual(0, map.Count);
    }

    [Test]
    public void SpeedCheck() {
      var       stopwatch = new Stopwatch();
      var       map       = new Map();
      var       dict      = new Dictionary<string, object>();
      var       keys      = new string[10];
      const int sets      = 10000;
      bool      result;

      for (var i = 0; i < keys.Length; i++) keys[i] = Guid.NewGuid().ToString();

      // ReSharper disable once CoVariantArrayConversion
      map.Set(keys);

      for (var i = 0; i < keys.Length; i++) dict.Add(keys[i], null);

      stopwatch.Start();

      for (var j = 0; j < sets; j++) {
        for (var i = 0; i < keys.Length; i++) {
          result = map[keys[i]].Found;
          Assert.IsTrue(result);
          Assert.IsNull(map.Value);
        }
      }

      stopwatch.Stop();
      var mapTime = stopwatch.ElapsedMilliseconds;
      stopwatch.Reset();
      stopwatch.Start();

      for (var j = 0; j < sets; j++) {
        for (var i = 0; i < keys.Length; i++) {
          result = dict.ContainsKey(keys[i]);
          Assert.IsTrue(result);
          Assert.IsNull(dict[keys[i]]);
        }
      }

      stopwatch.Stop();
      var dictTime = stopwatch.ElapsedMilliseconds;

      Debug.Log($"{$"{keys.Length}:",-8} Map: {mapTime,-10:F4} Dict: {dictTime,-10:F4}");
    }
  }
}
#endif