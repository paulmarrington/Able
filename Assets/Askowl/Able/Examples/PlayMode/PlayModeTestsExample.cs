// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.TestTools;

namespace Askowl.Examples {
  /// Using <see cref="PlayModeTests" />
  /// <inheritdoc />
  public class PlayModeTestsExample : PlayModeTests {
    /// Using <see cref="PlayModeTests.LoadScene"/>
    [UnityTest]
    public IEnumerator LoadScene() {
      LogAssert.Expect(LogType.Error, new Regex(@".*Nonsense Scene.*"));
      yield return LoadScene("Nonsense Scene");

      LogAssert.Expect(LogType.Assert, new Regex(@".*Values are not equal.*"));
    }

    /// Using <see cref="PlayModeTests.Component{T}"/>
    [UnityTest]
    public IEnumerator ComponentT() {
      yield return LoadScene("Askowl-Able-Examples");

      var buttonTwo = Component<RectTransform>("Canvas//Button Two");
      Assert.IsNotNull(buttonTwo);

      LogAssert.Expect(LogType.Assert, new Regex(@".*For button 'Nonsense..Button Fifty-Five'.*"));
      buttonTwo = Component<RectTransform>("Nonsense//Button Fifty-Five");
      Assert.IsNull(buttonTwo);
    }

    /// Using <see cref="PlayModeTests.FindGameObject"/>
    [UnityTest]
    public IEnumerator FindGameObject() {
      yield return LoadScene("Askowl-Able-Examples");

      LogAssert.Expect(LogType.Assert, new Regex(@".*Values are equal.*"));
      FindGameObject("Button 23");
    }

    /// Using <see cref="PlayModeTests.FindObject{T}"/>
    [UnityTest]
    public IEnumerator FindObject() {
      yield return LoadScene("Askowl-Able-Examples");

      FindObject<GameObject>("Button 23");
      LogAssert.Expect(LogType.Assert, new Regex(@".*Values are equal.*"));
    }

    /// Using <see cref="PlayModeTests.IsDisplayingInUI(string, bool, int)"/>
    [UnityTest]
    public IEnumerator IsDisplayingInUI() {
      yield return LoadScene("Askowl-Able-Examples");
      yield return IsDisplayingInUI("Button One", visible: true, repeats: 10);

      LogAssert.Expect(LogType.Assert, new Regex(@".*IsDisplayingInUI 'Button One' failed to act as expected.*"));
      yield return IsDisplayingInUI("Button One", visible: false, repeats: 20);
    }

    /// Using <see cref="PlayModeTests.PushButton(string[])"/>
    [UnityTest]
    public IEnumerator PushButton() {
      yield return LoadScene("Askowl-Able-Examples");

      LogAssert.Expect(LogType.Assert, new Regex(@".*For button 'Button 2367'.*"));
      yield return PushButton("Button 2367");
    }

    /// Using <see cref="PlayModeTests.CheckPattern"/>
    [UnityTest]
    public IEnumerator CheckPattern() {
      yield return LoadScene("Askowl-Able-Examples");

      CheckPattern(new Regex(".*Test Pattern.*"), "This is a Test Pattern for A");
    }
  }
}