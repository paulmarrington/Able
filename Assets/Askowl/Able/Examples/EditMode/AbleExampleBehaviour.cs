#if UNITY_EDITOR && Able
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Askowl.Examples {
  /// <inheritdoc />
  public class AbleExampleBehaviour : MonoBehaviour {
    internal void FindDisabledObject() {
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

    internal void FindGameObjectsAndReturnPath() {
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

    internal void FindComponentGlobal() {
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

    internal void FindComponentLocal() {
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

    internal void CreateComponent() {
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

    [SerializeField, RangeBounds(10, 20)] private Range range = new Range(min: 12, max: 18);

    internal void RangeExample() {
      for (int i = 0; i < 10; i++) {
        int value = (int) range.Pick();

        if ((value < 12) || (value > 18)) {
          Debug.LogError($"{value} is not in range");
          return;
        }
      }

      Debug.Log("RangeExample passed");
    }

    internal void Error(Json json, string fmt, params object[] args) {
      Debug.LogErrorFormat("{0} - {1}", string.Format(fmt, args), json.ErrorMessage);
    }
  }
}
#endif