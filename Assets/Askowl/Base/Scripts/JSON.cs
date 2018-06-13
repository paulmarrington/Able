using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Askowl {
  /// <summary>
  /// Parse JSON of unknown format to a dictionary and provide access methods
  /// </summary>
  public class Json {
    #region PublicInterface
    /// <summary>
    /// Constructor that can optionally parse a JSON string
    /// </summary>
    /// <param name="json">String with hopefully correctly formatted JSON</param>
    public Json(string json = null) { Parse(json); }

    /// <summary>
    /// Change the JSON to process on an existing Json instance. Recycling is good.
    /// </summary>
    /// <param name="jsonText">String with hopefully correctly formatted JSON</param>
    /// <returns>false on error and sets <see cref="ErrorMessage"/></returns>
    // ReSharper disable once UnusedMethodReturnValue.Global
    public bool Parse(string jsonText) {
      json         = jsonText;
      idx          = 0;
      ErrorMessage = null;
      root         = new Node();
      if (!CheckToken('{')) return false;

      ParseToNode(root);
      return ErrorMessage != null;
    }

    /// <summary>
    /// Parsing the JSON creates a tree of nodes, arrays and leaf objects. You should not need to get to a Node, but it is here for if I a wrong.
    /// </summary>
    public class Node : Dictionary<string, object> { };

    /// <summary>
    /// Returns the node at the location we have walked to in the tree - if it is of the type expected.
    /// </summary>
    /// <remarks>It will fill <see cref="ErrorMessage"/> if there is a fault.</remarks>
    /// <typeparam name="T">The type we anticipate this node to be</typeparam>
    /// <returns>The requested node in the correct form for (T) or default&lt;T> if it can't be accessed</returns>
    public T Here<T>() { return CheckIsA<T>() ? (T) here : default(T); }

    /// <summary>
    /// Checks to see if we can cast the node type to that provided.
    /// </summary>
    /// <remarks>It will fill <see cref="ErrorMessage"/> if there is a fault.</remarks>
    /// <typeparam name="T">The type we anticipate this node to be</typeparam>
    public bool IsA<T>() { return CheckIsA<T>(); }

    /// <summary>
    /// See if we have walked to a tree node, being a list of nodes accessed by name
    /// </summary>
    public bool IsNode { get { return here is Node; } }

    /// <summary>
    /// See if we have walked to an Array node, being a list of nodes accessed by index
    /// </summary>
    public bool IsArray { get { return here is Array; } }

    /// <summary>
    /// When completely lost, ask for the node type. It could be Node, Array, object, double, long, bool or null
    /// </summary>
    public Type NodeType { get { return here.GetType(); } }

    /// <summary>
    /// If we fail to parse the json, or later fail to retrieve a node by name, this will be set. When there are no errors it will be null.
    /// </summary>
    public string ErrorMessage { get; private set; }

    /// <summary>
    /// Retrieve a count of the number of child nodes from our location. Will return one for nodes that are not tree nodes or arrays.
    /// </summary>
    public int Count {
      get {
        return (IsNode)  ? ((Node) here).Count :
               (IsArray) ? ((object[]) here).Length : 1;
      }
    }

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
    public bool Walk(params object[] path) {
      here         = root;
      ErrorMessage = null;
      return WalkOn(path);
    }

    /// <summary>
    /// Shortcut to walk to a node and check the node is of the type expected.
    /// </summary>
    /// <remarks>It will fill <see cref="ErrorMessage"/> if there is a fault.</remarks>
    /// <param name="path">
    /// Path to the target node either as a list of objects or a single string with nodes separated by '.'.
    /// Array indexes can be numbers or strings that convert to numbers.
    /// <example>"root", "level1", 0, "level 3"</example>
    /// <example>"root.level1.0.level 3"</example>
    /// </param>
    /// <returns>false if path does not exist or the type of the destination node is not that expected</returns>
    public bool Walk<T>(params object[] path) {
      here         = root;
      ErrorMessage = null;
      return WalkOn<T>(path);
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
    public bool WalkOn(params object[] path) {
      if ((path.Length == 1) && (path[0] is string)) {
        string[] split = ((string) path[0]).Split('.');
        path = Array.ConvertAll(split, x => (object) x);
      }

      for (int i = 0; i < path.Length; i++) {
        if (!Stepper(path[i])) return false;
      }

      return true;
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
    public bool WalkOn<T>(params object[] path) { return WalkOn(path) && CheckIsA<T>(); }

    /// <summary>
    /// Returns a list of children. For a tree node, this will a list of keys.
    /// For an array it will be a list of string representations of the elements.
    /// On a path error it will return an empty list and set ErrorMessage
    /// Otherwise it will return a list with one entry - being the string representation of the current node
    /// </summary>
    /// <param name="path">Optional absolute path to the list</param>
    /// <returns>An array of strings as documented in the summary</returns>
    public string[] List(params object[] path) { return List<string>(x => x.ToString(), path); }

    /// <summary>
    /// Returns a list of children. For a tree node, this will a list of keys.
    /// For an array it will be a list elements.
    /// On a path error it will return an empty list and set ErrorMessage
    /// Otherwise it will return a list with one entry - being the current node
    /// </summary>
    /// <param name="path">Optional absolute path to the list</param>
    /// <returns>An array of strings as documented in the summary</returns>
    public T[] List<T>(params object[] path) {
      if ((path.Length > 0) && !Walk(path)) return new T[0];

      ((Node) here).Keys.Cast<object>();
      if (IsNode) return new List<T>(((Node) here).Keys.Cast<T>()).ToArray();
      if (IsArray) return Array.ConvertAll(((object[]) here), x => (T) x);

      return new[] {(T) here};
    }

    /// <summary>
    /// Returns a list of children. For a tree node, this will a list of keys.
    /// For an array it will be a list elements.
    /// On a path error it will return an empty list and set ErrorMessage
    /// Otherwise it will return a list with one entry - being the current node
    /// </summary>
    /// <param name="path">Optional absolute path to the list</param>
    /// <returns>An array of strings as documented in the summary</returns>
    public T[] List<T>(Func<object, T> converter, params object[] path) {
      if ((path.Length > 0) && !Walk(path)) return new T[0];

      ((Node) here).Keys.Cast<object>();
      if (IsNode) return new List<T>(((Node) here).Keys.Cast<T>()).ToArray();
      if (IsArray) return Array.ConvertAll(((object[]) here), converter);

      return new[] {(T) here};
    }
    #endregion
    #endregion

    #region SupportData
    private object here;
    private Node   root;

    private string json;
    private int    idx;
    #endregion

    #region AccessSupport
    private bool Stepper(object next) {
      if (IsNode) return Step(next.ToString());
      if (!IsArray) return AccessFailure("Expecting array for {0}", next);
      if (next is int) return Step((int) next);

      int index;

      if (!int.TryParse(next.ToString(), out index)) {
        return AccessFailure("Expecting array index '{0}'", next);
      }

      return Step(index);
    }

    private bool Step(int index) {
      var array = (object[]) here;

      if (index >= array.Length) {
        return AccessFailure("Array index {0} out of bounds for {1}", index, array.Length);
      }

      here = array[index];
      return true;
    }

    private bool Step(string next) {
      Node node = here as Node;

      // ReSharper disable once PossibleNullReferenceException
      if (!node.ContainsKey(next)) {
        string nodes = string.Join(", ", List());
        return AccessFailure("No node '{0}' in '{1}'", next, nodes);
      }

      here = node[next];
      return true;
    }
    #endregion

    #region Parsing
    private bool CheckToken(char token) {
      if (!SkipWhiteSpace()) return false;

      if (json[idx++] != token) {
        ParseError("Expecting token '{0}", token);
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
              double d;
              if (double.TryParse(word, out d)) return d;

              long i;
              if (long.TryParse(word, out i)) return i;

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

    private static bool IsWhiteSpace(char chr) { return char.IsWhiteSpace(chr) || (chr == ','); }

    private bool SkipWhiteSpace() {
      while ((idx < json.Length) && IsWhiteSpace(json[idx])) idx++;
      return idx < json.Length;
    }
    #endregion

    #region ErrorProcessing
    private void ParseError(string fmt, params object[] args) {
      string part = json.Substring((idx < 10) ? 10 : idx - 1, 32);

      ErrorMessage = string.Format("JSON Parsing Error: {0} - at {1}, from {2}",
                                   string.Format(fmt, args), idx, part);
    }

    private bool AccessFailure(string fmt, params object[] args) {
      ErrorMessage = string.Format("JSON Access Failure: {0} -  at {1}",
                                   string.Format(fmt, args), here.GetType().Name);

      return false;
    }

    private bool CheckIsA<T>() {
      if (IsA<T>()) return true;
      if ((typeof(T) == typeof(int))    && IsA<long>()) return true;
      if ((typeof(T) == typeof(float))  && IsA<double>()) return true;
      if ((typeof(T) == typeof(float))  && IsA<long>()) return true;
      if ((typeof(T) == typeof(double)) && IsA<long>()) return true;

      return AccessFailure("Expecting type {0}", typeof(T).Name);
    }
    #endregion
  }
}