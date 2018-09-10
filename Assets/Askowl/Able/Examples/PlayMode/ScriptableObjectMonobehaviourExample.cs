using UnityEngine;

namespace Askowl.Examples {
  /// Using <see cref="ScriptableObjectExample" />
  /// <inheritdoc />
  public class ScriptableObjectMonobehaviourExample : MonoBehaviour {
    // ReSharper disable once NotAccessedField.Local
    [SerializeField] private ScriptableObjectExample scriptableObjectExample;

    /// Using <see cref="PlayModeController"/>
    public void ButtonOne() { Debug.Log("Button One Pressed"); }

    /// Using <see cref="PlayModeController"/>
    public void ButtonTwo() { Debug.Log("Button Two Pressed"); }
  }
}