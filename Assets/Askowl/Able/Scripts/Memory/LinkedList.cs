// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

using System;
using UnityEngine;

namespace Askowl {
  /// <summary>
  /// A resource light linked list designed to recycle items rather than leave
  /// them to the garbage collector.
  /// </summary>
  /// <typeparam name="T">Type of item to be stored in each node</typeparam>
  /// <remarks><a href="http://unitydoc.marrington.net/Able#linkedlist-efficient-walking-movement"></a></remarks>
  public class LinkedList<T> {
    #region Node Structure
    /// <summary>
    /// Each node in the linked list. Can b treated as a black box.
    /// </summary>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#nodes"></a></remarks>
    public class Node {
      /// <summary>
      /// Links to nodes on each side - null for end of list
      /// </summary>
      /// <remarks><a href="http://unitydoc.marrington.net/Able#nodes"></a></remarks>
      public Node Previous, Next;

      /// <summary>
      /// List that currently owns this node is called Owner. It is changed by <see cref="MoveTo"/>
      /// Home is the list in which the node was first inserted. It is to this
      /// recycle bin it will return on Dispose().
      /// </summary>
      /// <remarks><a href="http://unitydoc.marrington.net/Able#node-home"></a></remarks>
      public LinkedList<T> Owner, Home;

      /// <summary>
      /// The item you are storing in each node. Can be value type or object
      /// </summary>
      /// <remarks><a href="http://unitydoc.marrington.net/Able#node"></a></remarks>
      public T Item;

      /// <summary>
      /// Test if this item is in range. The rule is used for sorting and
      /// completing foreach().
      /// </summary>
      /// <remarks><a href="http://unitydoc.marrington.net/Able#is-node-inrange"></a></remarks>
      public bool InRange => Owner.InRange(this, Next);

      /// <summary>
      /// Move this node to a new list
      /// </summary>
      /// <param name="to">List target of th move</param>
      /// <remarks><a href="http://unitydoc.marrington.net/Able#move-node-to-another-list"></a></remarks>
      public Node MoveTo(LinkedList<T> to) => to.Insert(this);

      /// <summary>
      /// Call Item.Dispose if the item is IDisposable, then move it to the recycle bin.
      /// </summary>
      /// <remarks><a href="http://unitydoc.marrington.net/Able#dispose-of-this-node"></a></remarks>
      public Node Dispose() {
        (Item as IDisposable)?.Dispose();
        return MoveTo(Home.RecycleBin);
      }

      /// <inheritdoc />
      public override string ToString() => Owner.Name;
    }
    #endregion

    #region New List
    /// <summary>
    /// Name for this list - either provided or generated from the type of item.
    /// </summary>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#name"></a></remarks>
    public string Name { get { return name ?? (name = $"{typeof(T).Name}"); } set { name = value; } }

    private string name;

    /// <summary>
    /// Given the current and next nodes, return true if the current node is in range.<br/>
    ///
    /// <code>new LinkedList&lt;int> {InRange = (node, next) => node.Item &lt; next.Item};</code>
    /// </summary>
    public Func<Node, Node, bool> InRange {
      get { return inRangeFunc; }
      set {
        ordered     = true;
        inRangeFunc = value;
      }
    }

    private Func<Node, Node, bool> inRangeFunc = (node, next) => true;

    /// <summary>
    ///
    /// </summary>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#linked-list-with-custom-create-item"></a></remarks>
    public Func<T> CreateItem { get; set; } = () => default(T);

    private bool ordered;

    // ReSharper disable once StaticMemberInGenericType
    private static int ordinal;
    #endregion

    #region Node Creation and Movement
    /// <summary>
    /// Give a new item, fetch a node from recycling or create it then insert into the linked list
    /// </summary>
    /// <param name="newItem"></param>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#add-an-item-to-the-current-list"></a></remarks>
    public Node Add(T newItem) => (RecycleBin?.Empty != true) ? Insert(NewNode(newItem)) : Recycle(() => newItem);

    /// <summary>
    /// Fetch a node from the recycle bin. If the bin is empty, use the creator
    /// function provided to create and Item and then a Node.
    /// </summary>
    /// <param name="newItemCreator">Function to create a new item</param>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#recycle-a-currently-unused-node"></a></remarks>
    public Node Recycle(Func<T> newItemCreator) =>
      (RecycleBin?.Empty != true) ? Add(newItemCreator()) : RecycleBin.MoveTo(this);

    /// <summary>
    /// Fetch a node from the recycle bin. If the bin is empty, use the creator
    /// function linked to the list to create and Item and then a Node.
    /// The default creator function returns `default(T)`, which is adequate for
    /// move value items. It will be null for object and can be filled in later
    /// if needed.
    /// </summary>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#recycle-a-currently-unused-node"></a></remarks>
    public Node Recycle() => Recycle(CreateItem);

    /// <summary>
    /// Moves the first node of this list to another.
    /// </summary>
    /// <param name="to">List to move to</param>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#move-the-first-node-to another-list"></a></remarks>
    public Node MoveTo(LinkedList<T> to) => Top.MoveTo(to);

    /// <summary>
    /// Call Item.Dispose if the item is IDisposable, then move it to the recycle bin.
    /// </summary>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#disposal-of-a-node"></a></remarks>
    public Node Dispose(Node node) => node.Dispose();

    private Node NewNode(T item) {
      Node node = new Node() {Item = item, Owner = this, Home = this};
      return node;
    }

    /// <summary>
    /// Every linked list has a recycle bin.
    /// </summary>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#node-creation-and-movement"></a></remarks>
    public LinkedList<T> RecycleBin => recycleBin ?? (recycleBin = new LinkedList<T> {Name = $"{Name} Recycling Bin"});

    private Node Insert(Node nodeToInsert) {
      if (DebugMode) DebugMessage(nodeToInsert);

      Unlink(nodeToInsert);
      nodeToInsert.Owner = this;
      Count++;
      if (Empty) return Top = nodeToInsert;

      Node next = Top;

      if (ordered) {
        for (next = Top; InRange(next, next.Next); next = next.Next) {
          if (next.Next == null) return next.Next = nodeToInsert;
        }
      }

      nodeToInsert.Next     = next;
      nodeToInsert.Previous = next.Previous;
      next.Previous         = nodeToInsert;
      if (next == Top) Top = nodeToInsert;
      return nodeToInsert;
    }

    private void Unlink(Node node) {
      Count--;

      if (node.Previous != null) {
        node.Previous.Next = node.Next;
      } else {
        node.Owner.Top = node.Next;
      }

      if (node.Next != null) node.Next.Previous = node.Previous;

      node.Previous = node.Next = null;
      node.Owner    = null;
    }

    private LinkedList<T> recycleBin;
    #endregion

    #region FiFo Stack
    /// <summary>
    /// The node that is in the front of the linked list or null if list is empty
    /// </summary>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#fifo"></a></remarks>
    public Node Top { get; private set; }

    /// <summary>
    /// The node that is second from the front of the linked list or null if list has less than two entries
    /// </summary>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#fifo"></a></remarks>
    public Node Next => Top?.Next;

    /// <summary>
    /// Use to see if there are any nodes in the list
    /// </summary>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#fifo"></a></remarks>
    public bool Empty => (Top == null);

    /// <summary>
    /// A count of the number of nodes in an active list, not including those in the recycle bin.
    /// </summary>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#fifo"></a></remarks>
    public int Count { get; private set; }

    /// <summary>
    /// Push a reference to or value of an item onto the FIFO stack. <see cref="Add"/>
    /// </summary>
    /// <param name="item"></param>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#fifo"></a></remarks>
    public Node Push(T item) => Add(item);

    /// <summary>
    ///
    /// </summary>
    /// Push an already existing node. It could have been on the same or another list
    /// or in a recycling bin. <see cref="MoveTo"/>
    /// <param name="node"></param>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#fifo"></a></remarks>
    public Node Push(Node node) => node.MoveTo(this);

    /// <summary>
    /// Pop a node off the top of the fifo stack. <see cref="Dispose"/>
    /// </summary>
    /// <returns></returns>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#fifo"></a></remarks>
    public Node Pop() => Dispose(Top);
    #endregion

    #region Walking
    /// <summary>
    /// Walk all nodes - or until the action says otherwise
    /// </summary>
    /// <param name="action">Do something with node. Stop walking if returns false</param>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#walk-all-nodes"></a></remarks>
    public Node WalkAll(Func<Node, Node, bool> action) {
      for (var node = Top; node != null; node = node.Next) {
        if (!action(node, node.Next)) {
          return node;
        }
      }

      return null;
    }

    /// <summary>
    /// Walk nodes that are in range - or until the action says otherwise
    /// </summary>
    /// <param name="action">Do something with node. Stop walking if returns false</param>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#walk-nodes-in-range"></a></remarks>
    public Node Walk(Func<Node, Node, bool> action) =>
      WalkAll((node, next) => InRange(node, next) && action(node, next));
    #endregion

    #region Debugging
    /// <summary>
    /// Set true and any call that modifies a linked list will write to the debug
    /// log. Invaluable when you want to track object usage.
    /// </summary>
    // ReSharper disable once StaticMemberInGenericType
    public static bool DebugMode { private get; set; } = false;

    private void DebugMessage(Node node) {
      if (node.Owner == this) {
        Debug.Log($"**** LinkedList: Add to {this}");
      } else {
        Debug.Log($"**** LinkedList: move {node.Owner} to {this}");
      }
    }

    /// <inheritdoc />
    public override string ToString() => Name;
    #endregion
  }
}