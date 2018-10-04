// Original version of the ConditionalHideAttribute created by Brecht Lecluyse (www.brechtos.com)
//
// Modified by: - Paul Marrington (askowl.net)

using System;
using UnityEditor;
using UnityEngine;

namespace Askowl {
  /// <a href="http://bit.ly/2NZbbe8">ConditionalHide Attribute</a> from
  /// <a href="http://www.brechtos.com/hiding-or-disabling-inspector-properties-using-propertydrawers-within-unity-5/">Brecht Lecluyse</a> <inheritdoc />
  [CustomPropertyDrawer(typeof(ConditionalHideAttribute))]
  public class ConditionalHidePropertyDrawer : PropertyDrawer {
    private Boolean displayProperty;

    /// <inheritdoc />
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
      string             sourceFieldName = ((ConditionalHideAttribute) attribute).SourceFieldName;
      string             conditionPath   = property.propertyPath.Replace(property.name, sourceFieldName);
      SerializedProperty sourceProperty  = property.serializedObject.FindProperty(conditionPath);
      displayProperty = true;

      if (sourceProperty != null) {
        switch (sourceProperty.propertyType) {
          case SerializedPropertyType.Boolean:
            displayProperty = sourceProperty.boolValue;
            break;
          case SerializedPropertyType.String:
            displayProperty = !string.IsNullOrWhiteSpace(sourceProperty.stringValue);
            break;
          case SerializedPropertyType.ObjectReference:
            displayProperty = sourceProperty.objectReferenceValue != null;
            break;
        }
      }

      if (displayProperty) {
        EditorGUI.PropertyField(position: position, property: property, label: label, includeChildren: true);
      }
    }

    /// <inheritdoc />
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
      if (displayProperty) { return EditorGUI.GetPropertyHeight(property, label); }
      else { return -EditorGUIUtility.standardVerticalSpacing; }
    }
  }
}