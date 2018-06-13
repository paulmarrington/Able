﻿using System;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

// ReSharper disable MissingXmlDoc

#if UNITY_EDITOR && AskowlBase
namespace Askowl.Samples {
  public class BaseExampleBehaviour : MonoBehaviour {
    public void FindDisabledObject() {
      MeshFilter[] cubes = Objects.Find<MeshFilter>("Disabled Cube");

      if (cubes.Length != 1) {
        Debug.LogErrorFormat("Found {0} matching game objects.", cubes.Length);
        return;
      }

      if (cubes.Length == 0) return;

      cubes[0].gameObject.SetActive(!cubes[0].gameObject.activeInHierarchy);
      Debug.LogFormat("Cube is now {0}abled", cubes[0].gameObject.activeInHierarchy ? "En" : "Dis");
      Debug.Log("FindDisabledObject passed");
    }

    public void FindGameObjectsAndReturnPath() {
      GameObject[] texts = Objects.FindGameObjects("Text 2");

      if (texts.Length != 1) {
        Debug.LogErrorFormat("Found {0} matching text game objects.", texts.Length);
        return;
      }

      string path = Objects.Path(texts[0]);

      if (path != "Canvas/FindGameObjectsAndPath/Text 2") {
        Debug.LogErrorFormat("Is at path '{0}'.", path);
        return;
      }

      Debug.Log("FindGameObjectsAndReturnPath passed");
    }

    public void FindComponentGlobal() {
      Text text = Components.Find<Text>("Canvas", "Text 2");

      if (text == null) {
        Debug.LogErrorFormat("Component 'Text 2' not found");
        return;
      }

      GameObject textObject = Objects.FindGameObject("Text 2");

      if (textObject == null) {
        Debug.LogErrorFormat("Objects.FindGameObject 'Text 2' not found");
        return;
      }

      if (text.gameObject != textObject) {
        Debug.LogErrorFormat(
          @"Components and Objects finds did not return the same object\n{0}\n{1}",
          Objects.Path(text.gameObject), Objects.Path(textObject));

        return;
      }

      Debug.Log("findComponentGlobal passed");
    }

    public void FindComponentLocal() {
      GameObject textObject = Objects.FindGameObject("Canvas");
      Text       text2      = Components.Find<Text>(textObject, "Text 2");

      if (text2 == null) {
        Debug.LogErrorFormat("Failed to find the game object text component with a local search");
        return;
      }

      Text textGlobal = Components.Find<Text>("Canvas", "Text 2");

      if (text2 != textGlobal) {
        Debug.LogErrorFormat("Found incorrect game object with a local search");
        return;
      }

      Debug.Log("findComponentGlobal passed");
    }

    public void CreateComponent() {
      if (Objects.Find<Text>("Created GameObject").Length != 0) {
        Debug.LogErrorFormat("Can't create what is already there");
        return;
      }

      Text newText = Components.Create<Text>("Created GameObject");

      Text[] created = Objects.Find<Text>("Created GameObject");

      if (created.Length != 1) {
        Debug.LogErrorFormat("Components.Create failed");
        return;
      }

      if (created[0] != newText) {
        Debug.LogErrorFormat("Components.Create returns an incorrect component");
        return;
      }

      Debug.Log("CreateComponent passed");
    }

    private class PickImplementation : Pick<string> {
      private int    count;
      public  string Pick() { return (++count).ToString(); }
    }

    public void PickExample() {
      PickImplementation nose = new PickImplementation();

      if ((nose.Pick() != "1") || (nose.Pick() != "2") || (nose.Pick() != "3")) {
        Debug.LogErrorFormat("Pick<T> failed to perform as expected");
      }

      Debug.Log("PickExample passed");
    }

    [SerializeField, RangeBounds(10, 20)] private Range range = new Range(min: 12, max: 18);

    public void RangeExample() {
      for (int i = 0; i < 10; i++) {
        int value = (int) range.Pick();

        if (value < 12 || value > 18) {
          Debug.LogErrorFormat("{0} is not in range", value);
          return;
        }
      }

      Debug.Log("RangeExample passed");
    }

    public void SelectorExample() {
      int[] ints   = {0, 1, 2, 3, 4};
      int[] counts = {0, 0, 0, 0, 0};

      // default is random
      Selector<int> selector = new Selector<int>(choices: ints);

      for (int i = 0; i < 10; i++) {
        int pick = selector.Pick();
        counts[pick]++;
      }

      bool failed = true;

      for (int i = 0; failed && (i < counts.Length); i++) {
        if (counts[i] != 2) failed = false;
      }

      if (failed) {
        Debug.LogErrorFormat("Random selector too even: {0}",
                             string.Join(", ", Array.ConvertAll(counts, i => i.ToString())));

        return;
      }

      // or we can be sequential
      selector = new Selector<int>(choices: ints, isRandom: false);

      for (int i = 0; i < 10; i++) {
        int pick = selector.Pick();

        if (pick != i % 5) {
          Debug.LogErrorFormat("Sequential failed with {0} on interation {1}", pick, i);
        }
      }

      // or we can be random, but exhaust all possibilities before going round again
      selector = new Selector<int>(choices: ints, exhaustiveBelow: 100);

      counts = new int[] {0, 0, 0, 0, 0};

      for (int i = 0; i < 10; i++) {
        int pick = selector.Pick();
        counts[pick]++;
      }

      for (int i = 0; i < counts.Length; i++) {
        if (counts[i] != 2) {
          Debug.LogErrorFormat("Exhaustive Random failure: {0}",
                               string.Join(", ", Array.ConvertAll(counts, j => j.ToString())));

          return;
        }
      }

      // Unless our number of choices are below a watermark value
      selector = new Selector<int>(choices: ints, exhaustiveBelow: 4);

      for (int i = 0; i < 10; i++) {
        int pick = selector.Pick();
        counts[pick]++;
      }

      failed = true;

      for (int i = 0; failed && (i < counts.Length); i++) {
        if (counts[i] != 2) failed = false;
      }

      if (failed) {
        Debug.LogErrorFormat("Exhaustive Random selector below minimum too even: {0}",
                             string.Join(", ", Array.ConvertAll(counts, i => i.ToString())));

        return;
      }

      Debug.Log("SelectorExample passed");
    }

    public void JsonExample() {
      Debug.LogFormat("Expecting: {0}", jsonSampler);

      // We cah pass in json for parsing...
      Json json = new Json(jsonSampler);
      // ... or use Reset later to change the string.
      json.Parse(jsonSampler);

      // Here will retrieve the current node or leaf based on type
      string rootAsString = json.Here<string>();
      if (rootAsString != null) Debug.LogErrorFormat("Root isn't a string");

      // You can also check the type of here
      if (json.IsA<String>() != false) Debug.LogErrorFormat("IsA failed to work for root");

      // If you know the structure of your JSON you can retrieve a leaf directly
      string id = json.Get<string>("items", "item", 0, "id");
      if (id != "0001") Debug.LogErrorFormat("Expecting an ID of '0001', not {0}", id);

      // If you provide the wrong type the default<T> is returned - 0 for an int
      int iid = json.Get<int>("items", "item", "0", "id");
      if (iid != 0) Debug.LogErrorFormat("Expecting a failure in kind, not {0}", iid);
      // If this is a problem, use IsA<>
      if (json.IsA<int>()) Debug.LogErrorFormat("Expecting a failure in kind, not {0}", iid);

      // You can separate walking the tree and retrievaly using Walk, WalkOn, IsA and Here
      if (!json.Walk("items.item.0.type")) Debug.LogErrorFormat("Can't find the donut");
      if (!json.IsA<string>()) Debug.LogErrorFormat("Expecting Donut to be a string");
      string donut = json.Here<string>();
      if (donut != "donut") Debug.LogErrorFormat("Expecting 'donut', not '{0}'", donut);

      // Walk is absolute, but you can use WalkOn to get where you want in steps
      if (!json.Walk("items.item")) Debug.LogErrorFormat("Can't find the first item");
      if (!json.WalkOn(0, "item")) Debug.LogErrorFormat("Can't walk on to the donut");
      donut = json.Here<string>();
      if (donut != "donut") Debug.LogErrorFormat("Expecting 'donut', not '{0}'", donut);

      // Because walking to a leave and expecting a certain type of data is common, we can combine them
      if (!json.Walk<double>("items", "item", 0, "ppu"))
        Debug.LogErrorFormat("Can't walk to a number");

      // note that floating point numbers are doubles and integers are longs
      double ppu = json.Here<double>();

      Debug.LogErrorFormat("MORE TO DO");
    }

    [SerializeField, Multiline] private string jsonSampler = @"{
  ""items"":
    {
      ""item"":
        [
          {
            ""id"": ""0001"",
            ""type"": ""donut"",
            ""name"": ""Cake"",
            ""ppu"": 0.55,
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