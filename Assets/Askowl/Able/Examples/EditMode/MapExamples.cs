// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

using System;
using System.Collections.Generic;
using System.Diagnostics;
using NUnit.Framework;
using Debug = UnityEngine.Debug;

namespace Askowl.Examples {
  /// <a href="">Using <see cref="Map"/></a>
  public class MapExamples {
    /// <a href="">Using <see cref="Map"/></a>
    [Test]
    public void NewMap() {
      var map = new Map();
      Assert.AreEqual(expected: 0, actual: map.Count);
    }

    /// <a href="">Using <see cref="Map.Count"/></a>
    [Test]
    public void Count() {
      var map = new Map("One", 1, "Two", 2, "Three", 3);

      Assert.AreEqual(expected: 3, actual: map.Count);
    }

    /// <a href="">Using <see cref="Map.Set"/></a>
    [Test]
    public void Set() {
      var set = new Map().Set(1, 3, 5, 7, 9);

      Assert.AreEqual(expected: 5, actual: set.Count);

      set.Set(11, 13, 15);

      Assert.AreEqual(expected: 8, actual: set.Count);
    }

    /// <a href="">Using <see cref="Map.Add"/></a>
    [Test]
    public void Add() {
      var map  = new Map("One", 1, "Two", 2, "Three", 3);
      var more = new object[] {"Six", 6, "Seven", 7};

      map.Add("Four", 4, "Five", 5).Add(more);

      Assert.AreEqual(expected: 7, actual: map.Count);
    }

    /// <a href="">Using <see cref="Map.Remove"/></a>
    [Test]
    public void Remove() {
      var map = new Map("One", 1, "Two", 2, "Three", 3);

      map.Remove("Two", "One");
      Assert.AreEqual(1, map.Count);
    }

    /// <a href="">Using <see cref="Map"/></a>
    [Test]
    public void Finder() {
      var map = new Map("One", 1, "Four", 4, "Three", 3, "Five", 5, "Two", 2);

      Assert.IsTrue(map["One"].Found);
      Assert.IsTrue(map["Three"].Found);
      Assert.IsTrue(map["Five"].Found);
      Assert.IsFalse(map["Seven"].Found);
    }

    /// <a href="">Using <see cref="Map.IsA{T}"/></a>
    [Test]
    public void IsA() {
      var map = new Map("One", 1, "Two", "2", "Three", new Map());

      Assert.IsTrue(map["One"].IsA<int>());
      Assert.IsTrue(map["Two"].IsA<string>());
      Assert.IsTrue(map["Three"].IsA<Map>());
      Assert.IsFalse(map["Two"].IsA<int>());
    }

    /// <a href="">Using <see cref="Map.As{T}"/></a>
    [Test]
    public void AsT() {
      var map = new Map("One", 111, "Two", "2", "Three", new Map());

      Assert.AreEqual(expected: 111, actual: map["One"].As<int>());
      Assert.AreEqual(expected: "2", actual: map["Two"].As<string>());
      Assert.AreEqual(expected: 0,   actual: map["Three"].As<Map>().Count);
      Assert.AreEqual(expected: 0,   actual: map["Two"].As<int>());
    }

    /// <a href="">Using <see cref="Map.Found"/></a>
    [Test]
    public void Found() {
      var map = new Map("One", 111, "Two", "2", "Three", new Map());

      Assert.IsTrue(map["Two"].Found);

      Assert.IsFalse(map["TryMe"].Found);
    }

    /// <a href="">Using <see cref="Map.Key"/></a>
    [Test]
    public void Key() {
      var map = new Map("One", 111, "Two", "2", "Three", new Map());

      Assert.AreEqual(expected: "Two", actual: map["Two"].Key);
    }

    /// <a href="">Using <see cref="Map.Value"/></a>
    [Test]
    public void Value() {
      var map = new Map("One", 111, "Two", "2", "Three", new Map());

      Assert.AreEqual(expected: "2", actual: map["Two"].Value);

      Assert.IsNull(map["TryMe"].Value);
    }

    /// <a href="">Using <see cref="Map.First"/></a>
    [Test]
    public void StringKeyIteration() {
      var    map  = new Map("One", 111, "Two", "2", "Three", new Map());
      string keys = "";

      for (var key = map.First; key != null; key = map.Next) {
        keys += key;
      }

      Assert.AreEqual("OneTwoThree", keys);
    }

    /// <a href="">Using <see cref="Map.Next"/></a>
    [Test]
    public void ObjectKeyIteration() {
      var    map  = new Map("One", 111, "Two", "2", "Three", new Map());
      string keys = "";

      for (int i = 0; i < map.Count; i++) {
        keys += map[i];
      }

      Assert.AreEqual("OneTwoThree", keys);
    }

    /// <a href="">Using <see cref="Map.TypeOf"/></a>
    [Test]
    public void TypeOf() {
      var map = new Map("One", 111, "Two", "2", "Three", new Map());

      Assert.AreEqual(typeof(int),    map["One"].TypeOf);
      Assert.AreEqual(typeof(string), map["Two"].TypeOf);
      Assert.AreEqual(map.GetType(),  map["Three"].TypeOf);

      var types = new Map(typeof(int), 'i', typeof(string), 's', typeof(Map), 'm');

      switch ((types[map["One"].TypeOf].As<char>())) {
        case 'i': break;
        default:
          Assert.Fail();
          break;
      }
    }

    /// <a href="">Using <see cref="Map.Sort()"/></a>
    [Test]
    public void Sort() {
      var    map    = new Map("One", 1, "Four", 4, "Three", 3, "Five", 5, "Two", 2);
      string actual = "";

      for (var key = map.Sort().First; key != null; key = map.Next) actual += key;

      Assert.AreEqual("FiveFourOneThreeTwo", actual);
    }

    /// <a href="">Using <see cref="Map.Sort(Comparison{object})"/></a>
    [Test]
    public void SortT() {
      var    map    = new Map("One", 1, "Four", 4, "Three", 3, "Five", 5, "Two", 2);
      string actual = "";

      map.Sort((x, y) => map[x].As<int>().CompareTo(map[y].As<int>()));
      for (var key = map.First; key != null; key = map.Next) actual += key;

      Assert.AreEqual("OneTwoThreeFourFive", actual);
    }

    private static int disposals;

    struct Disposable : IDisposable {
      public void Dispose() { disposals += 1; }
    }

    /// <a href="">Using <see cref="Map.Dispose"/></a>
    [Test]
    public void Dispose() {
      Disposable[] values = new Disposable[3];
      var          map    = new Map("One", values[0], "Two", values[1], "Three", values[2]);
      disposals = 0;
      map.Dispose();
      Assert.AreEqual(3, disposals);
      Assert.AreEqual(0, map.Count);
    }

    /// <a href="">Using <see cref="Map"/></a>
    [Test]
    public void SpeedCheck() {
      Stopwatch stopwatch = new Stopwatch();
      var       map       = new Map();
      var       dict      = new Dictionary<string, object>();
      var       keys      = new string[10];
      const int sets      = 10000;
      bool      result;

      for (int i = 0; i < keys.Length; i++) {
        keys[i] = Guid.NewGuid().ToString();
      }

      // ReSharper disable once CoVariantArrayConversion
      map.Set(keys);

      for (int i = 0; i < keys.Length; i++) {
        dict.Add(keys[i], null);
      }

      stopwatch.Start();

      for (int j = 0; j < sets; j++) {
        for (int i = 0; i < keys.Length; i++) {
          result = map[keys[i]].Found;
          Assert.IsTrue(result);
          Assert.IsNull(map.Value);
        }
      }

      stopwatch.Stop();
      var mapTime = stopwatch.ElapsedMilliseconds;
      stopwatch.Reset();
      stopwatch.Start();

      for (int j = 0; j < sets; j++) {
        for (int i = 0; i < keys.Length; i++) {
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