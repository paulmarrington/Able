/*
 * With thanks to Ryan Hipple -- https://github.com/roboryantron/Unite2017
 */

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Askowl {
  /// <inheritdoc cref="Pick" />
  /// <summary>
  /// Set of any serialised type as a custom asset.
  /// </summary>
  /// <remarks><a href="http://customassets.marrington.net#custom-asset-sets">More...</a></remarks>
  [Serializable]
  public class Set<T> : Pick<T> {
    #region Configuration
    [SerializeField] private List<T> elements;

    [SerializeField, Tooltip("true for sequential, false for random")]
    internal bool Cycle;

    [SerializeField, Tooltip(
       "If the list is shorter then select items randomly, but never choose one a second time until all have been picked. This is useful for short lists to reduce unexpected repeats.")]
    internal int ExhaustiveBelow;
    #endregion

    #region Access
    /// <summary>
    /// Elements that make up the set
    /// </summary>
    public List<T> Elements { get { return elements; } set { elements = value; } }

    /// <summary>See if a set contains a specific element.</summary>
    /// <remarks><a href="http://customassets.marrington.net#containsentry">More...</a></remarks>
    /// <param name="entry">Element that may or may not be in the set</param>
    /// <returns>True if the element supplied is in this set</returns>
    public bool Contains(T entry) { return Elements.Contains(entry); }

    /// <summary>Return the number of entries in the Set</summary>
    /// <remarks><a href="http://customassets.marrington.net#count">More...</a></remarks>
    public int Count { get { return Elements.Count; } }

    /// <summary>Call an action on every entry in the set. Order is from last to first so that items can be removed safely.</summary>
    /// <param name="action">Action called with one entry from the set</param>
    /// <remarks><a href="http://customasset.marrington.net#forall">More...</a></remarks>
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
    protected virtual void BuildSelector() {
      selector = new Selector<T>(Elements.ToArray()) {IsRandom = !Cycle, ExhaustiveBelow = ExhaustiveBelow};
    }

    /// <inheritdoc />
    public T Pick(params T[] references) => Selector.Pick();

    /// <summary>
    /// If something has changed the underlying data we need to tell the Selector
    /// that it is now out of date.
    /// </summary>
    public void Reset() => selector = null;

    /// <summary>
    /// Detector used to pick an element from the set
    /// </summary>
    protected Selector<T> Selector {
      get {
        if (selector == null) BuildSelector();
        return selector;
      }
    }

    private Selector<T> selector;
    #endregion

    #region Mutable
    /// <summary>Add an entry if one does not exist already - and trigger a change event.</summary>
    /// <remarks><a href="http://customassets.marrington.net#addentry">More...</a></remarks>
    /// <param name="entry">Element to add if it isn't in the list</param>
// ReSharper disable once UnusedMember.Global
    protected void Add(T entry) {
      if (Elements.Contains(entry)) return;

      Elements.Add(entry);
      Reset();
    }

    /// <summary>Remove an entry if it exists - and trigger a change event.</summary>
    /// <remarks><a href="http://customassets.marrington.net#removeentry">More...</a></remarks>
    /// <param name="entry">Element to remove if it is in the list</param>
// ReSharper disable once UnusedMember.Global
    protected void Remove(T entry) {
      if (!Elements.Contains(entry)) return;

      Elements.Remove(entry);
      Reset();
    }
    #endregion
  }
}