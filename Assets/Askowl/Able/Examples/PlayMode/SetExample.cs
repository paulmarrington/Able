﻿// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

#if AskowlAble
namespace Askowl.Examples {
  using System;
  using UnityEngine;

  /// Using <see cref="Set{T}" />
  /// <inheritdoc />
  [Serializable]
  public class SetInstance : Set<AudioClip> {
    public void Play() { AudioSource.PlayClipAtPoint(clip: Pick(), position: Vector3.zero); }
  }

  /// Using <see cref="Set{T}" />
  /// <inheritdoc />
  public class SetExample : MonoBehaviour {
    [SerializeField] private SetInstance setInstance;
  }
}
#endif