/*
 * With thanks to Ryan Hipple -- https://github.com/roboryantron/Unite2017
 */

namespace Askowl {
  using System;
  using System.Collections.Generic;
  using UnityEngine;

  /// <a href=""></a>
  /// <inheritdoc cref="Pick" />
  [Serializable]
  // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
  public class Set<T> : Pick<T> {
    #region Inspector Fields
    /// <a href=""></a>
    [SerializeField] public List<T> Elements;

    [SerializeField, Tooltip("true for sequential, false for random")]
    internal bool Cycle = false;

    [SerializeField, Tooltip(
       "If the list is shorter then select items randomly, but never choose one a second time until all have been picked. This is useful for short lists to reduce unexpected repeats.")]
    internal int ExhaustiveBelow = 0;
    #endregion

    #region Picker
    /// <a href=""></a>
    public Set() => selector = new Selector<T>() { ExhaustiveBelow = ExhaustiveBelow, IsRandom = !Cycle };

    /// <a href=""></a>
    // ReSharper disable once VirtualMemberNeverOverridden.Global
    protected virtual Selector<T> BuildSelector() {
      selector.Choices = Elements.ToArray();
      return selector;
    }

    /// <a href=""></a>
    /// <inheritdoc />
    public T Pick() => Selector.Pick();

    /// <a href=""></a>
    public void Reset() => selector.Reset();

    /// <a href=""></a>
    protected Selector<T> Selector => selector.Choices.Length == 0 ? BuildSelector() : selector;

    private Selector<T> selector;
    #endregion

    #region Mutable
    /// <a href=""></a>
    protected void Add(T entry) {
      Elements.Add(entry);
      Reset();
    }

    /// <a href=""></a>
    protected void Remove(T entry) {
      if (!Elements.Contains(entry)) return;

      Elements.Remove(entry);
      Reset();
    }
    #endregion
  }
}