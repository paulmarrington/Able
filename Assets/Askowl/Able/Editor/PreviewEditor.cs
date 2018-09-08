using UnityEditor;
using UnityEngine;

namespace Askowl {
  /// <a href=""></a>
  /// <inheritdoc />
  public abstract class PreviewEditor<T> : Editor where T : Component {
    /// <a href=""></a>
    protected T Source;

    private void OnEnable() {
      GameObject gameObject = EditorUtility.CreateGameObjectWithHideFlags(
        "Preview", HideFlags.HideAndDontSave, typeof(T));

      Source = gameObject.GetComponent<T>();
    }

    private void OnDisable() { DestroyImmediate(Source.gameObject); }

    /// <a href=""></a>
    /// <inheritdoc />
    public override void OnInspectorGUI() {
      DrawDefaultInspector();
      EditorGUI.BeginDisabledGroup(serializedObject.isEditingMultipleObjects);

      if (GUILayout.Button("Preview")) Preview();

      EditorGUI.EndDisabledGroup();
    }

    /// <a href=""></a>
    protected abstract void Preview();
  }
}