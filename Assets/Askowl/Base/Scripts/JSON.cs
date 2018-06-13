using System;
using System.Collections.Generic;
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

    public bool Parse(string jsonText) {
      json         = jsonText;
      idx          = 0;
      ErrorMessage = null;
      root         = new Node();
      if (!CheckToken('{')) return false;

      ParseToNode(root);
      return ErrorMessage != null;
    }

    public class Node : Dictionary<string, object> { };

    public T Here<T>() { return CheckIsA<T>() ? (T) here : default(T); }

    public bool IsA<T>() { return here is T; }

    public bool IsNode { get { return here is Node; } }

    public bool IsArray { get { return here is Array; } }

    public string ErrorMessage { get; private set; }

    public int Count {
      get {
        return (IsNode)  ? ((Node) here).Count :
               (IsArray) ? ((object[]) here).Length : 1;
      }
    }

    #region AccessNodes
    public T Get<T>(params object[] path) {
      Walk(path);
      return Here<T>();
    }

    public bool Walk(params object[] path) {
      here = root;
      return WalkOn(path);
    }

    public bool Walk<T>(params object[] path) {
      here = root;
      return WalkOn<T>(path);
    }

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

    public bool WalkOn<T>(params object[] path) { return WalkOn(path) && CheckIsA<T>(); }

    public string[] List(params string[] path) {
      if ((path.Length > 0) && !Walk(path)) return new string[0];

      if (IsNode) return (new List<string>(((Node) here).Keys)).ToArray();
      if (IsArray) return Array.ConvertAll(((object[]) here), x => x.ToString());

      return new string[] {here.ToString()};
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
      if (next is int?) return Step((int) next);

      int idx;

      if (!int.TryParse(next.ToString(), out idx)) {
        return AccessFailure("Expecting array index '{0}'", next);
      }

      return Step(idx);
    }

    private bool Step(int idx) {
      var array = (object[]) here;

      if (idx >= array.Length) {
        return AccessFailure("Array index {0} out of bounds for {1}", idx, array.Length);
      }

      here = array[idx];
      return true;
    }

    private bool Step(string next) {
      Node node = here as Node;

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

      return AccessFailure("Expecting type {0}", typeof(T).Name);
    }
    #endregion
  }
}