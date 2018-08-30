// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Askowl {
  /// <a href="">Tree Data Storage Container</a>
  public class TreeContainer : IDisposable {
    #region Private Functionality
    internal struct Node : IDisposable { //, IComparable {
      internal interface IValue : IDisposable, IComparable { }

      internal struct LeafNode : IValue {
        public object Value;
        public void   Dispose()               { (Value as IDisposable)?.Dispose(); }
        public int    CompareTo(object other) => Compare(this, other);
      }

      internal struct BranchNode : Map, IValue {
        public Map Value;
        public void Dispose() { Value.Dispose(); }
        public int    CompareTo(object other)  => Compare(this, other);
        public Node   Add(Node         node)   => Value.Add(node.Name, node).As<Node>();
      }

      internal struct ArrayNode : IValue {
        public List<Node> Value;

        public void Dispose() {
          for (int i = 0; i < Value.Count; i++) {
            (Value[i] as IDisposable)?.Dispose();
          }
        }

        public int CompareTo(object other) {
          var otherArray = other as object[];
          if (otherArray == null) return 1;

          for (int i = 0; i < Value.Count; i++) {
            if (i >= otherArray.Length) return 1;

            var result = Compare(Value[i], otherArray[i]);
            if (result != 0) return result;
          }

          return (otherArray.Length > List.Count) ? -1 : 0;
        }

        public Node Get(int    index) => (index < 0) || (index > List.Count) ? IsError : List[index];
        public Node Get(string key)   => IsError;

        public Node Add(Node node) {
          List.Add(node);
          return node;
        }

        public T Get<T>() where T : class => List as T;
      }

      internal string     Name;
      internal object     Parent;
      internal LeafNode   Leaf;
      internal BranchNode Branch;
      internal ArrayNode  Array;
      internal string     ErrorMessage;

      public bool Error                   => ErrorMessage != null;
      public void Dispose()               => Value.Dispose();
      public int  CompareTo(object other) => Value.CompareTo(other);

      public bool HasError => (Value == null) || ((Name == null) && (Parent == null) && (Value == null));
    }

    private static int Compare(object left, object right) =>
      (left as IComparable)?.CompareTo(right) ?? Comparer<object>.Default.Compare(left, right);
    #endregion

    private Node root = new Node(), here;

    #region Private Methods
    private bool Stepped(Node there, string message) {
      if (there.HasError) return AccessFailure($"Node '{message}' in '{here.Name}' not found.");

      here = there;
      return true;
    }

    private bool StepIn(int index) => Stepped(here.Value.Get(index), index.ToString());

    private bool StepIn(object step) {
      string key = step.ToString();

      if (Askowl.Compare.isDigitsOnly(key)) return StepIn(int.Parse(key));

      return Stepped(here.Value.Get(key), key);
    }

    private TreeContainer() {
      anchors = new Anchors {Stack = new LinkedList<Node>("TreeContainer Anchor Stack"), tree = this};
    }

    private TreeContainer Add(string name, Node.IValue value) {
      var node = new Node {Name = name, Parent = here, Value = value};
      here.Value.Add(node);
      here = node;
      return this;
    }
    #endregion

    #region Public Interface
    /// <a href=""></a>
    public TreeContainer AddLeaf(string name, object value) => Add(name, new Node.Leaf {Value = value});

    /// <a href=""></a>
    public TreeContainer AddBranch(string name) => Add(name, new Node.Branch {Value = new Map()});

    /// <a href=""></a>
    public TreeContainer AddArray(string name) => Add(name, new Node.Leaf {Value = new List<Node>()});

    /// <a href=""></a>
    public TreeContainer Root() {
      ErrorMessage = null;
      here         = root;
      return this;
    }

    /// <a href=""></a>
    public bool IsRoot => here.Value == root.Value;

    /// <a href=""></a>
    public TreeContainer Parent() {
      if (here.Parent != null) here = (Node) here.Parent;
      return this;
    }

    /// <a href=""></a>
    public TreeContainer To(params object[] path) {
      if ((path.Length == 1) && (path[0] is string)) {
        string[] split = ((string) path[0]).Split('.');
        path = Array.ConvertAll(split, x => (object) x);
      }

      if (path.Length == 0) return this;

      for (int i = 0; i < path.Length; i++) {
        if (!StepIn(path[i])) break;
      }

      return this;
    }

    /// <a href=""></a>
    public string Name => here.Name;

    /// <a href=""></a>
    public bool IsA<T>() => here.Value.Value is T;

    /// <a href=""></a>
    public T Leaf<T>() => (here.Value.Value is T) ? (T) here.Value.Value : default(T);

    /// <a href=""></a>
    public void Dispose() {
      Root().DisposeHere();
      anchors.Dispose();
    }

    /// <a href=""></a>
    public void DisposeHere() {
      var children = here.Item.Children;

      for (int i = 0; i < here.Item.NumberOfChildren; i++) children[i].Dispose();

      if (here == root) {
//        root.Item.Children = null;
//        root.Item.Leaf     = null;
      } else {
        var parent = here.Item.Parent;
        here.Dispose();
        here = parent;
      }
    }

    /// <summary>
    /// Use to set and return to a current location after some operations. Best for enumerations
    /// <code>using json.Anchor { json.Walk("first.one"); }</code>
    /// </summary>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#Tree Anchor">Mark Current Node Location</a></remarks>
    public IDisposable Anchor {
      get {
        anchors.Stack.Push(here);
        return anchors;
      }
    }

    private struct Anchors : IDisposable {
      public LinkedList<Node> Stack;
      public TreeContainer    tree;
      public void             Dispose() { tree.here = Stack.Pop().Item; }
    }

    private Anchors anchors;
    #endregion

    #region Error Processing
    /// <inheritdoc />
    public override string ToString() {
      stringBuilder.Clear();
      HereToString(indent: "");
      return stringBuilder.ToString();
    }

    private StringBuilder stringBuilder = new StringBuilder();

    /// <remarks><a href="http://unitydoc.marrington.net/Able#tree-error-processing">Error Indication and Message</a></remarks>
    public string ErrorMessage { get; set; }

    /// <summary>
    /// True if the last parsing call failed.
    /// </summary>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#tree-error-processing">Error Indication and Message</a></remarks>
    public bool Error => ErrorMessage != null;

    private bool AccessFailure(string message) {
      if (here == null) {
        ErrorMessage = $"No `here` reference: {message}";
      } else {
        stringBuilder.Clear();
        ForEach(() => stringBuilder.Append($"{here.Item.Name}={here.Item}, "));
        string nodeText = stringBuilder.Remove(stringBuilder.Length - 2, 2).ToString();
        ErrorMessage = $"JSON Access Failure: {message} -  at {here.GetType().Name}  [[{nodeText}]]";
      }

      return false;
    }

//    private char[] charsToTrim = {',', ' '};

    // ReSharper disable once UnusedParameter.Local
    private void HereToString(string indent) {
      stringBuilder.Append(Name).Append(":");
      if (here.Item.Leaf != null) stringBuilder.Append(" ").Append(here.Item.Leaf);
      stringBuilder.AppendLine();

      ForEach(() => HereToString($"{indent}  "));
    }
    #endregion
  }
}