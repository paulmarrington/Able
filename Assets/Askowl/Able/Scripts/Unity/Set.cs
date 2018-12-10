/*
 * With thanks to Ryan Hipple -- https://github.com/roboryantron/Unite2017
 */

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Askowl {
  /// <a href="http://bit.ly/2NXaCSb">Create a set in the inspector and provide an interface to pick one</a> <inheritdoc cref="Pick" />
  [Serializable]
  // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
  public class Set<T> : Pick<T> {
    #region Inspector Fields

    /// <a href="http://bit.ly/2NXaCSb">List of elements to pick from - set in the inspector</a>
    [SerializeField] public List<T> Elements;

    [SerializeField, Tooltip("true for sequential, false for random")]
    internal bool Cycle;

    [SerializeField, Tooltip(
       "If the list is shorter then select items randomly, but never choose one a second time until all have been picked. This is useful for short lists to reduce unexpected repeats.")]
    internal int ExhaustiveBelow;

    #endregion

    #region Picker

    /// <a href="http://bit.ly/2NTAqP3">Selector created to manage picking from a set</a>
    public Set() => selector = new Selector<T>() {ExhaustiveBelow = ExhaustiveBelow, IsRandom = !Cycle};

    /// <a href="http://bit.ly/2NWwH3e">Rebuild selections if we change contents</a>
    // ReSharper disable once VirtualMemberNeverOverridden.Global
    protected virtual Selector<T> BuildSelector() {
      selector.Choices = Elements.ToArray();
      return selector;
    }

    /// <a href="http://bit.ly/2NTAqP3">Pick one item from many</a> <inheritdoc />
    public T Pick() => Selector.Pick();

    /// <a href="http://bit.ly/2NWwH3e">Remove all set entries</a>
    public void Reset() => selector.Reset();

    /// <a href="http://bit.ly/2NTAqP3">Selector to call to get picked elements</a>
    protected Selector<T> Selector => selector.Choices.Length == 0 ? BuildSelector() : selector;

    private readonly Selector<T> selector;

    #endregion

    #region Mutable

    /// <a href="http://bit.ly/2NWwH3e">Add one entry to those open for selection</a>
    protected void Add(T entry) {
      Elements.Add(entry);
      Reset();
    }

    /// <a href="http://bit.ly/2NWwH3e">Remove one entry to those open for selection</a>
    protected void Remove(T entry) {
      if (!Elements.Contains(entry)) return;

      Elements.Remove(entry);
      Reset();
    }

    #endregion
  }
}