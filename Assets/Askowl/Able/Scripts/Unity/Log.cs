// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

namespace Askowl {
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Runtime.CompilerServices;
  using UnityEngine.Analytics;

  /// <a href="http://bit.ly/2RinksQ">Logging dirt cheap</a>
  public static class Log {
    #region Producer Interface
    /// <a href="http://bit.ly/2Oq9Bl0">Signature to write message log entries - this is what you use in your code</a>
    public delegate void MessageRecorder(string action, string message);

    /// <a href="http://bit.ly/2Oq9pSO">Signature to write event log entries - this is what you use in your code</a>
    public delegate void EventRecorder(string message);

    /// <a href="http://bit.ly/2Oq9Bl0">Method called to create a log calling function in your script</a>
    public static MessageRecorder Messages(
      [CallerMemberName] string memberName = "",
      [CallerFilePath]   string filePath   = "",
      [CallerLineNumber] int    lineNumber = 0) => (action, message) =>
      MessageEvent(Fill(action, memberName, message, filePath, lineNumber));

    /// <a href="http://bit.ly/2Re92cJ">>Method called to create a log calling function in your script</a>
    public static EventRecorder Warnings(
      string                    action,
      [CallerMemberName] string memberName = "",
      [CallerFilePath]   string filePath   = "",
      [CallerLineNumber] int    lineNumber = 0) =>
      (message) => WarningEvent(Fill(action, memberName, message, filePath, lineNumber));

    /// <a href="http://bit.ly/2Oq9pSO">>Method called to create a log calling function in your script</a>
    public static EventRecorder Errors(
      [CallerMemberName] string memberName = "",
      [CallerFilePath]   string filePath   = "",
      [CallerLineNumber] int    lineNumber = 0) =>
      (message) => ErrorEvent(Fill("Error", memberName, message, filePath, lineNumber));

    /// <a href="http://bit.ly/2Oq9pSO">>Method called to create a log calling function in your script</a>
    public static EventRecorder Events(
      string                    action,
      [CallerMemberName] string memberName = "",
      [CallerFilePath]   string filePath   = "",
      [CallerLineNumber] int    lineNumber = 0) =>
      (message) => MessageEvent(Fill(action, memberName, message, filePath, lineNumber));

    private static Contents Fill(string action, string member, string message, string path, int line) {
      var more       = "";
      int splitIndex = message.IndexOf(" :: ", StringComparison.Ordinal);
      if (splitIndex != -1) {
        more    = $"{message.Substring(splitIndex + 4)},";
        message = message.Substring(0, splitIndex);
      }
      return new Contents {
        component = Path.GetFileNameWithoutExtension(path), lineNumber = line, action = action, result = message
      , extras    = $"{more}line={line},member={member}", member       = member
      };
    }

    /// <a href="">Shortcut to call `Log.Debug` from anywhere</a> //#TBD#//
    public static void Debug(
      string                    message,
      [CallerMemberName] string memberName = "",
      [CallerFilePath]   string filePath   = "",
      [CallerLineNumber] int    lineNumber = 0) =>
      ConsoleWarning(Fill("Debug", memberName, message, filePath, lineNumber));

    /// <a href="">Shortcut to call `Log.Debug` from anywhere</a> //#TBD#//
    public static void Error(
      string                    message,
      [CallerMemberName] string memberName = "",
      [CallerFilePath]   string filePath   = "",
      [CallerLineNumber] int    lineNumber = 0) => ErrorEvent(Fill("Error", memberName, message, filePath, lineNumber));

    /// <a href="http://bit.ly/2O0Z5Bu">Implementation methods get information to record in this structure</a>
    public struct Contents {
      /// <a href="http://bit.ly/2O0Z5Bu"></a>
      // ReSharper disable InconsistentNaming
      public string component, action, result, member, extras;
      /// <a href="http://bit.ly/2O0Z5Bu"></a>
      // ReSharper disable once NotAccessedField.Global
      public int lineNumber;
      // ReSharper restore InconsistentNaming
    }
    #endregion

    #region Consumer Interface
    /// <a href="http://bit.ly/2Rj0VM2">method signature for message listeners (log writers)</a>
    public delegate void EventDelegate(Contents contents);

    /// <a href="http://bit.ly/2Rj0VM2">Event queue for log providers to listen to</a>
    public static event EventDelegate MessageEvent = ConsoleMessage;

    /// <a href="http://bit.ly/2Rj0VM2">Event queue for log providers to listen to</a>
    public static event EventDelegate WarningEvent = ConsoleWarning;

    /// <a href="http://bit.ly/2Rj0VM2">Event queue for log providers to listen to</a>
    public static event EventDelegate ErrorEvent = ConsoleError;

    /// <a href="http://bit.ly/2O0Z5Bu">Log formatting for extra data</a>
    public static string Extras(string[] list) =>
      string.Join(separator: ",", value: list).Trim(trimCharacters).Replace("=,", "=");

    /// <a href="http://bit.ly/2O0Z5Bu">Log formatting for extra data</a>
    public static string Extras(object[] list) => Extras(Array.ConvertAll(array: list, converter: x => x.ToString()));

    private static char[] trimCharacters = { ',', ' ' };

    /// <a href="Log Parameter Formatting">Log formatting for extra data</a>
    public static Dictionary<string, object> ToDictionary(Contents contents) {
      var dictionary = new Dictionary<string, object> {
        { "component", contents.component }, { "action", contents.action }, { "result", contents.result }
      , { "member", contents.member }
      };

      for (var i = 0; i < contents.extras.Length; i++) {
        string key   = contents.extras[i].ToString();
        object value = null;

        if (key.EndsWith("=")) {
          key   = key.Substring(0, key.Length - 1);
          value = contents.extras[++i];
        }

        dictionary[key] = value;
      }

      return dictionary;
    }
    #endregion

    #region Console Log Consumer
    /// <a href="http://bit.ly/2RezLpo">Set to false for no logging to Unity console</a>
    public static bool ConsoleEnabled = true;

    /// <a href="http://bit.ly/2RezLpo">Console logging actions - providing text formatting commands</a>
    public static readonly Map Actions;

    /// <a href="http://bit.ly/2Rj0VM2"></a>
    public static string Action(string text, string colour = "darkblue", bool bold = false, bool italics = false) =>
      $"{Open('b', bold)}{Open('i', italics)}<color={colour}>{text}</color>{Close('i', italics)}{Close('b', bold)}";

    private static string ToString(Contents contents) {
      string result = string.IsNullOrWhiteSpace(contents.result) ? "" : $"result={contents.result},";
      string action = Actions[contents.action.ToLower()].Found ? Actions.Value.ToString() : contents.action;
      return $"{action}: for '{contents.component}' -- {result}{contents.extras}";
    }

    private static void ConsoleMessage(Contents contents) {
      if (ConsoleEnabled) UnityEngine.Debug.Log(ToString(contents));
    }

    private static void ConsoleWarning(Contents contents) {
      if (ConsoleEnabled) UnityEngine.Debug.LogWarning(ToString(contents));
    }

    private static void ConsoleError(Contents contents) {
      if (ConsoleEnabled) UnityEngine.Debug.LogError(ToString(contents));
    }

    private static string Open(char  tag, bool set) => set ? $"<{tag}>" : "";
    private static string Close(char tag, bool set) => set ? $"</{tag}>" : "";

    static Log() {
      string tbd = Action(text: "TO-BE-DONE", colour: "maroon", bold: true, italics: true);
      string fix = Action(text: "FIX-BUG",    colour: "red",    bold: true);

      Actions = new Map();
      Actions.Add("tbd", tbd).Add("todo", tbd).Add("later", tbd).Add("incomplete", tbd)
             .Add("fixme", fix).Add("fix-me", fix).Add("bug", fix).Add("outstanding", fix);
      #if !UNITY_EDITOR // So that mobile host logs don't get too crowded to read.
      Application.SetStackTraceLogType(logType:LogType.Log,stackTraceType:StackTraceLogType.None);
      ConsoleEnabled = false;
      #endif
      if (Analytics.enabled) {
        MessageEvent += UnityAnalyticsMessage;
        WarningEvent += UnityAnalyticsWarning;
        ErrorEvent   += UnityAnalyticsError;
      }
    }
    #endregion

    #region Unity Analytics Log Consumer
    private static void UnityAnalyticsMessage(Contents contents) =>
      Analytics.CustomEvent(contents.action, ToDictionary(contents));

    private static void UnityAnalyticsWarning(Contents contents) =>
      Analytics.CustomEvent("WARNING", ToDictionary(contents));

    private static void UnityAnalyticsError(Contents contents) =>
      Analytics.CustomEvent("ERROR", ToDictionary(contents));
    #endregion
  }
}