using UnityEngine;
using System;

// Original version of the ConditionalHideAttribute created by Brecht Lecluyse (www.brechtos.com)
// Modified by: - Paul Marrington (askowl.net)
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property |
                AttributeTargets.Class | AttributeTargets.Struct)]
public class ConditionalHideAttribute : PropertyAttribute {
  public string sourceFieldName;

  // Use this for initialization
  public ConditionalHideAttribute(string sourceFieldName) {
    this.sourceFieldName = sourceFieldName;
  }
}