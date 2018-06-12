using UnityEngine;

namespace Askowl {
  using System;
  using System.Collections.Generic;
  using System.Text;
  using System.Text.RegularExpressions;

  public class JSON {
    public class Node : Dictionary<string, object> { };

    private Node dict = new Node();

    private string text;
    private object here;

    public T Here<T>() { return (here is T) ? (T) here : default(T); }

    public JSON(string json = null) { Reset(json); }

    public void Reset(string json) {
      dict.Clear();
      text = json;
      here = dict;
    }

    public T Get<T>(params string[] path) {
      here = dict;
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
      if (here is string) {
        here = JsonUtility.FromJson<Node>((string) here);
      }

      if (!(here is Node)) return false;

      Node node = (Node) here;
      if (!node.ContainsKey(next)) return false;

      here = node[next];
      return true;
    }
  }
}