// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

#if AskowlAble
namespace Askowl.Examples {
  using NUnit.Framework;

  /// Using <see cref="Json"/>
  public class JsonExamples {
    private Json json = Json.Instance;

    /// Using <see cref="Json.Parse"/>
    [Test]
    public void Parse() {
      // reasonably accurate JSON
      json.Parse(@"{""Not my age"":23}");
      Assert.AreEqual("23", json.Node["Not my age"]);
      // no starting braces
      json.Parse(@"""Not my age"":24");
      Assert.AreEqual("24", json.Node["Not my age"]);
      // no colon either
      json.Parse(@"""Not my age"" 25");
      Assert.AreEqual("25", json.Node["Not my age"]);
      // no quotes around key
      json.Parse(@"Not my age: 26");
      Assert.AreEqual("26", json.Node["Not my age"]);
    }

    [Test]
    public void ParseSampler() {
      json.Parse(jsonSampler);
      Assert.AreEqual(1, json.Node.To("items").Children.Length);
      Assert.AreEqual(1, json.Node.To("items.item").Children.Length);
      Assert.AreEqual(8, json.Node.To("items.item.0").Children.Length);
      json.Node.To("items.item.0");

      using (json.Node.Anchor()) Assert.AreEqual(5, json.Node.Next("magic").Children.Length);

      Assert.AreEqual("Maple", json.Node.Next("topping.6.type").Value);
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
#endif