// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace Askowl {
  /// <inheritdoc />
  /// <summary> Pick one item from a list. </summary>
  /// <typeparam name="T">Type of item. It can be a primative, object or even a Unity Asset</typeparam>
  /// <remarks><a href="http://unitydoc.marrington.net/Able#selectorcs-pick-form-a-list">Select from a list</a></remarks>
  public sealed class Selector<T> : Pick<T> {
    /// <summary>
    /// Defaults to random. Set false to cycle through entries sequentially
    /// </summary>
    public bool IsRandom = true;

    /// <summary>
    /// If the list is shorter then select items randomly, but never choose one a second time until
    /// all have been picked. This is useful for short lists to reduce repeats.
    /// </summary>
    public int ExhaustiveBelow = 1;

    private T[]     choices = { };
    private Func<T> picker;

    /// <inheritdoc />
    /// <remarks><a href="http://unitydoc.marrington.net/Able#selectorcs-pick-form-a-list">Select from a list</a></remarks>
    public T Pick() {
      if (choices.Length == 0) return default(T);
      if (picker         != null) return picker();

      if (choices.Length == 0) {
        picker = () => default(T);
      } else if (!IsRandom) { // cycle through list
        cycleIndex = 0;
        picker     = () => choices[cycleIndex++ % choices.Length];
      } else if (choices.Length >= ExhaustiveBelow) { // random selection
        picker = () => choices[Random.Range(0, choices.Length)];
      } else {
        remainingSelections = new List<T>(collection: choices);

        picker = () => { // different random choice until list exhausted, then repeat
          if (remainingSelections.Count == 0) {
            remainingSelections = new List<T>(collection: choices);
          }

          cycleIndex = Random.Range(0, remainingSelections.Count);
          T result = remainingSelections[index: cycleIndex];
          remainingSelections.RemoveAt(index: cycleIndex);
          return result;
        };
      }

      return picker();
    }

    /// <summary>
    /// Used to update the choices to a new set using the same picker.
    /// </summary>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#selector-choices">Selector List Update</a></remarks>
    public T[] Choices {
      get { return choices; }
      set {
        choices = value;
        picker  = null;
      }
    }

    private int cycleIndex;

    /// <summary>
    /// The location of the next choice in the sequence.
    /// </summary>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#selector-cycleindex">Array index in Choices of last pick</a></remarks>
    public int CycleIndex => cycleIndex % choices.Length;

    private List<T> remainingSelections;
  }
}