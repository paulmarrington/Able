/*
 * With thanks to Ryan Hipple -- https://github.com/roboryantron/Unite2017
 */

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Askowl {
  /// <inheritdoc cref="Pick" />
  /// <summary>
  /// Set of any serialised type to use in a component or custom asset
  /// </summary>
  /// <remarks><a href="http://unitydoc.marrington.net/Able#setcs-unity-component-implementing-a-selector">Sets</a></remarks>
  [Serializable]
  public class Set<T> : Pick<T> {
    #region Inspector Fields
    [SerializeField] public List<T> Elements;

    [SerializeField, Tooltip("true for sequential, false for random")]
    internal bool Cycle;

    [SerializeField, Tooltip(
       "If the list is shorter then select items randomly, but never choose one a second time until all have been picked. This is useful for short lists to reduce unexpected repeats.")]
    internal int ExhaustiveBelow;
    #endregion

    #region Access
    /// <summary>See if a set contains a specific element.</summary>
    /// <param name="entry">Element that may or may not be in the set</param>
    /// <returns>True if the element supplied is in this set</returns>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#containsentry">Contains(entry)</a></remarks>
    public bool Contains(T entry) => Elements.Contains(entry);

    /// <summary>Return the number of entries in the Set</summary>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#count">Count</a></remarks>
    public int Count => Elements.Count;

    /// <summary>Call an action on every entry in the set. Order is from last to first so that items can be removed safely.</summary>
    /// <param name="action">Action called with one entry from the set</param>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#foreach">ForEach</a></remarks>
    public void ForEach(Func<T, bool> action) {
      // Loop backwards since the list may change when disabling
      for (int i = Elements.Count - 1; i >= 0; i--) {
        if (!action(Elements[i])) break;
      }
    }
    #endregion

    #region Picker
    /// <summary>
    /// If the contents have changed we will need to rebuild the selector. This is normally
    /// at the next Pick() call. It can be overridden by more complex Set types.
    /// </summary>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#build-the-selector">Build the Selector</a></remarks>
    protected virtual Selector<T> BuildSelector() =>
      selector = new Selector<T> {Choices = Elements.ToArray(), IsRandom = !Cycle, ExhaustiveBelow = ExhaustiveBelow};

    /// <inheritdoc />
    /// <remarks><a href="http://unitydoc.marrington.net/Able#pick-from-selector">Pick an Item</a></remarks>
    public T Pick() => Selector.Pick();

    /// <summary>
    /// If something has changed the underlying data we need to tell the Selector
    /// that it is now out of date.
    /// </summary>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#reset">Force a Selector Rebuild</a></remarks>
    public void Reset() => selector = null;

    /// <summary>
    /// Detector used to pick an element from the set
    /// </summary>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#set-selector">Selector in Use</a></remarks>
    protected Selector<T> Selector => selector ?? BuildSelector();

    private Selector<T> selector;
    #endregion

    #region Mutable
    /// <summary>Add an entry if one does not exist already - and trigger a change event.</summary>
    /// <param name="entry">Element to add if it isn't in the list</param>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#saddentry">Add an Entry</a></remarks>
    protected void Add(T entry) {
      if (Elements.Contains(entry)) return;

      Elements.Add(entry);
      Reset();
    }

    /// <summary>Remove an entry if it exists - and trigger a change event.</summary>
    /// <param name="entry">Element to remove if it is in the list</param>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#removeentry">Remove an Entry</a></remarks>
    protected void Remove(T entry) {
      if (!Elements.Contains(entry)) return;

      Elements.Remove(entry);
      Reset();
    }
    #endregion
  }
}