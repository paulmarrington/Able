// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

#if AskowlTests
namespace Askowl.Able.Examples {
  using NUnit.Framework;

  public class PickExamples {
    [Test] internal void PickExample() {
      var nose = new PickImplementation();

      Assert.AreEqual("1", nose.Pick());
      Assert.AreEqual("2", nose.Pick());
      Assert.AreEqual("3", nose.Pick());
    }

    private class PickImplementation : Pick<string> {
      private int    count;
      public  string Pick() => (++count).ToString();
    }
  }
}
#endif