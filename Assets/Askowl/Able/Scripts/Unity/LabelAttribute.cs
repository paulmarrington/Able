namespace Askowl {
  using System;
  using UnityEngine;

  /// <a href="http://bit.ly/2Rk1o06">Change label for generic components</a><inheritdoc />
  public class LabelAttribute : PropertyAttribute { }

  /// <a href="http://bit.ly/2Rk1o06">Save label name to change attribute field descriptors</a>
  public class LabelsAttribute : Attribute {
    private readonly Map    labels = new Map();
    private readonly string label;

    /// <a href="http://bit.ly/2Rk1o06">[Labels("name")]</a>
    public LabelsAttribute(string label) => this.label = label;

    /// <a href="http://bit.ly/2Rk1o06">Many labels</a>
    public LabelsAttribute(params string[] labels) {
      for (var i = 0; i < labels.Length; i++) this.labels.Add(labels[i]);
    }

    /// <a href="http://bit.ly/2Rk1o06">Make the switch in the Inspector</a>
    public void Change(GUIContent guiLabel) =>
      guiLabel.text = labels[guiLabel.text].Found ? labels.Value.ToString() : label ?? guiLabel.text;
  }
}