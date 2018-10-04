// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

namespace Askowl {
  using System;
  using System.Text;

  /// <a href="http://bit.ly/2NX3NjD">Deserialisation from JSON format</a>
  public class Json : IDisposable {
    #region PublicInterface
    /// <a href="http://bit.ly/2NX3NjD">Retrieve a Json instance used to load data into</a>
    public static Json Instance => new Json();

    private Json() { }

    /// <a href="http://bit.ly/2Oo6eer">Parse text containing valid but unknown format JSON</a>
    public Json Parse(string jsonText) {
      tree = Trees.Instance;
      json = jsonText;
      idx  = 0;

      if (!SkipWhiteSpace()) return this;

      if (json[idx] == '{') { ParseToNode(); }
      else { ParseOneEntryToNode(); }

      return this;
    }

    /// <a href="http://bit.ly/2NX3NjD">Clear deserialised JSON tree</a> <inheritdoc />
    public void Dispose() { tree.Dispose(); }

    /// <a href="http://bit.ly/2NX3NjD">Retrieve the current node in the parsed JSON tree</a>
    public Trees Node => tree;
    #endregion

    #region SupportData
    private Trees tree;

    private string json = "";
    private int    idx;

    private StringBuilder builder = new StringBuilder();
    #endregion

    #region Parsing
    private void ParseToNode() {
      while (ParseOneEntryToNode()) {
        if (!SkipWhiteSpace() || (json[idx] == '}')) {
          idx++;
          break;
        }
      }
    }

    private bool ParseOneEntryToNode() {
      if (!SkipWhiteSpace()) return false;

      string key = json[idx++] == '"' ? ParseString() : NextWord(-1);
      if (json[idx] == ':') idx++;

      using (tree.Anchor()) {
        tree.Add(key);
        ParseObject();
      }

      return idx < json.Length;
    }

    private void ParseObject() {
      if (!SkipWhiteSpace()) return;

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
          tree.Leaf = NextWord(-1);
          break;
      }

      SkipWhiteSpace();
    }

    private void ParseArray() {
      var index = 0;

      while (SkipWhiteSpace() && (json[idx] != ']')) {
        using (tree.Anchor()) {
          tree.Add(index++.ToString());
          ParseObject();
        }
      }

      idx++; // skip ']'
    }

    private string ParseString() {
      builder.Clear();

      while ((idx < json.Length) && (json[idx] != '"')) builder.Append(json[idx] == '\\' ? Escape() : json[idx++]);

      idx++; // drop closing quote
      SkipWhiteSpace();
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

    private string NextWord(int offset = 0) {
      int first = idx += offset;
      while ((idx < json.Length) && ("{}[]\",:".IndexOf(json[idx]) == -1)) idx++;
      string word = json.Substring(startIndex: first, length: idx - first);
      SkipWhiteSpace();
      return word.Trim();
    }

    private static bool IsWhiteSpace(char chr) => char.IsWhiteSpace(chr) || (chr == ',');

    private bool SkipWhiteSpace() {
      while ((idx < json.Length) && IsWhiteSpace(json[idx])) idx++;
      return idx < json.Length;
    }
    #endregion
  }
}