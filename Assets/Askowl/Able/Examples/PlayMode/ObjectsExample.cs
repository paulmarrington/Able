// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

using UnityEditor;
#if AskowlTests
namespace Askowl.Able.Examples {
  using System.Collections;
  using NUnit.Framework;
  using UnityEngine;
  using UnityEngine.TestTools;

  /// Using <see cref="T:Askowl.Objects" /> <inheritdoc />
  public class ObjectsExamples : PlayModeTests {
    private static string sceneName = "Askowl-Able-Examples";
    #if UNITY_EDITOR
    [InitializeOnLoadMethod] private static void AddSceneToBuildSettings() => AddSceneToBuildSettings(sceneName);
    #endif

    /// Using <see cref="Objects.Find{T}"/>
    [UnityTest]
    public IEnumerator Find() {
      yield return LoadScene(sceneName);

      var gameObject = Objects.Find<GameObject>("Level");
      Assert.IsNotNull(gameObject);

      gameObject = Objects.Find<GameObject>("No Level");
      Assert.IsNull(gameObject);

      gameObject = Objects.Find<GameObject>("Text");
      Assert.AreEqual("Button Two", gameObject.transform.parent.name);
    }

    /// Using <see cref="Objects.FindAll{T}"/>
    [UnityTest]
    public IEnumerator FindAll() {
      yield return LoadScene(sceneName);

      var gameObjects = Objects.FindAll<GameObject>("Level");
      Assert.AreEqual(1, gameObjects.Length);

      gameObjects = Objects.FindAll<GameObject>("No Level");
      Assert.AreEqual(0, gameObjects.Length);

      gameObjects = Objects.FindAll<GameObject>("Text");
      Assert.AreEqual(2, gameObjects.Length);
    }

    /// Using <see cref="Objects.Path"/>
    [UnityTest]
    public IEnumerator Path() {
      yield return LoadScene(sceneName);

      var gameObject = Objects.Find<GameObject>("Text");
      string path       = Objects.Path(gameObject);
      Assert.AreEqual("Canvas/Level/Button Two/Text", path);
    }

    /// Using <see cref="Objects.CreateGameObject"/>
    [UnityTest]
    public IEnumerator CreateGameObject() {
      yield return LoadScene(sceneName);

      var gameObject  = Objects.CreateGameObject("Canvas/Level Two/Button Three");
      var gameObject2 = Objects.Find<GameObject>("Button Three");
      Assert.AreEqual(gameObject, gameObject2);
    }
  }
}
#endif