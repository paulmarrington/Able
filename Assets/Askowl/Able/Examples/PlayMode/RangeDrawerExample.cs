#if UNITY_EDITOR && Able
using UnityEngine;

namespace Askowl.Examples {
  /// <inheritdoc />
  public class RangeDrawerExample : MonoBehaviour {
    [SerializeField, RangeBounds(min: 10, max: 20)]
    private Range range = new Range(min: 12, max: 18);

    internal void RangeExample() {
      for (int i = 0; i < 10; i++) {
        int value = (int) range.Pick();

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