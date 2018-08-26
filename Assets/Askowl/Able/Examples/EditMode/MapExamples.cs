// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

using System;
using System.Collections.Generic;
using System.Diagnostics;
using NUnit.Framework;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Askowl.Examples {
  /// <a href="">Using <see cref="Map"/></a>
  public class MapExamples {
    /// <a href="">Using <see cref="Map"/></a>
    [Test]
    public void NewMap() {
      var map = new Map();
      Assert.AreEqual(0, map.Count);
    }

    /// <a href="">Using <see cref="Map.Count"/></a>
    [Test]
    public void Count() {
      var map = new Map("One", 1, "Two", 2, "Three", 3);

      Assert.AreEqual(3, map.Count);
    }

    /// <a href="">Using <see cref="Map.Set"/></a>
    [Test]
    public void Set() {
      var set = new Map().Set(1, 3, 5, 7, 9);

      Assert.AreEqual(5, set.Count);

      set.Set(11, 13, 15);

      Assert.AreEqual(8, set.Count);
    }

    /// <a href="">Using <see cref="Map.Add"/></a>
    [Test]
    public void Add() {
      var map = new Map("One", 1, "Two", 2, "Three", 3);

      map.Add("Four", 4, "Five", 5);

      Assert.AreEqual(5, map.Count);
    }

    /// <a href="">Using <see cref="Map.Contains"/></a>
    [Test]
    public void Contains() {
      var map = new Map("One", 1, "Four", 4, "Three", 3, "Five", 5, "Two", 2);

      Assert.IsTrue(map.Contains("One"));
      Assert.IsTrue(map.Contains("Three"));
      Assert.IsTrue(map.Contains("Five"));
      Assert.IsFalse(map.Contains("Seven"));
    }

    /// <a href="">Using <see cref="Map.IsA{T}"/></a>
    [Test]
    public void IsA() {
      var map = new Map("One", 1, "Two", "2", "Three", new Map());

      Assert.IsTrue(map.IsA<int>("One"));
      Assert.IsTrue(map.IsA<string>("Two"));
      Assert.IsTrue(map.IsA<Map>("Three"));
      Assert.IsFalse(map.IsA<int>("Two"));
    }

    /// <a href="">Using <see cref="Map.Get{T}"/></a>
    [Test]
    public void Get() {
      var map = new Map("One", 111, "Two", "2", "Three", new Map());

      Assert.AreEqual(111, map.Get<int>("One"));
      Assert.AreEqual("2", map.Get<string>("Two"));
      Assert.AreEqual(0,   map.Get<Map>("Three").Count);
      Assert.AreEqual(0,   map.Get<int>("Two"));
    }

    /// <a href="">Using <see cref="Map.Found"/></a>
    [Test]
    public void Found() {
      var map = new Map("One", 111, "Two", "2", "Three", new Map());

      map.Contains("Two");
      Assert.IsTrue(map.Found);

      map.Contains("TryMe");
      Assert.IsFalse(map.Found);
    }

    /// <a href="">Using <see cref="Map.Key"/></a>
    [Test]
    public void Key() {
      var map = new Map("One", 111, "Two", "2", "Three", new Map());

      map.Contains("Two");
      Assert.AreEqual(expected: "Two", actual: map.Key);

      map.Contains("TryMe");
      Assert.AreEqual(expected: "Two", actual: map.Key);
    }

    /// <a href="">Using <see cref="Map.Value"/></a>
    [Test]
    public void Value() {
      var map = new Map("One", 111, "Two", "2", "Three", new Map());

      map.Contains("Two");
      Assert.AreEqual(expected: "2", actual: map.Value);

      map.Contains("TryMe");
      Assert.IsNull(map.Value);
    }

    /// <a href="">Using <see cref="Map"/></a>
    [Test]
    public void SpeedCheck() {
      Stopwatch stopwatch = new Stopwatch();
      var       map       = new Map();
      var       dict      = new Dictionary<string, int>();
      var       keys      = new string[10];
      const int sets      = 10000;
      bool      result;

      for (int i = 0; i < keys.Length; i++) {
        keys[i] = Guid.NewGuid().ToString();
      }

      map.Set(keys);

      for (int i = 0; i < keys.Length; i++) {
        dict.Add(keys[i], i);
      }

      stopwatch.Start();

      for (int j = 0; j < sets; j++) {
        for (int i = 0; i < keys.Length; i++) {
          result = map.Contains(keys[i]);
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
          Assert.AreEqual(i, dict[keys[i]]);
        }
      }

      stopwatch.Stop();
      var dictTime = stopwatch.ElapsedMilliseconds;

      Debug.Log($"{$"{keys.Length}:",-8} Map: {mapTime,-10:F4} Dict: {dictTime,-10:F4}");
    }
  }
}