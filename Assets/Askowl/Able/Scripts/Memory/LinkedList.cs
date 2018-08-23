// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

using System;
using System.Reflection;
using System.Text;
using UnityEngine;

// ReSharper disable StaticMemberInGenericType

namespace Askowl {
  /// <a href="">LinkedList - a different perspective</a>
  // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
  public class LinkedList<T> : IDisposable {
    /// <remarks><a href="">LinkedList node</a></remarks>
    public class Node : IDisposable {
      /// <remarks><a href="">LinkedList node</a></remarks>
      public Node Previous, Next;

      /// <summary>
      /// List that currently owns this node is called Owner. It is changed by <see cref="MoveTo"/>
      /// Home is the list in which the node was first inserted. It is to this
      /// recycle bin it will return on Dispose().
      /// </summary>
      /// <remarks><a href="http://unitydoc.marrington.net/Able#node-home">Node Home List</a></remarks>
      public LinkedList<T> Owner, Home;

      /// <remarks><a href="">Item can be value type (int, float ... struct) or object (class instance)</a></remarks>
      public T Item;

      /// <remarks><a href="">NodeComparison Operator</a></remarks>
      public static bool operator <(Node left, Node right) => CompareItem(left, right) < 0;

      /// <remarks><a href="http://unitydoc.marrington.net/Able#node-comparison">NodeComparison Operator</a></remarks>
      public static bool operator <=(Node left, Node right) => CompareItem(left, right) <= 0;

      /// <remarks><a href="http://unitydoc.marrington.net/Able#node-comparison">NodeComparison Operator</a></remarks>
      public static bool operator >(Node left, Node right) => CompareItem(left, right) > 0;

      /// <remarks><a href="http://unitydoc.marrington.net/Able#node-comparison">NodeComparison Operator</a></remarks>
      public static bool operator >=(Node left, Node right) => CompareItem(left, right) >= 0;

      /// <remarks><a href="http://unitydoc.marrington.net/Able#move-node-to-another-list">Move Node to Another List</a></remarks>
      public Node MoveTo(LinkedList<T> to) => to.Insert(this);

      /// <remarks><a href="http://unitydoc.marrington.net/Able#move-node-to-another-list">Move Node to Another List</a></remarks>
      public Node MoveToEndOf(LinkedList<T> to) => to.Append(this);

      /// <remarks><a href="http://unitydoc.marrington.net/Able#dispose-of-this-node">Dispose of a Node Forever</a></remarks>
      public void Destroy() {
        DeactivateItem(this);
        Item = default(T);
        Home.Unlink(this);
        Home = null;
      }

      /// <a href="http://unitydoc.marrington.net/Able#dispose-of-this-node">Calls Item.Dispose if the item is IDisposable, then move it to the recycle bin</a>
      public Node Recycle() {
        DeactivateItem(this);
        MoveToEndOf(Home.RecycleBin);

        return this;
      }

      /// <inheritdoc />
      public void Dispose() => Recycle();

      /// <inheritdoc />
      /// <remarks><a href="http://unitydoc.marrington.net/Able#node-name">Node Naming Convention</a></remarks>
      public override string ToString() => $"{Home} // {Owner}:: {Item}";

      /// <remarks><a href="http://unitydoc.marrington.net/Able#update-node-contents">Update Node Contents</a></remarks>
      public Node Update(T newItem) {
        Item = newItem;
        return this;
      }
    }

    #region Private create, deactivation and activation support
    private static Func<T> CallConstructor() {
      var flags       = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
      var constructor = typeof(T).GetConstructor(flags, null, Type.EmptyTypes, null);
      if (constructor == null) return null;

      return () => (T) constructor.Invoke(parameters: null);
    }

    private static Func<T> DefaultCreateItem() {
      var flags  = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
      var method = typeof(T).GetMethod("CreateItem", flags, null, CallingConventions.Standard, Type.EmptyTypes, null);
      if (method != null) return () => (T) method.Invoke(null, null);

      return CallConstructor() ?? (() => default(T));
    }

    private static MethodInfo DefaultMethod(string name, Type[] parameters) {
      var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
      return typeof(T).GetMethod(name, flags, null, CallingConventions.HasThis, parameters, null);
    }

    private static Type[] ActivationParameters = {typeof(T)}, ComparisonParameters = {typeof(T), typeof(T)};

    private static Action<Node> DefaultActivation(string name) {
      var method = DefaultMethod(name, ActivationParameters);
      if (method == null) return null;

      return node => method.Invoke(node.Item, null);
    }

    private static Func<Node, Node, int> DefaultComparison() {
      var method = DefaultMethod("CompareItem", ComparisonParameters);
      if (method == null) return null;

      ordered = true;
      return (node, other) => (int) method.Invoke(node.Item, new object[] {other.Item});
    }

    private static Action<Node> DefaultDeactivateItem() =>
      DefaultActivation("DeactivateItem") ?? (node => (node.Item as IDisposable)?.Dispose());

    private static Action<Node> DefaultReactivateItem() =>
      DefaultActivation("ReactivateItem") ?? (node => { });

    private static Func<Node, Node, int> DefaultCompareItem() =>
      DefaultComparison() ?? ((Node, other) => 0);

    private static bool ordered;
    private static int  ordinal;
    #endregion

    #region Public List Creation and Destruction
    /// <remarks><a href="">Get an instance of this type of linked list</a></remarks>
    public static LinkedList<T> Instance(string name) {
      var list = new LinkedList<T>();
      list.Name = string.IsNullOrWhiteSpace(name) ? $"{typeof(T).Name}-{++ordinal}" : name;
      return list;
    }

    /// <remarks><a href="">Linked List Name</a></remarks>
    public string Name { get; private set; }

    /// <remarks><a href="">Item Creation and Preparation when new() is not enough</a></remarks>
    public static Func<T> CreateItem { private get; set; } = DefaultCreateItem();

    /// <a href="">Prepare an idle item for reuse</a>
    public static Action<Node> ReactivateItem { private get; set; } = DefaultReactivateItem();

    /// <remarks><a href="">For Deactivation when Dispose() is not enough</a></remarks>
    public static Action<Node> DeactivateItem { private get; set; } = DefaultDeactivateItem();

    /// <remarks><a href="">For Deactivation when Dispose() is not enough</a></remarks>
    public static Func<Node, Node, int> CompareItem { private get; set; } = DefaultCompareItem();

    /// <remarks><a href="">For the rare times we need to clear a linked list</a></remarks>
    public void Destroy() {
      Dispose();
      while (recycleBin.Top != null) recycleBin.Top.Destroy();
    }

    /// <remarks><a href="">For the rare times we need to clear a linked list</a></remarks>
    public void Dispose() {
      while (Top != null) Top.Dispose();
    }
    #endregion

    #region Node Creation and Movement
    /// <remarks><a href="t">Default comparison where everything is equal</a></remarks>
    protected virtual int Compare(Node left, Node right) => 0;

    /// <summary>
    /// Give a new item, fetch a node from recycling or create it then insert into the linked list.
    /// </summary>
    /// <remarks><a href="t">Add an Item to a List</a></remarks>
    public Node Add(T newItem) =>
      RecycleBin.Empty ? Insert(NewNode(newItem)) : RecycleBin.Top.MoveTo(this).Update(newItem);

    /// <summary>
    /// Fetch a node from the recycle bin. If the bin is empty, use the creator
    /// function linked to the list to create and Item and then a Node.
    /// The default creator function returns `default(T)`, which is adequate for
    /// move value items. It will be null for object and can be filled in later
    /// if needed.
    /// </summary>
    /// <returns>node for chained calls</returns>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#recycle-a-currently-unused-node">Get a recycled or new node</a></remarks>
    public Node Fetch() {
      if (RecycleBin.Empty) return Insert(NewNode(CreateItem()));

      var node = RecycleBin.Top.MoveTo(this);
      ReactivateItem(node);
      return node;
    }

    private Node NewNode(T item) {
      Node node = new Node() {Item = item, Owner = this, Home = this};
      return node;
    }

    private Node Insert(Node nodeToInsert) {
      if (DebugMode) DebugMessage(nodeToInsert);

      Unlink(nodeToInsert);
      nodeToInsert.Owner = this;
      if (Empty) return Bottom = Top = nodeToInsert;

      Node after = Top;

      if (ordered) {
        after = Walk((node, _) => nodeToInsert >= node);

        if (after == null) {
          nodeToInsert.Previous = Bottom;
          return Bottom = (Bottom.Next = nodeToInsert);
        }
      }

      nodeToInsert.Next     = after;
      nodeToInsert.Previous = after.Previous;
      after.Previous        = nodeToInsert;
      if (Bottom == null) Bottom = nodeToInsert;
      if (after  == Top) Top     = nodeToInsert;
      return nodeToInsert;
    }

    private Node Append(Node nodeToAppend) {
      if (DebugMode) DebugMessage(nodeToAppend, "end of ");

      Unlink(nodeToAppend);
      nodeToAppend.Owner = this;
      if (Empty) return Bottom = Top = nodeToAppend;

      nodeToAppend.Previous = Bottom;
      Bottom.Next           = nodeToAppend;
      Bottom                = nodeToAppend;
      return nodeToAppend;
    }

    private void Unlink(Node node) {
      if (node == node.Owner.Top) {
        node.Owner.Top = node.Next;
      } else if (node == node.Owner.Bottom) {
        node.Owner.Bottom = node.Previous;
      } else if ((node.Previous == null) && (node.Next == null)) {
        return; // Node doesn't belong to anyone
      }

      if (node.Previous != null) node.Previous.Next = node.Next;
      if (node.Next     != null) node.Next.Previous = node.Previous;

      node.Previous = node.Next = null;
      node.Owner    = null;
    }

    /// <remarks><a href="http://unitydoc.marrington.net/Able#node-creation-and-movement">List for Unused Nodes</a></remarks>
    public LinkedList<T> RecycleBin =>
      recycleBin ?? (recycleBin = isRecycleBin ? null : newRecycleBin);

    private LinkedList<T> newRecycleBin => new LinkedList<T>
      {isRecycleBin = true, Name = $"{Name} Recycling Bin"};

    private bool          isRecycleBin;
    private LinkedList<T> recycleBin;
    #endregion

    #region FiFo Stack
    /// <remarks><a href="http://unitydoc.marrington.net/Able#fifo">First Node in List or null</a></remarks>
    public Node Top;

    /// <remarks><a href="http://unitydoc.marrington.net/Able#fifo">Last Node in List or null</a></remarks>
    public Node Bottom;

    /// <remarks><a href="http://unitydoc.marrington.net/Able#fifo">Second item in the list or null</a></remarks>
    public Node Next => Top?.Next;

    /// <remarks><a href="http://unitydoc.marrington.net/Able#fifo">Is list empty?</a></remarks>
    public bool Empty => (Top == null);

    /// <remarks><a href="http://unitydoc.marrington.net/Able#fifo">Calculate number of items in a list</a></remarks>
    public int Count {
      get {
        int count = 0;
        Walk((node, next) => ++count != 0);
        return count;
      }
    }

    /// <see cref="Add"/>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#fifo">Add an item to the list</a></remarks>
    public Node Push(T item) => Add(item);

    /// <remarks><a href="http://unitydoc.marrington.net/Able#fifo">Add a node to the list</a></remarks>
    public Node Push(Node node) => node.MoveTo(this);

    /// <remarks><a href="http://unitydoc.marrington.net/Able#fifo">Retrieve the first list item - <see cref="Node.Recycle"/></a></remarks>
    public Node Pop() => Top?.Recycle();
    #endregion

    #region Traversal
    /// <summary>
    /// Walk all nodes - or until the action says otherwise
    /// </summary>
    /// <param name="action">Do something with node. Stop walking if returns false</param>
    /// <returns>node that returned false in the action or null if all done</returns>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#walk-all-nodes">Node Walking</a></remarks>
    public Node Walk(Func<Node, Node, bool> action) {
      for (Node next, node = Top; node != null; node = next) {
        if (!action(node, next = node.Next)) return node;
      }

      return null;
    }
    #endregion

    #region Debugging
    /// <remarks><a href="http://unitydoc.marrington.net/Able#debug-mode">Debug mode logs changes</a></remarks>
    // ReSharper disable once StaticMemberInGenericType
    public static bool DebugMode { private get; set; } = false;

    private void DebugMessage(Node node, string append = "") {
      Debug.Log(node.Owner == this
                  ? $"**** LinkedList: Add to {this}"
                  : $"**** LinkedList: move {node.Owner} to {append}{this}");
    }

    /// <remarks><a href="http://unitydoc.marrington.net/Able#dump">Return list contents as a string</a></remarks>
    public string Dump(int maxEntriesToDump = 1000) {
      var builder = new StringBuilder();
      int line    = 0;

      Walk((node, next) => {
        builder.AppendLine($"{++line}:\t{node}");
        return --maxEntriesToDump > 0;
      });

      return builder.ToString();
    }

    /// <inheritdoc />
    public override string ToString() => Name;
    #endregion
  }
}