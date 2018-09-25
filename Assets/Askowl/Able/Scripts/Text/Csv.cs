namespace Askowl {
  using System;

  /// <a href=""></a>
  public static class Csv {
    /// <a href=""></a>
    public static string ToString<T>(T[] list) =>
      string.Join(separator: ",", value: Array.ConvertAll(array: list, converter: x => x.ToString()));
  }
}