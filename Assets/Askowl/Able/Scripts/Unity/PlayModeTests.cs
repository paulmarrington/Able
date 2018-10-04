// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

namespace Askowl {
  using System.Collections;
  using System.Text.RegularExpressions;
  using UnityEngine;
  using UnityEngine.Assertions;
  using UnityEngine.UI;

  /// <a href="http://bit.ly/2NZZYKj">PlayModeController wrapper with asserts</a> <inheritdoc />
  public class PlayModeTests : PlayModeController {
    /*
     * Built-In Helpers
     * https://docs.unity3d.com/Manual/PlaymodeTestFramework.html
     * LogAssert.Expect(LogType, string);
     * LogAssert.Expect(LogType, Regex);
     */

    /// <a href="http://bit.ly/2NZZYKj">Load scene. Assert on failure</a>
    /// <inheritdoc />
    protected override IEnumerator LoadScene(string name) {
      yield return base.LoadScene(name);

      Assert.AreEqual(name, Scene.name);
    }

    /// <a href="http://bit.ly/2NTezHo">Components.Find, assert on failure</a>
    protected static T Component<T>(string path) where T : Component {
      var component = Components.Find<T>(path);
      Assert.IsNotNull(value: component, message: $"For '{path}'");
      return component;
    }

    /// <a href="http://bit.ly/2Rj0WQ6">FindObject{GameObject} shortcut</a>
    // ReSharper disable once UnusedMethodReturnValue.Global
    protected static GameObject FindGameObject(string name) => FindObject<GameObject>(name);

    /// <a href="http://bit.ly/2Rj0WQ6">Objects.Find, assert if none found</a>
    protected static T FindObject<T>(string name) where T : Object {
      var objects = Objects.FindAll<T>(name);
      Assert.AreNotEqual(0, objects?.Length);
      return objects?.Length > 0 ? objects[0] : null;
    }

    /// <a href="http://bit.ly/2NZZYKj">IsDisplayingInUI, assert if not visible/invisible as expected after nn frames</a>
    public IEnumerator IsDisplayingInUI(string path, bool visible = true, int repeats = 300) {
      for (var count = 0; count < repeats; count++) {
        if (base.IsDisplayingInUI(Components.Find<RectTransform>(path)) == visible) yield break;

        yield return null;
      }

      Assert.IsFalse(true, $"IsDisplayingInUI '{path}' failed to act as expected");
    }

    /// <a href="http://bit.ly/2Oq9xBM">Push button, assert if it can't be found</a>
    protected IEnumerator PushButton(string path) {
      yield return PushButton(Component<Button>(path)); // so it uses test version with assert
    }

    /// <a href="http://bit.ly/2OrQoPH">Check string against regex, assert if no match</a>
    protected static void CheckPattern(Regex regex, string against) {
      MatchCollection matches = regex.Matches(against);
      Assert.AreEqual(matches.Count, 1, against);
    }
  }
}