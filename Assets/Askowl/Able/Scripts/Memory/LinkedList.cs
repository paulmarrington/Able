// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

// ReSharper disable StaticMemberInGenericType

namespace Askowl {
  using System;
  using System.Reflection;
  using System.Text;
  using UnityEngine;

  /// <a href="">LinkedList - a different perspective</a>
  // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
  public class LinkedList<T> : IDisposable {
    /// <a href="">LinkedList node</a>
    public class Node : IDisposable {
      /// <a href="">LinkedList node</a>
      public Node Previous, Next;

      /// <summary>
      /// List that currently owns this node is called Owner. It is changed by <see cref="MoveTo"/>
      /// Home is the list in which the node was first inserted. It is to this
      /// recycle bin it will return on Dispose().
      /// </summary>
      /// <a href="http://unitydoc.marrington.net/Able#node-home">Node Home List</a>
      public LinkedList<T> Owner, Home;

      /// <a href="">Item can be value type (int, float ... struct) or object (class instance)</a>
      public T Item { get; protected internal set; }

      /// <a href="">NodeComparison Operator</a>
      public static bool operator <(Node left, Node right) => left.Owner.CompareItem(left, right) < 0;

      /// <a href="http://unitydoc.marrington.net/Able#node-comparison">NodeComparison Operator</a>
      public static bool operator <=(Node left, Node right) => left.Owner.CompareItem(left, right) <= 0;

      /// <a href="http://unitydoc.marrington.net/Able#node-comparison">NodeComparison Operator</a>
      public static bool operator >(Node left, Node right) => left.Owner.CompareItem(left, right) > 0;

      /// <a href="http://unitydoc.marrington.net/Able#node-comparison">NodeComparison Operator</a>
      public static bool operator >=(Node left, Node right) => left.Owner.CompareItem(left, right) >= 0;

      /// <a href="http://unitydoc.marrington.net/Able#move-node-to-another-list">Move Node to Another List</a>
      public Node MoveTo(LinkedList<T> to) => to.Insert(this);

      /// <a href="http://unitydoc.marrington.net/Able#move-node-to-another-list">Move Node to Another List</a>
      // ReSharper disable once UnusedMethodReturnValue.Global
      public Node MoveToEndOf(LinkedList<T> to) => to.Append(this);

      /// <a href="http://unitydoc.marrington.net/Able#dispose-of-this-node">Dispose of a Node Forever</a>
      public void Destroy() {
        Owner.DeactivateItem(this);
        Item = default;
        Home.Unlink(this);
        reverseLookup?.Remove(this);
        Home = null;
      }

      /// <a href=""></a>
      public Node Recycle() {
        Owner.DeactivateItem(this);
        MoveToEndOf(Home.RecycleBin);

        return this;
      }

      /// <inheritdoc />
      public void Dispose() => Recycle();

      /// <a href="">Node Naming Convention</a>
      /// <inheritdoc />
      public override string ToString() => $"{Owner,25}  <<  {Home,-25}::  {Item}";

      /// <a href=""></a>
      public Node Fetch() => Home.Fetch().MoveTo(Owner);

      /// <a href=""></a>
      public Node Push(T t) => Home.Push(item: t).MoveTo(Owner);
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

    private static Action<Node> GetDefaultDeactivateItem() => DefaultActivation("DeactivateItem") ??
                                                              (node => { }); //(node.Item as IDisposable)?.Dispose());

    private static Action<Node> GetDefaultReactivateItem() => DefaultActivation("ReactivateItem") ?? (node => { });

    private static bool orderedStatic;
    private static bool orderedDynamic;
    private        bool ordered = orderedStatic || orderedDynamic;
    private static int  ordinal;
    #endregion

    #region Public List Creation and Destruction
    /// <a href="">Get an instance of this type of linked list</a>
    public LinkedList(string name = null) =>
      Name = string.IsNullOrWhiteSpace(name) ? $"{typeof(T).Name}-{++ordinal}" : name;

    /// <a href="">Linked List Name</a>
    public string Name { get; }

    /// <a href="">Item Creation and Preparation when new() is not enough</a>
    public static readonly Func<T> CreateItemStatic = GetDefaultCreateItem();

    /// <a href="">Item Creation and Preparation when new() is not enough</a>
    public Func<T> CreateItem { private get; set; } = () => CreateItemStatic();

    /// <a href=""></a>
    public static readonly Action<Node> ReactivateItemStatic = GetDefaultReactivateItem();

    /// <a href="">Prepare an idle item for reuse</a>
    public Action<Node> ReactivateItem { private get; set; } = (node) => ReactivateItemStatic(node);

    /// <a href=""></a>
    public static Action<Node> DeactivateItemStatic = GetDefaultDeactivateItem();

    /// <a href="">For Deactivation when Dispose() is not enough</a>
    public Action<Node> DeactivateItem { private get; set; } = (node) => DeactivateItemStatic(node);

    /// <a href=""></a>
    public static Func<Node, Node, int> CompareItemStatic {
      set {
        orderedStatic     = true;
        compareItemStatic = value;
      }
    }

    private static Func<Node, Node, int> compareItemStatic = GetDefaultCompareItem();

    /// <a href=""></a>
    public Func<Node, Node, int> CompareItem {
      private get { return compareItem; }
      set {
        orderedDynamic = ordered = true;
        compareItem    = value;
      }
    }

    private Func<Node, Node, int> compareItem = (left, right) => compareItemStatic(left, right);

    /// <a href="">For the rare times we need to clear a linked list</a>
    public void Destroy() {
      Dispose();
      if (recycleBin == null) return;

      while (recycleBin.First != null) recycleBin.First.Destroy();
      recycleBin.First = recycleBin.Last = null;
    }

    /// <a href="">For the rare times we need to clear a linked list</a>
    public void Dispose() {
      reverseLookup = null;
      while (First != null) First.Dispose();
      First = Last = null;
    }
    #endregion

    #region Node Creation and Movement
    /// <a href="">Add an Item to a List</a>
    public Node Add(params T[] newItems) {
      Node node                                            = null;
      for (var idx = 0; idx < newItems.Length; idx++) node = Set(newItems[idx]);
      return node;
    }

    /// <summary>
    /// Fetch a node from the recycle bin. If the bin is empty, use the creator
    /// function linked to the list to create and Item and then a Node.
    /// The default creator function returns `default(T)`, which is adequate for
    /// move value items. It will be null for object and can be filled in later
    /// if needed.
    /// </summary>
    /// <returns>node for chained calls</returns>
    /// <a href="http://unitydoc.marrington.net/Able#recycle-a-currently-unused-node">Get a recycled or new node</a>
    public Node Fetch() {
      if (RecycleBin.Empty) return Insert(NewNode(CreateItem()));

      var node = RecycleBin.First.MoveTo(this);
      ReactivateItem(node);
      return node;
    }

    /// <a href=""></a>
    public Node Set(T item) {
      var node = Fetch();
      node.Item = item;
      return node;
    }

    /// <a href="bit.ly/">Node Dispose using reverse lookup</a>
    public Node ReverseLookup(T item) {
      if (reverseLookup == null) {
        reverseLookup = new Map();
        for (var node = First; node != null; node = node.Next) reverseLookup.Add(node.Item, node);
      }

      return reverseLookup?[item].Value as Node;
    }

    /// <a href="bit.ly/">Node Dispose using reverse lookup</a>
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
      if (node == node.Owner.First) { node.Owner.First = node.Next; } else if (node == node.Owner.Last
      ) { node.Owner.Last                              = node.Previous; } else if ((node.Previous == null)
                                                                                && (node.Next
                                                                                 == null)) {
        return; // Node doesn't belong to anyone
      }

      node.Owner.Count--;

      if (node.Previous != null) node.Previous.Next = node.Next;
      if (node.Next     != null) node.Next.Previous = node.Previous;

      node.Previous = node.Next = null;
      node.Owner    = null;
    }

    /// <a href="http://unitydoc.marrington.net/Able#node-creation-and-movement">List for Unused Nodes</a>
    public LinkedList<T> RecycleBin => isRecycleBin ? null : recycleBin ?? (recycleBin = NewRecycleBin);

    private LinkedList<T> NewRecycleBin => new LinkedList<T>($"{Name} Recycling Bin") { isRecycleBin = true };

    private bool          isRecycleBin;
    private LinkedList<T> recycleBin;
    #endregion

    #region FiFo Stack
    /// <a href="http://unitydoc.marrington.net/Able#fifo">First Node in List or null</a>
    public Node First;

    /// <a href="http://unitydoc.marrington.net/Able#fifo">Last Node in List or null</a>
    public Node Last;

    /// <a href="http://unitydoc.marrington.net/Able#fifo">Second item in the list or null</a>
    public Node Second => First?.Next;

    /// <a href="http://unitydoc.marrington.net/Able#fifo">Is list empty?</a>
    public bool Empty => First == null;

    /// <a href="http://unitydoc.marrington.net/Able#fifo">Calculate number of items in a list</a>
    public int Count { get; private set; }

    /// <see cref="Add"/>
    /// <a href="http://unitydoc.marrington.net/Able#fifo">Add an item to the list</a>
    public Node Push(T item) => Add(item);

    /// <a href="http://unitydoc.marrington.net/Able#fifo">Add a node to the list</a>
    public Node Push(Node node) => node.MoveTo(this);

    /// <a href="http://unitydoc.marrington.net/Able#fifo">Retrieve the first list item - <see cref="Node.Recycle"/></a>
    public Node Pop() => First?.Recycle();
    #endregion

    #region Debugging
    /// <a href="http://unitydoc.marrington.net/Able#debug-mode">Debug mode logs changes</a>
    // ReSharper disable once StaticMemberInGenericType
    public static bool DebugMode { get; } = false;

    private void DebugMessage(Node node, string append = "") {
      Debug.Log(
        node.Owner == this
          ? $"**** LinkedList: Add to {this}"
          : $"**** LinkedList: move {node.Owner} to {append}{this}");
    }

    /// <a href="http://unitydoc.marrington.net/Able#dump">Return list contents as a string</a>
    public string Dump(int maxEntriesToDump = 1000) {
      var builder = new StringBuilder();
      var line    = 0;

      for (var node = First; (node != null) && (maxEntriesToDump-- > 0); node = node.Next) {
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