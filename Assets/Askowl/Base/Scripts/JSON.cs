using UnityEngine;

namespace Askowl {
  using System;
  using System.Collections.Generic;
  using System.Text;
  using System.Text.RegularExpressions;

  public class JSON {
    private class Node : Dictionary<string, object> { };

    private class Raw {
      internal string Json;
    }

    private object here;
    private Node   root;

    private static readonly Node EmptyRoot = new Node();

    public T Here<T>() { return (here is T) ? (T) here : default(T); }

    public bool IsA<T>() { return here is T; }

    public JSON(string json = null) { Reset(json); }

    public void Reset(string json) { root = Parse(json); }

    public T Get<T>(params string[] path) {
      here = root;
      Walk(path);
      return Here<T>();
    }

    public bool Walk(params string[] path) {
      if (path.Length == 1) path = path[0].Split('.');

      for (int i = 0; i < path.Length; i++) {
        if (!Step(path[i])) return false;
      }

      return true;
    }

    private bool Step(string next) {
      if (here is Node) {
        Node node = here as Node;
        if (!node.ContainsKey(next)) return false;

        here = node[next];
      } else if (here is Array) {
        var array = (object[]) here;
        int idx;
        if (!int.TryParse(next, out idx) || (idx >= array.Length)) return false;

        here = array[idx];
      } else {
        return false;
      }

      return true;
    }

    private Node Parse(string json) {
      Node node = new Node();
      int  idx  = 0;

      if (!CheckToken('{', json, ref idx)) return node;

      return ParseToNode(node, json, ref idx);
    }

    private bool CheckToken(char token, string json, ref int idx) {
      if (!SkipWhiteSpace(json, ref idx)) return false;

      if (json[idx++] != token) {
        string part = json.Substring((idx < 10) ? 10 : idx - 1, 100);
        Debug.LogErrorFormat("Bad JSON, expecting '{0}' for {1}", token, part);
        return false;
      }

      return SkipWhiteSpace(json, ref idx);
    }

    private Node ParseToNode(Node node, string json, ref int idx) {
      while (ParseOneEntryToNode(node, json, ref idx)) {
        if (!SkipWhiteSpace(json, ref idx) || (json[idx] == '}')) {
          idx++;
          break;
        }
      }

      return node;
    }

    private bool ParseOneEntryToNode(Node node, string json, ref int idx) {
      if (!CheckToken('"', json, ref idx)) return false;

      string key = ParseString(json, ref idx);

      if (!CheckToken(':', json, ref idx)) return false;
      if (!SkipWhiteSpace(json, ref idx)) return false;

      node.Add(key, ParseObject(json, ref idx));
      return idx < json.Length;
    }

    private object ParseObject(string json, ref int idx) {
      char token = json[idx++];

      switch (token) {
        case '{':
          return ParseNode(json, ref idx);
        case '[':
          return ParseArray(json, ref idx);
        case '"':
          return ParseString(json, ref idx);
        default:
          string word = NextWord(json, ref idx);

          switch (word) {
            case "true":  return true;
            case "false": return false;
            case "null":  return null;
            default:
              double d;
              if (double.TryParse(word, out d)) return d;

              long i;
              if (long.TryParse(word, out i)) return i;

              string part = json.Substring((idx < 10) ? 10 : idx - 1, 100);
              Debug.LogErrorFormat("JSON error, word '{0}' unknown for {1}", word, part);
              return word;
          }
      }
    }

    private object ParseArray(string json, ref int idx) {
      List<object> list = new List<object>();

      while (SkipWhiteSpace(json, ref idx) && (json[idx] != ']')) {
        list.Add(ParseObject(json, ref idx));
      }

      idx++;
      return list.ToArray();
    }

    private Node ParseNode(string json, ref int idx) {
      Node node = new Node();
      return ParseToNode(node, json, ref idx);
    }

    private string ParseString(string json, ref int idx) {
      StringBuilder builder = new StringBuilder();

      while (json[idx] != '"') {
        builder.Append((json[idx] == '\\') ? Escape(json, ref idx) : json[idx++]);
      }

      idx++; // drop closing quote
      return builder.ToString();
    }

    private char Escape(string json, ref int idx) {
      switch (json[++idx]) {
        case 'b': return '\b';
        case 'f': return '\f';
        case 'n': return '\n';
        case 'r': return '\t';
        case 't': return '\t';
        default:  return json[idx];
      }
    }

    private static string NextWord(string json, ref int idx) {
      int first = idx - 1;

      while (!char.IsWhiteSpace(json[idx]) && ("{}[]\",:".IndexOf(json[idx]) == -1)) {
        if (++idx >= json.Length) return null;
      }

      if (idx == first) {
        string part = json.Substring((idx < 10) ? 10 : idx - 1, 100);
        Debug.LogErrorFormat("JSON error, no word at {0}", part);
        idx++;
      }

      return json.Substring(startIndex: first, length: idx - first);
    }

    private static bool IsWhiteSpace(char chr) { return char.IsWhiteSpace(chr) || (chr == ','); }

    private static bool SkipWhiteSpace(string json, ref int idx) {
      while ((idx < json.Length) && IsWhiteSpace(json[idx])) idx++;
      return idx < json.Length;
    }
  }
}