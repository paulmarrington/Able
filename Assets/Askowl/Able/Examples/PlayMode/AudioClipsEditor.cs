using UnityEditor;
using UnityEngine;

namespace Askowl.Examples {
  /// Using <see cref="PreviewEditor{T}" />
  /// <inheritdoc />
  [CustomEditor(typeof(AudioClip))]
  public class AudioClipsEditor : PreviewEditor<AudioSource> {
    /// <inheritdoc />
    protected override void Preview() =>
      AudioSource.PlayClipAtPoint(clip: (AudioClip) target, position: Vector3.zero);
  }
}