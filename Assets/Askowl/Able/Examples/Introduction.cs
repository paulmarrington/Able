// Copyright 2019 (C) paul@marrington.net http://www.askowl.net/unity-packages

using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Askowl.Fibers.Examples {
  /// Servicing "Introduction to Fibers" scene
  public class Introduction : MonoBehaviour {
    [SerializeField] private string homePageUrl      = "http://www.askowl.net/";
    [SerializeField] private string documentationUrl = "https://paulmarrington.github.io/Unity-Documentation/";
    [SerializeField] private string doxygenUrl =
      "https://paulmarrington.github.io/Unity-Documentation/xxxx/Doxygen/html/annotated.html";
    [SerializeField] private string videosUrl      = "https://www.youtube.com/playlist?list=";
    [SerializeField] private string supportUrl     = "https://www.patreon.com/paulmarrington";
    [SerializeField] private string exampleScene   = default;
    [SerializeField] private Toggle testsAvailable = default;

    private bool inStart;

    private void Start() {
      inStart = true;
      #if AskowlTests
      testsAvailable.isOn = true;
      #else
      testsAvailable.isOn = false;
      #endif
      inStart = false;
    }

    /// Checkbox to enable or disable tests
    public void EnableTests() {
      if (inStart) return;
      var value = testsAvailable.isOn ? 1 : 0;
      File.WriteAllText(
        "Assets/Askowl/Able/Configuration.json", $"{{\"EnableTesting\": {value}\n\"Update\": \"{DateTime.Now}\"}}");
      EditorApplication.isPlaying = false;
    }

    /// When "Home Page" button is pressed
    public void HomePageButton() => Application.OpenURL(homePageUrl);

    /// When "Home Page" button is pressed
    public void DocumentationButton() => Application.OpenURL(documentationUrl);

    /// When "Home Page" button is pressed
    public void VideosButton() => Application.OpenURL(videosUrl);

    /// When "Home Page" button is pressed
    public void DoxygenButton() => Application.OpenURL(doxygenUrl);

    /// When "Home Page" button is pressed
    public void SupportButton() => Application.OpenURL(supportUrl);

    /// When "Example Scene" button is pressed
    public void ExampleSceneButton() {
      if (!Application.CanStreamedLevelBeLoaded(exampleScene)) PlayModeController.AddSceneToBuildSettings(exampleScene);
      SceneManager.LoadScene(exampleScene);
    }
  }

  /// <inheritdoc />
  [InitializeOnLoad] public sealed class AskowlTest : DefineSymbols {
    static AskowlTest() => EditorApplication.playModeStateChanged += OnPlayModeState;
    private static void OnPlayModeState(PlayModeStateChange state) {
      if (state == PlayModeStateChange.EnteredEditMode) {
        using (var json = Json.Instance.Parse(File.ReadAllText("Assets/Askowl/Able/Configuration.json"))) {
          var enableTests = json.Node.To("EnableTesting").Found && (json.Node.Long == 1);
          AddOrRemoveDefines(enableTests, "AskowlTests");
        }
      }
    }
  }
}