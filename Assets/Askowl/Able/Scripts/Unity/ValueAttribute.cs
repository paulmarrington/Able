using System;
using UnityEngine;

namespace Askowl {
  public class ValueAttribute : PropertyAttribute {
    public string Label;
  }

  public class ValueNameAttribute : Attribute {
    public readonly string Label;
    public ValueNameAttribute(string label) { Label = label; }
  }
}