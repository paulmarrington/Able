// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

namespace Askowl {
  using System;
  using System.Collections.Generic;
  using JetBrains.Annotations;

  /// <inheritdoc />
  /// <summary> Pick one item from a list. </summary>
  /// <typeparam name="T">Type of item. It can be a primative, object or even a Unity Asset</typeparam>
  /// <remarks><a href="http://customassets.marrington.net#selector">More...</a></remarks>
  public sealed class Selector<T> : Pick<T> {
    private          T[]     choices = { };
    private readonly Func<T> picker;

    /// <summary>Constructor to create selection list.</summary>
    /// <remarks><a href="http://customassets.marrington.net#selectorinitialiser">More...</a></remarks>
    /// <param name="choices">The list to choose an item from</param>
    /// <param name="isRandom">Defaults to random. Set false to cycle through entries sequentially</param>
    /// <param name="exhaustiveBelow">If the list is shorter then select items randomly, but never choose one a second time until all have been picked. This is useful for short lists to reduce repeats.</param>
    public Selector(T[] choices = null, bool isRandom = true, int exhaustiveBelow = 0) {
      if (choices != null) this.choices = choices;
      choices = this.choices;

      if (!isRandom) { // cycle through list
        picker = () => choices[cycleIndex++ % choices.Length];
      } else if (choices.Length >= exhaustiveBelow) { // randoms election
        picker = () => choices[random.Next(minValue: 0, maxValue: choices.Length)];
      } else {
        picker = () => { // different random choice until list exhausted, then repeat
          if (remainingSelections.Count == 0) {
            remainingSelections = new List<T>(collection: choices);
          }

          cycleIndex = random.Next(minValue: 0, maxValue: remainingSelections.Count);
          T result = remainingSelections[index: cycleIndex];
          remainingSelections.RemoveAt(index: cycleIndex);
          return result;
        };
      }

      Init();
    }

    private readonly Random random = new Random();

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
        Init();
      }
    }

    private int cycleIndex;

    /// <summary>
    /// The location of the next choice in the sequence.
    /// <remarks><a href="http://customassets.marrington.net#selectorcycleindex">More...</a></remarks>
    /// </summary>

    public int CycleIndex { get { return cycleIndex % choices.Length; } }

    private List<T> remainingSelections;

    /// <inheritdoc />
    /// <remarks><a href="http://customassets.marrington.net#selector">More...</a></remarks>
    public T Pick() { return (choices.Length > 0) ? picker() : default(T); }
  }
}