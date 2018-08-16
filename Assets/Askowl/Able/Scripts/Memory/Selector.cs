// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace Askowl {
//  using System;
//  using System.Collections.Generic;

  /// <inheritdoc />
  /// <summary> Pick one item from a list. </summary>
  /// <typeparam name="T">Type of item. It can be a primative, object or even a Unity Asset</typeparam>
  /// <remarks><a href="http://customassets.marrington.net#selector">More...</a></remarks>
  public sealed class Selector<T> : Pick<T> {
    /// <summary>
    /// Defaults to random. Set false to cycle through entries sequentially
    /// </summary>
    public bool IsRandom = true;

    /// <summary>
    /// If the list is shorter then select items randomly, but never choose one a second time until
    /// all have been picked. This is useful for short lists to reduce repeats.
    /// </summary>
    public int ExhaustiveBelow = 30;

    private T[]     choices = { };
    private Func<T> picker;

    /// <summary>Constructor to create selection list.</summary>
    /// <remarks><a href="http://customassets.marrington.net#selectorinitialiser">More...</a></remarks>
    /// <param name="choices">The list to choose an item from</param>
    /// <code>selector = new Selector{T}(Elements.ToArray()) {IsRandom = !Cycle, ExhaustiveBelow = ExhaustiveBelow};</code>
    public Selector(T[] choices = null) {
      if (choices != null) this.choices = choices;
      ChoosePicker();
      Init();
    }

    private void ChoosePicker() {
      if (choices.Length == 0) {
        picker = () => default(T);
      } else if (!IsRandom) { // cycle through list
        picker = () => choices[cycleIndex++ % choices.Length];
      } else if (choices.Length >= ExhaustiveBelow) { // randoms election
        picker = () => choices[Random.Range(0, choices.Length)];
      } else {
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
    }

    private void Init() {
      remainingSelections = new List<T>(collection: choices);
      cycleIndex          = 0;
    }

    /// <summary>
    /// Used to update the choices to a new set using the same picker.
    /// </summary>
    /// <remarks><a href="http://customassets.marrington.net#selectorchoices">More...</a></remarks>
    public T[] Choices {
      get { return choices; }
      set {
        choices = value;
        ChoosePicker();
        Init();
      }
    }

    private int cycleIndex;

    /// <summary>
    /// The location of the next choice in the sequence.
    /// <remarks><a href="http://customassets.marrington.net#selectorcycleindex">More...</a></remarks>
    /// </summary>

    public int CycleIndex => cycleIndex % choices.Length;

    private List<T> remainingSelections;

    /// <inheritdoc />
    /// <remarks><a href="http://customassets.marrington.net#selector">More...</a></remarks>
    public T Pick(params T[] _) => (choices.Length > 0) ? picker() : default(T);
  }
}