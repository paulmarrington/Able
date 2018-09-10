// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Askowl {
  /// <a href=""></a>
  /// <inheritdoc />
  public class PlayModeTests : PlayModeController {
    /*
     * Built-In Helpers
     * https://docs.unity3d.com/Manual/PlaymodeTestFramework.html
     * LogAssert.Expect(LogType, string);
     * LogAssert.Expect(LogType, Regex);
     */

    /// <a href=""></a>
    /// <inheritdoc />
    protected override IEnumerator LoadScene(string name) {
      yield return base.LoadScene(name);

      Assert.AreEqual(name, Scene.name);
    }

    /// <a href=""></a>
    protected static T Component<T>(params string[] path) where T : Component {
      T component = Components.Find<T>(path);

      Assert.IsNotNull(value: component, message: $"For button '{Csv.ToString(path)}'");

      return component;
    }

    /// <a href=""></a>
    protected static GameObject FindGameObject(string name) => FindObject<GameObject>(name);

    /// <a href=""></a>
    protected static T FindObject<T>(string name) where T : Object {
      T[] objects = Objects.FindAll<T>(name);
      Assert.AreNotEqual(0, objects?.Length);
      return (objects?.Length > 0) ? objects[0] : null;
    }

    /// <a href=""></a>
    public IEnumerator IsDisplayingInUI(string path, bool visible = true, int repeats = 300) {
      for (var count = 0; count < repeats; count++) {
        if (base.IsDisplayingInUI(Components.Find<RectTransform>(path)) == visible) yield break;

        yield return null;
      }

      Assert.IsFalse(true, $"IsDisplayingInUI '{path}' failed to act as expected");
    }

    /// <a href=""></a>
    protected IEnumerator PushButton(params string[] path) {
      yield return PushButton(Component<Button>(path)); // so it uses test version with assert
    }

    /// <a href=""></a>
    protected static void CheckPattern(Regex regex, string against) {
      MatchCollection matches = regex.Matches(against);
      Assert.AreEqual(matches.Count, 1, against);
    }
  }
}