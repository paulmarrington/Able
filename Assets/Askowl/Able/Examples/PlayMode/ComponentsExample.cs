// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

#if AskowlAble
namespace Askowl.Examples {
  using System;
  using System.Collections;
  using NUnit.Framework;
  using UnityEngine;
  using UnityEngine.TestTools;
  using UnityEngine.UI;

  /// Using <see cref="T:Askowl.Components" /> <inheritdoc />
  public class ComponentsExample : PlayModeTests {
    /// Using <see cref="Components.Find{T}(string)"/>
    [UnityTest]
    public IEnumerator FindPath() {
      yield return LoadScene("Askowl-Able-Examples");

      // No path, returns null (not found)
      var text = Components.Find<Text>();
      Assert.IsNull(text);
      // Sparse GameObject path leading to component of specified type
      text = Components.Find<Text>("Canvas/Level");
      Assert.IsNotNull(text);
      // path can also be a single string. // is convenience to show sparse path
      text = Components.Find<Text>("Canvas/Level//Text");
      Assert.IsNotNull(text);
      // Returns first if path is not unique enough
      text = Components.Find<Text>("Canvas//Text");
      Assert.AreEqual("Button One", text.text);
      // We don't even need to specify the root
      text = Components.Find<Text>("Level");
      Assert.AreEqual("Button Two", text.text);
    }

    /// Using <see cref="Components.Find{T}(GameObject, string[])"/>
    [UnityTest]
    public IEnumerator FindGameObjectPath() {
      yield return LoadScene("Askowl-Able-Examples");

      // if we know the parent object we can start from there.
      var canvas = GameObject.Find("Canvas");
      var text   = Components.Find<Text>(canvas, "Text");
      Assert.AreEqual("Button One", text.text);
    }

    /// Using <see cref="Components.Create{T}"/>
    [UnityTest]
    public IEnumerator Create() {
      yield return LoadScene("Askowl-Able-Examples");

      // create a root GameObject with the specified name and add component
      var text = Components.Create<Text>("Canvas/New Path/Created Text");
      text.text = "Created Text Component";
      Assert.AreEqual("Created Text Component", Components.Find<Text>("Created Text").text);
    }

    /// Using <see cref="Components.Establish{T}"/>
    [UnityTest]
    public IEnumerator Establish() {
      yield return LoadScene("Askowl-Able-Examples");

      // First we create, then we retrieve again
      string path = $"Canvas/{Guid.NewGuid().ToString()}";
      var    text = Components.Establish<Text>(path);
      Assert.AreEqual("", text.text);
      text.text = "Created";
      Assert.AreEqual("Created", Components.Establish<Text>(path).text);
      // but if it exists somewhere we won't create
      text = Components.Find<Text>("Canvas/Level//Text");
      Assert.AreEqual("Button Two", text.text);
    }
  }
}
#endif