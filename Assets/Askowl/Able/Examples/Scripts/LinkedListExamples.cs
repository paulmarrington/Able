// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

#if UNITY_EDITOR && Able

using System;
using NUnit.Framework;
using Assert = UnityEngine.Assertions.Assert;

namespace Askowl.Examples {
  /// <remarks><a href="http://unitydoc.marrington.net/Able#linkedlist"></a></remarks>
  public class LinkedListExamples {
    /// <remarks><a href="http://unitydoc.marrington.net/Able#unordered-link-lists"></a></remarks>
    [Test]
    public LinkedList<int> NewUnordered() {
      var linkedList = new LinkedList<int> {Name = "Freddy"};
      Assert.AreEqual("Freddy", linkedList.Name);
      return linkedList;
    }

    /// <remarks><a href="http://unitydoc.marrington.net/Able#ordered-link-lists"></a></remarks>
    [Test]
    public LinkedList<int> NewOrdered() {
      var linkedList = new LinkedList<int> {InRange = (node, next) => node.Item < next.Item};
      Assert.IsTrue(!string.IsNullOrWhiteSpace(linkedList.Name));
      return linkedList;
    }

    /// <remarks><a href="http://unitydoc.marrington.net/Able#linked-list-with-custom-create-item"></a></remarks>
    [Test]
    public LinkedList<int> NewWithCustomCreateItem() {
      var linkedList = new LinkedList<int> {CreateItem = () => 33};
      return linkedList;
    }

    /// <remarks><a href="http://unitydoc.marrington.net/Able#add-an-item-to-the-current-list"></a></remarks>
    [Test]
    public void AddUnordered() {
      var linkedList = NewUnordered();
      Assert.IsTrue(linkedList.Empty);
      linkedList.Add(3);
      linkedList.Add(1);
      Assert.IsFalse(linkedList.Empty);
      Assert.AreEqual(expected: 1, actual: linkedList.Count);

      Assert.AreEqual(expected: 3, actual: linkedList.Top.Item);
      Assert.AreEqual(expected: 1, actual: linkedList.Next.Item);
    }

    /// <remarks><a href="http://unitydoc.marrington.net/Able#add-an-item-to-the-current-list"></a></remarks>
    [Test]
    public void AddOrdered() {
      var linkedList = NewOrdered();
      Assert.IsTrue(linkedList.Empty);
      linkedList.Add(3);
      linkedList.Add(1);
      Assert.IsFalse(linkedList.Empty);
      Assert.AreEqual(expected: 1, actual: linkedList.Count);

      Assert.AreEqual(expected: 1, actual: linkedList.Top.Item);
      Assert.AreEqual(expected: 3, actual: linkedList.Next.Item);
    }

    /// <remarks><a href="http://unitydoc.marrington.net/Able#recycle-a-currently-unused-node"></a></remarks>
    [Test]
    public void RecycleWithAdd() {
      var linkedList = NewWithCustomCreateItem();
      var node       = linkedList.Add(3);

      node.Dispose();
      Assert.IsTrue(linkedList.Empty);
      Assert.AreEqual(expected: 1, actual: linkedList.RecycleBin.Count);

      node = linkedList.Add(4);
      Assert.IsTrue(linkedList.RecycleBin.Empty);
      Assert.AreEqual(expected: 1, actual: linkedList.Count);
      Assert.AreEqual(expected: 4, actual: linkedList.Top.Item);
    }

    /// <remarks><a href="http://unitydoc.marrington.net/Able#recycle-a-currently-unused-node"></a></remarks>
    [Test]
    public void RecycleWithCreateItem() {
      var linkedList = NewWithCustomCreateItem();

      var newFromRecycler = linkedList.Recycle();
      Assert.AreEqual(expected: 1,              actual: linkedList.Count);
      Assert.AreEqual(expected: 33,             actual: newFromRecycler.Item);
      Assert.AreEqual(expected: linkedList.Top, actual: newFromRecycler);
    }

    /// <remarks><a href="http://unitydoc.marrington.net/Able#recycle-a-currently-unused-node"></a></remarks>
    [Test]
    public void RecycleWithWithoutCreateItem() {
      var linkedList = NewUnordered();

      var newFromRecycler = linkedList.Recycle();
      Assert.AreEqual(expected: 1,            actual: linkedList.Count);
      Assert.AreEqual(expected: default(int), actual: newFromRecycler.Item);
    }

    /// <remarks><a href="http://unitydoc.marrington.net/Able#recycle-a-currently-unused-node"></a></remarks>
    [Test]
    public void RecycleWithCreatorFunction() {
      var linkedList      = NewUnordered();
      var newFromRecycler = linkedList.Recycle(() => 1234);
      Assert.AreEqual(expected: 1234, actual: newFromRecycler.Item);
    }

    /// <remarks><a href="http://unitydoc.marrington.net/Able#move-items-between-lists"></a></remarks>
    [Test]
    public void ListMoveTo() {
      var list1 = NewUnordered();
      var list2 = NewUnordered();

      var node1 = list1.Add(21);
      var node2 = list1.Add(18);

      list1.MoveTo(list2);

      Assert.AreEqual(expected: 1,     actual: list1.Count);
      Assert.AreEqual(expected: 1,     actual: list2.Count);
      Assert.AreEqual(expected: node1, actual: list1.Top);
      Assert.AreEqual(expected: node2, actual: list2.Top);
    }

    /// <remarks><a href="http://unitydoc.marrington.net/Able#move-items-between-lists"></a></remarks>
    [Test]
    public void UnorderedMoveTo() {
      var list1 = NewUnordered();
      var list2 = NewUnordered();

      var node1 = list1.Add(21);
      var node2 = list1.Add(18);

      node1.MoveTo(list2);

      Assert.AreEqual(expected: node1, actual: list2.Top);
      Assert.AreEqual(expected: node2, actual: list1.Top);
    }

    /// <remarks><a href="http://unitydoc.marrington.net/Able#move-items-between-lists"></a></remarks>
    [Test]
    public void OrderedMoveTo() {
      var list1 = NewUnordered();
      var list2 = NewOrdered();

      var node21 = list1.Add(21);
      var node18 = list1.Add(18);
      var node44 = list1.Add(44);

      node21.MoveTo(list2);
      node44.MoveTo(list2);
      list1.MoveTo(list2);

      Assert.AreEqual(expected: 0,      actual: list1.Count);
      Assert.AreEqual(expected: 3,      actual: list2.Count);
      Assert.AreEqual(expected: node18, actual: list2.Top);
      Assert.AreEqual(expected: node21, actual: list2.Next);
      node18.Dispose();
      Assert.AreEqual(expected: node44, actual: list2.Next);
    }

    /// <remarks><a href="http://unitydoc.marrington.net/Able#Node Disposal"></a></remarks>
    [Test]
    public void Dispose() {
      var linkedList = NewOrdered();
      var node       = linkedList.Add(5);

      node.Dispose();
      Assert.IsTrue(linkedList.Empty);
      Assert.AreEqual(expected: 1, actual: linkedList.RecycleBin.Count);
      Assert.AreEqual(expected: 5, actual: linkedList.RecycleBin.Top.Item);
    }

    /// <remarks><a href="http://unitydoc.marrington.net/Able#Node Disposal"></a></remarks>
    [Test]
    public void IDisposable() {
      var linkedList = new LinkedList<DisposableInt>();
      var node       = linkedList.Add(new DisposableInt {counter = 333});
      Assert.AreEqual(expected: 33, actual: node.Item.counter);

      node.Dispose();
      Assert.AreEqual(expected: -1, actual: node.Item.counter);
    }

    private class DisposableInt : IDisposable {
      internal int  counter;
      public   void Dispose() => counter = -1;
    }

    /// <remarks><a href="http://unitydoc.marrington.net/Able#Node Disposal"></a></remarks>
    [Test]
    public void DisposeAfterMove() {
      var list1 = new LinkedList<int>();
      var list2 = new LinkedList<int>();

      var node = list1.Add(12);
      node.MoveTo(list2);

      Assert.AreEqual(expected: node, actual: list2.Top);

      node.Dispose();
      Assert.IsFalse(list1.RecycleBin.Empty);

      Assert.IsTrue(list1.Empty);
      Assert.IsTrue(list2.Empty);
      Assert.IsTrue(list2.RecycleBin.Empty);
    }

    /// <remarks><a href="http://unitydoc.marrington.net/Able#fifo"></a></remarks>
    [Test]
    public void Fifo() {
      var list1 = new LinkedList<int>();
      var list2 = new LinkedList<int>();

      var node1 = list1.Push(23);
      var node2 = list1.Push(14);

      var node3 = list2.Push(99);
      list1.Push(node3);
      Assert.IsTrue(list2.Empty);
      Assert.IsTrue(list2.Empty);
      Assert.AreEqual(expected: 3, actual: list1.Count);

      var node4 = list1.Pop();
      Assert.AreEqual(node3, node4);
      Assert.AreEqual(node4, list1.RecycleBin.Top);

      Assert.AreEqual(node2, list1.Top);
      Assert.AreEqual(node1, list1.Next);

      var node5 = list1.Push(567);
      Assert.AreEqual(3, list1.Count);
      var node6 = list1.Top;
      Assert.AreEqual(567, node6.Item);
    }

    /// <remarks><a href="http://unitydoc.marrington.net/Able#linkedlist-walk-inrange"></a></remarks>
    [Test]
    public void WalkInRange() { }

    /// <remarks><a href="http://unitydoc.marrington.net/Able#linkedlist-add-inrange"></a></remarks>
    [Test]
    public void AddInRange() { }

    /// <remarks><a href="http://unitydoc.marrington.net/Able#linkedlist-oncomplete"></a></remarks>
    [Test]
    public void OnComplete() { }

    /// <remarks><a href="http://unitydoc.marrington.net/Able#linkedlist-moveto"></a></remarks>
    [Test]
    public void MoveTo() { }

    /// <remarks><a href="http://unitydoc.marrington.net/Able#linkedlist-first"></a></remarks>
    [Test]
    public void First() { }

    /// <remarks><a href="http://unitydoc.marrington.net/Able#linkedlist-walk"></a></remarks>
    [Test]
    public void Walk() { }

    /// <remarks><a href="http://unitydoc.marrington.net/Able#linkedlist-empty"></a></remarks>
    [Test]
    public void Empty() { }

    /// <remarks><a href="http://unitydoc.marrington.net/Able#linkedlist-count"></a></remarks>
    [Test]
    public void Count() { }

    /// <remarks><a href="http://unitydoc.marrington.net/Able#linkedlist-createitem"></a></remarks>
    [Test]
    public void CreateItem() { }

    /// <remarks><a href="http://unitydoc.marrington.net/Able#linkedlist-recycle"></a></remarks>
    [Test]
    public void Recycle() { }

    /// <remarks><a href="http://unitydoc.marrington.net/Able#linkedlist-debugmode"></a></remarks>
    [Test]
    public void DebugMode() { }

    /// <remarks><a href="http://unitydoc.marrington.net/Able#linkedlist-tostring"></a></remarks>
    [Test]
    public void ToStringExample() { }
  }
}
#endif