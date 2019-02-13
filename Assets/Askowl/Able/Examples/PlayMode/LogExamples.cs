// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

#if AskowlTests
namespace Askowl.Able.Examples {
  using System.Collections;
  using System.Text.RegularExpressions;
  using Askowl.RichText;
  using UnityEngine;
  using UnityEngine.Assertions;
  using UnityEngine.TestTools;

  /// Using <see cref="Log" /><inheritdoc />
  public class LogExamples : PlayModeTests {
    private const  string              sceneName = "Askowl-Able-Examples";
    private static Log.MessageRecorder log       = Log.Messages();
    private static Log.EventRecorder   warning   = Log.Warnings("Warning");
    private static Log.EventRecorder   logEvent  = Log.Events(action: "Lost Health");
    private static Log.EventRecorder   todo      = Log.Warnings(action: "Todo");
    private static Log.EventRecorder   error     = Log.Errors();

    /// Using <see cref="Log.Messages"/>
    [UnityTest] public IEnumerator Messages() {
      yield return LoadScene(sceneName);

      LogAssert.Expect(LogType.Log, new Regex("Run Away: .* result=all is well.*line=.*,member="));
      log(action: "Run Away", message: "all is well");
    }

    /// Using <see cref="Log.Events"/>
    [UnityTest] public IEnumerator Events() {
      yield return LoadScene(sceneName);

      LogAssert.Expect(LogType.Log, new Regex("Lost Health: .* result=fell down well"));
      logEvent("fell down well");
    }

    /// Using <see cref="Log.Warnings"/>
    [UnityTest] public IEnumerator Warnings() {
      yield return LoadScene(sceneName);

      LogAssert.Expect(LogType.Warning, new Regex("Warning: .* result=Too many objects"));
      warning("Too many objects");
    }

    /// Using <see cref="Log.Errors"/>
    [UnityTest] public IEnumerator Errors() {
      yield return LoadScene(sceneName);

      LogAssert.Expect(LogType.Error, new Regex("Error: .* result=we should never get here"));
      error("we should never get here");
    }

    /// Using <see cref="Log.Extras(object[])"/>
    [UnityTest] public IEnumerator More() {
      yield return LoadScene(sceneName);

      string more = Log.Extras(new object[] { "count=", 1, "hello=", "world", "one", "two" });
      Assert.AreEqual(expected: "count=1,hello=world,one,two", actual: more);
    }

    /// Using <see cref="Log.ToDictionary"/>
    [UnityTest] public IEnumerator ToDictionary() {
      yield return LoadScene(sceneName);

      var dictionary = Log.ToDictionary(
        new Log.Contents {
          component = "My Component", action = "Act Now", result = "Success", extras = "count=1,hello=world,one,two"
        });

      Assert.IsTrue(dictionary.ContainsKey("action"));
      Assert.AreEqual("Act Now", dictionary["action"]);
      Assert.AreEqual("Success", dictionary["result"]);
      Assert.AreEqual("1",       dictionary["count"]);
      Assert.AreEqual("world",   dictionary["hello"]);
      Assert.IsTrue(dictionary.ContainsKey("one"));
      Assert.IsTrue(dictionary.ContainsKey("two"));
    }

    /// Using <see cref="Log.ConsoleEnabled"/>
    [UnityTest] public IEnumerator ConsoleEnabled() {
      yield return LoadScene(sceneName);

      Log.ConsoleEnabled = false;
      LogAssert.NoUnexpectedReceived();
      logEvent("fell down well");
      Log.ConsoleEnabled = true;
    }

    /// Using <see cref="Log.Actions"/>
    [UnityTest] public IEnumerator Actions() {
      yield return LoadScene(sceneName);

      LogAssert.Expect(LogType.Warning, new Regex("<b><i><color=maroon>TO-BE-DONE</color></i></b>: .* result=TBD"));
      todo("TBD");
    }

    /// Using <see cref="Log.Action"/>
    [UnityTest] public IEnumerator NextAction() {
      yield return LoadScene(sceneName);

      Log.Actions.Add("deleteme", Log.Action("Delete Me", Colour.Aqua, bold: true, italics: true));
      LogAssert.Expect(LogType.Log, new Regex("<b><i><color=aqua>Delete Me</color></i></b>:"));
      log(action: "DeleteMe", message: "later");
    }
  }
}
#endif