namespace Askowl {
  using System;
  using System.Text.RegularExpressions;

  /// <a href="http://bit.ly/2NU54b0">Serialisation to CSV format</a>
  public static class Csv {
    /// <a href="http://bit.ly/2RezHpE">Static CSV serialisation method</a>
    public static string ToString<T>(T[] list) => string.Join(
      separator: ",", value: Array.ConvertAll(array: list, converter: x => x.ToString()));

    /// <a href=""></a> //#TBD#//
    public static Map ToMap(string csv) {
      Map             map     = Map.Instance;
      MatchCollection matches = kvpCsvRegex.Matches(csv);
      int             entries = matches.Count;

      for (var i = 0; i < entries; i++) { map.Add(matches[i].Groups["key"].Value, matches[i].Groups["value"].Value); }
      return map;
    }

    private static Regex kvpCsvRegex = new Regex(
      @"\s*""(?<key>[^""]*)""\s*=\s*""(?<value>[^""]*)""\s*,?|
        \s*""(?<key>[^""]*)""\s*,?|
        \s*'(?<key>[^']*)'\s*=\s*'(?<value>[^']*)'\s*,?|
        \s*'(?<key>[^']*)'\s*,?|
        \s*(?<key>[^=]+)\s*=\s*""(?<value>[^""]*)""\s*,?|
        \s*(?<key>[^=]+)\s*=\s*'(?<value>[^']*)'\s*,?|
        \s*(?<key>[^=]+)\s*=\s*(?<value>[^,]*),?|
        \s*(?<key>[^,]+)"
    , RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled | RegexOptions.ExplicitCapture);
  }
}