using System;
using UnityEngine;

namespace Askowl {
  public class LabelAttribute : PropertyAttribute { }

  public class LabelsAttribute : Attribute {
    private readonly Map    labels = new Map();
    private readonly string label;

    public LabelsAttribute(string label) { this.label = label; }

    public LabelsAttribute(params object[] labels) { this.labels.Add(labels); }

    public void Change(GUIContent guiLabel) =>
      guiLabel.text = (labels[guiLabel.text].Found) ? labels.Value.ToString() : label ?? guiLabel.text;
  }
}