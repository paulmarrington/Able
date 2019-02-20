// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Askowl {
  /// <a href="http://bit.ly/2NW3mGj">Control a running scene (skeleton implementation)</a>
  public partial class PlayModeController {
    /// <a href="http://bit.ly/2RhLCD5"></a>
    protected Scene Scene;

    /// <a href="http://bit.ly/2NZbbe8">Load a scene by name (must be in build)</a>
    protected virtual IEnumerator LoadScene(string sceneName) {
      if (!Application.CanStreamedLevelBeLoaded(sceneName)) {
        Debug.Log( //throw new Exception(
          $@"Add the following to your file:
          private static string sceneName = ""{sceneName}"";
          #if UNITY_EDITOR
          [InitializeOnLoadMethod] private static void AddSceneToBuildSettings() => AddSceneToBuildSettings(sceneName);
          #endif
          ");
      }
      var handle = SceneManager.LoadSceneAsync(sceneName: sceneName, mode: LoadSceneMode.Single);
      if (handle == null) yield break;
      while (!handle.isDone) yield return null;
      Scene = SceneManager.GetActiveScene();
      yield return null;
    }

    /// <a href=""></a> //#TBD#//
    public static void AddSceneToBuildSettings(string path) {
      #if UNITY_EDITOR
      path = Objects.FindFile($"{path}.unity");
      if (path == null) return;
      var scenes = EditorBuildSettings.scenes;
      for (int i = scenes.Length - 1; i >= 0; i--) { // most likely at the end
        if (scenes[i].path == path) return;
      }

      var newSettings = new EditorBuildSettingsScene[scenes.Length + 1];
      Array.Copy(scenes, newSettings, scenes.Length);
      var sceneToAdd = new EditorBuildSettingsScene(path, true);
      newSettings[newSettings.Length - 1] = sceneToAdd;
      EditorBuildSettings.scenes          = newSettings;
      #endif
    }

    /// <a href="http://bit.ly/2NUH87i">Push a GUI button</a>
    public static IEnumerator PushButton(Button button) {
      if (button != null) {
        button.Select();
        button.onClick.Invoke();
        yield return null;
      }
    }

    /// <a href="http://bit.ly/2NZZYKj">See if component is visible on the UI display/screen</a>
    public bool IsDisplayingInUi(RectTransform transform) {
      if ((transform == null) || !transform.gameObject.activeInHierarchy) return false;

      Rect screenRect    = new Rect(0, 0, Screen.width, Screen.height);
      var  objectCorners = new Vector3[4];
      transform.GetWorldCorners(fourCornersArray: objectCorners);
      if (Compare.AlmostEqual(objectCorners[0].y, objectCorners[1].y)) return false;
      if (Compare.AlmostEqual(objectCorners[1].x, objectCorners[2].x)) return false;
      return screenRect.Contains(point: objectCorners[1]);
    }
  }
}