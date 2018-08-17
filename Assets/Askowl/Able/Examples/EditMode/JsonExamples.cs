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
      Assert.IsTrue(json.Walk("items.item.0.type"));

      Assert.IsTrue(json.IsA<string>());

      string donut = json.Here<string>();
      Assert.AreEqual("donut", donut);

      // //#TBD# Walk<T>
    }

    [Test]
    public void WalkOn() { } //#TBD#

    [Test]
    public void AnchorAndDispose() { } //#TBD#

    [Test]
    public void Fetch() { } //#TBD#

    [Test]
    public void ArrayAccess() { } //#TBD#

    [Test]
    public void Enumerable() { } //#TBD#

    internal void JsonExample() {
      // You can separate walking the tree and retrievaly using Walk, WalkOn, IsA and Here
      if (!json.Walk("items.item.0.type")) Error(json, "Can't find the donut");
      if (!json.IsA<string>()) Error(json,             "Expecting Donut to be a string");
      string donut = json.Here<string>();
      if (donut != "donut") Error(json, "Expecting 'donut', not '{0}'", donut);

      // Walk is absolute, but you can use WalkOn to get where you want in steps
      if (!json.Walk("items.item")) Error(json, "Can't find the first item");
      if (!json.WalkOn(0, "type")) Error(json,  "Can't walk on to the donut");
      donut = json.Here<string>();
      if (donut != "donut") Error(json, "Expecting 'donut', not '{0}'", donut);

      // Because walking to a leave and expecting a certain type of data is common, we can combine them
      if (!json.Walk<double>("items", "item", 0, "ppu")) Error(json, "Can't walk to ppu");

      // A floating point can be retrieved as a double or float
      double ppu = json.Here<double>();
      if (json.ErrorMessage != null) Error(json, "Can't get double '{0}'", ppu);
      float fppu = json.Here<float>();
      if (json.ErrorMessage != null) Error(json, "Can't get float '{0}'", fppu);

      if ((Math.Abs(ppu - fppu) > 1e5) || (Math.Abs(ppu - 0.55) > 1e5)) {
        Error(json, "Retrieving double and float failed since {0} != {1}", ppu, fppu);
      }

      // A whole number can be retrieved as an int, long, float or double
      if (!json.Walk("items", "item", 0, "qty")) Error(json, "Can't walk to qty");
      long lqty = json.Here<long>();
      if (json.ErrorMessage != null) Error(json, "Can't get long '{0}'", lqty);
      int iqty = json.Get<int>("items.item.0.qty");
      if (json.ErrorMessage != null) Error(json, "Can't get int '{0}'", iqty);
      float fqty = json.Get<float>("items.item.0.qty");
      if (json.ErrorMessage != null) Error(json, "Can't get float '{0}'", fqty);
      double dqty = json.Get<double>("items.item.0.qty");
      if (json.ErrorMessage != null) Error(json, "Can't get double '{0}'", dqty);

      if ((lqty != iqty) || (Math.Abs(fqty - iqty) > 1e5) || (Math.Abs(dqty - iqty) > 1e5)) {
        Error(json, "Whole number mismatch {0} == {1} == {2} == {3}", lqty, iqty, fqty, dqty);
      }

      // When a node is a leaf of type Node we may want fetch an individual child using [] or generic Fetch
      json.Walk("items.item.0");

      if (json["name"] == null) {
        Error(json, "No key `name`");
      } else if (json["name"].ToString() != "Cake") {
        Error(json, "Expecting Cake");
      }

      if (json.Fetch<string>("id") != "0001") Error(json, "Expecting id of 0001");

      // When a node is a leaf of type Array we may want fetch an individual element using [] or generic Fetch
      json.Walk("items.item.0.magic");

      if (json[2] == null) {
        Error(json, "No index 2");
      } else if (((long) json[2]) != 333) Error(json, "Expecting 333, not '{0}'", json[2]);

      if (json.Fetch<int>(3) != 4444) Error(json, "expecting 4444, not '{0}'", json[3]);

      // json is also an iterator for processing children of tree nodes
      json.Walk("items.item.0");
      string keys = "id,type,name,ppu,qty,magic,batters,topping";

      foreach (string key in json.Where(key => keys.IndexOf(key, StringComparison.Ordinal) == -1)) {
        Error(json, "Unexpected key '{0}'", key);
      }

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