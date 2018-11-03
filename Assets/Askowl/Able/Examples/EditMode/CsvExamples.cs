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

    [Test] public void ToMap() {
      var csv = @"""a"",'b','c'='23',d='com,ma',e=""eee"",count=1,hello=world,one,two";
      Map map = Csv.ToMap(csv);
      Assert.IsTrue(map["a"].Found);
      Assert.IsTrue(map["b"].Found);
      Assert.AreEqual("23",     map["c"].Value);
      Assert.AreEqual("com,ma", map["d"].Value);
      Assert.AreEqual("eee",    map["e"].Value);
      Assert.AreEqual("1",      map["count"].Value);
      Assert.AreEqual("world",  map["hello"].Value);
      Assert.IsTrue(map["one"].Found);
      Assert.IsTrue(map["two"].Found);
      Assert.IsFalse(map["three"].Found);
    }
  }
}
#endif