/*
 * With thanks to Ryan Hipple -- https://github.com/roboryantron/Unite2017
 */

using System;
using UnityEngine;

namespace Askowl {
  /// <a href="http://bit.ly/2NXaCSb">Create a set in the inspector and provide an interface to pick one</a> <inheritdoc cref="Pick" />
  [Serializable] public class Set<T> : Pick<T> {
    #region Inspector Fields

    /// <a href="http://bit.ly/2NXaCSb">List of elements to pick from - set in the inspector</a>
    [SerializeField] private T[] elements = default;

    [SerializeField, Tooltip("true for sequential, false for random")]
    private bool cycle = false;

    [SerializeField
   , Tooltip(
       "If the list is shorter then select items randomly, but never choose one a second time until all have been picked. This is useful for short lists to reduce unexpected repeats.")]
    private int exhaustiveBelow = 10;

    #endregion

    #region Picker

    /// <a href=""></a> //#TBD#// <inheritdoc />
    public override string ToString() =>
      $"Set {GetType().Name}: Size = {elements.Length}, Cycle = {cycle}, Exhaustive Below = {exhaustiveBelow}";

    /// <a href=""></a> //#TBD#//
    public int InitialSize => elements.Length;

    /// <a href="http://bit.ly/2NWwH3e">Rebuild selections if we change contents</a>
    protected virtual void BuildSelector() {
      built            = true;
      selector.Choices = elements;
    }

    /// <a href="http://bit.ly/2NTAqP3">Pick one item from many</a> <inheritdoc />
    public T Pick() => Selector.Pick();

    /// <a href="http://bit.ly/2NWwH3e">Remove all set entries</a>
    public void Reset() {
      built = false;
      selector?.Reset();
    }

    /// <a href="http://bit.ly/2NTAqP3">Selector to call to get picked elements</a>
    protected Selector<T> Selector {
      get {
        if (selector == null) {
          selector = new Selector<T>() {ExhaustiveBelow = exhaustiveBelow, IsRandom = !cycle};
        }
        if (!built) BuildSelector();
        return selector;
      }
    }

    private Selector<T> selector;
    private bool        built;

    #endregion
  }
}