namespace Askowl {
  using System;
  using UnityEngine;

  /// <a href=""></a><inheritdoc />
  public class LabelAttribute : PropertyAttribute { }

  /// <a href=""></a>
  public class LabelsAttribute : Attribute {
    private readonly Map    labels = new Map();
    private readonly string label;

    /// <a href=""></a>
    public LabelsAttribute(string label) => this.label = label;

    /// <a href=""></a>
    public LabelsAttribute(params string[] labels) {
      for (var i = 0; i < labels.Length; i++) this.labels.Add(labels[i]);
    }

    /// <a href=""></a>
    public void Change(GUIContent guiLabel) =>
      guiLabel.text = labels[guiLabel.text].Found ? labels.Value.ToString() : label ?? guiLabel.text;
  }
}