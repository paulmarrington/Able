// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

using System;
using NUnit.Framework;

namespace Askowl.Examples {
  /// <a href="">Using <see cref=""/></a>
  public class MapExamples {
    /// <a href="">Using <see cref=""/></a>
    [Test]
    public void Map() {
      var map = new Map();
      Assert.AreEqual(0, map.Count);
    }

    /// <a href="">Using <see cref=""/></a>
    [Test]
    public void Count() {
      var map = new Map("One", 1, "Two", 2, "Three", 3);

      Assert.AreEqual(3, map.Count);
    }

    /// <a href="">Using <see cref=""/></a>
    [Test]
    public void Set() {
      var set = new Map().Set(1, 3, 5, 7, 9);

      Assert.AreEqual(5, set.Count);

      set.Set(11, 13, 15);

      Assert.AreEqual(8, set.Count);
    }

    /// <a href="">Using <see cref=""/></a>
    [Test]
    public void Add() {
      var map = new Map("One", 1, "Two", 2, "Three", 3);

      map.Add("Four", 4, "Five", 5);

      Assert.AreEqual(5, map.Count);
    }

    /// <a href="">Using <see cref=""/></a>
    [Test]
    public void Contains() {
      var map = new Map("One", 1, "Two", 2, "Three", 3, "Four", 4, "Five", 5);

      Assert.IsTrue(map.Contains("One"));
      Assert.IsTrue(map.Contains("Three"));
      Assert.IsTrue(map.Contains("Five"));
      Assert.IsFalse(map.Contains("Seven"));
    }

    /// <a href="">Using <see cref=""/></a>
    [Test]
    public void IsA() {
      var map = new Map("One", 1, "Two", "2", "Three", new Map());

      Assert.IsTrue(map.IsA<int>("One"));
      Assert.IsTrue(map.IsA<string>("Two"));
      Assert.IsTrue(map.IsA<Map>("Three"));
      Assert.IsFalse(map.IsA<int>("Two"));
    }

    /// <a href="">Using <see cref=""/></a>
    [Test]
    public void Get() {
      var map = new Map("One", 111, "Two", "2", "Three", new Map());

      Assert.AreEqual(111,  map.Get<int>("One"));
      Assert.AreEqual("2",  map.Get<string>("Two"));
      Assert.AreEqual(0,    map.Get<Map>("Three").Count);
      Assert.AreEqual(null, map.Get<int>("Two"));
    }

    /// <a href="">Using <see cref=""/></a>
    [Test]
    public void Closest() { }

    /// <a href="">Using <see cref=""/></a>
    [Test]
    public void Key() { }

    /// <a href="">Using <see cref=""/></a>
    [Test]
    public void Value() { }
  }
}