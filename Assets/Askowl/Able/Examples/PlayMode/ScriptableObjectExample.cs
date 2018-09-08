using System.Collections;
using UnityEngine;

namespace Askowl.Examples {
  /// Using <see cref="ScriptableObjectDrawer" />
  /// <inheritdoc />
  [CreateAssetMenu(menuName = "Examples/BasicScriptableObject", fileName = "ScriptableObjectExample")]
  public class ScriptableObjectExample : ScriptableObject {
    public                   IEnumerator a() { yield return null; }
    [SerializeField] private string      stringOne;
    [SerializeField] private string      stringTwo;
  }
}