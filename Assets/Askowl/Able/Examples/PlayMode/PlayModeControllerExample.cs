// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

#if AskowlAble
namespace Askowl.Examples {
  using System.Collections;
  using NUnit.Framework;
  using UnityEngine;
  using UnityEngine.TestTools;
  using UnityEngine.UI;

  /// Using <see cref="PlayModeController" /> <inheritdoc />
  public class PlayModeControllerExample : PlayModeController {
    /// Using <see cref="PlayModeController.LoadScene"/>
    [UnityTest]
    public IEnumerator SceneReference() {
      yield return LoadScene("Askowl-Able-Examples");

      Assert.AreEqual("Askowl-Able-Examples", Scene.name);
    }

    /// Using <see cref="PlayModeController.LoadScene"/>
    [UnityTest]
    public IEnumerator LoadScene() {
      yield return LoadScene("Askowl-Able-Examples");

      Assert.IsTrue(Scene.isLoaded);
    }

    /// Using <see cref="PlayModeController.PushButton(Button)"/>
    [UnityTest]
    public IEnumerator PushButton() {
      yield return LoadScene("Askowl-Able-Examples");

      var button = Components.Find<Button>("Button One");

      yield return PushButton(button);

      LogAssert.Expect(LogType.Log, "Button One Pressed");
    }

    /// Using <see cref="PlayModeController.IsDisplayingInUI"/>
    [UnityTest]
    public IEnumerator IsDisplayingInUI() {
      yield return LoadScene("Askowl-Able-Examples");

      var rectTransform = Components.Find<RectTransform>("Button Two");
      var transform     = rectTransform.gameObject.transform;
      var was           = transform.position;

      Assert.IsTrue(IsDisplayingInUI(rectTransform));

      transform.position = new Vector3(was.x + Screen.width, was.y, was.z);
      yield return null;

      Assert.IsFalse(IsDisplayingInUI(rectTransform));

      transform.position = new Vector3(was.x, was.y, was.z);
      yield return null;

      Assert.IsTrue(IsDisplayingInUI(rectTransform));
    }
  }
}
#endif