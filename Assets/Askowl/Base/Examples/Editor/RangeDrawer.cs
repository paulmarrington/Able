// With thanks to Jason Weimann  -- jason@unity3d.college

using UnityEditor;
using UnityEngine;

namespace Askowl {
  /// <inheritdoc />
  /// <summary>
  /// Unity Editor drawer used for <see cref="T:Askowl.Range" /> fields. Displays minimum and maximum values along with a dual-slider.
  /// </summary>
  [CustomPropertyDrawer(type: typeof(Range), useForChildren: true)]
  public class RangeDrawer : PropertyDrawer {
    private          SerializedProperty minProp,  maxProp;
    private          float              minValue, maxValue;
    private          float              rangeMin, rangeMax;
    private          string             boundsFormat;
    private readonly GUIStyle           style = new GUIStyle(GUI.skin.textField);

    /// <inheritdoc />
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
      LoadCurrentMinMax(property);
      LoadRangeBounds();

      label    = EditorGUI.BeginProperty(position, label, property);
      position = EditorGUI.PrefixLabel(position, label);
      EditorGUI.BeginChangeCheck();

      var rect = new Rect(position) {width = 40};
      style.alignment = TextAnchor.MiddleRight;
      float.TryParse(GUI.TextField(rect, minValue.ToString(boundsFormat), style), out minValue);

      rect            = new Rect(position);
      rect.xMin       = rect.xMax - 40;
      style.alignment = TextAnchor.MiddleLeft;
      float.TryParse(GUI.TextField(rect, maxValue.ToString(boundsFormat), style), out maxValue);

      position.xMin += 45;
      position.xMax -= 45;
      EditorGUI.MinMaxSlider(position, ref minValue, ref maxValue, rangeMin, rangeMax);

      if (EditorGUI.EndChangeCheck()) {
        minProp.floatValue = minValue;
        maxProp.floatValue = maxValue;
      }

      EditorGUI.EndProperty();
    }

    private void LoadCurrentMinMax(SerializedProperty property) {
      minProp  = property.FindPropertyRelative(("min"));
      maxProp  = property.FindPropertyRelative(("max"));
      minValue = minProp.floatValue;
      maxValue = maxProp.floatValue;
    }

    private void LoadRangeBounds() {
      var  ranges = fieldInfo.GetCustomAttributes(typeof(RangeBoundsAttribute), inherit: true);
      bool set    = (ranges.Length > 0);

      rangeMin = set ? ((RangeBoundsAttribute) ranges[0]).Min : 0;
      rangeMax = set ? ((RangeBoundsAttribute) ranges[0]).Max : 1;
      int places = (rangeMax < 10) ? 2 : (rangeMax < 100) ? 1 : 0;

      boundsFormat = string.Format("F{0}", places);
    }
  }
}