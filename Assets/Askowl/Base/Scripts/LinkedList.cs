using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Askowl {
  public class LinkedList<T> : IEnumerable<T> {
    public class Node {
      internal Node          Previous, Next;
      internal T             Item;
    }

    private Node          first, current;
    private Func<T, bool> insertBeforeHereComparator = (t) => true;

    public T First => first.Item;

    public bool Empty => (first == null);

    public Node Mark { get { return current; } set { current = value; } }

    public void Add(T newItem) { Insert(new Node() {Item = newItem}); }

    public void InsertBefore(T newItem, Func<T, bool> insertBeforeHere) {
      InsertBefore(new Node() {Item = newItem}, insertBeforeHere);
    }

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

    private void Insert(Node node) { InsertBefore(node, insertBeforeHereComparator); }

    private Node InsertBefore(Node mark, Node nodeToInsert) {
      if (mark == null) mark = first;

      Unlink(nodeToInsert);

      if (mark == null) {
        first = nodeToInsert;
      } else {
        nodeToInsert.Next     = mark;
        nodeToInsert.Previous = mark.Previous;
        mark.Previous         = nodeToInsert;
        if (mark == first) first = nodeToInsert;
      }

      return nodeToInsert;
    }

    private Node InsertBefore(Node nodeToInsert, Func<T, bool> insertBeforeHere) {
      var current = first;
      if (first == null) return first = nodeToInsert;

      while (true) {
        if (insertBeforeHere(current.Item)) return InsertBefore(current, nodeToInsert);
        if (current.Next == null) return current.Next = nodeToInsert;
      }
    }

    private Node Unlink(Node node) {
      if (node.Previous != null) node.Previous.Next = node.Next;
      if (node.Next     != null) node.Next.Previous = node.Previous;

      node.Previous = node.Next = null;
      return node;
    }
  }
}