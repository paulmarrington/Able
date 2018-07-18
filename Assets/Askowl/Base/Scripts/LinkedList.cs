using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Askowl {
  public class LinkedList<T> {
    public class Node {
      internal Node Previous, Next;
      public   T    Item;
    }

    public LinkedList(bool sorted) {
      inRange = (t => false);
      if (sorted) Debug.LogWarning($"Can't sort {typeof(T).Name} without a comparator");
    }

    public LinkedList(Func<T, bool> inRangeComparator) { inRange = inRangeComparator; }

    private Node          first, current;
    private Func<T, bool> inRange;

    public T First => Empty ? default(T) : (current = first).Item;

    public T Next => ((current = current?.Next) == null) ? default(T) : current.Item;

    public T Previous =>
      (current?.Previous == null) ? current.Item : (current = current.Previous).Item;

    public bool InRange => (current != null) && inRange(current.Item);

    public bool Empty => ((current = first) == null);

    public object Mark { get { return current ?? first; } set { current = (Node) value; } }

    public object Add(T newItem) => Insert(NewNode(newItem));

    public void Link(object o) {
      var node = o as Node;
      Debug.Log($"**** LinkedList:37 To Be Done!!!"); //#DM#// 18 Jul 2018
    }

    public T Unlink() {
      var node = current;
      current = first;
      Unlink(node);
      return node.Item;
    }

    public T MoveTo(LinkedList<T> to) {
      var node = current;
      current = current.Next;
      to.Unlink(node);
      return node.Item;
    }

    private Node NewNode(T item) {
      Node node = new Node() {Item = item};
      return node;
    }

    private Node Insert(Node nodeToInsert) {
      var next = first;
      if (first == null) return first = nodeToInsert;

      while (inRange(next.Item)) {
        if (next.Next == null) return next.Next = nodeToInsert;

        next = next.Next;
      }

      Unlink(nodeToInsert);
      nodeToInsert.Next     = next;
      nodeToInsert.Previous = next.Previous;
      next.Previous         = nodeToInsert;
      if (next == first) first = nodeToInsert;
      return nodeToInsert;
    }

    private Node Unlink(Node node) {
      if (node.Previous != null) {
        node.Previous.Next = node.Next;
      } else {
        first = node.Next;
      }

      if (node.Next != null) node.Next.Previous = node.Previous;

      node.Previous = node.Next = null;
      return node;
    }
  }
}