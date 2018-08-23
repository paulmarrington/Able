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
      public T Item;

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
      public Node MoveToEndOf(LinkedList<T> to) => to.Append(this);

      /// <a href="http://unitydoc.marrington.net/Able#dispose-of-this-node">Dispose of a Node Forever</a>
      public void Destroy() {
        Owner.DeactivateItem(this);
        Item = default(T);
        Home.Unlink(this);
        Home = null;
      }

      /// <a href="http://unitydoc.marrington.net/Able#dispose-of-this-node">Calls Item.Dispose if the item is IDisposable, then move it to the recycle bin</a>
      public Node Recycle() {
        Owner.DeactivateItem(this);
        MoveToEndOf(Home.RecycleBin);

        return this;
      }

      /// <inheritdoc />
      public void Dispose() => Recycle();

      /// <inheritdoc />
      /// <a href="http://unitydoc.marrington.net/Able#node-name">Node Naming Convention</a>
      public override string ToString() => $"{Home} // {Owner}:: {Item}";

      /// <a href="http://unitydoc.marrington.net/Able#update-node-contents">Update Node Contents</a>
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

    private static Func<Node, Node, int> GetDefaultCompareItem() {
      var method = DefaultMethod("CompareItem", ComparisonParameters);
      if (method == null) return (node, other) => 0;

      ordered = true;
      return (node, other) => (int) method.Invoke(node.Item, new object[] {other.Item});
    }

    private static Func<T> GetDefaultCreateItem() {
      var flags  = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
      var method = typeof(T).GetMethod("CreateItem", flags, null, CallingConventions.Standard, Type.EmptyTypes, null);
      if (method != null) return () => (T) method.Invoke(null, null);

      return CallConstructor() ?? (() => default(T));
    }

    private static Action<Node> GetDefaultDeactivateItem() =>
      DefaultActivation("DeactivateItem") ?? (node => (node.Item as IDisposable)?.Dispose());

    private static Action<Node> GetDefaultReactivateItem() =>
      DefaultActivation("ReactivateItem") ?? (node => { });

    private static bool ordered;
    private static int  ordinal;
    #endregion

    #region Public List Creation and Destruction
    /// <a href="">Get an instance of this type of linked list</a>
    public LinkedList(string name) { Name = string.IsNullOrWhiteSpace(name) ? $"{typeof(T).Name}-{++ordinal}" : name; }

    /// <a href="">Linked List Name</a>
    public string Name { get; }

    /// <a href="">Item Creation and Preparation when new() is not enough</a>
    public static readonly Func<T> CreateItemStatic = GetDefaultCreateItem();

    /// <a href="">Item Creation and Preparation when new() is not enough</a>
    public Func<T> CreateItem { private get; set; } = CreateItemStatic;

    /// <a href=""></a>
    public static readonly Action<Node> ReactivateItemStatic = GetDefaultReactivateItem();

    /// <a href="">Prepare an idle item for reuse</a>
    public Action<Node> ReactivateItem { private get; set; } = ReactivateItemStatic;

    /// <a href=""></a>
    public static readonly Action<Node> DeactivateItemStatic = GetDefaultDeactivateItem();

    /// <a href="">For Deactivation when Dispose() is not enough</a>
    public Action<Node> DeactivateItem { private get; set; } = DeactivateItemStatic;

    /// <a href=""></a>
    public static readonly Func<Node, Node, int> CompareItemStatic = GetDefaultCompareItem();

    /// <a href="">For Deactivation when Dispose() is not enough</a>
    public Func<Node, Node, int> CompareItem { private get; set; } = CompareItemStatic;

    /// <a href="">For the rare times we need to clear a linked list</a>
    public void Destroy() {
      Dispose();
      while (recycleBin.Top != null) recycleBin.Top.Destroy();
    }

    /// <a href="">For the rare times we need to clear a linked list</a>
    public void Dispose() {
      while (Top != null) Top.Dispose();
    }
    #endregion

    #region Node Creation and Movement
    /// <a href="">Add an Item to a List</a>
    public Node Add(params T[] newItems) {
      Node node = null;
      int  idx  = 0;

      if (RecycleBin.Empty && (newItems.Length > 0)) node = Insert(NewNode(newItems[idx++]));

      while (idx < newItems.Length) node = RecycleBin.Top.MoveTo(this).Update(newItems[idx++]);

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

    /// <a href="http://unitydoc.marrington.net/Able#node-creation-and-movement">List for Unused Nodes</a>
    public LinkedList<T> RecycleBin =>
      recycleBin ?? (recycleBin = isRecycleBin ? null : newRecycleBin);

    private LinkedList<T> newRecycleBin => new LinkedList<T>($"{Name} Recycling Bin") {isRecycleBin = true};

    private bool          isRecycleBin;
    private LinkedList<T> recycleBin;
    #endregion

    #region FiFo Stack
    /// <a href="http://unitydoc.marrington.net/Able#fifo">First Node in List or null</a>
    public Node Top;

    /// <a href="http://unitydoc.marrington.net/Able#fifo">Last Node in List or null</a>
    public Node Bottom;

    /// <a href="http://unitydoc.marrington.net/Able#fifo">Second item in the list or null</a>
    public Node Next => Top?.Next;

    /// <a href="http://unitydoc.marrington.net/Able#fifo">Is list empty?</a>
    public bool Empty => (Top == null);

    /// <a href="http://unitydoc.marrington.net/Able#fifo">Calculate number of items in a list</a>
    public int Count {
      get {
        int count = 0;
        Walk((node, next) => ++count != 0);
        return count;
      }
    }

    /// <see cref="Add"/>
    /// <a href="http://unitydoc.marrington.net/Able#fifo">Add an item to the list</a>
    public Node Push(T item) => Add(item);

    /// <a href="http://unitydoc.marrington.net/Able#fifo">Add a node to the list</a>
    public Node Push(Node node) => node.MoveTo(this);

    /// <a href="http://unitydoc.marrington.net/Able#fifo">Retrieve the first list item - <see cref="Node.Recycle"/></a>
    public Node Pop() => Top?.Recycle();
    #endregion

    #region Traversal
    /// <summary>
    /// Walk all nodes - or until the action says otherwise
    /// </summary>
    /// <param name="action">Do something with node. Stop walking if returns false</param>
    /// <returns>node that returned false in the action or null if all done</returns>
    /// <a href="http://unitydoc.marrington.net/Able#walk-all-nodes">Node Walking</a>
    public Node Walk(Func<Node, Node, bool> action) {
      for (Node next, node = Top; node != null; node = next) {
        if (!action(node, next = node.Next)) return node;
      }

      return null;
    }
    #endregion

    #region Debugging
    /// <a href="http://unitydoc.marrington.net/Able#debug-mode">Debug mode logs changes</a>
    // ReSharper disable once StaticMemberInGenericType
    public static bool DebugMode { private get; set; } = false;

    private void DebugMessage(Node node, string append = "") {
      Debug.Log(node.Owner == this
                  ? $"**** LinkedList: Add to {this}"
                  : $"**** LinkedList: move {node.Owner} to {append}{this}");
    }

    /// <a href="http://unitydoc.marrington.net/Able#dump">Return list contents as a string</a>
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