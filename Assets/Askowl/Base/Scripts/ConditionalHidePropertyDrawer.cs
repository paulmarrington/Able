using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ConditionalHideAttribute))]
public class ConditionalHidePropertyDrawer : PropertyDrawer {
  public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
    string sourceFieldName = ((ConditionalHideAttribute) attribute).sourceFieldName;

    string conditionPath = property.propertyPath.Replace(property.name, sourceFieldName);
    Debug.Log("ConditionalHidePropertyDrawer " + sourceFieldName + " " + conditionPath);

    SerializedProperty sourceProperty = property.serializedObject.FindProperty(conditionPath);

    bool display = true;

    if (sourceProperty != null) {
      switch (sourceProperty.propertyType) {
        case SerializedPropertyType.Boolean:
          display = sourceProperty.boolValue;
          break;
        case SerializedPropertyType.ObjectReference:
          display = (sourceProperty.objectReferenceValue != null);
          break;
      }
    }

    if (display) EditorGUI.PropertyField(position, property, label, true);
  }
}