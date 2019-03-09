// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

using UnityEditor;
using UnityEngine;

namespace Askowl {
  /// <a href="http://bit.ly/2NZZmEv">Add a preview button for a component</a> <inheritdoc />
  public abstract class PreviewEditor<T> : Editor where T : Component {
    /// <a href="http://bit.ly/2NZZmEv">Source component reference</a>
    protected T Source;

    /// <a href="">Name printed on button - defaulting to "Preview"</a> //#TBD#//
    protected string buttonName = "Preview";

    private void OnEnable() {
      GameObject gameObject = EditorUtility.CreateGameObjectWithHideFlags(
        "Preview", HideFlags.HideAndDontSave, typeof(T));
      Source = gameObject.GetComponent<T>();
    }

    private void OnDisable() => DestroyImmediate(Source.gameObject);

    /// <inheritdoc />
    public override void OnInspectorGUI() {
      DrawDefaultInspector();
      EditorGUI.BeginDisabledGroup(serializedObject.isEditingMultipleObjects);

      if (GUILayout.Button(buttonName)) Preview();

      EditorGUI.EndDisabledGroup();
    }

    /// <a href="http://bit.ly/2NZZmEv">Override this to play the preview</a>
    protected abstract void Preview();
  }
}