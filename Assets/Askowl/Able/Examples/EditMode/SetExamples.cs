// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

using System;
using UnityEngine;
using UnityEngine.TestTools;

namespace Askowl.Examples {
  /// <remarks><a href="http://unitydoc.marrington.net/Able#setcs-unity-component-implementing-a-selector">Sets</a></remarks>
  public class SetExamples : MonoBehaviour {
    [Serializable]
    public class StringSet : Set<string> { }

    [SerializeField] private StringSet stringSet;

    /// <remarks><a href="http://unitydoc.marrington.net/Able#pick-from-selector">Pick an Item</a></remarks>
    [UnityTest]
    public void Pick() { }

    /// <remarks><a href="http://unitydoc.marrington.net/Able#saddentry">Add an Entry</a></remarks>
    [UnityTest]
    public void Add() { }

    /// <remarks><a href="http://unitydoc.marrington.net/Able#saddentry">Add an Entry</a></remarks>
    [UnityTest]
    public void Remove() { }

    /// <remarks><a href="http://unitydoc.marrington.net/Able#containsentry">Contains(entry)</a></remarks>
    [UnityTest]
    public void Contains() { }

    /// <remarks><a href="http://unitydoc.marrington.net/Able#count">Count</a></remarks>
    [UnityTest]
    public void Count() { }

    /// <remarks><a href="http://unitydoc.marrington.net/Able#foreach">ForEach</a></remarks>
    [UnityTest]
    public void ForEach() { }

    /// <remarks><a href="http://unitydoc.marrington.net/Able#build-the-selector">Build the Selector</a></remarks>
    [UnityTest]
    public void BuildSelector() { }

    /// <remarks><a href="http://unitydoc.marrington.net/Able#reset">Force a Selector Rebuild</a></remarks>
    [UnityTest]
    public void ResetExample() { }
  }
}