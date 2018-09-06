// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

using System;
using System.Collections.Generic;

namespace Askowl {
  /// <a href="">Tree Data Storage Container</a>
  public class Trees : IDisposable {
    #region Private Functionality
    // ReSharper disable once ClassNeverInstantiated.Global
    internal class Node : IDisposable {
      private Node() { }

      internal struct LeafNode : IDisposable {
        public object Value;
        public void   Dispose() { (Value as IDisposable)?.Dispose(); }
      }

      internal struct BranchNode : IDisposable {
        public Map  Value;
        public void Dispose() { Value?.Dispose(); }
      }

      internal string     Name;
      internal Node       Parent;
      private  LeafNode   leafNode;
      private  BranchNode branchNode;

      internal object Leaf   { get { return leafNode.Value; } set { leafNode.Value = value; } }
      internal Map    Branch => branchNode.Value ?? (branchNode.Value = new Map());

      public static Node New(object name, Node parent) {
        var node = Cache<Node>.Instance;
        node.Name   = name.ToString();
        node.Parent = parent;
        return node;
      }

      public int Count => (branchNode.Value == null) ? 0 : Branch.Count;

      public void Dispose() {
        leafNode.Dispose();
        branchNode.Dispose();
        leafNode   = new LeafNode();
        branchNode = new BranchNode();
        Parent     = null;
        Name       = null;
      }

      public override string ToString() => Name;
    }

    private readonly Node   root;
    private          Node   here;
    private          long   integer;
    private          double floatingPoint;
    private          int    isNumber;

    private struct Anchors : IDisposable {
      public LinkedList<Node> Stack;
      public Trees            tree;

      public void Dispose() { tree.here = Stack.Pop().Item; }

      static Anchors() { LinkedList<Node>.DeactivateItemStatic = (node) => { }; }
    }

    private Anchors anchors;

    private Trees() {
      root    = here = Node.New("~ROOT~", null);
      anchors = new Anchors {Stack = new LinkedList<Node>("Trees Anchor Stack"), tree = this};
    }

    /// <a href=""></a>
    private Trees Walk(bool create, params object[] path) {
      Failed   = false;
      isNumber = -1;
      if (path.Length == 0) return this;

      if ((path.Length == 1) && (path[0] is string)) {
        string[] split = ((string) path[0]).Split('.');
        path = Array.ConvertAll(split, x => (object) x);
      }

      for (int i = 0; i < path.Length; i++) {
        if (here == null) return Failure();

        if (here.Branch[path[i]].Found) {
          here = here.Branch.Value as Node;
        } else if (path[i] is int) {
          object obj = path[i];
          here = here.Branch[obj]?.Value as Node ?? (Node) here.Branch.Add(obj, Node.New(obj, here)).Value;
        } else {
          var key = path[i].ToString();

          if (string.IsNullOrWhiteSpace(key)) {
            here = (i == 0) ? here : here.Parent;
          } else if (Compare.IsDigitsOnly(key)) {
            here = here.Branch[int.Parse(key) as object].Value as Node;
          } else if (create) {
            here = (Node) here.Branch.Add(path[i], Node.New(path[i], here)).Value;
          } else {
            return Failure();
          }
        }
      }

      return this;
    }

    private Trees Failure() {
      Failed = true;
      return this;
    }
    #endregion

    #region Public Interface
    /// <a href=""></a>
    public static Trees Instance => new Trees();

    /// <a href=""></a>
    public bool Failed { get; private set; }

    /// <a href=""></a>
    public bool IsRoot => here == root;

    /// <a href=""></a>
    public Trees Root() {
      here = root;
      return this;
    }

    /// <a href=""></a>
    public Trees To(params object[] path) => Root().Walk(create: false, path: path);

    /// <a href=""></a>
    public Trees Next(params object[] path) => Walk(create: false, path: path);

    /// <a href="bit.ly/">Has</a>
    public Trees Add(params object[] path) => Walk(create: true, path: path);

    /// <a href=""></a>
    public string Name => here.Name;

    /// <a href=""></a>
    public object Leaf {
      get { return here.Leaf; }
      set {
        here.Leaf = value;
        isNumber  = -1;
      }
    }

    /// <a href=""></a>
    public string Value { get { return (string) here.Leaf; } set { here.Leaf = value; } }

    /// <a href="bit.ly/">IsNumber</a>
    public bool IsNumber {
      get {
        if (isNumber != -1) return isNumber != 0;

        var word = Value;

        if (long.TryParse(word, out integer)) {
          floatingPoint = integer;
        } else if (double.TryParse(word, out floatingPoint)) {
          integer = (long) floatingPoint;
        } else {
          Failed = true;
          return (isNumber = 0) == 1;
        }

        Failed = false;
        return (isNumber = 1) == 1;
      }
    }

    /// <a href="bit.ly/">Value</a>
    public long Long => IsNumber ? integer : 0;

    /// <a href="bit.ly/">Value</a>
    public double Double => IsNumber ? floatingPoint : 0;

    /// <a href="bit.ly/">Value</a>
    public bool Boolean => Value.ToLower()[0] == 't';

    /// <a href="bit.ly/">Value</a>
    public bool IsNull => (Value == null) || (Value.ToLower() == "null");

    /// <a href=""></a>
    public object this[object key] {
      get { return (here.Branch[key].Value as Node)?.Leaf; }
      set { ((Node) here.Branch[key].Value).Leaf = value; }
    }

    /// <a href="bit.ly/">Keys</a>
    public object[] Children => here.Branch.Keys;

    /// <a href="bit.ly/">First</a>
    public object FirstChild => here.Branch.First;

    /// <a href="bit.ly/">First</a>
    public object NextChild => here.Branch.Next;

    /// <a href=""></a>
    public void Dispose() {
      var parent = here.Parent;
      var key    = here.Name;
      here.Dispose();
      here = parent ?? root;
      here.Branch.Remove(key);
    }

    /// <a href=""></a>
    public IDisposable Anchor(params object[] path) {
      anchors.Stack.Push(here);

      Next(path);
      return anchors;
    }

    /// <inheritdoc />
    public override string ToString() => Leaf.ToString();

    /// <a href=""></a>
    public string Key {
      get {
        if (here == null) return "No Path";

        tsPath.Clear();
        for (var there = here; there.Parent != null; there = there.Parent) tsPath.Add(there.Name);
        tsPath.Reverse();
        return string.Join(separator: ".", values: tsPath);
      }
    }

    private List<string> tsPath = new List<string>();
    #endregion
  }
}