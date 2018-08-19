using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Askowl {
  /// <summary>
  /// Parse JSON of unknown format to a dictionary and provide access methods
  /// </summary>
  public class Json : IDisposable {
    #region PublicInterface
    /// <summary>
    /// Change the JSON to process on an existing Json instance. Recycling is good.
    /// </summary>
    /// <param name="jsonText">String with hopefully correctly formatted JSON</param>
    /// <returns>false on error and sets <see cref="ErrorMessage"/></returns>
    public Json Parse(string jsonText) {
      if (string.IsNullOrEmpty(jsonText)) return ParseError("json input has no content");

      json         = jsonText;
      idx          = 0;
      ErrorMessage = null;
      here         = root = new Node();
      if (CheckToken('{')) ParseToNode(root);
      return me;
    }

    /// <summary>
    /// Parsing the JSON creates a tree of nodes, arrays and leaf objects. You should not need to get to a Node, but it is here for if I a wrong.
    /// </summary>
    internal class Node : Dictionary<string, object> { };

    /// <summary>
    /// Returns the node at the location we have walked to in the tree - if it is of the type expected.
    /// </summary>
    /// <remarks>It will fill <see cref="ErrorMessage"/> if there is a fault.</remarks>
    /// <typeparam name="T">The type we anticipate this node to be</typeparam>
    /// <returns>The requested node in the correct form for (T) or default&lt;T> if it can't be accessed</returns>
    public T Here<T>() => Convert<T>(here);

    /// <summary>
    /// Checks to see if we can cast the node type to that provided.
    /// </summary>
    /// <remarks>It will fill <see cref="ErrorMessage"/> if there is a fault.</remarks>
    /// <typeparam name="T">The type we anticipate this node to be</typeparam>
    public bool IsA<T>() => CheckIsA<T>(here);

    /// <summary>
    /// Checks to see if we can cast the node type to that provided.
    /// </summary>
    /// <remarks>It will fill <see cref="ErrorMessage"/> if there is a fault.</remarks>
    /// <typeparam name="T">The type we anticipate this node to be</typeparam>
    public bool IsA<T>(object node) => CheckIsA<T>(node);

    /// <summary>
    /// See if we have walked to a tree node, being a list of nodes accessed by name
    /// </summary>
    public bool IsNode => here is Node;

    /// <summary>
    /// See if we have walked to an Array node, being a list of nodes accessed by index
    /// </summary>
    public bool IsArray => here is Array;

    /// <summary>
    /// When completely lost, ask for the node type. It could be Node, Array, object, double, long, bool or null
    /// </summary>
    public Type NodeType => here.GetType();

    /// <summary>
    /// If we fail to parse the json, or later fail to retrieve a node by name, this will be set. When there are no errors it will be null.
    /// </summary>
    public string ErrorMessage { get; private set; }

    /// <summary>
    /// True if the last parsing call failed.
    /// </summary>
    public bool Error => ErrorMessage != null;

    #region AccessNodes
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
    public T Get<T>(params object[] path) {
      Walk(path);
      return Here<T>();
    }

    /// <summary>
    /// Use to take a stroll down JSON lane - from the root/start.
    /// </summary>
    /// <remarks>It will fill <see cref="ErrorMessage"/> if there is a fault.</remarks>
    /// <param name="path">
    /// Path to the target node either as a list of objects or a single string with nodes separated by '.'.
    /// Array indexes can be numbers or strings that convert to numbers.
    /// <example>"root", "level1", 0, "level 3"</example>
    /// <example>"root.level1.0.level 3"</example>
    /// </param>
    /// <returns>false if path does not exist</returns>
    public Json Walk(params object[] path) {
      here         = root;
      ErrorMessage = null;
      return WalkOn(path);
    }

    /// <summary>
    /// Use to take a stroll down JSON lane - from where our last call to Walk or WalkOn
    /// </summary>
    /// <remarks>It will fill <see cref="ErrorMessage"/> if there is a fault.</remarks>
    /// <param name="path">
    /// Path to the target node either as a list of objects or a single string with nodes separated by '.'.
    /// Array indexes can be numbers or strings that convert to numbers.
    /// <example>0, "level 3"</example>
    /// <example>"0.level 3"</example>
    /// </param>
    /// <returns>false if path does not exist</returns>
    public Json WalkOn(params object[] path) {
      if ((path.Length == 1) && (path[0] is string)) {
        string[] split = ((string) path[0]).Split('.');
        path = Array.ConvertAll(split, x => (object) x);
      }

      for (int i = 0; i < path.Length; i++) {
        if (!Step(path[i])) break;
      }

      return me;
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
    public bool WalkOn<T>(params object[] path) => WalkOn(path).IsA<T>();
    #endregion

    #region NodeEnumeration
    /// <summary>
    /// Retrieve a count of the number of child nodes from our location. Will return one for nodes that are not tree nodes or arrays.
    /// </summary>
    public int Count => (IsNode)  ? ((Node) here).Count :
                        (IsArray) ? ((object[]) here).Length : 1;

    /// <summary>
    /// Use to set and return to a current location after some operations. Best for enumerations
    /// <code>using json.Anchor { json.Walk("first.one"); }</code>
    /// </summary>
    public Json Anchor {
      get {
        anchor = here;
        return me;
      }
    }

    public struct ChildWalker {
      public Func<bool> More;

      public Action Next;

      public bool IsA<T>() => ValueObject is T;

      public T Value<T>() => (ValueObject is T) ? (T) ValueObject : default(T);

      public string Value() => ValueObject.ToString();

      public string Name;
      public object ValueObject;
    }

    public ChildWalker Children() {
      var node = here as Node;
      if (node != null) return Children(node);

      var array = here as object[];
      if (array != null) return Children(array);

      return OneChild();
    }

    private ChildWalker Children(Node node) {
      var childWalker = new ChildWalker();
      int cursor      = 0;
      int length      = node.Count;

      childWalker.Next = () => {
        var element = node.ElementAt(cursor++);
        childWalker.Name        = element.Key;
        childWalker.ValueObject = element.Value;
      };

      childWalker.More = () => cursor <= length;

      childWalker.Next();

      return childWalker;
    }

    private ChildWalker Children(object[] array) {
      var childWalker = new ChildWalker();
      int cursor      = 0;

      childWalker.Next = () => {
        childWalker.Name        = $"[{cursor}]";
        childWalker.ValueObject = array[cursor++];
      };

      childWalker.More = () => cursor <= array.Length;
      childWalker.Next();
      return childWalker;
    }

    private ChildWalker OneChild() {
      var more = true;

      var childWalker = new ChildWalker {
        Name        = "Here",
        ValueObject = here,
        Next        = () => more = false,
        More        = () => more
      };

      return childWalker;
    }

    /// <inheritdoc />
    public void Dispose() { here = anchor; }

    private object anchor;
    #endregion
    #endregion

    #region SupportData
    private object here;
    private Node   root;

    private string json;
    private int    idx;
    #endregion

    #region AccessSupport
    private bool Step(object next) {
      if (IsNode) return (here as Node)?.ContainsKey(next.ToString()) == true;
      if (!IsArray) return AccessFailure($"Expecting node or array for {next}");
      if (next is int) return ((int) next) >= ((object[]) here).Length;

      int index;

      if (!int.TryParse(next.ToString(), out index)) {
        return AccessFailure($"Expecting array index '{next}'");
      }

      return index >= ((object[]) here).Length;
    }
    #endregion

    #region Parsing
    private bool CheckToken(char token) {
      if (!SkipWhiteSpace()) return false;

      if (json[idx++] != token) {
        ParseError("Expecting token '{0}'", token);
        return false;
      }

      return SkipWhiteSpace();
    }

    private Node ParseToNode(Node node) {
      while (ParseOneEntryToNode(node)) {
        if (!SkipWhiteSpace() || (json[idx] == '}')) {
          idx++;
          break;
        }
      }

      return node;
    }

    private bool ParseOneEntryToNode(Node node) {
      if (!CheckToken('"')) return false;

      string key = ParseString();
      if (!CheckToken(':')) return false;
      if (!SkipWhiteSpace()) return false;

      node.Add(key, ParseObject());
      return idx < json.Length;
    }

    private object ParseObject() {
      char token = json[idx++];

      switch (token) {
        case '{':
          return ParseNode();
        case '[':
          return ParseArray();
        case '"':
          return ParseString();
        default:
          string word = NextWord();

          switch (word) {
            case "true":  return true;
            case "false": return false;
            case "null":  return null;
            default:
              long i;
              if (long.TryParse(word, out i)) return i;

              double d;
              if (double.TryParse(word, out d)) return d;

              ParseError("word '{0}' unknown", word);
              return word;
          }
      }
    }

    private object ParseArray() {
      List<object> list = new List<object>();

      while (SkipWhiteSpace() && (json[idx] != ']')) {
        list.Add(ParseObject());
      }

      idx++;
      return list.ToArray();
    }

    private Node ParseNode() {
      Node node = new Node();
      return ParseToNode(node);
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
    private Json me => Error ? null : this;

    private Json ParseError(string fmt, params object[] args) {
      int    length = Mathf.Min(32, json.Length - idx);
      string part   = (length > 0) ? json.Substring(idx - 1, length) : "";

      ErrorMessage = $"JSON Parsing Error: {string.Format(fmt, args)} - at {idx}, from {part}";

      return null;
    }

    private bool AccessFailure(string message) {
      if (here == null) {
        ErrorMessage = $"No `here` reference: {message}";
      } else {
        List<string> texts = new List<string>();

        for (var child = Children(); child.More(); child.Next()) {
          texts.Append($"{child.Name}={child.Value()}");
        }

        string nodeText = string.Join(separator: ",", values: texts);
        ErrorMessage = $"JSON Access Failure: {message} -  at {here.GetType().Name}  [[{nodeText}]]";
      }

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