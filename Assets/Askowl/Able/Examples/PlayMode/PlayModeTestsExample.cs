// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

using System.Collections;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.TestTools;
#if AskowlTests

namespace Askowl.Able.Examples {
  /// Using <see cref="PlayModeTests" />
  /// <inheritdoc />
  public class PlayModeTestsExample : PlayModeTests {
    private static string sceneName = "Askowl-Able-Examples";
    #if UNITY_EDITOR
    [InitializeOnLoadMethod] private static void AddSceneToBuildSettings() => AddSceneToBuildSettings(sceneName);
    #endif

    /// Using <see cref="PlayModeTests.LoadScene"/>
    [UnityTest] public IEnumerator LoadScene() {
      LogAssert.Expect(LogType.Error, new Regex(@".*Nonsense Scene.*"));
      yield return LoadScene("Nonsense Scene");

      LogAssert.Expect(LogType.Assert, new Regex(@".*Values are not equal.*"));
    }

    /// Using <see cref="PlayModeTests.Component{T}"/>
    [UnityTest] public IEnumerator ComponentT() {
      yield return LoadScene(sceneName);

      var buttonTwo = Component<RectTransform>("Canvas//Button Two");
      Assert.IsNotNull(buttonTwo);

      LogAssert.Expect(LogType.Assert, new Regex(@".*'Nonsense//Button Fifty-Five' not found.*"));
      buttonTwo = Component<RectTransform>("Nonsense//Button Fifty-Five");
      Assert.IsNull(buttonTwo);
    }

    /// Using <see cref="PlayModeTests.FindGameObject"/>
    [UnityTest] public IEnumerator FindGameObject() {
      yield return LoadScene(sceneName);

      LogAssert.Expect(LogType.Assert, new Regex(@".*Values are equal.*"));
      FindGameObject("Button 23");
    }

    /// Using <see cref="PlayModeTests.FindObject{T}"/>
    [UnityTest] public IEnumerator FindObject() {
      yield return LoadScene(sceneName);

      FindObject<GameObject>("Button 23");
      LogAssert.Expect(LogType.Assert, new Regex(@".*Values are equal.*"));
    }

    /// Using <see cref="PlayModeTests.IsDisplayingInUi(string, bool, int)"/>
    [UnityTest] public IEnumerator IsDisplayingInUi() {
      yield return LoadScene(sceneName);
      yield return IsDisplayingInUi("Button One", visible: true, repeats: 10);

      LogAssert.Expect(LogType.Assert, new Regex(@".*IsDisplayingInUI 'Button One' failed to act as expected.*"));
      yield return IsDisplayingInUi("Button One", visible: false, repeats: 20);
    }

    /// Using <see cref="PlayModeTests.PushButton(string)"/>
    [UnityTest] public IEnumerator PushButton() {
      yield return LoadScene(sceneName);

      LogAssert.Expect(LogType.Assert, new Regex(@".*'Button 2367' not found.*"));
      yield return PushButton("Button 2367");
    }

    /// Using <see cref="PlayModeTests.CheckPattern"/>
    [UnityTest] public IEnumerator CheckPattern() {
      yield return LoadScene(sceneName);

      CheckPattern(new Regex(".*Test Pattern.*"), "This is a Test Pattern for A");
    }
  }
}
#endif