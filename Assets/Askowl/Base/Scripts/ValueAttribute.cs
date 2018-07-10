using System;
using UnityEngine;

namespace CustomAsset {
  public class ValueAttribute : PropertyAttribute {
    public string Label;
  }

  public class ValueNameAttribute : Attribute {
    public readonly string Label;
    public ValueNameAttribute(string label) { Label = label; }
  }
}