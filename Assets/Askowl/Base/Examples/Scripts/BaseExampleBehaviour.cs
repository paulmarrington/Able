using System;
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
  }
}
#endif