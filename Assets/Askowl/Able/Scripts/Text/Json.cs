// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

using System;
using System.Text;
using UnityEngine;

namespace Askowl {
  /// <a href=""></a>
  public class Json : IDisposable {
    #region PublicInterface
    /// <a href=""></a>
    public static Json Instance => new Json();

    private Json() { }

    /// <a href=""></a>
    public Json Parse(string jsonText) {
      if (string.IsNullOrEmpty(jsonText)) {
        ParseError("json input has no content");
        return this;
      }

      tree = Trees.Instance;
      json = jsonText;
      idx  = 0;

      if (!SkipWhiteSpace()) {
        ParseError("Empty JSON");
        return this;
      }

      if (json[idx] == '{') {
        CheckToken('{').ParseToNode();
      } else {
        ParseOneEntryToNode();
      }

      return this;
    }

    /// <a href=""></a>
    /// <inheritdoc />
    public void Dispose() { tree.Dispose(); }

    /// <a href=""></a>
    public String Name => tree.Name;

    /// <a href=""></a>
    public T Here<T>() => Convert<T>(tree.Leaf);

    /// <a href=""></a>
    public bool IsA<T>() => CheckIsA<T>(tree.Leaf);

    /// <a href=""></a>
    public string ErrorMessage;

    /// <a href=""></a>
    public bool Error => ErrorMessage != null;

    #region AccessNodes
    /// <a href=""></a>
    public bool IsRoot => tree.IsRoot;

    /// <a href=""></a>
    public T Get<T>(params object[] path) {
      tree.To(path);
      return Here<T>();
    }

    /// <a href=""></a>
    public Json To(params object[] path) {
      tree.To(path);
      return this;
    }

    /// <a href=""></a>
    public Json Next(params object[] path) {
      tree.Next(path);
      return this;
    }
    #endregion

    #region NodeEnumeration
    /// <a href=""></a>
    public object[] Children => tree.Children;

    /// <a href=""></a>
    public IDisposable Anchor() => tree.Anchor();
    #endregion
    #endregion

    #region SupportData
    private Trees tree;

    private string json = "";
    private int    idx;
    #endregion

    #region Parsing
    private Json CheckToken(char token) {
      if (!SkipWhiteSpace()) return this;

      if (json[idx++] != token) {
        ParseError($"Expecting token '{token}'");
        return this;
      }

      SkipWhiteSpace();
      return this;
    }

    private Json ParseToNode() {
      while (ParseOneEntryToNode()) {
        if (!SkipWhiteSpace() || (json[idx] == '}')) {
          idx++;
          break;
        }
      }

      return this;
    }

    private bool ParseOneEntryToNode() {
      string key = CheckToken('"').ParseString();
      if (!CheckToken(':').SkipWhiteSpace()) return false;

      using (tree.Anchor()) {
        tree.Add(key);
        ParseObject();
      }
      return idx < json.Length;
    }

    private void ParseObject() {
      char token = json[idx++];

      switch (token) {
        case '{':
          ParseToNode();
          return;
        case '[':
          ParseArray();
          return;
        case '"':
          tree.Leaf = ParseString();
          return;
        default:
          string word = NextWord();

          switch (word) {
            case "true":
              tree.Leaf = true;
              return;
            case "false":
              tree.Leaf = false;
              return;
            case "null":
              tree.Leaf = null;
              return;

            default:
              long   i;
              double d;

              if (long.TryParse(word, out i)) {
                tree.Leaf = i;
              } else if (double.TryParse(word, out d)) {
                tree.Leaf = d;
              } else {
                ParseError($"word '{word}' unknown");
              }

              break;
          }

          break;
      }
    }

    private void ParseArray() {
      var index = 0;

      while (SkipWhiteSpace() && (json[idx] != ']')) {
        tree.Add(index++);
        ParseObject();
        tree.To("..");
      }
    }

    private string ParseString() {
      StringBuilder builder = new StringBuilder();

      while (json[idx] != '"') {
        builder.Append((json[idx] == '\\') ? Escape() : json[idx++]);
      }

      idx++; // drop closing quote
      return builder.ToString();
    }

    private char Escape() {
      switch (json[++idx]) {
        case 'b': return '\b';
        case 'f': return '\f';
        case 'n': return '\n';
        case 'r': return '\t';
        case 't': return '\t';
        default:  return json[idx];
      }
    }

    private string NextWord() {
      int first = idx - 1;

      while (!char.IsWhiteSpace(json[idx]) && ("{}[]\",:".IndexOf(json[idx]) == -1)) {
        if (++idx >= json.Length) return null;
      }

      if (idx == first) {
        ParseError("Expecting a word or number");
        idx++;
      }

      return json.Substring(startIndex: first, length: idx - first);
    }

    private static bool IsWhiteSpace(char chr) => char.IsWhiteSpace(chr) || (chr == ',');

    private bool SkipWhiteSpace() {
      while ((idx < json.Length) && IsWhiteSpace(json[idx])) idx++;
      return idx < json.Length;
    }
    #endregion

    #region ErrorProcessing
    private void ParseError(string msg) {
      int    length = Mathf.Min(32, json.Length - idx);
      string part   = (length > 0) ? json.Substring(idx - 1, length) : "";

      ErrorMessage = $"JSON Parsing Error: {msg} - at {idx}, from {part}";
    }

    private bool AccessFailure(string message) {
      ErrorMessage = $"JSON Access Failure: {message} -  at {tree.Leaf.GetType().Name}";
      return false;
    }

    private bool CheckIsA<T>(object node) {
      if (node is T) return true;
      if ((typeof(T) == typeof(int))    && (node is long)) return true;
      if ((typeof(T) == typeof(float))  && (node is double)) return true;
      if ((typeof(T) == typeof(float))  && (node is long)) return true;
      if ((typeof(T) == typeof(double)) && (node is long)) return true;

      return AccessFailure($"Expecting type {typeof(T).Name}");
    }

    private T Convert<T>(object node) {
      // ReSharper disable EnforceIfStatementBraces
      if (node           == null) node                                 = default(T);
      else if (typeof(T) == typeof(string)) node                       = node.ToString();
      else if ((typeof(T) == typeof(int))    && (node is long)) node   = (int) ((long) node);
      else if ((typeof(T) == typeof(float))  && (node is double)) node = (float) ((double) node);
      else if ((typeof(T) == typeof(float))  && (node is long)) node   = (float) ((long) node);
      else if ((typeof(T) == typeof(double)) && (node is long)) node   = (double) ((long) node);
      // ReSharper restore EnforceIfStatementBraces

      if (node is T) return (T) node;

      AccessFailure($"Expecting type {typeof(T)} for conversion - was {node?.GetType().Name}");
      return default(T);
    }
    #endregion
  }
}