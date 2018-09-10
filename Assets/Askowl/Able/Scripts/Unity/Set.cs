/*
 * With thanks to Ryan Hipple -- https://github.com/roboryantron/Unite2017
 */

using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Askowl {
  /// <a href=""></a>
  /// <inheritdoc cref="Pick" />
  [Serializable]
  public class Set<T> : Pick<T> {
    #region Inspector Fields
    /// <a href=""></a>
    [SerializeField] public List<T> Elements;

    [SerializeField, Tooltip("true for sequential, false for random")]
    internal bool Cycle;

    [SerializeField, Tooltip(
       "If the list is shorter then select items randomly, but never choose one a second time until all have been picked. This is useful for short lists to reduce unexpected repeats.")]
    internal int ExhaustiveBelow;
    #endregion

    #region Picker
    /// <a href=""></a>
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
    protected Selector<T> Selector =>
      (selector.Choices.Length == 0) ? BuildSelector() : selector;

    private Selector<T> selector = new Selector<T>();
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