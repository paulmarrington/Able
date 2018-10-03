// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

namespace Askowl {
  using System;
  using System.Collections.Generic;

  /// <a href="http://bit.ly/2Oq9Axs">Tree Container</a>
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

      internal object Leaf   { get => leafNode.Value; set => leafNode.Value = value; }
      internal Map    Branch => branchNode.Value ?? (branchNode.Value = new Map());

      public static Node New(object name, Node parent) {
        var node = Cache<Node>.Instance;
        node.Name   = name.ToString();
        node.Parent = parent;
        return node;
      }

      public int Count => branchNode.Value == null ? 0 : Branch.Count;

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
      public Trees            Tree;

      public void Dispose() { Tree.here = Stack.Pop().Item; }

      static Anchors() { LinkedList<Node>.DeactivateItemStatic = (node) => { }; }
    }

    private Anchors anchors;

    private Trees() {
      root    = here = Node.New("~ROOT~", null);
      anchors = new Anchors { Stack = new LinkedList<Node>("Trees Anchor Stack"), Tree = this };
    }

    private Trees Walk(bool create, string path) {
      Failed   = false;
      isNumber = -1;
      var split = path.Split('.');

      for (var i = 0; i < split.Length; i++) {
        string key = split[i];
        if (here == null) return Failure();

        if (here.Branch[key].Found) { here = here.Branch.Value as Node; }
        else {
          if (string.IsNullOrWhiteSpace(key)) { here = i == 0 ? here : here.Parent; }
          else if (create) { here                    = (Node) here.Branch.Add(key, Node.New(key, here)).Value; }
          else { return Failure(); }
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
    /// <a href="http://bit.ly/2Rj0Qbc"></a>
    public static Trees Instance => Cache<Trees>.Instance;

    /// <a href="http://bit.ly/2Oq9u94"></a>
    public bool Failed { get; private set; }

    /// <a href="http://bit.ly/2RezGSC"></a>
    public bool IsRoot => here == root;

    /// <a href="http://bit.ly/2RezGSC"></a>
    public Trees Root() {
      here = root;
      return this;
    }

    /// <a href="http://bit.ly/2RezJxM"></a>
    public Trees To(string path) => Root().Walk(create: false, path: path);

    /// <a href="http://bit.ly/2OuLhhP"></a>
    // ReSharper disable once UnusedMethodReturnValue.Global
    public Trees Next(string path) => Walk(create: false, path: path);

    /// <a href="http://bit.ly/2Oq9Axs">Has</a>
    // ReSharper disable once UnusedMethodReturnValue.Global
    public Trees Add(string path) => Walk(create: true, path: path);

    /// <a href="http://bit.ly/2Oq9u94"></a>
    public string Name => here.Name;

    /// <a href="http://bit.ly/2NTfgR0"></a>
    public object Leaf {
      get => here.Leaf;
      set {
        here.Leaf = value;
        isNumber  = -1;
      }
    }
    /// <a href="http://bit.ly/2NTfgR0"></a>
    public string Value { get => (string) here.Leaf; set => here.Leaf = value; }

    /// <a href="http://bit.ly/2NTfgR0"></a>
    public bool IsNumber {
      get {
        if (isNumber != -1) return isNumber != 0;

        string word = Value;

        if (long.TryParse(word, out integer)) { floatingPoint = integer; }
        else if (double.TryParse(
          word, out floatingPoint)) { integer = (long) floatingPoint; }
        else {
          Failed = true;
          return (isNumber = 0) == 1;
        }

        Failed = false;
        return (isNumber = 1) == 1;
      }
    }

    /// <a href="http://bit.ly/2NTfgR0"></a>
    public long Long => IsNumber ? integer : 0;

    /// <a href="http://bit.ly/2NTfgR0"></a>
    public double Double => IsNumber ? floatingPoint : 0;

    /// <a href="http://bit.ly/2NTfgR0"></a>
    public bool Boolean => Value.ToLower()[0] == 't';

    /// <a href="http://bit.ly/2NTfgR0"></a>
    public bool IsNull => (Value == null) || (Value.ToLower() == "null");

    /// <a href="http://bit.ly/2NTfgR0"></a>
    public object this[object key] {
      get => (here.Branch[key].Value as Node)?.Leaf;
      set => ((Node) here.Branch[key].Value).Leaf = value;
    }

    /// <a href="http://bit.ly/2Oq9u94">Keys</a>
    public object[] Children => here.Branch.Keys;

    /// <a href="http://bit.ly/2Oq9u94">First</a>
    public object FirstChild => here.Branch.First;

    /// <a href="http://bit.ly/2Oq9u94">First</a>
    public object NextChild => here.Branch.Next;

    /// <a href="http://bit.ly/2RezGlA"></a>
    public void Dispose() {
      var    parent = here.Parent;
      string key    = here.Name;
      here.Dispose();
      here = parent ?? root;
      here.Branch.Remove(key);
    }

    /// <a href="http://bit.ly/2NYOE0W"></a>
    public IDisposable Anchor(string path = "") {
      anchors.Stack.Push(here);
      Next(path);
      return anchors;
    }

    /// <inheritdoc />
    public override string ToString() => Leaf.ToString();

    /// <a href="http://bit.ly/2Rj0Vf0"></a>
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