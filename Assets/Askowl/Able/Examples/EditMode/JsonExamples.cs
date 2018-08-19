// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

using System;
using NUnit.Framework;

namespace Askowl.Examples {
  /// <remarks><a href="http://unitydoc.marrington.net/Able#jsoncs-parse-any-json-to-dictionary">Json Parser</a></remarks>
  public class JsonExamples {
    private Json json = new Json();

    /// <remarks><a href="http://unitydoc.marrington.net/Able#jsoncs-parse-any-json-to-dictionary">Json Parser</a></remarks>
    [Test]
    public void Parse() {
      bool error = json.Parse(@"{""Not my age"":23}").Error;
      Assert.IsFalse(error);

      // root braces are optional ...
      error = json.Parse(@"""Not my age"":23").Error;
      Assert.IsFalse(error);

      // .. but invalid JSON isn't
      error = json.Parse(@"{""Not my age"":23").Error;
      Assert.IsTrue(error);
    }

    public void Here() { }

    [Test]
    public void IsA() {
      bool stringAtRoot = json.Parse(jsonSampler).IsA<string>();

      Assert.IsTrue(stringAtRoot);

      // You can use `IsA` on a retrieved node
      json.Walk("items.item.0.magic");
      bool isInt = json.IsA<int>(json[2]);

      Assert.IsTrue(isInt);
    }

    [Test]
    public void IsNode() {
      json.Parse(@"{""A"":194, ""B"": {3 : 4}");

      var isNode = json.Walk("A").IsNode;

      Assert.IsFalse(isNode);

      isNode = json.Walk("B").IsNode;

      Assert.IsTrue(isNode);
    }

    [Test]
    public void IsArray() {
      json.Parse(@"{""A"":[194,286,3], ""B"": {3 : 4}");

      var isArray = json.Walk("A").IsArray;

      Assert.IsTrue(isArray);

      isArray = json.Walk("B").IsArray;

      Assert.IsFalse(isArray);
    }

    [Test]
    public void NodeType() {
      json.Parse(@"{""A"":[194,286,3], ""B"": {3 : 4}");

      Type nodeType = json.Walk("A", 1).NodeType;

      Assert.AreEqual(typeof(long), nodeType);
    }

    [Test]
    public void ErrorMessage() {
      var errorMessage = json.Parse(@"{""Not my age"":23}").ErrorMessage;

      Assert.IsNull(errorMessage);

      errorMessage = json.Parse("").ErrorMessage;

      Assert.IsFalse(string.IsNullOrEmpty(errorMessage));
    }

    [Test]
    public void Error() {
      bool error = json.Parse("").Error;

      Assert.IsTrue(json.Error);
    }

    [Test]
    public void Get() { //#TBD#
      json.Parse(jsonSampler);
      // If you know the structure of your JSON you can retrieve a leaf directly
      string id = json.Get<string>("items", "item", 0, "id");
      Assert.AreEqual("0001", id);
      Assert.IsFalse(json.Error);

      // If you provide the wrong type then default<T> is returned - 0 for an int
      int iid = json.Get<int>("items", "item", "0", "id");
      Assert.AreEqual(0, iid);
      Assert.IsTrue(json.Error);

      // If this is a problem, use IsA<>
      Assert.IsFalse(json.IsA<int>());
    }

    [Test]
    public void Walk() { //#TBD#
      json.Parse(jsonSampler);
      // You can separate walking the tree and retrievaly using Walk, WalkOn, IsA and Here
      bool isString = json.Walk("items.item.0.type").IsA<string>();

      Assert.IsTrue(isString);

      string donut = json.Here<string>();

      Assert.AreEqual("donut", donut);
    }

    [Test]
    public void WalkOn() {
      json.Parse(jsonSampler).Walk("items.item");

      // Walk is absolute, but you can use WalkOn to get where you want in steps
      var donut = json.WalkOn(0, "type").Here<string>();

      Assert.AreEqual("donut", donut);
    }

    [Test]
    public void FloatingPoint() {
      json.Parse(@"""A"": 12.34").Walk("A");

      // A floating point can be retrieved as a double or float
      double ppu = json.Here<double>();
      Assert.AreEqual(12.34, ppu, 1e5);

      float fppu = json.Here<float>();
      Assert.AreEqual(12.34f, fppu, 1e5f);
    }

    [Test]
    public void Integer() {
      json.Parse(@"""A"": 83").Walk("A");

      // A whole number can be retrieved as an int, long, float or double
      int iqty = json.Here<int>();
      Assert.AreEqual(83, iqty);

      long lqty = json.Here<long>();
      Assert.AreEqual(83, lqty);

      float fqty = json.Here<float>();
      Assert.AreEqual(83f, fqty, 1e5f);

      double dqty = json.Here<double>();
      Assert.AreEqual(83, dqty, 1e5);
    }

    [Test]
    public void AnchorAndDispose() { } //#TBD#

    [Test]
    public void NodeChildren() {
      json.Parse(@"""A"": {""One"": 1, ""Two"": 2, ""Three"": 3, }").Walk("A");

      string expected = "One=1 Two=2 Three=3 ";
      string actual   = "";

      for (var child = json.Children(); child.More(); child.Next()) {
        actual += $"{child.Name}={child.Value()} ";
      }

      Assert.AreEqual(expected, actual);
    }

    [Test]
    public void ArrayChildren() {
      json.Parse(@"""A"": [23,195,67]").Walk("A");

      string expected = "0=23 1=195 2=67 ";
      string actual   = "";

      for (var child = json.Children(); child.More(); child.Next()) {
        actual += $"{child.Name}={child.Value()} ";
      }

      Assert.AreEqual(expected, actual);
    }

    [Test]
    public void OneChild() {
      json.Parse(@"""A"": 567").Walk("A");

      string expected = "567";
      string actual   = "";

      for (var child = json.Children(); child.More(); child.Next()) {
        actual += child.Value();
      }

      Assert.AreEqual(expected, actual);
    }

    [Test]
    public void ChildrenValueT() { }

    [Test]
    public void ArrayAccess() { } //#TBD#

    internal void JsonExample() {
//      if (json.Fetch<string>("id") != "0001") Error(json, "Expecting id of 0001");

      // When a node is a leaf of type Array we may want fetch an individual element using [] or generic Fetch
      json.Walk("items.item.0.magic");
//      if (json.Fetch<int>(3) != 4444) Error(json, "expecting 4444, not '{0}'", json[3]);

      // json is also an iterator for processing children of tree nodes
      json.Walk("items.item.0");
      string keys = "id,type,name,ppu,qty,magic,batters,topping";

//      foreach (string key in json.Where(key => keys.IndexOf(key, StringComparison.Ordinal) == -1)) {
//        Error(json, "Unexpected key '{0}'", key);
//      }

      // json is also an iterator for processing entries in array
      // `As` returns default(T) if it cannot be case or converted
      json.Walk("items.item.0.magic");
      int[] values = {1, 22, 333, 4444, 55555};
      int   i      = 0;

      foreach (int entry in json.As<int>()) {
        if (i >= values.Length) {
          Error(json, "Too many values");
          break;
        }

        if (entry != values[i++]) Error(json, "Array retrieval error for {0}", i);
      }

      if (i != values.Length) Error(json, "incorrect array length {0}", i);

      // If we want to keep processing from an iterator we need to set an anchor to return to
      json.Walk("items.item.0");

      foreach (string key in json) {
        using (json.Anchor) {
          json.WalkOn(key);

          if (key == "topping") {
            if (json.Count != 7) Error(json, "Probably Anchor failure ({0})", json.Count);
          }
        }
      }

      // Get back, get back, get back to where we once belonged
      // This will work even if we are use Parse
      json.Walk("items.item.0.name");
      var pin = json.Pin();
      if (json.Here<string>() != "Cake") Error(json, "Expecting cake");

      json.Walk("items.item.0.batters.batter.1.id");
      if (json.Here<string>() != "1002") Error(json, "Expecting batter 1002");

      json.Reset(pin);
      if (json.Here<string>() != "Cake") Error(json, "Expecting cake");

      Debug.Log("JsonExample passed");
    }

    private string jsonSampler = @"{
  ""items"":
    {
      ""item"":
        [
          {
            ""id"": ""0001"",
            ""type"": ""donut"",
            ""name"": ""Cake"",
            ""ppu"": 0.55,
            ""qty"": 12,
            ""magic"":
              [ 1, 22, 333, 4444, 55555 ],
            ""batters"":
              {
                ""batter"":
                  [
                    { ""id"": ""1001"", ""type"": ""Regular"" },
                    { ""id"": ""1002"", ""type"": ""Chocolate"" },
                    { ""id"": ""1003"", ""type"": ""Blueberry"" },
                    { ""id"": ""1004"", ""type"": ""Devil's Food"" }
                  ]
              },
            ""topping"":
              [
                { ""id"": ""5001"", ""type"": ""None"" },
                { ""id"": ""5002"", ""type"": ""Glazed"" },
                { ""id"": ""5005"", ""type"": ""Sugar"" },
                { ""id"": ""5007"", ""type"": ""Powdered Sugar"" },
                { ""id"": ""5006"", ""type"": ""Chocolate with Sprinkles"" },
                { ""id"": ""5003"", ""type"": ""Chocolate"" },
                { ""id"": ""5004"", ""type"": ""Maple"" }
              ]
          }
        ]
    }
}
";
  }
}