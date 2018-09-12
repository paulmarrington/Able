// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

using System;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Askowl {
  /// <a href=""></a>
  public class Log {
    #region Producer Interface
    /// <a href=""></a>
    public delegate void MessageRecorder(string nextAction, string message, params object[] more);

    /// <a href=""></a>
    public delegate void EventRecorder(string message = "", params object[] more);

    /// <a href=""></a>
    public static MessageRecorder Messages([CallerMemberName] string memberName = "",
                                           [CallerFilePath]   string filePath   = "",
                                           [CallerLineNumber] int    lineNumber = 0) =>
      (nextAction, message, more) => MessageEvent(Fill(nextAction, memberName, message, filePath, lineNumber, more));

    /// <a href=""></a>
    public static MessageRecorder Warnings([CallerMemberName] string memberName = "",
                                           [CallerFilePath]   string filePath   = "",
                                           [CallerLineNumber] int    lineNumber = 0) =>
      (nextAction, message, more) => WarningEvent(Fill(nextAction, memberName, message, filePath, lineNumber, more));

    /// <a href=""></a>
    public static EventRecorder Errors([CallerMemberName] string memberName = "",
                                       [CallerFilePath]   string filePath   = "",
                                       [CallerLineNumber] int    lineNumber = 0) =>
      (message, more) => ErrorEvent(Fill(null, memberName, message, filePath, lineNumber, more));

    /// <a href=""></a>
    public static EventRecorder Events(string                    nextAction,
                                       [CallerMemberName] string memberName = "",
                                       [CallerFilePath]   string filePath   = "",
                                       [CallerLineNumber] int    lineNumber = 0) =>
      (message, more) => MessageEvent(Fill(nextAction, memberName, message, filePath, lineNumber, more));

    private static Contents Fill(
      string act, string me, string msg, string path, int line, object[] more) => new Contents {
      component = Path.GetFileNameWithoutExtension(path), nextAction             = act, result = msg,
      extras    = More(More(more), $"path={path},member={me},line={line}"), more = more
    };

    /// <a href=""></a>
    public struct Contents {
      /// <a href=""></a>
      public string component, nextAction, result, extras;

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
      return string.Join(separator: ",", value: items);
    }

    /// <a href=""></a>
    public static Map ToMap(string action, string result, params object[] more) {
      Map map = new Map("action", action, "result", result);

      for (int i = 0; i < more.Length; i++) {
        string key   = more[i].ToString();
        object value = null;

        if (key.EndsWith("=")) {
          key   = key.Substring(0, key.Length - 1);
          value = more[++i];
        }

        map.Add(key, value);
      }

      return map;
    }
    #endregion

    #region Console Log Consumer
    /// <a href=""></a>
    public static bool ConsoleEnabled = true;

    /// <a href=""></a>
    public static readonly Map Actions;

    /// <a href=""></a>
    public static string NextAction(string text, string colour = "darkblue", bool bold = false, bool italics = false) =>
      $"{Open('b', bold)}{Open('i', italics)}<color={colour}>{text}</color>{Close('i', italics)}{Close('b', bold)}";

    private static string ToString(string component, string action, string result, params object[] more) {
      var mort = More(more);
      mort   = (string.IsNullOrWhiteSpace(mort)) ? "" : $", more: {mort}";
      result = (string.IsNullOrWhiteSpace(result)) ? "" : $"result: {result}";
      action = Actions[action.ToLower()].Found ? Actions.Value.ToString() : action;
      return $"{action}: for '{component}' -- {result}{mort}";
    }

    private static void ConsoleMessage(Contents msg) {
      if (ConsoleEnabled) {
        Debug.Log(ToString(msg.component, msg.nextAction, msg.result, msg.more), Obj(msg.more));
      }
    }

    private static void ConsoleWarning(Contents msg) {
      if (ConsoleEnabled) {
        Debug.LogWarning(ToString(msg.component, msg.nextAction, msg.result, msg.more), Obj(msg.more));
      }
    }

    private static void ConsoleError(Contents msg) {
      if (ConsoleEnabled) {
        Debug.LogError(ToString(msg.component, msg.nextAction ?? "Error", msg.result, msg.more), Obj(msg.more));
      }
    }

    private static string Open(char  tag, bool set) => set ? $"<{tag}>" : "";
    private static string Close(char tag, bool set) => set ? $"</{tag}>" : "";

    private static Object Obj(object[] more) => (more.Length != 1) ? null : more[0] as Object;

    static Log() {
      var tbd = NextAction(text: "TBD", colour: "maroon", bold: true, italics: true);
      Actions = new Map("tbd", tbd, "todo", tbd);
#if !UNITY_EDITOR
// So that mobile host logs don't get too crowded to read.
      Application.SetStackTraceLogType(logType:LogType.Log,stackTraceType:StackTraceLogType.None);
      ConsoleEnabled = false;
#endif
    }
    #endregion
  }
}