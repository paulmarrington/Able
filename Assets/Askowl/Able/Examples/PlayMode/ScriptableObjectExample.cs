using System.Collections;
using UnityEngine;

namespace Askowl.Examples {
  /// Using <see cref="ScriptableObjectDrawer" />
  /// <inheritdoc />
  [CreateAssetMenu(menuName = "Examples/BasicScriptableObject", fileName = "ScriptableObjectExample"),
   Labels("String One", "Replaced One", "String Two", "Replaced Two")]
  public class ScriptableObjectExample : ScriptableObject {
    public                          IEnumerator a() { yield return null; }
    [SerializeField, Label] private string      stringOne;
    [SerializeField, Label] private string      stringTwo;
  }
}