// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

#if AskowlTests
namespace Askowl.Able.Examples {
  using System;
  using UnityEngine;
  using UnityEngine.TestTools;

  /// <a href=""></a>
  /// <inheritdoc />
  public class SetExamples : MonoBehaviour {
    [Serializable] public class StringSet : Set<string> { }

    // ReSharper disable once NotAccessedField.Local
    [SerializeField] private StringSet stringSet;

    [UnityTest] public void Pick() { }

    [UnityTest] public void Add() { }

    [UnityTest] public void Remove() { }

    [UnityTest] public void Contains() { }

    [UnityTest] public void Count() { }

    [UnityTest] public void ForEach() { }

    [UnityTest] public void BuildSelector() { }

    [UnityTest] public void ResetExample() { }
  }
}
#endif