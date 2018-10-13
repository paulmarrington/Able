// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

// ReSharper disable StaticMemberInGenericType

namespace Askowl {
  using System;
  using System.Reflection;
  using System.Text;
  using UnityEngine;

  /// <a href="http://bit.ly/2RhsEws">LinkedList - a different perspective</a>
  // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
  public class LinkedList<T> : IDisposable {
    /// <a href="http://bit.ly/2OssV13">LinkedList node</a>
    public class Node : IDisposable {
      /// <a href="http://bit.ly/2OssV13">Node links</a>
      public Node Previous, Next;

      /// <a href="http://bit.ly/2RezEKu">List that currently owns this node is called Owner. It is changed by `MoveTo` Home is the list in which the node was first inserted. It is to this recycle bin it will return on Dispose().</a>
      public LinkedList<T> Owner, Home;

      /// <a href="http://bit.ly/2OssV13">Item can be value type (int, float ... struct) or object (class instance)</a>
      public T Item { get; protected internal set; }

      /// <a href="http://bit.ly/2NX3sgR">Node comparison Operator</a>
      public static bool operator <(Node left, Node right) => left.Owner.CompareItem(left, right) < 0;

      /// <a href="http://bit.ly/2NX3sgR">Node comparison Operator</a>
      public static bool operator <=(Node left, Node right) => left.Owner.CompareItem(left, right) <= 0;

      /// <a href="http://bit.ly/2NX3sgR">Node comparison Operator</a>
      public static bool operator >(Node left, Node right) => left.Owner.CompareItem(left, right) > 0;

      /// <a href="http://bit.ly/2NX3sgR">Node comparison Operator</a>
      public static bool operator >=(Node left, Node right) => left.Owner.CompareItem(left, right) >= 0;

      /// <a href="http://bit.ly/2Rh1Igm">Move Node to Another List</a>
      public Node MoveTo(LinkedList<T> to) => to.Insert(this);

      /// <a href="http://bit.ly/2Rh1Igm">Move Node to Another List</a>
      // ReSharper disable once UnusedMethodReturnValue.Global
      public Node MoveToEndOf(LinkedList<T> to) => to.Append(this);

      /// <a href="http://bit.ly/2Rh3dv0">Dispose of a Node Forever</a>
      public void Destroy() {
        Owner.DeactivateItem(this);
        Item = default;
        Home.Unlink(this);
        reverseLookup?.Remove(this);
        Home = null;
      }

      /// <a href="http://bit.ly/2Rh3dv0">Deactivate an entry and put the containing node in the recycling bin</a>
      public Node Recycle() {
        Owner.DeactivateItem(this);
        MoveToEndOf(Home.RecycleBin);
        return this;
      }

      /// <inheritdoc />
      public void Dispose() => Recycle();

      /// <inheritdoc />
      public override string ToString() => $"{Owner,25}  <<  {Home,-25}::  {Item}";

      /// <a href="http://bit.ly/2RhcvXR">Get a node and item either recycled or created new.</a>
      public Node GetRecycledOrNew() => Home.GetRecycledOrNew().MoveTo(Owner);

      /// <a href="http://bit.ly/2NXfQgH">Fetch an unused node and use it as the container for the provided new item</a>
      public Node Push(T t) => Home.Push(item: t).MoveTo(Owner);

      /// <a href="http://bit.ly/2OvNqd4">Fetch an unused node and use it as the container for the provided new item</a>
      public Node Add(T t) => Home.Add(item: t).MoveTo(Owner);
    }

    #region Private create, deactivation and activation support
    private static Func<T> CallConstructor() {
      var flags       = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
      var constructor = typeof(T).GetConstructor(flags, null, Type.EmptyTypes, null);
      if (constructor == null) return null;

      return () => (T) constructor.Invoke(parameters: null);
    }

    private static MethodInfo DefaultMethod(string name, Type[] parameters) {
      var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
      return typeof(T).GetMethod(name, flags, null, CallingConventions.HasThis, parameters, null);
    }

    private static Type[] ComparisonParameters = { typeof(T), typeof(T) };

    private static Action<Node> DefaultActivation(string name) {
      var method = DefaultMethod(name, Type.EmptyTypes);
      if (method == null) return null;

      return node => method.Invoke(node.Item, null);
    }

    private static Func<Node, Node, int> GetDefaultCompareItem() {
      var method = DefaultMethod("CompareItem", ComparisonParameters);
      if (method == null) return (node, other) => 0;

      orderedDynamic = true;
      return (node, other) => (int) method.Invoke(node.Item, new object[] { node.Item, other.Item });
    }

    private static Func<T> GetDefaultCreateItem() {
      var flags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

      var method = typeof(T).GetMethod(
        "CreateItem", flags, null, CallingConventions.Standard,
        Type.EmptyTypes, null);

      if (method != null) return () => (T) method.Invoke(null, null);

      return CallConstructor() ?? (() => default);
    }

    private static Action<Node> GetDefaultDeactivateItem() => DefaultActivation("DeactivateItem") ?? (node => { });

    private static Action<Node> GetDefaultReactivateItem() => DefaultActivation("ReactivateItem") ?? (node => { });

    private static bool orderedStatic;
    private static bool orderedDynamic;
    private        bool ordered = orderedStatic || orderedDynamic;
    private static int  ordinal;
    #endregion

    #region Public List Creation and Destruction
    /// <a href="http://bit.ly/2RhsEws">Get an instance of this type of linked list</a>
    public LinkedList(string name = null) =>
      Name = string.IsNullOrWhiteSpace(name) ? $"{typeof(T).Name}-{++ordinal}" : name;

    /// <a href="http://bit.ly/2RhsEws">Linked List Name</a>
    // ReSharper disable once FieldCanBeMadeReadOnly.Global
    public string Name;

    /// <a href="http://bit.ly/2NVwTjo">Item Creation and Preparation when new() is not enough</a>
    public static Func<T> CreateItemStatic = GetDefaultCreateItem();

    /// <a href="http://bit.ly/2NVwTjo">Item Creation and Preparation when new() is not enough</a>
    public Func<T> CreateItem { private get; set; } = () => CreateItemStatic();

    /// <a href="http://bit.ly/2Oq9C8y">Actor we can create if reactivation requires additional code</a>
    public static Action<Node> ReactivateItemStatic = GetDefaultReactivateItem();

    /// <a href="http://bit.ly/2Oq9C8y">Prepare an idle item for reuse</a>
    public Action<Node> ReactivateItem { private get; set; } = (node) => ReactivateItemStatic(node);

    /// <a href="http://bit.ly/2Rj0PEa">Called before an item is returned to recycling</a>
    public static Action<Node> DeactivateItemStatic = GetDefaultDeactivateItem();

    /// <a href="http://bit.ly/2Rj0PEa">For Deactivation when Dispose() is not enough</a>
    public Action<Node> DeactivateItem { private get; set; } = (node) => DeactivateItemStatic(node);

    /// <a href="http://bit.ly/2OzziQl">Used to insert items into the correct location for sorted lists</a>
    public static Func<Node, Node, int> CompareItemStatic {
      set {
        orderedStatic     = true;
        compareItemStatic = value;
      }
    }

    private static Func<Node, Node, int> compareItemStatic = GetDefaultCompareItem();

    /// <a href="http://bit.ly/2OzziQl">Used to insert items into the correct location for sorted lists</a>
    public Func<Node, Node, int> CompareItem {
      private get { return compareItem; }
      set {
        orderedDynamic = ordered = true;
        compareItem    = value;
      }
    }

    private Func<Node, Node, int> compareItem = (left, right) => compareItemStatic(left, right);

    /// <a href="http://bit.ly/2OvDxfs">For the rare times we need to clear a linked list</a>
    public void Destroy() {
      Dispose();
      if (recycleBin == null) return;

      while (recycleBin.First != null) recycleBin.First.Destroy();
      recycleBin.First = recycleBin.Last = null;
    }

    /// <a href="http://bit.ly/2OzDMGF">For the rare times we need to clear a linked list</a>
    public void Dispose() {
      reverseLookup = null;
      while (First != null) First.Dispose();
      First = Last = null;
    }
    #endregion

    #region Node Creation and Movement
    /// <a href="http://bit.ly/2OtGxZV">Add an Item to a List</a>
    public Node Add(T item) {
      Node node = GetRecycledOrNew();
      reverseLookup?.Remove(node.Item).Add(item, node);
      node.Item = item;
      return node;
    }

    /// <a href="http://bit.ly/2OtGxZV">Retrieve a node - either from recycling or creating it anew</a>
    public Node GetRecycledOrNew() {
      if (RecycleBin.Empty) return Insert(NewNode(CreateItem()));

      var node = RecycleBin.First.MoveTo(this);
      ReactivateItem(node);
      return node;
    }

    /// <a href="http://bit.ly/2OvDxfs">For node disposal using reverse lookup</a>
    public Node ReverseLookup(T item) {
      if (reverseLookup == null) {
        reverseLookup = new Map();
        for (var node = First; node != null; node = node.Next) reverseLookup.Add(node.Item, node);
      }

      return reverseLookup?[item].Value as Node;
    }

    /// <a href="http://bit.ly/2OvDxfs">Node Dispose using reverse lookup</a>
    public void Dispose(T item) => ReverseLookup(item)?.Dispose();

    private static Map reverseLookup;

    private Node NewNode(T item) {
      Node newNode = new Node() { Item = item, Owner = this, Home = this };
      reverseLookup?.Add(item, newNode);
      return newNode;
    }

    private Node Insert(Node nodeToInsert) {
      if (DebugMode) DebugMessage(nodeToInsert);
      Count++;
      Unlink(nodeToInsert);

      nodeToInsert.Owner = this;
      if (Empty) return Last = First = nodeToInsert;

      Node after = First;

      if (ordered) {
        while (nodeToInsert > after) {
          if ((after = after.Next) == null) {
            nodeToInsert.Previous = Last;
            Last.Next             = nodeToInsert;
            return Last = nodeToInsert;
          }
        }
      }

      nodeToInsert.Next     = after;
      nodeToInsert.Previous = after.Previous;

      if (after.Previous != null) after.Previous.Next = nodeToInsert;
      after.Previous = nodeToInsert;

      if (Last  == null) Last   = nodeToInsert;
      if (after == First) First = nodeToInsert;
      return nodeToInsert;
    }

    private Node Append(Node nodeToAppend) {
      if (DebugMode) DebugMessage(nodeToAppend, "end of ");
      Count++;
      Unlink(nodeToAppend);

      nodeToAppend.Owner = this;
      if (Empty) return Last = First = nodeToAppend;

      nodeToAppend.Previous = Last;
      Last.Next             = nodeToAppend;
      Last                  = nodeToAppend;
      return nodeToAppend;
    }

    private void Unlink(Node node) {
      if (node      == node.Owner.First) { node.Owner.First = node.Next; }
      else if (node == node.Owner.Last
      ) { node.Owner.Last = node.Previous; }
      else if ((node.Previous == null) && (node.Next == null)) {
        return; // Node doesn't belong to anyone
      }

      node.Owner.Count--;

      if (node.Previous != null) node.Previous.Next = node.Next;
      if (node.Next     != null) node.Next.Previous = node.Previous;

      node.Previous = node.Next = null;
      node.Owner    = null;
    }

    /// <a href="http://bit.ly/2OvDxfs">List for Unused Nodes</a>
    public LinkedList<T> RecycleBin => isRecycleBin ? null : recycleBin ?? (recycleBin = NewRecycleBin);

    private LinkedList<T> NewRecycleBin => new LinkedList<T>($"{Name} Recycling Bin") { isRecycleBin = true };

    private bool          isRecycleBin;
    private LinkedList<T> recycleBin;
    #endregion

    #region FiFo Stack
    /// <a href="http://bit.ly/2O11goE">First Node in List or null</a>
    public Node First;

    /// <a href="http://bit.ly/2Oq9ypk">Last Node in List or null</a>
    public Node Last;

    /// <a href="http://bit.ly/2Rj0O34">Second item in the list or null</a>
    public Node Second => First?.Next;

    /// <a href="http://bit.ly/2Ov3MCP">Is list empty?</a>
    public bool Empty => First == null;

    /// <a href="http://bit.ly/2NUH8Ek">Calculate number of items in a list</a>
    public int Count { get; private set; }

    /// <a href="http://bit.ly/2O02sID">Add an item to the list</a>
    public Node Push(T item) => Add(item);

    /// <a href="http://bit.ly/2O02sID">Add a node to the list</a>
    public Node Push(Node node) => node.MoveTo(this);

    /// <a href="http://bit.ly/2Ov490b">Retrieve the first list item - `Node.Recycle`</a>
    public Node Pop() => First?.Recycle();

    /// <a href="http://bit.ly/2NTD9Ih">Pull the last item from the list</a>
    public Node Pull() => Last?.Recycle();
    #endregion

    #region Debugging
    /// <a href="http://bit.ly/2RezKBQ">Debug mode logs changes</a>
    // ReSharper disable once StaticMemberInGenericType
    public static bool DebugMode = false;

    private void DebugMessage(Node node, string append = "") {
      Debug.Log(
        node.Owner == this
          ? $"**** LinkedList: Add to {this}"
          : $"**** LinkedList: move {node.Owner} to {append}{this}");
    }

    /// <a href="http://bit.ly/2OxOL3m">Return list contents as a string</a>
    public string Dump(int maxEntriesToDump = 1000) {
      var builder = new StringBuilder();
      var line    = 0;

      for (Node node = First; (node != null) && (maxEntriesToDump-- > 0); node = node.Next) {
        builder.AppendLine($"{++line}:\t{node}");
      }

      return builder.ToString();
    }

    /// <inheritdoc />
    public override string ToString() {
      string count = recycleBin != null ? $"({Count}/{recycleBin.Count})" : $"({Count})";
      return $"{Name} {count,8}";
    }
    #endregion
  }
}