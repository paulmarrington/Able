// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

using System;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Askowl {
  /// <a href="http://bit.ly/2NZZYKj">PlayModeController wrapper with asserts</a> <inheritdoc />
  public class PlayModeTests : PlayModeController {
    /*
     * Built-In Helpers
     * https://docs.unity3d.com/Manual/PlaymodeTestFramework.html
     * LogAssert.Expect(LogType, string);
     * LogAssert.Expect(LogType, Regex);
     */

    /// <a href="http://bit.ly/2NZZYKj">Load scene. Assert on failure</a> <inheritdoc />
    protected override IEnumerator LoadScene(string sceneName) {
      yield return base.LoadScene(sceneName);
      CheckPattern(new Regex($"^({Scene.name}|.*/{Scene.name})"), sceneName);
    }

    /// <a href="http://bit.ly/2NTezHo">Components.Find, assert on failure</a>
    protected static T Component<T>(string path) where T : Component {
      var component = Components.Find<T>(path);
      Assert.IsNotNull(value: component, message: $"'{path}' not found");
      return component;
    }

    /// <a href="http://bit.ly/2Rj0WQ6">FindObject{GameObject} shortcut</a>
    protected static GameObject FindGameObject(string name) => FindObject<GameObject>(name);

    /// <a href="http://bit.ly/2Rj0WQ6">Objects.Find, assert if none found</a>
    protected static T FindObject<T>(string name) where T : Object {
      var objects = Objects.FindAll<T>(name);
      Assert.AreNotEqual(0, objects?.Length);
      return objects?.Length > 0 ? objects[0] : null;
    }

    /// <a href="http://bit.ly/2NZZYKj">IsDisplayingInUI, assert if not visible/invisible as expected after nn frames</a>
    public IEnumerator IsDisplayingInUi(string path, bool visible = true, int repeats = 300) {
      for (var count = 0; count < repeats; count++) {
        if (IsDisplayingInUi(Components.Find<RectTransform>(path)) == visible) yield break;

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

    /// <a href=""></a> //#TBD#//
    protected static void Fail(string message) => throw new Exception(message);

    #region Unity Editor Only Methods
    #if UNITY_EDITOR
    /// <a href=""></a> //#TBD#//
    protected static void SelectMenuItem(string path) =>
      Assert.IsTrue(EditorApplication.ExecuteMenuItem(path));
    #endif
    #endregion
  }
}