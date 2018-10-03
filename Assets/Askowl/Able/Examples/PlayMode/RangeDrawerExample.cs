// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

#if AskowlAble
using UnityEngine;

namespace Askowl.Examples {
  /// <inheritdoc />
  public class RangeDrawerExample : MonoBehaviour {
    [SerializeField, RangeBounds(min: 10, max: 20)]
    private Range range = new Range(min: 12, max: 18);

    internal void RangeExample() {
      for (var i = 0; i < 10; i++) {
        var value = (int) range.Pick();

        if ((value < 12) || (value > 18)) {
          Debug.LogError($"{value} is not in range");
          return;
        }
      }

      Debug.Log("RangeExample passed");
    }
  }
}
#endif