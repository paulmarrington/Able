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
      public void Dispose() {
        (Item as IDisposable)?.Dispose();
        MoveTo(Home.RecycleBin);
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
    public string Name { get; set; }

    public Func<Node, Node, bool> InRange {
      get { return inRangeFunc; }
      set {
        ordered     = true;
        inRangeFunc = value;
      }
    }

    private Func<Node, Node, bool> inRangeFunc = (node, next) => true;

    public  Func<T> CreateItem { get { return createItemFunc; } set { createItemFunc = value; } }
    private Func<T> createItemFunc = () => default(T);

    private bool ordered;

    // ReSharper disable once StaticMemberInGenericType
    private static int ordinal;
    #endregion

    #region Node Creation and Movement
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
    public void Dispose(Node node) => node.Dispose();

    /// <summary>
    /// Give a new item, fetch a node from recycling or create it then insert into the linked list
    /// </summary>
    /// <param name="newItem"></param>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#add-an-item-to-the-current-list"></a></remarks>
    public Node Add(T newItem) => (RecycleBin?.Empty != true) ? Insert(NewNode(newItem)) : Recycle(() => newItem);

    /// <summary>
    ///
    /// </summary>
    /// <param name="newItemCreator"></param>
    /// <returns></returns>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#retrieve-a-node-from-the-recycle-bin"></a></remarks>
    public Node Recycle(Func<T> newItemCreator) =>
      (RecycleBin?.Empty != true) ? Add(newItemCreator()) : RecycleBin.MoveTo(this);

    public Node Recycle() => Recycle(CreateItem);

    private Node NewNode(T item) {
      Node node = new Node() {Item = item, Owner = this, Home = this};
      return node;
    }

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

    private Node Unlink(Node node) {
      Count--;

      if (node.Previous != null) {
        node.Previous.Next = node.Next;
      } else {
        node.Owner.Top = node.Next;
      }

      if (node.Next != null) node.Next.Previous = node.Previous;

      node.Previous = node.Next = null;
      node.Owner    = null;
      return node;
    }

    private LinkedList<T> RecycleBin => recycleBin ?? new LinkedList<T> {Name = $"{Name} Recycling Bin"};

    private LinkedList<T> recycleBin;
    #endregion

    #region FiFo Stack
    /// <summary>
    /// The node that is in the front of the linked list or null if list is empty
    /// </summary>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#the-first-node"></a></remarks>
    public Node Top { get; private set; }

    /// <summary>
    /// Use to see if there are any nodes in the list
    /// </summary>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#number-of-nodes"></a></remarks>
    public bool Empty => (Top == null);

    /// <summary>
    /// A count of the number of nodes in an active list, not including those in the recycle bin.
    /// </summary>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#number-of-nodes"></a></remarks>
    public int Count { get; private set; }
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
    public Node Walk(Func<Node, Node, bool> action) => Walk((node, next) => InRange(node, next) && action(node, next));
    #endregion

    #region Debugging
    public static bool DebugMode = false;

    private void DebugMessage(Node node) {
      if (node.Owner == this)
        Debug.Log($"**** LinkedList: Add to {this}");
      else
        Debug.Log($"**** LinkedList: move {node.Owner} to {this}");
    }

    /// <inheritdoc />
    public override string ToString() => Name;
    #endregion
  }
}