// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

using System;
using System.Collections.Generic;

namespace Askowl {
  /// <a href="">Tree Data Storage Container</a>
  public class Trees : IDisposable {
    #region Private Functionality
    #region Nodes
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

      internal struct ArrayNode : IDisposable {
        public List<Node> Value;

        public void Dispose() {
          for (int i = 0; i < Value?.Count; i++) {
            (Value[i] as IDisposable)?.Dispose();
          }
        }
      }

      internal string     Name;
      internal Node       Parent;
      private  LeafNode   leafNode;
      private  BranchNode branchNode;
      private  ArrayNode  arrayNode;

      internal object     Leaf   { get { return leafNode.Value; } set { leafNode.Value = value; } }
      internal Map        Branch => branchNode.Value ?? (branchNode.Value = new Map());
      internal List<Node> Array  => arrayNode.Value  ?? (arrayNode.Value = new List<Node>());

      public static Node New(object name, Node parent) {
        var node = Cache<Node>.Instance;
        node.Name   = name.ToString();
        node.Parent = parent;
        return node;
      }

      public int  Count    => (arrayNode.Value == null) ? 0 : Array.Count;
      public T    As<T>()  => (Leaf is T) ? ((T) Leaf) : default(T);
      public bool IsA<T>() => Leaf is T;

      public Node this[int index] {
        get {
          while (index >= Count) Array.Add(New(index, this));
          return Array[index];
        }
        set {
          while (index >= Count) Array.Add(New(index, this));
          Array[index] = value;
        }
      }

      public bool Has(object key) => Branch[key].Found;

      public Node this[object key] {
        get { return Branch[key].Found ? Branch.As<Node>() : this[key] = New(key, this); }
        set { Branch.Add(key, value); }
      }

      public void Dispose() {
        leafNode.Dispose();
        branchNode.Dispose();
        arrayNode.Dispose();
        leafNode   = new LeafNode();
        branchNode = new BranchNode();
        arrayNode  = new ArrayNode();
        Parent     = null;
      }

      public override string ToString() => Name;
    }
    #endregion

    private Node root, here;

    private struct Anchors : IDisposable {
      public LinkedList<Node> Stack;
      public Trees            tree;
      public void             Dispose() { tree.here = Stack.Pop().Item; }
    }

    private Anchors anchors;

    private Trees() {
      root    = here = Node.New("~ROOT~", null);
      anchors = new Anchors {Stack = new LinkedList<Node>("TreeContainer Anchor Stack"), tree = this};
    }
    #endregion

    #region Public Interface
    /// <a href=""></a>
    public static Trees Instance => new Trees();

    /// <a href=""></a>
    public Trees Root {
      get {
        here = root;
        return this;
      }
    }

    /// <a href=""></a>
    public bool IsRoot => here == root;

    /// <a href=""></a>
    public Trees Parent {
      get {
        if (here.Parent != null) here = here.Parent;
        return this;
      }
    }

    /// <a href=""></a>
    public Trees To(params object[] path) {
      if ((path.Length == 1) && (path[0] is string)) {
        string[] split = ((string) path[0]).Split('.');
        path = Array.ConvertAll(split, x => (object) x);
      }

      if (path.Length == 0) return this;

      for (int i = 0; i < path.Length; i++) {
        if (path[i] is int) {
          here = here[(int) path[i]];
        } else {
          var key = path[i].ToString();

          if (Has(key) || !Compare.IsDigitsOnly(key)) {
            here = here[key];
          } else {
            here = here[int.Parse(key)];
          }
        }
      }

      return this;
    }

    /// <a href="bit.ly/">Has</a>
    public bool Has(object key) => here.Has(key);

    /// <a href=""></a>
    public string Name => here.Name;

    /// <a href=""></a>
    public bool IsA<T>() => here.IsA<T>();

    /// <a href=""></a>
    public object Leaf { get { return here.Leaf; } set { here.Leaf = value; } }

    /// <a href=""></a>
    public T As<T>() => here.As<T>();

    /// <a href=""></a>
    public int Count => here.Count;

    /// <a href=""></a>
    public void Dispose() {
      Root.DisposeHere();
      anchors.Dispose();
    }

    /// <a href=""></a>
    public void DisposeHere() {
      var parent = here.Parent;
      here.Dispose();
      here = parent ?? root;
    }

    /// <a href=""></a>
    public IDisposable Anchor {
      get {
        anchors.Stack.Push(here);
        return anchors;
      }
    }

    /// <inheritdoc />
    public override string ToString() {
      if (here == null) return "No Path";

      tsPath.Clear();
      for (var there = here; there.Parent != null; there = there.Parent) tsPath.Add(there.Name);
      tsPath.Reverse();
      return string.Join(separator: ".", values: tsPath);
    }

    private List<string> tsPath = new List<string>();
    #endregion
  }
}