// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

using NUnit.Framework;

namespace Askowl.Examples {
  /// Using <see cref="Json"/>
  public class JsonExamples {
    private Json json = Json.Instance;

    /// Using <see cref="Json.Parse"/>
    [Test]
    public void Parse() {
      bool error = json.Parse(@"{""Not my age"":23}").Error;
      Assert.IsFalse(error);

      // invalid JSON is not acceptable
      json.Parse(@"""Not my age"":23");
      Assert.IsTrue(json.Error);
    }

    /// Using <see cref="Json"/>
    [Test]
    public void Here() {
      json.Parse(@"{""A"": 12.34}").To("A");

      // A floating point can be retrieved as a double or float
      double ppu = json.Here<double>();
      Assert.AreEqual(12.34, ppu, 1e5);
    }

    /// Using <see cref="Json"/>
    [Test]
    public void IsA() {
      json.Parse(@"{""A"": 12.34}").To("A");

      // You can use `IsA` on a retrieved node
      bool isFloat = json.IsA<float>();

      Assert.IsTrue(isFloat);
    }

    /// Using <see cref="Json"/>
    [Test]
    public void ErrorMessage() {
      var errorMessage = json.Parse(@"{""Not my age"":23}").ErrorMessage;

      Assert.IsNull(errorMessage);

      errorMessage = json.Parse("").ErrorMessage;

      Assert.IsFalse(string.IsNullOrEmpty(errorMessage));
    }

    /// Using <see cref="Json"/>
    [Test]
    public void Error() {
      bool error = json.Parse("").Error;

      Assert.IsTrue(error);
    }

    /// Using <see cref="Json"/>
    [Test]
    public void Get() {
      json.Parse(jsonSampler);
      // If you know the structure of your JSON you can retrieve a leaf directly
      string id = json.To("items", "item", 0, "id").Here<string>();
      Assert.AreEqual("0001", id);
      Assert.IsFalse(json.Error);
    }

    /// Using <see cref="Json"/>
    [Test]
    public void RelativeTo() {
      json.Parse(jsonSampler).To("items.item");

      // Walk is absolute, but you can use WalkOn to get where you want in steps
      var donut = json.Next(0, "type").Here<string>();

      Assert.AreEqual("donut", donut);
    }

    /// Using <see cref="Json"/>
    [Test]
    public void FloatingPoint() {
      json.Parse(@"{""A"": 12.34}").To("A");

      // A floating point can be retrieved as a double or float
      double ppu = json.Here<double>();
      Assert.AreEqual(12.34, ppu, 1e5);

      float fppu = json.Here<float>();
      Assert.AreEqual(12.34f, fppu, 1e5f);
    }

    /// Using <see cref="Json"/>
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

    /// Using <see cref="Json"/>
    [Test]
    public void NodeChildren() {
      json.Parse(@"{""A"": {""One"": 1, ""Two"": 2, ""Three"": 3 }}").To("A");

      string expected = "One=1 Two=2 Three=3 ";
      string actual   = "";

      var children = json.Children;

      for (int i = 0; i < children.Length; i++) {
        actual += $"{json.Name}={json.Here<string>()} ";
      }

      Assert.AreEqual(expected, actual);
    }

    /// Using <see cref="Json"/>
    [Test]
    public void ArrayChildren() {
      json.Parse(@"{""A"": [23,195,67]}").To("A");

      string expected = "0=23 1=195 2=67 ";
      string actual   = "";

      var children = json.Children;

      for (int i = 0; i < children.Length; i++) {
        actual += $"{json.Name}={json.Here<string>()} ";
      }

      Assert.AreEqual(expected, actual);
    }

    /// Using <see cref="Json"/>
    [Test]
    public void AnchorAndDispose() {
      json.Parse(jsonSampler).To("items.item.0");

      string names    = "";
      var    children = json.Children;

      for (int i = 0; i < children.Length; i++) {
        names += children[i];

        if ((string) children[i] == "magic") {
          using (json.Anchor()) {
            string expected = "122333444455555";
            string actual   = "";
            var    magics   = json.To(children[i]).Children;

            for (int j = 0; j < magics.Length; j++) {
              actual += json.Here<string>();
            }

            Assert.AreEqual(expected, actual);
          }
        }

        // If we want to keep processing from an iterator we need to set an anchor to return to
        names += json.Name;

        if (json.Name == "magic") {
          using (json.Anchor()) {
            string expected = "122333444455555";
            string actual   = "";
            var    magics   = json.Children;

            for (int j = 0; j < magics.Length; j++) {
              actual += json.Here<string>();
            }

            Assert.AreEqual(expected, actual);
          }
        }
      }

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