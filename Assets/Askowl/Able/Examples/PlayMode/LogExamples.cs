// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.TestTools;

namespace Askowl.Examples {
  /// Using <see cref="Log" />
  public class LogExamples : PlayModeTests {
    private        string              SceneName  = "Askowl-Able-Examples";
    private static Log.MessageRecorder logMessage = Log.Messages();
    private static Log.MessageRecorder logWarning = Log.Warnings();
    private static Log.EventRecorder   LogEvent   = Log.Events(nextAction: "Lost Health");
    private static Log.EventRecorder   Todo       = Log.Events(nextAction: "Todo");
    private static Log.EventRecorder   LogError   = Log.Errors();

    /// Using <see cref="Log.Messages"/>
    [UnityTest]
    public IEnumerator Messages() {
      yield return LoadScene(SceneName);

      LogAssert.Expect(LogType.Log, new Regex("Run Away: .* result: all is well"));
      logMessage(nextAction: "Run Away", message: "all is well");
    }

    /// Using <see cref="Log.Events"/>
    [UnityTest]
    public IEnumerator Events() {
      yield return LoadScene(SceneName);

      LogAssert.Expect(LogType.Log, new Regex("Lost Health: .* result: fell down well"));
      LogEvent("fell down well");
    }

    /// Using <see cref="Log.Warnings"/>
    [UnityTest]
    public IEnumerator Warnings() {
      yield return LoadScene(SceneName);

      LogAssert.Expect(LogType.Warning, new Regex("Removing old objects: .* result: Too many objects"));
      logWarning(nextAction: "Removing old objects", message: "Too many objects");
    }

    /// Using <see cref="Log.Errors"/>
    [UnityTest]
    public IEnumerator Errors() {
      yield return LoadScene(SceneName);

      LogAssert.Expect(LogType.Error, new Regex("Error: .* result: we should never get here"));
      LogError("we should never get here");
    }

    /// Using <see cref="Log.More"/>
    [UnityTest]
    public IEnumerator More() {
      yield return LoadScene(SceneName);

      var more = Log.More("count=", 1, "hello=", "world", "one", "two");
      Assert.AreEqual(expected: "count=,1,hello=,world,one,two", actual: more);
    }

    /// Using <see cref="Log.ToMap"/>
    [UnityTest]
    public IEnumerator ToMap() {
      yield return LoadScene(SceneName);

      var map = Log.ToMap("Act Now", "Success", "count=", 1, "hello=", "world", "one", "two");
      Assert.IsTrue(map["action"].Found);
      Assert.AreEqual("Act Now", map.Value);
      Assert.AreEqual("Success", map["result"].Value);
      Assert.AreEqual(1,       map["count"].Value);
      Assert.AreEqual("world",   map["hello"].Value);
      Assert.IsTrue(map["one"].Found);
      Assert.IsTrue(map["two"].Found);
    }

    /// Using <see cref="Log.ConsoleEnabled"/>
    [UnityTest]
    public IEnumerator ConsoleEnabled() {
      yield return LoadScene(SceneName);

      Log.ConsoleEnabled = false;
      LogAssert.NoUnexpectedReceived();
      LogEvent("fell down well");
      Log.ConsoleEnabled = true;
    }

    /// Using <see cref="Log.Actions"/>
    [UnityTest]
    public IEnumerator Actions() {
      yield return LoadScene(SceneName);

      LogAssert.Expect(LogType.Log, new Regex("<b><i><color=maroon>TBD</color></i></b>: .* result: TBD"));
      Todo("TBD");
    }

    /// Using <see cref="Log.NextAction"/>
    [UnityTest]
    public IEnumerator NextAction() {
      yield return LoadScene(SceneName);

      Log.Actions.Add("deleteme", Log.NextAction("Delete Me", RichText.Colours.Aqua, bold: true, italics: true));
      LogAssert.Expect(LogType.Log, new Regex("<b><i><color=aqua>Delete Me</color></i></b>:"));
      logMessage(nextAction: "DeleteMe", message: "later");
    }
  }
}