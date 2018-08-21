// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

using System;
using System.Text;
using UnityEngine;

namespace Askowl {
  /// <summary>
  /// A resource light linked list designed to recycle items rather than leave
  /// them to the garbage collector.
  /// </summary>
  /// <typeparam name="T">Type of item to be stored in each node</typeparam>
  /// <remarks><a href="http://unitydoc.marrington.net/Able#linkedlist-a-different-perspective">LinkedList - a different perspective</a></remarks>
  public class LinkedList<T> : IDisposable {
    /// <summary>
    /// Each node in the linked list is one of these. Can be treated as a black box (mostly).
    /// </summary>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#nodes">LinkedList node</a></remarks>
    public class Node : IDisposable {
      /// <remarks><a href="http://unitydoc.marrington.net/Able#nodes">LinkedList node</a></remarks>
      public Node Previous, Next;

      /// <summary>
      /// List that currently owns this node is called Owner. It is changed by <see cref="MoveTo"/>
      /// Home is the list in which the node was first inserted. It is to this
      /// recycle bin it will return on Dispose().
      /// </summary>
      /// <remarks><a href="http://unitydoc.marrington.net/Able#node-home">Node Home List</a></remarks>
      public LinkedList<T> Owner, Home;

      /// <summary>
      /// It can be value type (int, float ... struct) or object (class instance)
      /// </summary>
      /// <remarks><a href="http://unitydoc.marrington.net/Able#nodes">LinkedList node</a></remarks>
      public T Item;

      /// <remarks><a href="http://unitydoc.marrington.net/Able#node-comparison">NodeComparison Operator</a></remarks>
      public static bool operator <(Node left, Node right) => left.Owner.compare(left, right) < 0;

      /// <remarks><a href="http://unitydoc.marrington.net/Able#node-comparison">NodeComparison Operator</a></remarks>
      public static bool operator <=(Node left, Node right) => left.Owner.compare(left, right) <= 0;

      /// <remarks><a href="http://unitydoc.marrington.net/Able#node-comparison">NodeComparison Operator</a></remarks>
      public static bool operator >(Node left, Node right) => left.Owner.compare(left, right) > 0;

      /// <remarks><a href="http://unitydoc.marrington.net/Able#node-comparison">NodeComparison Operator</a></remarks>
      public static bool operator >=(Node left, Node right) => left.Owner.compare(left, right) >= 0;

      /// <remarks><a href="http://unitydoc.marrington.net/Able#move-node-to-another-list">Move Node to Another List</a></remarks>
      public Node MoveTo(LinkedList<T> to) => to.Insert(this);

      /// <remarks><a href="http://unitydoc.marrington.net/Able#move-node-to-another-list">Move Node to Another List</a></remarks>
      public Node MoveToEndOf(LinkedList<T> to) => to.Append(this);

      /// <summary>
      /// Calls Item.Dispose if the item is IDisposable, then move it to the recycle bin.
      /// </summary>
      /// <remarks><a href="http://unitydoc.marrington.net/Able#dispose-of-this-node">Dispose of a Node</a></remarks>
      public Node Discard() {
        Home.DeactivateItem(this);
        MoveToEndOf(Home.RecycleBin);

        return this;
      }

      /// <inheritdoc />
      public void Dispose() => Discard();

      /// <inheritdoc />
      /// <remarks><a href="http://unitydoc.marrington.net/Able#node-name">Node Naming Convention</a></remarks>
      public override string ToString() => $"{Home} // {Owner}:: {Item}";

      /// <remarks><a href="http://unitydoc.marrington.net/Able#update-node-contents">Update Node Contents</a></remarks>
      public Node Update(T newItem) {
        Item = newItem;
        return this;
      }
    }

    #region New List
    /// <remarks><a href="http://unitydoc.marrington.net/Able#name">Linked List Name</a></remarks>
    public string Name { get { return name ?? (name = $"{typeof(T).Name}-{++ordinal}"); } set { name = value; } }

    private string name;

    /// <code>new LinkedList&lt;int> {InRange = (node, next) => node.Item &lt; next.Item};</code>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#ordered-linked-lists">Node Comparison</a></remarks>
    public Func<Node, Node, int> CompareNodes {
      get { return compare; }
      set {
        ordered = true;
        compare = value;
      }
    }

    private Func<Node, Node, int> compare = (left, right) => 0;

    /// <summary>
    /// Create an instance of T, doing whatever is necessary to prepare (i.e. open connection)
    /// </summary>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#linked-list-with-custom-create-item">For Implicit Default Item Creation</a></remarks>
    public Func<T> CreateItem { private get; set; } = () => default(T);

    /// <summary>
    /// Activate an instance of T when it is taken from the recycler, doing
    /// whatever is necessary to prepare it for life in a Linked List
    /// </summary>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#linked-list-with-custom-create-item">For Implicit Default Item Creation</a></remarks>
    public Action<Node> ReactivateItem { private get; set; } = (node) => { };

    public Action<Node> DeactivateItem { private get; set; } = (node) => (node.Item as IDisposable)?.Dispose();

    /// <inheritdoc />
    /// <remarks><a href="http://unitydoc.marrington.net/Able#linked-listdisposal">For the rare times we need to clear a linked list</a></remarks>
    public void Discard() {
      Dispose();
      if (recycleBin == null) return;

      for (Node node = recycleBin.Top, next = null; node != null; node = next, next = node.Next) {
        node.Home     = node.Owner = null;
        node.Previous = node.Next  = null;
        (node.Item as IDisposable)?.Dispose();
      }
    }

    /// <remarks><a href="http://unitydoc.marrington.net/Able#linked-listdisposal">For the rare times we need to clear a linked list</a></remarks>
    public void Dispose() {
      while (Top != null) Top.Dispose();
    }

    private bool ordered;

    // ReSharper disable once StaticMemberInGenericType
    private static int ordinal;
    #endregion

    #region Node Creation and Movement
    /// <summary>
    /// Give a new item, fetch a node from recycling or create it then insert into the linked list.
    /// </summary>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#add-an-item-to-the-current-list">Add an Item to a List</a></remarks>
    public Node Add(T newItem) =>
      RecycleBin.Empty ? Insert(NewNode(newItem)) : RecycleBin.MoveFirstNodeTo(this).Update(newItem);

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

      var node = RecycleBin.MoveFirstNodeTo(this);
      ReactivateItem(node);
      return node;
    }

    /// <remarks><a href="http://unitydoc.marrington.net/Able#move-the-first-node-to-another-list">Move Node Between Lists</a></remarks>
    public Node MoveFirstNodeTo(LinkedList<T> to) => Top.MoveTo(to);

    /// <remarks><a href="http://unitydoc.marrington.net/Able#move-the-first-node-to-another-list">Move Node Between Lists</a></remarks>
    public Node MoveFirstNodeToEndOf(LinkedList<T> to) => Top.MoveToEndOf(to);

    /// <summary>
    /// Call Item.Dispose if the item is IDisposable, then move it to the recycle bin.
    /// </summary>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#disposal-of-a-node">Dispose a Node</a></remarks>
    public Node Discard(Node node) => node.Discard();

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
    public LinkedList<T> RecycleBin => recycleBin ?? (recycleBin = isRecycleBin ? null : newRecycleBin);

    private LinkedList<T> newRecycleBin => new LinkedList<T> {isRecycleBin = true, Name = $"{Name} Recycling Bin"};
    private bool          isRecycleBin;
    private LinkedList<T> recycleBin;
    #endregion

    #region FiFo Stack
    /// <summary>
    /// The node that is in the front/start of the linked list or null if list is empty
    /// </summary>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#fifo">First Node in List</a></remarks>
    public Node Top;

    /// <summary>
    /// The node that is in the back/end of the linked list or null if list is empty
    /// </summary>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#fifo">Last Node in List</a></remarks>
    public Node Bottom;

    /// <summary>
    /// The node that is second from the front of the linked list or null if list has less than two entries
    /// </summary>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#fifo">Second item in the list</a></remarks>
    public Node Next => Top?.Next;

    /// <remarks><a href="http://unitydoc.marrington.net/Able#fifo">Is list empty?</a></remarks>
    public bool Empty => (Top == null);

    /// <summary>
    /// A count of the number of nodes in an active list, not including those in the recycle bin.
    /// </summary>
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

    /// <see cref="MoveFirstNodeTo"/>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#fifo">Add a node to the list</a></remarks>
    public Node Push(Node node) => node.MoveTo(this);

    /// <see cref="Discard"/>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#fifo">Retrieve the first list item</a></remarks>
    public Node Pop() => Discard(Top);
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
    /// <summary>
    /// Set true and any call that modifies a linked list will write to the debug
    /// log. Invaluable when you want to track object usage.
    /// </summary>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#debug-mode">Set debug mode</a></remarks>
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