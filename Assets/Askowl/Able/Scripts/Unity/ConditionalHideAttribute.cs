// Original version of the ConditionalHideAttribute created by Brecht Lecluyse (www.brechtos.com)
// Modified by: - Paul Marrington (askowl.net)

namespace Askowl {
  using System;
  using UnityEngine;

  /// <a href="http://bit.ly/2OqbrlU">Hide on field in the inspector based on another</a> <inheritdoc />
  [AttributeUsage(
    AttributeTargets.Field | AttributeTargets.Property |
    AttributeTargets.Class | AttributeTargets.Struct)]
  public class ConditionalHideAttribute : PropertyAttribute {
    /// <a href="http://bit.ly/2OqbrlU">The name of the field to inspect for hiding another</a>
    public readonly string SourceFieldName;

    /// <a href="http://bit.ly/2OqbrlU">The attribute trigger to save the source field name</a>
    public ConditionalHideAttribute(string sourceFieldName) => SourceFieldName = sourceFieldName;
  }
}