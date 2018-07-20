using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Askowl {
  public class LinkedList<T> {
    public class Node {
      public Node          Previous, Next;
      public LinkedList<T> owner,    lastOwner;
      public T             Item;

      public bool InRange => owner.InRange(Item);

      public Node MoveTo(LinkedList<T> to) => to.Insert(this);
    }

    public LinkedList() { InRange = (t => false); }

    public LinkedList(Func<T, bool> inRangeComparator) { InRange = inRangeComparator; }

    public Func<T, bool> InRange;

    public Node First { get; private set; }

    public bool Empty => (First == null);

    public Node Add(T newItem) => Insert(NewNode(newItem));

    private Node NewNode(T item) {
      Node node = new Node() {Item = item, owner = this};
      return node;
    }

    private Node Insert(Node nodeToInsert) {
      Unlink(nodeToInsert);
      nodeToInsert.lastOwner = nodeToInsert.owner;
      nodeToInsert.owner     = this;
      if (Empty) return First = nodeToInsert;

      Node next;

      for (next = First; InRange(next.Item); next = next.Next) {
        if (next.Next == null) return next.Next = nodeToInsert;
      }

      nodeToInsert.Next     = next;
      nodeToInsert.Previous = next.Previous;
      next.Previous         = nodeToInsert;
      if (next == First) First = nodeToInsert;
      return nodeToInsert;
    }

    private Node Unlink(Node node) {
      if (node.Previous != null) {
        node.Previous.Next = node.Next;
      } else {
        First = node.Next;
      }

      if (node.Next != null) node.Next.Previous = node.Previous;

      node.Previous = node.Next = null;
      node.owner    = null;
      return node;
    }
  }
}