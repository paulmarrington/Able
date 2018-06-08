// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Askowl {
  /// <summary>
  /// Helper code to control a running application with the goal of preparing for live testing.
  /// </summary>
  /// <remarks><a href="http://customassets.marrington.net#playmodecontroller">More...</a></remarks>
  public class PlayModeController {
    /// <summary>
    /// Current scene as seen on the screen
    /// </summary>
    /// <remarks><a href="http://customassets.marrington.net#scene">More...</a></remarks>
    protected Scene Scene = default(Scene);

    /// <summary>
    /// Load scene by name. The scene must be registered in the build for this to be successful
    /// </summary>
    /// <remarks><a href="http://customassets.marrington.net#loadscene">More...</a></remarks>
    /// <param name="name">Name of scene</param>
    /// <returns>Enumerator that will allow a delay until the scene loading is complete</returns>
    protected virtual IEnumerator LoadScene(string name) {
      var handle = SceneManager.LoadSceneAsync(sceneName: name, mode: LoadSceneMode.Single);
      while (!handle.isDone) yield return null;

      Scene = SceneManager.GetActiveScene();
    }

    /// <summary>
    /// Given a reference to an active UI Button, provide the same functionality as a player pressing it.
    /// </summary>
    /// <remarks><a href="http://customassets.marrington.net#pushbutton">More...</a></remarks>
    /// <param name="button">Reference to `Button` instance</param>
    /// <returns>Waits one update cycle so that button actions get a chance to start</returns>
    protected static IEnumerator PushButton(Button button) {
      button.Select();
      button.onClick.Invoke();
      yield return null;
    }

    /// <summary>Given the name of an active UI Button, provide the same functionality as a player pressing it.</summary>
    /// <remarks><a href="http://customassets.marrington.net#pushbutton">More...</a></remarks>
    /// <param name="path">Path to button as a non-contiguous / separated or string array in the project hierarchy</param>
    /// <returns>Waits one update cycle so that button actions get a chance to start</returns>
    protected virtual IEnumerator PushButton(params string[] path) {
      yield return PushButton(Components.Find<Button>(path));
    }
  }
}