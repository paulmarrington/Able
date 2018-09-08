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

  /// <a href=""></a>
  /// <inheritdoc />
  [CustomPropertyDrawer(type: typeof(ScriptableObject), useForChildren: true)]
  public class ScriptableObjectDrawer : PropertyDrawer {
    private static readonly Dictionary<string, bool> FoldoutByType = new Dictionary<string, bool>();

    private Editor editor;

    /// <inheritdoc />
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
      EditorGUI.PropertyField(position, property, label, includeChildren: true);

      if (property.objectReferenceValue == null) return;

      if (label.text.StartsWith("Element ")) return;

      string typeKey = property.objectReferenceValue.name;
      bool   foldout = FoldoutByType.ContainsKey(typeKey) && FoldoutByType[typeKey];

      FoldoutByType[typeKey] = EditorGUI.Foldout(position, foldout, GUIContent.none);

      if (!FoldoutByType[typeKey]) return;

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
   CustomEditor(inspectedType: typeof(ValueNameAttribute), editorForChildClasses: true)]
  public class CustomAssetEditor : Editor {
//    public override void OnInspectorGUI() {
//      var serializedProperties = serializedObject.GetIterator();
//
//      if (serializedProperties.NextVisible(true)) {
//        do {
//        } while (serializedProperties.NextVisible(false));
//      }
//
//      base.OnInspectorGUI();
//    }
  }

  /// <inheritdoc />
  [CanEditMultipleObjects,
   CustomEditor(inspectedType: typeof(SerializedObject), editorForChildClasses: true)]
  public class SerializedObjectEditor : Editor { }

  /// <a href=""></a>
  /// <inheritdoc />
  [CustomPropertyDrawer(type: typeof(ValueAttribute), useForChildren: true)]
  public class CustomAssetLabelDrawer : PropertyDrawer {
    /// <inheritdoc />
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
      var attr = property.serializedObject.targetObject.GetType()
                         .GetCustomAttributes(typeof(ValueNameAttribute), false);

      if ((attr.Length > 0) && attr[0] is ValueNameAttribute) {
        var valueName = (ValueNameAttribute) attr[0];
        label.text = valueName.Label;
      }

      EditorGUI.PropertyField(position, property, label, includeChildren: true);
    }

    /// <inheritdoc />
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) =>
      EditorGUI.GetPropertyHeight(property);
  }
  #endregion
}