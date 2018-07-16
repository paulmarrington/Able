using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Askowl {
  public class LinkedList<T> : IEnumerable<T> {
    public class Node {
      internal Node Previous, Next;
      public   T    Item;
    }

    private Node          first, current;
    private Func<T, bool> insertBeforeHereComparator = (t) => true;

    public T First => (current = first).Item;

    public bool Empty => (first == null);

    public Node Mark { get { return current ?? first; } set { current = value; } }

    public void Add(T newItem) { Insert(NewNode(newItem)); }

    public void Link(object o) {
      var node = o as Node;
    }

    public T Unlink() {
      current = first;
      Unlink(current);
      return current.Item;
    }

//    public void InsertBefore(T newItem, Func<T, bool> insertBeforeHere) {
//      InsertBefore(NewNode(newItem), insertBeforeHere);
//    }

    public T MoveTo(LinkedList<T> to) {
      var node = current;
      current = current.Next;
      to.Insert(Unlink(node));
      return node.Item;
    }

    public IEnumerator<T> GetEnumerator() {
      for (var node = first; node != null; node = node.Next) {
        current = node;
        yield return node.Item;
      }

      current = first;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private Node NewNode(T item) {
      Node node = new Node() {Item = item};
      return node;
    }

    private Node Insert(Node nodeToInsert) {
      var next = first;
      if (first == null) return first = nodeToInsert;

      while (true) {
        if (insertBeforeHereComparator(next.Item)) {
          Unlink(nodeToInsert);
          nodeToInsert.Next     = next;
          nodeToInsert.Previous = next.Previous;
          next.Previous         = nodeToInsert;
          if (next == first) first = nodeToInsert;
          return nodeToInsert;
        }

        if (next.Next == null) return next.Next = nodeToInsert;
      }
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