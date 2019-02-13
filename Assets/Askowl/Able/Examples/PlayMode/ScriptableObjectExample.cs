// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

// ReSharper disable NotAccessedField.Local

#if AskowlTests
namespace Askowl.Able.Examples {
  using System.Collections;
  using UnityEngine;

  /// Using <see cref="ScriptableObjectDrawer" /> <inheritdoc />
  [CreateAssetMenu(menuName = "Examples/BasicScriptableObject", fileName = "ScriptableObjectExample"),
   Labels("String One", "Replaced One", "String Two", "Replaced Two")]
  public class ScriptableObjectExample : ScriptableObject {
    /// A do-nothing coroutine
    public IEnumerator A() { yield return null; }

    [SerializeField, Label] private string stringOne;
    [SerializeField, Label] private string stringTwo;
  }
}
#endif