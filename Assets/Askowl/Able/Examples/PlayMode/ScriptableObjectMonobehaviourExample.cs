// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

#if !ExcludeAskowlTests
namespace Askowl.Able.Examples {
  using UnityEngine;

  /// Using <see cref="ScriptableObjectExample" /><inheritdoc />
  public class ScriptableObjectMonoBehaviourExample : MonoBehaviour {
    // ReSharper disable once NotAccessedField.Local
    [SerializeField] private ScriptableObjectExample scriptableObjectExample;

    /// Using <see cref="PlayModeController"/>
    public void ButtonOne() { Debug.Log("Button One Pressed"); }

    /// Using <see cref="PlayModeController"/>
    public void ButtonTwo() { Debug.Log("Button Two Pressed"); }
  }
}
#endif