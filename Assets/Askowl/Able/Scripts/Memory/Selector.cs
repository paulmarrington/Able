// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace Askowl {
  /// <a href="http://bit.ly/2OrRfQp">Pick one item from a list.</a> <inheritdoc />
  public sealed class Selector<T> : Pick<T> {
    /// <a href="http://bit.ly/2OrRfQp">Defaults to random. Set false to cycle through entries sequentially</a>
    public bool IsRandom = true;

    /// <a href="http://bit.ly/2OvCQ5Q">If the list is shorter then select items randomly, but never choose one a second time until all have been picked. This is useful for short lists to reduce repeats.</a>
    public int ExhaustiveBelow = 1;

    private T[]     choices = { };
    private Func<T> picker;

    /// <a href="http://bit.ly/2OrRfQp">Method called to pick an item</a> <inheritdoc />
    public T Pick() {
      if (choices.Length == 0) return default;
      if (picker         != null) return picker();

      if (choices.Length == 0) {
        picker = () => default;
      } else if (!IsRandom) { // cycle through list
        cycleIndex = 0;
        picker     = () => choices[cycleIndex++ % choices.Length];
      } else if (choices.Length >= ExhaustiveBelow) { // random selection
        picker = () => choices[Random.Range(0, choices.Length)];
      } else {
        remainingSelections = new List<T>(collection: choices);

        picker = () => { // different random choice until list exhausted, then repeat
          if (remainingSelections.Count == 0) remainingSelections = new List<T>(collection: choices);

          cycleIndex = Random.Range(0, remainingSelections.Count);
          T result = remainingSelections[index: cycleIndex];
          remainingSelections.RemoveAt(index: cycleIndex);
          return result;
        };
      }

      return picker();
    }

    /// <a href="http://bit.ly/2OvDtMK">Used to update the choices to a new set using the same picker.</a>
    public T[] Choices {
      get => choices;
      set {
        choices = value;
        picker  = null;
      }
    }

    private int cycleIndex;

    /// <a href="http://bit.ly/2NU0GsC">The location of the next choice in the sequence.</a>
    public int CycleIndex => cycleIndex % choices.Length;

    private List<T> remainingSelections;

    /// <a href="http://bit.ly/2OrRfQp">Remove all choices for an empty list</a>
    public void Reset() => choices = emptyChoices;

    private readonly T[] emptyChoices = { };
  }
}