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

    public JSON(string json = null) { Reset(json); }

    public void Reset(string json) { root = ParseChildren(json) ?? EmptyRoot; }

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

    private Node Step() {
      if (here is string) {
        Node child = JsonUtility.FromJson<Node>((string) here);
      }

      return (here as Node) ?? EmptyRoot;
    }

    private bool Step(string next) {
      Node node = Step();
      if (!node.ContainsKey(next)) return false;

      here = node[next];
      return true;
    }

    private Node ParseChildren(string json) {
      Node node = new Node();
      int  idx  = 0;

      if (!CheckToken('{', json, ref idx)) return node;

      while (ParseToNode(node, json, ref idx)) {
        if (!SkipWhiteSpace(json, ref idx) || (json[idx++] == '}')) break;
      }

      return node;
    }

    private bool CheckToken(char token, string json, ref int idx) {
      if (!SkipWhiteSpace(json, ref idx)) return false;

      if (json[idx++] != token) {
        Debug.LogErrorFormat("Bad JSON, expecting '{token}' at {idx} for {json}");
        return false;
      }

      return SkipWhiteSpace(json, ref idx);
    }

    private bool ParseToNode(Node node, string json, ref int idx) {
      string key = ParseString(json, ref idx);
    }

    private string ParseString(string json, ref int idx) {
      StringBuilder builder = new StringBuilder();
      if (!CheckToken('"', json, ref idx)) return null;

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

    private static bool ParseToken(string json, ref int idx) {
      switch (json[idx++]) {
        case '{': break;
        case '}': break;
        case '[': break;
        case ']': break;
        case '"': break;
        case ',': break;
        case ':': break;
        default:
          string word = NextWord(json, ref idx);

          if (char.IsDigit(word[0])) { }
          break;
      }

      return idx >= json.Length;
    }

    private static string NextWord(string json, ref int idx) {
      int first = idx - 1;

      while (!char.IsWhiteSpace(json[idx]) && ("{}[]\",:".IndexOf(json[idx]) == -1)) {
        if (++idx >= json.Length) return null;
      }

      return json.Substring(startIndex: first, length: idx - first);
    }

    private static bool SkipWhiteSpace(string json, ref int idx) {
      while ((idx < json.Length) && !char.IsWhiteSpace(json[idx])) idx++;
      return idx < json.Length;
    }
  }
}