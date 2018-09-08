// Original version of the ConditionalHideAttribute created by Brecht Lecluyse (www.brechtos.com)
// Modified by: - Paul Marrington (askowl.net)

using UnityEngine;
using System;

namespace Askowl {
  /// <a href="http://www.brechtos.com/hiding-or-disabling-inspector-properties-using-propertydrawers-within-unity-5/"></a>
  /// <inheritdoc />
  [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property |
                  AttributeTargets.Class | AttributeTargets.Struct)]
  public class ConditionalHideAttribute : PropertyAttribute {
    /// <a href=""></a>
    public readonly string sourceFieldName;

    /// <a href=""></a>
    public ConditionalHideAttribute(string sourceFieldName) { this.sourceFieldName = sourceFieldName; }
  }
}