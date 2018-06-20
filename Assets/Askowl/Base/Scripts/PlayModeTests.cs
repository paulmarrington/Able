// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Askowl {
  /// <inheritdoc />
  /// <summary>
  /// Base class for PlayMode Unity tests. Provides explicit `Setup` and `Teardown` functions.
  /// </summary>
  /// <remarks><a href="http://customassets.marrington.net#playmodetests">More...</a></remarks>
  public class PlayModeTests : PlayModeController {
    /*
     * Built-In Helpers
     * https://docs.unity3d.com/Manual/PlaymodeTestFramework.html
     * LogAssert.Expect(LogType, string);
     * LogAssert.Expect(LogType, Regex);
     */

    /// <inheritdoc />
    /// <remarks><a href="http://customassets.marrington.net#playmodetests">More...</a></remarks>
    protected override IEnumerator LoadScene(string name) {
      yield return base.LoadScene(name);

      Assert.AreEqual(name, Scene.name);
    }

    /// <summary>
    /// Override for Objects.Component to check the result for compliance. Finds an active GameObject in the scene and retrieved a component by type from it.
    /// </summary>
    /// <see cref="Objects"/>
    /// <param name="path">Path to GameObject - that need only include unique elements</param>
    /// <typeparam name="T">Type of component to retrieve</typeparam>
    /// <returns></returns>
    /// <remarks><a href="http://customassets.marrington.net#playmodetests">More...</a></remarks>
    protected static T Component<T>(params string[] path) where T : Component {
      T component = Components.Find<T>(path);

      Assert.AreNotEqual(expected: default(T), actual: component,
                         message: "For button " + Csv<string>.Instance(path));

      return component;
    }

    /// <summary>
    /// Override for Objects.Find to check the result for compliance. Finds an active object. Used to retrieve custom assets that have been loaded into the scene elsewhere.
    /// </summary>
    /// <remarks><a href="http://customassets.marrington.net#playmodetests">More...</a></remarks>
    /// <see cref="Objects"/>
    /// <param name="name">Name of object to find</param>
    /// <returns>GameObject</returns>
    protected static GameObject FindGameObject(string name) { return FindObject<GameObject>(name); }

    /// <summary>
    /// Override for Objects.Find to check the result for compliance. Finds an active object. Used to retrieve custom assets that have been loaded into the scene elsewhere.
    /// </summary>
    /// <remarks><a href="http://customassets.marrington.net#playmodetests">More...</a></remarks>
    /// <see cref="Objects"/>
    /// <param name="name">Name of object to find</param>
    /// <typeparam name="T">Type of object to find</typeparam>
    /// <returns>Object</returns>
    protected static T FindObject<T>(string name) where T : Object {
      T[] objects = Objects.Find<T>(name);
      Assert.AreNotEqual(objects.Length, 0);
      return objects[0];
    }

    /// <summary>
    /// Wait for a UI component to be visible or invisible to the player
    /// </summary>
    /// <param name="gameObjectPath">Path to the game object we want (in)visible</param>
    /// <param name="visible">true to wait until visible, fals to wait until not visible</param>
    /// <returns></returns>
    public IEnumerator IsDisplayingInUI(string gameObjectPath, bool visible = true) {
      while (base.IsDisplayingInUI(gameObjectPath) != visible) yield return null;
    }

    /// <inheritdoc />
    /// <remarks><a href="http://customassets.marrington.net#playmodetests">More...</a></remarks>
    protected override IEnumerator PushButton(params string[] path) {
      yield return PushButton(Component<Button>(path)); // so it uses test version with assert
    }

    /// <summary>
    /// Given text extracted from the running game, check it against a regular expression and freak out if it doesn't match
    /// </summary>
    /// <remarks><a href="http://customassets.marrington.net#checkpattern">More...</a></remarks>
    /// <param name="pattern">String representation of a regular expression</param>
    /// <param name="against">String to check against the regular expression</param>
    protected static void CheckPattern(string pattern, string against) {
      Regex           regex   = new Regex(pattern);
      MatchCollection matches = regex.Matches(against);
      Assert.AreEqual(matches.Count, 1, against);
    }
  }
}