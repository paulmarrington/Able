// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

#if AskowlAble

namespace Askowl.Examples {
  using UnityEngine;

  /// Using <see cref="PreviewEditor{T}" /><inheritdoc />
  public class PreviewEditorExample : MonoBehaviour {
    // ReSharper disable once NotAccessedField.Local
    [SerializeField] private AudioClip audioClip;
  }

  /// place-holder for player - if it were created
  public class AudioClips { }
}
#endif