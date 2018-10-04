namespace Askowl {
  using System;

  /// <a href="http://bit.ly/2NU54b0">Serialisation to CSV format</a>
  public static class Csv {
    /// <a href="http://bit.ly/2RezHpE">Static CSV serialisation method</a>
    public static string ToString<T>(T[] list) => string.Join(
      separator: ",", value: Array.ConvertAll(array: list, converter: x => x.ToString()));
  }
}