using UnityEngine;

namespace Askowl {
  using System;
  using System.Collections.Generic;
  using System.Text;
  using System.Text.RegularExpressions;

  public class JSON {
    private class Node : Dictionary<string, object> { };

    private class Raw {
      private string json;
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
      try {
        Node t = JsonUtility.FromJson<Node>(json);

        Debug.LogWarningFormat("**** JSON:58 t={0}  #### DELETE-ME #### 12/6/18 1:18 PM",
                               t.Count); //#DM#//

        return JsonUtility.FromJson<Node>(json);
      } catch {
        return EmptyRoot;
      }
    }
  }
}