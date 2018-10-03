// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

#if AskowlAble
namespace Askowl.Examples {
  using NUnit.Framework;

  public class CsvExamples {
    [Test] public void ToCsv() {
      var    expected = "1,3,5,7,9";
      string actual   = Csv.ToString(new[] { 1, 3, 5, 7, 9 });
      Assert.AreEqual(expected, actual);

      expected = "1,3,5,7,9";
      actual   = Csv.ToString(new[] { 1, 3, 5, 7, 9 });
      Assert.AreEqual(expected, actual);

      expected = "One,Two,Three,Four,Five";
      actual   = Csv.ToString(new[] { "One", "Two", "Three", "Four", "Five" });
      Assert.AreEqual(expected, actual);
    }
  }
}
#endif