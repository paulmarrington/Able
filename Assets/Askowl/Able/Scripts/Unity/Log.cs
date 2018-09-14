// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Askowl {
  /// <a href=""></a>
  public class Log {
    #region Producer Interface
    /// <a href=""></a>
    public delegate void MessageRecorder(string action, string message, params object[] more);

    /// <a href=""></a>
    public delegate void EventRecorder(string message = "", params object[] more);

    /// <a href=""></a>
    public static MessageRecorder Messages([CallerMemberName] string memberName = "",
                                           [CallerFilePath]   string filePath   = "",
                                           [CallerLineNumber] int    lineNumber = 0) =>
      (action, message, more) => MessageEvent(Fill(action, memberName, message, filePath, lineNumber, more));

    /// <a href=""></a>
    public static EventRecorder Warnings(string                    action,
                                         [CallerMemberName] string memberName = "",
                                         [CallerFilePath]   string filePath   = "",
                                         [CallerLineNumber] int    lineNumber = 0) =>
      (message, more) => WarningEvent(Fill(action, memberName, message, filePath, lineNumber, more));

    /// <a href=""></a>
    public static EventRecorder Errors([CallerMemberName] string memberName = "",
                                       [CallerFilePath]   string filePath   = "",
                                       [CallerLineNumber] int    lineNumber = 0) =>
      (message, more) => ErrorEvent(Fill("Error", memberName, message, filePath, lineNumber, more));

    /// <a href=""></a>
    public static EventRecorder Events(string                    action,
                                       [CallerMemberName] string memberName = "",
                                       [CallerFilePath]   string filePath   = "",
                                       [CallerLineNumber] int    lineNumber = 0) =>
      (message, more) => MessageEvent(Fill(action, memberName, message, filePath, lineNumber, more));

    private static Contents Fill(
      string action, string member, string message, string path, int line, object[] more) => new Contents {
      component  = Path.GetFileNameWithoutExtension(path),
      lineNumber = line, action = action, result = message, more = more, member = member,
      extras     = More(More(more), $"line={line},member={member}")
    };

    /// <a href=""></a>
    public struct Contents {
      /// <a href=""></a>
      public string component, action, result, member, extras;

      /// <a href=""></a>
      public int lineNumber;

      /// <a href=""></a>
      public object[] more;
    }
    #endregion

    #region Consumer Interface
    /// <a href=""></a>
    public delegate void EventDelegate(Contents contents);

    /// <a href=""></a>
    public static event EventDelegate MessageEvent = ConsoleMessage;

    /// <a href=""></a>
    public static event EventDelegate WarningEvent = ConsoleWarning;

    /// <a href=""></a>
    public static event EventDelegate ErrorEvent = ConsoleError;

    /// <a href=""></a>
    public static string More(params object[] list) {
      string[] items = Array.ConvertAll(array: list, converter: x => x.ToString());
      return string.Join(separator: ",", value: items).Trim(trimCharacters).Replace("=,", "=");
    }

    private static char[] trimCharacters = {',', ' '};

    /// <a href=""></a>
    public static Dictionary<string, object> ToDictionary(Contents contents) {
      Dictionary<string, object> dictionary = new Dictionary<string, object> {
        {"component", contents.component}, {"action", contents.action},
        {"result", contents.result}, {"member", contents.member}
      };

      for (int i = 0; i < contents.more.Length; i++) {
        string key   = contents.more[i].ToString();
        object value = null;

        if (key.EndsWith("=")) {
          key   = key.Substring(0, key.Length - 1);
          value = contents.more[++i];
        }

        dictionary[key] = value;
      }

      return dictionary;
    }
    #endregion

    #region Console Log Consumer
    /// <a href=""></a>
    public static bool ConsoleEnabled = true;

    /// <a href=""></a>
    public static readonly Map Actions;

    /// <a href=""></a>
    public static string Action(string text, string colour = "darkblue", bool bold = false, bool italics = false) =>
      $"{Open('b', bold)}{Open('i', italics)}<color={colour}>{text}</color>{Close('i', italics)}{Close('b', bold)}";

    private static string ToString(Contents contents) {
      var result = (string.IsNullOrWhiteSpace(contents.result)) ? "" : $"result={contents.result},";
      var action = Actions[contents.action.ToLower()].Found ? Actions.Value.ToString() : contents.action;
      return $"{action}: for '{contents.component}' -- {result}{contents.extras}";
    }

    private static void ConsoleMessage(Contents contents) {
      if (ConsoleEnabled) {
        Debug.Log(ToString(contents), Obj(contents.more));
      }
    }

    private static void ConsoleWarning(Contents contents) {
      if (ConsoleEnabled) {
        Debug.LogWarning(ToString(contents), Obj(contents.more));
      }
    }

    private static void ConsoleError(Contents contents) {
      if (ConsoleEnabled) {
        Debug.LogError(ToString(contents), Obj(contents.more));
      }
    }

    private static string Open(char  tag, bool set) => set ? $"<{tag}>" : "";
    private static string Close(char tag, bool set) => set ? $"</{tag}>" : "";

    private static Object Obj(object[] more) => (more.Length != 1) ? null : more[0] as Object;

    static Log() {
      var tbd = Action(text: "TO-BE-DONE", colour: "maroon", bold: true, italics: true);
      var fix = Action(text: "FIX-BUG",    colour: "red",    bold: true);

      Actions = new Map("tbd", tbd, "todo", tbd, "later", tbd, "incomplete", tbd,
                        "fixme", fix, "fix-me", fix, "bug", fix, "outstanding", fix);
#if !UNITY_EDITOR
// So that mobile host logs don't get too crowded to read.
      Application.SetStackTraceLogType(logType:LogType.Log,stackTraceType:StackTraceLogType.None);
      ConsoleEnabled = false;
#endif
      if (UnityEngine.Analytics.Analytics.enabled) {
        MessageEvent += UnityAnalyticsMessage;
        WarningEvent += UnityAnalyticsWarning;
        ErrorEvent   += UnityAnalyticsError;
      }
    }
    #endregion

    #region Unity Analytics Log Consumer
    private static void UnityAnalyticsMessage(Contents contents) =>
      UnityEngine.Analytics.Analytics.CustomEvent(contents.action, ToDictionary(contents));

    private static void UnityAnalyticsWarning(Contents contents) =>
      UnityEngine.Analytics.Analytics.CustomEvent("WARNING", ToDictionary(contents));

    private static void UnityAnalyticsError(Contents contents) =>
      UnityEngine.Analytics.Analytics.CustomEvent("ERROR", ToDictionary(contents));
    #endregion
  }
}