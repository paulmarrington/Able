/*
 * With great thanks and full attribution to
 * https://forum.unity.com/members/thevastbernie.589052/
 * https://forum.unity.com/threads/editor-tool-better-scriptableobject-inspector-editing.484393/
 */

using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Askowl {
  #region CustomAssetsInMonoBehaviour

  /// <inheritdoc />
  [CanEditMultipleObjects,
   CustomEditor(inspectedType: typeof(MonoBehaviour), editorForChildClasses: true)]
  public class MonoBehaviourEditor : Editor { }

  /// <inheritdoc />
  [CanEditMultipleObjects,
   CustomEditor(inspectedType: typeof(ScriptableObject), editorForChildClasses: true)]
  public class ScriptableObjectEditor : Editor { }

  /// <a href="http://bit.ly/2NTAoXr">Edit ScriptableObject data directly in component</a> <inheritdoc />
  [CustomPropertyDrawer(type: typeof(ScriptableObject), useForChildren: true)]
  public class ScriptableObjectDrawer : PropertyDrawer {
    private static readonly Dictionary<string, bool> foldoutByType = new Dictionary<string, bool>();

    private Editor editor;

    /// <inheritdoc />
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
      EditorGUI.PropertyField(position, property, label, includeChildren: true);

      if (property.objectReferenceValue == null) return;

      if (label.text.StartsWith("Element ")) return;

      string typeKey = property.objectReferenceValue.name;
      bool   foldout = foldoutByType.ContainsKey(typeKey) && foldoutByType[typeKey];

      foldoutByType[typeKey] = EditorGUI.Foldout(position, foldout, GUIContent.none);

      if (!foldoutByType[typeKey]) return;

      EditorGUI.indentLevel++;

      if (!editor) {
        Editor.CreateCachedEditor(
          targetObject: property.objectReferenceValue, editorType: null,
          previousEditor: ref editor);
      }

      editor.OnInspectorGUI();
      EditorGUI.indentLevel--;
    }
  }

  #endregion

  #region ValueLabelChange

  /// <inheritdoc />
  [CanEditMultipleObjects,
   CustomEditor(inspectedType: typeof(LabelsAttribute), editorForChildClasses: true)]
  public class CustomAssetEditor : Editor { }

  /// <inheritdoc />
  [CanEditMultipleObjects,
   CustomEditor(inspectedType: typeof(SerializedObject), editorForChildClasses: true)]
  public class SerializedObjectEditor : Editor { }

  /// <a href="http://bit.ly/2NTezHo">Label Generic Custom Assets</a> <inheritdoc />
  [CustomPropertyDrawer(type: typeof(LabelAttribute), useForChildren: true)]
  public class CustomAssetLabelDrawer : PropertyDrawer {
    /// <inheritdoc />
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
      var attributeType = property.serializedObject.targetObject.GetType();
      var attributes    = attributeType.GetCustomAttributes(typeof(LabelsAttribute), false);
      if (attributes.Length > 0) (attributes[0] as LabelsAttribute)?.Change(label);
      EditorGUI.PropertyField(position, property, label, includeChildren: true);
    }

    /// <inheritdoc />
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) =>
      EditorGUI.GetPropertyHeight(property);
  }

  #endregion
}