// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

using System;
using NUnit.Framework;
using UnityEngine;

namespace Askowl.Examples {
  /// <remarks><a href="http://unitydoc.marrington.net/Able#jsoncs-parse-any-json-to-dictionary">Json Parser</a></remarks>
  public class JsonExamples {
    private Json json = Json.Instance;

    /// <remarks><a href="http://unitydoc.marrington.net/Able#parse">Parse JSON from text</a></remarks>
    [Test]
    public void Parse() {
      bool error = json.Parse(@"{""Not my age"":23}").Error;
      Assert.IsFalse(error);

      // invalid JSON is not acceptable
      json.Parse(@"""Not my age"":23");
      Assert.IsTrue(json.Error);
    }

    /// <remarks><a href="http://unitydoc.marrington.net/Able#current-location">Here - The Current Location</a></remarks>
    [Test]
    public void Here() {
      json.Parse(@"{""A"": 12.34}").To("A");

      // A floating point can be retrieved as a double or float
      double ppu = json.Here<double>();
      Assert.AreEqual(12.34, ppu, 1e5);
    }

    /// <remarks><a href="http://unitydoc.marrington.net/Able#information-retrieval">Retrieve Value at Current Location</a></remarks>
    [Test]
    public void IsA() {
      json.Parse(@"{""A"": 12.34}").To("A");

      // You can use `IsA` on a retrieved node
      bool isFloat = json.IsA<float>();

      Assert.IsTrue(isFloat);
    }

    /// <remarks><a href="http://unitydoc.marrington.net/Able#information-retrieval">Retrieve Value at Current Location</a></remarks>
    [Test]
    public void IsNode() {
      json.Parse(@"{""A"":194, ""B"": {3 : 4}}");

      var isNode = json.To("A").IsNode;

      Assert.IsFalse(isNode);

      isNode = json.To("B").IsNode;

      Assert.IsTrue(isNode);
    }

    /// <remarks><a href="http://unitydoc.marrington.net/Able#information-retrieval">Retrieve Value at Current Location</a></remarks>
    [Test]
    public void IsArray() {
      json.Parse(@"{""A"":[194,286,3], ""B"": {3 : 4}}");

      var isArray = json.To("A").IsArray;

      Assert.IsTrue(isArray);

      isArray = json.To("B").IsArray;

      Assert.IsFalse(isArray);
    }

    /// <remarks><a href="http://unitydoc.marrington.net/Able#error-processing">Error Indication and Message</a></remarks>
    [Test]
    public void ErrorMessage() {
      var errorMessage = json.Parse(@"{""Not my age"":23}").ErrorMessage;

      Assert.IsNull(errorMessage);

      errorMessage = json.Parse("").ErrorMessage;

      Assert.IsFalse(string.IsNullOrEmpty(errorMessage));
    }

    /// <remarks><a href="http://unitydoc.marrington.net/Able#error-processing">Error Indication and Message</a></remarks>
    [Test]
    public void Error() {
      bool error = json.Parse("").Error;

      Assert.IsTrue(error);
    }

    /// <remarks><a href="http://unitydoc.marrington.net/Able#information-retrieval">Retrieve Value at Current Location</a></remarks>
    [Test]
    public void Get() {
      json.Parse(jsonSampler);
      // If you know the structure of your JSON you can retrieve a leaf directly
      string id = json.Root.To("items", "item", 0, "id").Here<string>();
      Assert.AreEqual("0001", id);
      Assert.IsFalse(json.Error);

      // If you provide the wrong type then default<T> is returned - 0 for an int
      int iid = json.Root.To("items", "item", "0", "id").Here<int>();
      Assert.AreEqual(0, iid);
      Assert.IsTrue(json.Error);

      // If this is a problem, use IsA<>
      Assert.IsFalse(json.IsA<int>());
    }

    /// <remarks><a href="http://unitydoc.marrington.net/Able#tree-traversal">Traversing the JSON Node Tree</a></remarks>
    [Test]
    public void Walk() {
      json.Parse(jsonSampler);
      // You can separate walking the tree and retrievaly using Walk, WalkOn, IsA and Here
      bool isString = json.Root.To("items.item.0.type").IsA<string>();

      Assert.IsTrue(isString);

      string donut = json.Here<string>();

      Assert.AreEqual("donut", donut);
    }

    /// <remarks><a href="http://unitydoc.marrington.net/Able#tree-traversal">Traversing the JSON Node Tree</a></remarks>
    [Test]
    public void RelativeTo() {
      json.Parse(jsonSampler).Root.To("items.item");

      // Walk is absolute, but you can use WalkOn to get where you want in steps
      var donut = json.To(0, "type").Here<string>();

      Assert.AreEqual("donut", donut);
    }

    /// <remarks><a href="http://unitydoc.marrington.net/Able#information-retrieval">Retrieve Value at Current Location</a></remarks>
    [Test]
    public void FloatingPoint() {
      json.Parse(@"{""A"": 12.34}").To("A");

      // A floating point can be retrieved as a double or float
      double ppu = json.Here<double>();
      Assert.AreEqual(12.34, ppu, 1e5);

      float fppu = json.Here<float>();
      Assert.AreEqual(12.34f, fppu, 1e5f);
    }

    /// <remarks><a href="http://unitydoc.marrington.net/Able#information-retrieval">Retrieve Value at Current Location</a></remarks>
    [Test]
    public void Integer() {
      json.Parse(@"{""A"": 83}").To("A");

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

    /// <remarks><a href="http://unitydoc.marrington.net/Able#tree-traversal">Traversing the JSON Node Tree</a></remarks>
    [Test]
    public void NodeChildren() {
      json.Parse(@"{""A"": {""One"": 1, ""Two"": 2, ""Three"": 3 }}").To("A");

      string expected = "One=1 Two=2 Three=3 ";
      string actual   = "";

      json.ForEach(() => actual += $"{json.Name}={json.Here<string>()} ");

      Assert.AreEqual(expected, actual);
    }

    /// <remarks><a href="http://unitydoc.marrington.net/Able#tree-traversal">Traversing the JSON Node Tree</a></remarks>
    [Test]
    public void ArrayChildren() {
      json.Parse(@"{""A"": [23,195,67]}").To("A");

      string expected = "0=23 1=195 2=67 ";
      string actual   = "";

      json.ForEach(() => actual += $"{json.Name}={json.Here<string>()} ");

      Assert.AreEqual(expected, actual);
    }

    /// <remarks><a href="http://unitydoc.marrington.net/Able#tree-traversal">Traversing the JSON Node Tree</a></remarks>
    [Test]
    public void OneChild() {
      json.Parse(@"{""A"": 567}").To("A");

      string expected = "567";
      string actual   = "";

      json.ForEach(() => actual += json.Here<string>());

      Assert.AreEqual(expected, actual);
    }

    /// <remarks><a href="http://unitydoc.marrington.net/Able#Anchor">Mark Current Node Location</a></remarks>
    [Test]
    public void AnchorAndDispose() {
      json.Parse(jsonSampler).To("items.item.0");

      string names = "";

      // If we want to keep processing from an iterator we need to set an anchor to return to
      json.ForEach(() => {
        names += json.Name;

        if (json.Name == "magic") {
          using (json.Anchor) {
            string expected = "122333444455555";
            string actual   = "";

            json.ForEach(() => actual += json.Here<string>());

            Assert.AreEqual(expected, actual);
          }
        }
      });

      Assert.AreEqual(expected: "idtypenameppuqtymagicbatterstopping", actual: names);
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