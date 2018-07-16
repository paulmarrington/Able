using UnityEditor;
using UnityEngine;

namespace CustomAsset {
  /// <inheritdoc />
  /// <summary>
  /// A Unity Inspector Editor that has a *Preview* to preview the component without having to run the application
  /// </summary>
  /// <typeparam name="T">The type of component required for the preview - which may be different to the custom editor target</typeparam>
  /// <remarks><a href="http://customasset.marrington.net#previewcustomeditor">More...</a></remarks>
  public abstract class PreviewEditor<T> : Editor where T : Component {
    /// <summary>
    /// A component the target is relient on
    /// </summary>
    protected T Source;

    private void OnEnable() {
      GameObject gameObject = EditorUtility.CreateGameObjectWithHideFlags(
        "Audio Preview", HideFlags.HideAndDontSave, typeof(T));

      Source = gameObject.GetComponent<T>();
    }

    private void OnDisable() { DestroyImmediate(Source.gameObject); }

    /// <inheritdoc />
    public override void OnInspectorGUI() {
      DrawDefaultInspector();
      EditorGUI.BeginDisabledGroup(serializedObject.isEditingMultipleObjects);

      if (GUILayout.Button("Preview")) Preview();

      EditorGUI.EndDisabledGroup();
    }

    /// <summary>
    /// Implemented by concrete custom editors to act when the preview button is pressed
    /// </summary>
    protected abstract void Preview();
  }
}