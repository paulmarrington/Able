// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

#if AskowlAble
using NUnit.Framework;

namespace Askowl.Examples {
  /// <remarks><a href="http://unitydoc.marrington.net/Able#Csvcs-serialization-of-commaseparated-lists">Serialise List to CSV</a></remarks>
  public class CsvExamples {
    /// <remarks><a href="http://unitydoc.marrington.net/Able#tocsv">Convert an array to a CSV string</a></remarks>
    [Test]
    public void ToCsv() {
      var expected = "1,3,5,7,9";
      var actual = Csv.ToString(1, 3, 5, 7, 9);
      Assert.AreEqual(expected, actual);

      expected = "1,3,5,7,9";
      actual = Csv.ToString(new[] {1, 3, 5, 7, 9});
      Assert.AreEqual(expected, actual);

      expected = "One,Two,Three,Four,Five";
      actual = Csv.ToString("One", "Two", "Three", "Four", "Five");
      Assert.AreEqual(expected, actual);

      expected = "One,Two,Three,Four,Five";
      actual = Csv.ToString(new[] {"One", "Two", "Three", "Four", "Five"});
      Assert.AreEqual(expected, actual);
    }
  }
}
#endif