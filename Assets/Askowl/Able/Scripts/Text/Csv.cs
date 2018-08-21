using System;

namespace Askowl {
  /// <summary>
  /// Data serialisation to CSV (comma separated variable).
  /// </summary>
  /// <remarks><a href="http://unitydoc.marrington.net/Able#Csvcs-serialization-of-commaseparated-lists">Serialise List to CSV</a></remarks>
  public static class Csv {
    /// <summary>
    /// Convert a list directly to a CsV line
    /// </summary>
    /// <param name="list">List to serialise</param>
    /// <returns>CSV representation</returns>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#tocsv">Convert an array to a CSV string</a></remarks>
    public static string ToString<T>(params T[] list) =>
      string.Join(separator: ",", value: Array.ConvertAll(array: list, converter: x => x.ToString()));
  }
}