// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Askowl {
  /// <a href=""></a>
  public class PlayModeController {
    /// <a href=""></a>
    protected Scene Scene = default(Scene);

    /// <a href=""></a>
    protected virtual IEnumerator LoadScene(string name) {
      var handle = SceneManager.LoadSceneAsync(sceneName: name, mode: LoadSceneMode.Single);

      if (handle != null) {
        while (!handle.isDone) yield return null;

        Scene = SceneManager.GetActiveScene();
      }
    }

    /// <a href=""></a>
    protected static IEnumerator PushButton(Button button) {
      if (button != null) {
        button.Select();
        button.onClick.Invoke();
        yield return null;
      }
    }

    /// <a href=""></a>
    public bool IsDisplayingInUI(RectTransform transform) {
      if ((transform == null) || !transform.gameObject.activeInHierarchy) return false;

      Rect      screenRect    = new Rect(0, 0, Screen.width, Screen.height);
      Vector3[] objectCorners = new Vector3[4];
      transform.GetWorldCorners(fourCornersArray: objectCorners);
      return screenRect.Contains(point: objectCorners[1]);
    }
  }
}