// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

using System;
using System.Text;
using UnityEngine;

namespace Askowl {
  /// <summary>
  /// Parse JSON of unknown format to a dictionary and provide access methods
  /// </summary>
  /// <remarks><a href="http://unitydoc.marrington.net/Able#jsoncs-parse-any-json-to-dictionary">Json Parser</a></remarks>
  public class Json : IDisposable {
    #region PublicInterface
    /// <remarks><a href="http://unitydoc.marrington.net/Able#parse">Parse JSON from text</a></remarks>
    public Json Parse(string jsonText) {
      if (string.IsNullOrEmpty(jsonText)) {
        ParseError("json input has no content");
        return this;
      }

      tree = TreeContainer.Instance;
      json = jsonText;
      idx  = 0;
      return CheckToken('{').ParseToNode();
    }

    public static Json Instance => new Json();
    private Json() { }

    /// <inheritdoc />
    public void Dispose() { tree.Dispose(); }

    public String Name => tree.Name;

    /// <remarks><a href="http://unitydoc.marrington.net/Able#current-location">Here - The Current Location</a></remarks>
    public T Here<T>() => Convert<T>(tree.Leaf());

    /// <typeparam name="T">The type we anticipate this node to be</typeparam>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#information-retrieval">Retrieve Value at Current Location</a></remarks>
    public bool IsA<T>() => CheckIsA<T>(tree.Leaf());

    /// <summary>
    /// Checks to see if we can cast the node type to that provided.
    /// </summary>
    /// <remarks>It will fill <see cref="ErrorMessage"/> if there is a fault.</remarks>
    /// <typeparam name="T">The type we anticipate this node to be</typeparam>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#information-retrieval">Retrieve Value at Current Location</a></remarks>
//    public bool IsA<T>(object node) => CheckIsA<T>(node);

    /// <remarks><a href="http://unitydoc.marrington.net/Able#information-retrieval">Retrieve Value at Current Location</a></remarks>
    public bool IsNode => tree.Leaf() == null;

    /// <remarks><a href="http://unitydoc.marrington.net/Able#information-retrieval">Retrieve Value at Current Location</a></remarks>
    public bool IsArray => tree.HasAnonymousChildren;

    /// <summary>
    /// When completely lost, ask for the node type. It could be Node, Array, object, double, long, bool or null
    /// </summary>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#information-retrieval">Retrieve Value at Current Location</a></remarks>
//    public Type NodeType => tree.Leaf().GetType();

    /// <remarks><a href="http://unitydoc.marrington.net/Able#error-processing">Error Indication and Message</a></remarks>
    public string ErrorMessage => tree.ErrorMessage;

    /// <remarks><a href="http://unitydoc.marrington.net/Able#error-processing">Error Indication and Message</a></remarks>
    public bool Error => tree.ErrorMessage != null;

    #region AccessNodes
    public Json Root {
      get {
        tree.Root();
        return this;
      }
    }

    public bool IsRoot => tree.IsRoot;

    public Json Parent() {
      tree.Parent();
      return this;
    }
    /// <summary>
    /// Use Get if you know exactly what you are looking for.
    /// </summary>
    /// <remarks>It will fill <see cref="ErrorMessage"/> if there is a fault.</remarks>
    /// <param name="path">
    /// Path to the target node either as a list of objects or a single string with nodes separated by '.'.
    /// Array indexes can be numbers or strings that convert to numbers.
    /// <example>"root", "level1", 0, "level 3"</example>
    /// <example>"root.level1.0.level 3"</example>
    /// </param>
    /// <typeparam name="T">The type we anticipate this node to be</typeparam>
    /// <returns>what you asked for or default&lt;T> if not found</returns>
//    public T Get<T>(params object[] path) {
//      Walk(path);
//      return Here<T>();
//    }

    /// <param name="path">
    /// Path to the target node either as a list of objects or a single string with nodes separated by '.'.
    /// Array indexes can be numbers or strings that convert to numbers.
    /// <example>"root", "level1", 0, "level 3"</example>
    /// <example>"root.level1.0.level 3"</example>
    /// </param>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#tree-traversal">Traversing the JSON Node Tree</a></remarks>
//    public Json Walk(params object[] path) {
//      here         = root;
//      NodeName     = "Root";
//      ErrorMessage = null;
//      return WalkOn(path);
//    }

    /// <param name="path">
    /// Path to the target node either as a list of objects or a single string with nodes separated by '.'.
    /// Array indexes can be numbers or strings that convert to numbers.
    /// <example>0, "level 3"</example>
    /// <example>"0.level 3"</example>
    /// </param>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#tree-traversal">Traversing the JSON Node Tree</a></remarks>
    public Json To(params object[] path) {
      tree.To(path);
      return this;
    }

    /// <summary>
    /// Shortcut to walk further to a node and check the node is of the type expected.
    /// </summary>
    /// <remarks>It will fill <see cref="ErrorMessage"/> if there is a fault.</remarks>
    /// <param name="path">
    /// Path to the target node either as a list of objects or a single string with nodes separated by '.'.
    /// Array indexes can be numbers or strings that convert to numbers.
    /// <example>0, "level 3"</example>
    /// <example>"0.level 3"</example>
    /// </param>
    /// <returns>false if path does not exist</returns>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#tree-traversal">Traversing the JSON Node Tree</a></remarks>
//    public bool WalkOn<T>(params object[] path) => WalkOn(path).IsA<T>();
    #endregion

    #region NodeEnumeration
    /// <remarks><a href="http://unitydoc.marrington.net/Able#tree-traversal">Traversing the JSON Node Tree</a></remarks>
    public int ChildCount => tree.ChildCount;

    /// <remarks><a href="http://unitydoc.marrington.net/Able#tree-traversal">Traversing the JSON Node Tree</a></remarks>
    public TreeContainer ForEach(Action action) => tree.ForEach(action);

    /// <summary>
    /// Use to set and return to a current location after some operations. Best for enumerations
    /// <code>using json.Anchor { json.Walk("first.one"); }</code>
    /// </summary>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#Anchor">Mark Current Node Location</a></remarks>
    public IDisposable Anchor => tree.Anchor;
    #endregion
    #endregion

    #region SupportData
    private TreeContainer tree;
    private string        json = "";
    private int           idx;
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

      tree.Add(key);
      ParseObject();
      tree.Parent();
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
          tree.Leaf(ParseString());
          return;
        default:
          string word = NextWord();

          switch (word) {
            case "true":
              tree.Leaf(setTo: true);
              return;
            case "false":
              tree.Leaf(setTo: false);
              return;
            case "null":
              tree.Leaf(setTo: null);
              return;

            default:
              long   i;
              double d;

              if (long.TryParse(word, out i)) {
                tree.Leaf(setTo: i);
              } else if (double.TryParse(word, out d)) {
                tree.Leaf(setTo: d);
              } else {
                ParseError($"word '{word}' unknown");
              }

              break;
          }

          break;
      }
    }

    private void ParseArray() {
      while (SkipWhiteSpace() && (json[idx] != ']')) {
        tree.AddAnonymous();
        ParseObject();
      }

      tree.Parent();
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
      if (Error) return false;

      while ((idx < json.Length) && IsWhiteSpace(json[idx])) idx++;
      return idx < json.Length;
    }
    #endregion

    #region ErrorProcessing
    private void ParseError(string msg) {
      int    length = Mathf.Min(32, json.Length - idx);
      string part   = (length > 0) ? json.Substring(idx - 1, length) : "";

      tree.ErrorMessage = $"JSON Parsing Error: {msg} - at {idx}, from {part}";
    }

    private bool AccessFailure(string message) {
      tree.ErrorMessage = $"JSON Access Failure: {message} -  at {tree.Leaf().GetType().Name}  [[{tree}]]";
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

      AccessFailure("Expecting type {typeof(T)} for conversion - was {node.GetType().Name}");
      return default(T);
    }
    #endregion
  }
}