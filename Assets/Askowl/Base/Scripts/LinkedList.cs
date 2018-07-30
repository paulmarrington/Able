﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Askowl {
  public class LinkedList<T> {
    public class Node {
      public Node          Previous, Next;
      public LinkedList<T> Owner,    LastOwner;
      public T             Item;

      public bool InRange => Owner.InRange(this);

      public Node MoveTo(LinkedList<T> to) => to.Insert(this);

      public void MoveBack() => LastOwner.Insert(this);

      public override string ToString() => Owner.Name;
    }

    public string Name;

    public LinkedList() { InRange = (t => false); }

    public LinkedList(Func<Node, bool> inRangeComparator) { InRange = inRangeComparator; }

    public Func<Node, bool> InRange;

    public Node First { get; private set; }

    public bool Empty => (First == null);

    public int Count {
      get {
        int count = 0;
        for (Node next = First; next != null; next = next.Next) count++;
        return count;
      }
    }

    public Node Add(T newItem) => Insert(NewNode(newItem));

    private Node NewNode(T item) {
      Node node = new Node() {Item = item, Owner = this};
      return node;
    }

    private Node Insert(Node nodeToInsert) {
      if (DebugMode) DebugMessage(nodeToInsert);
      Unlink(nodeToInsert);
      nodeToInsert.Owner = this;
      if (Empty) return First = nodeToInsert;

      Node next;

      for (next = First; InRange(next); next = next.Next) {
        if (next.Next == null) return next.Next = nodeToInsert;
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
        Debug.Log($"**** LinkedList: Add to {this}"); //#DM#//
      else
        Debug.Log($"**** LinkedList: move {node.Owner} to {this}"); //#DM#//
    }

    private Node Unlink(Node node) {
      if (node.Previous != null) {
        node.Previous.Next = node.Next;
      } else {
        node.Owner.First = node.Next;
      }

      if (node.Next != null) node.Next.Previous = node.Previous;

      node.Previous  = node.Next = null;
      node.LastOwner = node.Owner;
      node.Owner     = null;
      return node;
    }

    /// <inheritdoc />
    public override string ToString() => Name;
  }
}