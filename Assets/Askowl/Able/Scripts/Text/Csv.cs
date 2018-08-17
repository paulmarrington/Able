using System;

namespace Askowl {
  /// <summary>
  /// Data serialisation to CSV (comma separated variable).
  /// </summary>
  public static class Csv {
    /// <summary>
    /// Convert a list directly to a CsV line
    /// </summary>
    /// <param name="list">List to serialise</param>
    /// <returns>CSV representation</returns>
    public static string ToString<T>(params T[] list) =>
      string.Join(separator: ",", value: Array.ConvertAll(array: list, converter: x => x.ToString()));
  }
}