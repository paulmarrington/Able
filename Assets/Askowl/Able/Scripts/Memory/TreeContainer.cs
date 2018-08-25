// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

using System;
using System.Text;

namespace Askowl {
  /// <inheritdoc cref="Cached{T}" />
  /// <remarks><a href="http://unitydoc.marrington.net/Able#tree">Non-binary Tree</a></remarks>
  public class TreeContainer : Cached<TreeContainer> {
    #region Private Data
    private class Node : LinkedList<Branch>.Node { }

    private struct Branch : IDisposable, IComparable {
      internal string Name;
      internal object Leaf;
      internal Node   Parent;
      internal Node[] Children;
      internal int    NumberOfChildren;

      public void Dispose() {
        (Leaf as IDisposable)?.Dispose();

        while (NumberOfChildren > 0) {
          Children[NumberOfChildren--].Dispose();
        }
      }

      public int CompareTo(object obj) =>
        obj is Branch ? String.Compare(Name, ((Branch) obj).Name, StringComparison.Ordinal) : 0;
    }

    private static LinkedList<Branch> branches = new LinkedList<Branch>("TreeContainer Branches");

    private Node root = (Node) branches.Add(new Branch {Name = "Root"});
    private Node here;

    private bool StepIn(int index) {
      if (Error || (here.Item.Children == null) || (index >= here.Item.Children.Length)) {
        return AccessFailure($"Node '{index}' in '{here.Item.Name}' not found.");
      }

      here = here.Item.Children[index];
      return true;
    }

    private bool StepIn(object step) {
      if (step is int) return StepIn((int) step);

      string key = step.ToString();

      if (Compare.isDigitsOnly(key)) return StepIn(int.Parse(key));

      int index = Array.BinarySearch(array: here.Item.Children, value: key);

      if (index >= 0) StepIn(index);

      index = ~index;
      var closest = (index < here.Item.Children.Length) ? $" Closest is '{here.Item.Children[index]}'." : "";
      AccessFailure($"Node named '{key}' in '{here.Item.Name}' not found.{closest}");
      return false;
    }

    private void AddChild(Node parentNode, Node childNode) {
      var parent                                   = parentNode.Item;
      if (parent.Children == null) parent.Children = new Node[4];

      if (parent.NumberOfChildren >= parent.Children.Length) {
        Node[] newChildren = new Node[parent.Children.Length * 2];
        Array.Copy(parent.Children, newChildren, parent.Children.Length);
        parent.Children = newChildren;
      }

      parent.Children[parent.NumberOfChildren++] = childNode;
      Array.Sort(parent.Children);
    }

    private TreeContainer() {
      anchors = new Anchors {Stack = new LinkedList<Node>("TreeContainer Anchor Stack"), tree = this};
    }

    static TreeContainer() {
//      DeactivateItem = (tree) => tree.Root(); // so all the branches are disposed of correctly //#TBD#
//      ReactivateItem = (tree) => tree.anchors.tree = tree;
    }
    #endregion

    #region Public Interface
    /// <a href=""></a>
    public TreeContainer Add(string name) {
      var node = (Node) branches.Add(new Branch {Name = name, Leaf = null, Parent = here, Children = null});
      AddChild(here, node);
      here = node;
      return this;
    }

    /// <a href=""></a>
    public TreeContainer AddAnonymous() => Add(null);

    /// <a href=""></a>
    public TreeContainer Root() {
      ErrorMessage = null;
      here         = root;
      return this;
    }

    /// <a href=""></a>
    public bool IsRoot => here == root;

    /// <a href=""></a>
    public TreeContainer Parent() {
      if (here.Item.Parent != null) here = here.Item.Parent;
      return this;
    }

    /// <param name="path">
    /// Path to the target node either as a list of objects or a single string with nodes separated by '.'.
    /// Array indexes can be numbers or strings that convert to numbers.
    /// <example>0, "level 3"</example>
    /// <example>"0.level 3"</example>
    /// </param>
    /// <returns>false if path does not exist</returns>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#tree-traversal">Traversing the Tree</a></remarks>
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
    public string Name => here.Item.Name;

    /// <inheritdoc />
    public override string ToString() {
      stringBuilder.Clear();
      HereToString(indent: "");
      return stringBuilder.ToString();
    }

    private StringBuilder stringBuilder = new StringBuilder();

    /// <a href=""></a>
    public TreeContainer Leaf<T>(T setTo) {
      here.Item.Leaf = setTo;
      return this;
    }

    /// <a href=""></a>
    public TreeContainer Leaf(object setTo) {
      here.Item.Leaf = setTo;
      return this;
    }

    /// <a href=""></a>
    public bool HasLeaf => here.Item.Leaf != null;

    /// <a href=""></a>
    public bool IsA<T>() => here.Item.Leaf is T;

    /// <a href=""></a>
    public T Leaf<T>() => (here.Item.Leaf is T) ? (T) here.Item.Leaf : default(T);

    /// <a href=""></a>
    public object Leaf() => here.Item.Leaf;

    /// <a href=""></a>
    public bool HasAnonymousChildren => (here.Item.NumberOfChildren > 0) && (here.Item.Children[0].Item.Name == null);

    /// <a href=""></a>
    public int ChildCount => here.Item.NumberOfChildren;

    /// <summary>
    /// After calling `action` for each child branch it will return here to the parent.
    /// No need for an explicit `using(Anchor)`
    /// </summary>
    public TreeContainer ForEach(Action action) {
      using (Anchor) {
        var parent = here.Item;

        for (int i = 0; i < parent.NumberOfChildren; i++) {
          here = parent.Children[i];
          action();
        }
      }

      return this;
    }

    /// <a href=""></a>
    public override void Dispose() { Root().DisposeHere(); }

    /// <a href=""></a>
    public void DisposeHere() {
      var children = here.Item.Children;

      for (int i = 0; i < here.Item.NumberOfChildren; i++) children[i].Dispose();

      if (here == root) {
        root.Item.Children = null;
        root.Item.Leaf     = null;
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