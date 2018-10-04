// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Askowl {
  /// <a href="http://bit.ly/2NW3mGj">Control a running scene (skeleton implementation)</a>
  public class PlayModeController {
    /// <a href="http://bit.ly/2RhLCD5"></a>
    protected Scene Scene;

    /// <a href="http://bit.ly/2NZbbe8">Load a scene by name (must be in build)</a>
    protected virtual IEnumerator LoadScene(string name) {
      var handle = SceneManager.LoadSceneAsync(sceneName: name, mode: LoadSceneMode.Single);

      if (handle != null) {
        while (!handle.isDone) yield return null;

        Scene = SceneManager.GetActiveScene();
      }
    }

    /// <a href="http://bit.ly/2NUH87i">Push a GUI button</a>
    protected static IEnumerator PushButton(Button button) {
      if (button != null) {
        button.Select();
        button.onClick.Invoke();
        yield return null;
      }
    }

    /// <a href="http://bit.ly/2NZZYKj">See if component is visible on the UI display/screen</a>
    public bool IsDisplayingInUI(RectTransform transform) {
      if ((transform == null) || !transform.gameObject.activeInHierarchy) return false;

      Rect screenRect    = new Rect(0, 0, Screen.width, Screen.height);
      var  objectCorners = new Vector3[4];
      transform.GetWorldCorners(fourCornersArray: objectCorners);
      return screenRect.Contains(point: objectCorners[1]);
    }
  }
}