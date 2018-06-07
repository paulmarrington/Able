/*
 * With great thanks and full attribution to
 * https://forum.unity.com/members/thevastbernie.589052/
 * https://forum.unity.com/threads/editor-tool-better-scriptableobject-inspector-editing.484393/
 */

using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects,
 CustomEditor(inspectedType: typeof(MonoBehaviour), editorForChildClasses: true)]
public class MonoBehaviourEditor : Editor { }

[CustomPropertyDrawer(type: typeof(ScriptableObject), useForChildren: true)]
public class ScriptableObjectDrawer : PropertyDrawer {
  private static readonly Dictionary<string, bool>
    FoldoutByType = new Dictionary<string, bool>();

  private Editor editor = null;

  public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
    EditorGUI.PropertyField(position, property, label, includeChildren: true);

    bool foldout = false;

    if (property.objectReferenceValue != null) {
      string objectReferenceValueType = property.objectReferenceValue.name;

      bool foldoutExists =
        FoldoutByType.TryGetValue(key: objectReferenceValueType, value: out foldout);

      foldout = EditorGUI.Foldout(position: position, foldout: foldout, content: GUIContent.none);

      if (foldoutExists) {
        FoldoutByType[objectReferenceValueType] = foldout;
      } else {
        FoldoutByType.Add(objectReferenceValueType, foldout);
      }
    }

    if (!foldout) return;

    EditorGUI.indentLevel++;

    if (!editor) {
      Editor.CreateCachedEditor(
        targetObject: property.objectReferenceValue, editorType: null, previousEditor: ref editor);
    }

    editor.OnInspectorGUI();
    EditorGUI.indentLevel--;
  }
}