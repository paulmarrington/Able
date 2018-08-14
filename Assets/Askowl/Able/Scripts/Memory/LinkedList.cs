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
      public bool InRange => Owner.inRange(Item);

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
        MoveTo(Home.recycleBin);
      }

      /// <inheritdoc />
      public override string ToString() => Owner.Name;
    }

    /// <summary>
    /// Name for this list - either provided or generated from the type of item.
    /// </summary>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#name"></a></remarks>
    public string Name { get; private set; }

    private bool          ordered;
    private LinkedList<T> recycleBin;

    // ReSharper disable once StaticMemberInGenericType
    private static int ordinal;

    private Func<T, bool> inRange = (t => true);

    private LinkedList() { recycleBin      = new LinkedList<T>($"{Name} Recycling Bin"); }
    private LinkedList(string name) { Name = name; }

    /// <summary>
    /// Create an anonymous unordered linked list.
    /// </summary>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#create-a-new-linked-list"></a></remarks>
    public static LinkedList<T> New() =>
      new LinkedList<T> {Name = $"{typeof(T).Name}-{ordinal++}", inRange = (t) => true, ordered = false};

    /// <summary>
    /// Create an named unordered linked list.
    /// </summary>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#unordered-linked-lists"></a></remarks>
    public static LinkedList<T> New(string name) =>
      new LinkedList<T> {Name = name, inRange = (t) => true, ordered = false};

    /// <summary>
    /// Create an named ordered linked list by providing a comparison function.
    /// </summary>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#ordered-linked-lists"></a></remarks>
    public static LinkedList<T> New(string name, Func<T, bool> inRange, bool ordered) =>
      new LinkedList<T> {Name = name, inRange = inRange, ordered = ordered};

    /// <summary>
    /// Moves the first node of this list to another.
    /// </summary>
    /// <param name="to">List to move to</param>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#move-the-first-node-to another-list"></a></remarks>
    public Node MoveTo(LinkedList<T> to) => First.MoveTo(to);

    /// <summary>
    /// Call Item.Dispose if the item is IDisposable, then move it to the recycle bin.
    /// </summary>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#dispose-of-a-node"></a></remarks>
    public void Dispose(Node node) => node.Dispose();

    /// <summary>
    /// The node that is in the front of the linked list or null if list is empty
    /// </summary>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#the-first-node"></a></remarks>
    public Node First { get; private set; }

    /// <summary>
    /// Walk all nodes - or until the action says otherwise
    /// </summary>
    /// <param name="action">Do something with node. Stop walking if returns false</param>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#walk-all-nodes"></a></remarks>
    public Node WalkAll(Func<Node, bool> action) {
      for (var node = First; node != null; node = node.Next) {
        if (!action(node)) {
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
    public Node Walk(Func<Node, bool> action) => Walk((node) => inRange(node.Item) && action(node));

    /// <summary>
    /// Use to see if there are any nodes in the list
    /// </summary>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#number-of-nodes"></a></remarks>
    public bool Empty => (First == null);

    /// <summary>
    /// A count of the number of nodes in an active list, not including those in the recycle bin.
    /// </summary>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#number-of-nodes"></a></remarks>
    public int Count { get; private set; }

    public Node Add(T newItem) => Insert(NewNode(newItem));

    public Node Recycle(Func<T> newItemCreator) =>
      (recycleBin?.Empty != true) ? Add(newItemCreator()) : recycleBin.MoveTo(this);

    public Node Recycle() => Recycle(CreateItem);

    public virtual T CreateItem() => default(T);

    private Node NewNode(T item) {
      Node node = new Node() {Item = item, Owner = this, Home = this};
      return node;
    }

    private Node Insert(Node nodeToInsert) {
      if (DebugMode) DebugMessage(nodeToInsert);

      Unlink(nodeToInsert);
      nodeToInsert.Owner = this;
      Count++;
      if (Empty) return First = nodeToInsert;

      Node next = First;

      if (ordered) {
        for (next = First; inRange(next.Item); next = next.Next) {
          if (next.Next == null) return next.Next = nodeToInsert;
        }
      }

      nodeToInsert.Next     = next;
      nodeToInsert.Previous = next.Previous;
      next.Previous         = nodeToInsert;
      if (next == First) First = nodeToInsert;
      return nodeToInsert;
    }

    public static bool DebugMode = false;

    private void DebugMessage(Node node) {
      if (node.Owner == this)
        Debug.Log($"**** LinkedList: Add to {this}");
      else
        Debug.Log($"**** LinkedList: move {node.Owner} to {this}");
    }

    private Node Unlink(Node node) {
      Count--;

      if (node.Previous != null) {
        node.Previous.Next = node.Next;
      } else {
        node.Owner.First = node.Next;
      }

      if (node.Next != null) node.Next.Previous = node.Previous;

      node.Previous = node.Next = null;
      node.Owner    = null;
      return node;
    }

    /// <inheritdoc />
    public override string ToString() => Name;
  }
}