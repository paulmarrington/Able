// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

#if UNITY_EDITOR && Able

using System;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEditor.Graphs;
using UnityEngine;
using UnityEngine.TestTools;
using Assert = UnityEngine.Assertions.Assert;

// ReSharper disable StaticMemberInGenericType disable ClassNeverInstantiated.Global disable MissingXmlDoc disable UnusedMember.Local disable MemberHidesStaticFromOuterClass

namespace Askowl.Examples {
  /// <remarks><a href="http://unitydoc.marrington.net/Able#linkedlist-a-different-perspective">LinkedList - a different perspective</a></remarks>
  public class LinkedListExamples {
    public sealed class SealedClass {
      public string State { get; set; } = "New";
    }

    public sealed class SealedClassProcessed {
      public string State { get; set; } = "New";
    }

    private struct MyStruct {
      public string State { get; set; }

      // A struct is useless in a linked list if not initialised
      private static MyStruct CreateItem() => new MyStruct {State = "StructCreated"};
    }

    private class MyClassRaw {
      public string State { get; set; }
    }

    private class MyClassProcessed {
      private static MyClassProcessed CreateItem() => new MyClassProcessed {State = "Created"};

      private void DeactivateItem()                    => State = "Deactivated";
      private void ReactivateItem()                    => State += "Reactivated";
      private int  CompareItem(MyClassProcessed other) => State.CompareTo(other);

      public string State { get; set; }
    }

    static LinkedListExamples() {
      Cache<SealedClassProcessed>.CreateItem =
        () => new SealedClassProcessed {State = "CreateItem"};

      Cache<SealedClassProcessed>.DeactivateItem = (seal) => seal.State =  "DeactivateItem";
      Cache<SealedClassProcessed>.ReactivateItem = (seal) => seal.State += " ReactivateItem";
    }

    /// <remarks><a href="">Set a list name</a></remarks>
    [Test]
    public void ListName() {
      var linkedList = LinkedList<int>.Instance("List Name");
      Assert.AreEqual("List Name", linkedList.Name);
      linkedList = LinkedList<int>.Instance("");
      Assert.AreEqual("int-1", linkedList.Name);
      linkedList = LinkedList<int>.Instance(null);
      Assert.AreEqual("int-2", linkedList.Name);
    }

    /// <remarks><a href="">Are there active entries in the list?</a></remarks>
    [Test]
    public void Empty() {
      var linkedList = LinkedList<int>.Instance("Empty");
      Assert.IsTrue(linkedList.Empty);
      linkedList.Add(3);
      Assert.IsFalse(linkedList.Empty);
    }

    /// <remarks><a href="">Count number of items in a list</a></remarks>
    [Test]
    public void Count() {
      var linkedList = LinkedList<int>.Instance("Count");
      linkedList.Add(1);
      linkedList.Add(3);
      Assert.AreEqual(expected: 2, actual: linkedList.Count);
    }

    /// <remarks><a href="">First item on the linked list</a></remarks>
    [Test]
    public void Top() {
      var linkedList = LinkedList<int>.Instance("Top");
      Assert.IsNull(linkedList.Top);
      linkedList.Add(1);
      linkedList.Add(3);
      Assert.AreEqual(expected: 3, actual: linkedList.Top.Item);
    }

    /// <remarks><a href="">Second item on the linked list</a></remarks>
    [Test]
    public void Next() {
      var linkedList = LinkedList<int>.Instance("Next");
      Assert.IsNull(linkedList.Next);
      linkedList.Add(1);
      Assert.IsNull(linkedList.Next);
      linkedList.Add(3);
      Assert.AreEqual(expected: 1, actual: linkedList.Next.Item);
    }

    /// <remarks><a href="">Add an Item</a></remarks>
    [Test]
    public void AddUnordered() {
      var linkedList = LinkedList<int>.Instance("Add Unordered");
      linkedList.Add(1);
      linkedList.Add(3);
      Assert.AreEqual(expected: 3, actual: linkedList.Top.Item);
      Assert.AreEqual(expected: 1, actual: linkedList.Next.Item);
    }

    /// <remarks><a href="">Add an Item specifying order</a></remarks>
    [Test]
    public void AddOrderedSealed() {
      var linkedList = LinkedList<int>.Instance("Add Ordered Sealed");

      // Normally this would go into a static constructor that is loaded before first use
      LinkedList<int>.CompareItem = (node, other) => node.Item.CompareTo(other.Item);

      linkedList.Add(1);
      linkedList.Add(3);

      Assert.AreEqual(expected: 1, actual: linkedList.Top.Item);
      Assert.AreEqual(expected: 3, actual: linkedList.Next.Item);
    }

    /// <remarks><a href="">Add an Item specifying order</a></remarks>
    [Test]
    public void AddOrderedUnsealed() {
      var linkedList = LinkedList<MyClassProcessed>.Instance("Add Ordered Unsealed");

      linkedList.Add(new MyClassProcessed {State = "First Added"});
      linkedList.Fetch().Item.State = "Second uses Fetch";

      Assert.AreEqual(expected: "Second uses Fetch", actual: linkedList.Top.Item.State);
      Assert.AreEqual(expected: "First Added",       actual: linkedList.Next.Item.State);
    }

    /// <remarks><a href="http://unitydoc.marrington.net/Able#recycle-a-currently-unused-node">Dispose by Recycling</a></remarks>
    [Test]
    public void RecycleWithAdd() {
      var linkedList = new LinkedList<int> {CreateItem = () => 33};
      var node       = linkedList.Add(3);
      node.Dispose();
      Assert.IsTrue(linkedList.Empty);
      Assert.AreEqual(expected: 1, actual: linkedList.RecycleBin.Count);
      linkedList.Add(4);
      Assert.IsTrue(linkedList.RecycleBin.Empty);
      Assert.AreEqual(expected: 1, actual: linkedList.Count);
      Assert.AreEqual(expected: 4, actual: linkedList.Top.Item);
    }

    /// <remarks><a href="http://unitydoc.marrington.net/Able#recycle-a-currently-unused-node">Dispose by Recycling</a></remarks>
    [Test]
    public void RecycleWithCreateItem() {
      var linkedList      = new LinkedList<int> {CreateItem = () => 33};
      var newFromRecycler = linkedList.Fetch();
      Assert.AreEqual(expected: 1,              actual: linkedList.Count);
      Assert.AreEqual(expected: 33,             actual: newFromRecycler.Item);
      Assert.AreEqual(expected: linkedList.Top, actual: newFromRecycler);
    }

    /// <remarks><a href="http://unitydoc.marrington.net/Able#recycle-a-currently-unused-node">Dispose by Recycling</a></remarks>
    [Test]
    public void RecycleWithWithoutCreateItem() {
      var linkedList      = new LinkedList<int>();
      var newFromRecycler = linkedList.Fetch();
      Assert.AreEqual(expected: 1,            actual: linkedList.Count);
      Assert.AreEqual(expected: default(int), actual: newFromRecycler.Item);
    }

    /// <remarks><a href="http://unitydoc.marrington.net/Able#implicit-item-creation-and-activation">Dispose by Recycling</a></remarks>
    [Test]
    public void RecycleWithActivator() {
      var list = new LinkedList<int> {ReactivateItem = (node) => node.Item *= 2};
      list.Add(44).Recycle();
      var reactivated = list.Fetch();
      Assert.AreEqual(expected: 88, actual: reactivated.Item);
    }

    /// <remarks><a href="http://unitydoc.marrington.net/Able#move-items-between-lists">Move Node</a></remarks>
    [Test]
    public void UnorderedMoveTo() {
      var list1 = new LinkedList<int>();
      var list2 = new LinkedList<int>();
      var node1 = list1.Add(21);
      var node2 = list1.Add(18);
      node1.MoveTo(list2);
      Assert.AreEqual(expected: node1, actual: list2.Top);
      Assert.AreEqual(expected: node2, actual: list1.Top);
    }

    /// <remarks><a href="http://unitydoc.marrington.net/Able#move-items-between-lists">Move Node</a></remarks>
    [Test]
    public void OrderedMoveTo() {
      var list1 = new LinkedList<int>();

      var list2 = new LinkedList<int>
        {CompareNodes = (node, cursor) => node.Item.CompareTo(cursor.Item)};

      var node21 = list1.Add(21);
      var node18 = list1.Add(18);
      var node44 = list1.Add(44);
      node21.MoveTo(list2);
      node44.MoveTo(list2);
      list1.Top?.MoveTo(list2);
      Assert.AreEqual(expected: 0,      actual: list1.Count);
      Assert.AreEqual(expected: 3,      actual: list2.Count);
      Assert.AreEqual(expected: node18, actual: list2.Top);
      Assert.AreEqual(expected: node21, actual: list2.Next);
      node18.Dispose();
      Assert.AreEqual(expected: node44, actual: list2.Next);
    }

    /// <remarks><a href="http://unitydoc.marrington.net/Able#move-items-between-lists">Move Node</a></remarks>
    [Test]
    public void MoveToEndOf() {
      var list  = new LinkedList<int>();
      var node1 = list.Add(11);
      var node2 = list.Add(22);
      var node3 = list.Add(33);
      var node4 = list.Add(44); // 44 33 22 11
      node3.MoveToEndOf(list);  // 44 22 11 33
      Assert.AreEqual(expected: node3, actual: list.Bottom);
      Assert.AreEqual(expected: 4,     actual: list.Count);
      Assert.AreEqual(expected: node4, actual: list.Top);
      Assert.AreEqual(expected: node2, actual: list.Next);
      Assert.AreEqual(expected: node4, actual: list.Pop());
      Assert.AreEqual(expected: node1, actual: list.Next);
      Assert.AreEqual(expected: node3, actual: list.Bottom);
    }

    /// <remarks><a href="http://unitydoc.marrington.net/Able#Node Disposal">Dispose of a Node</a></remarks>
    [Test]
    public void Dispose() {
      var linkedList = new LinkedList<int>
        {CompareNodes = (node, cursor) => node.Item.CompareTo(cursor.Item)};

      var node2 = linkedList.Add(5);
      node2.Dispose();
      Assert.IsTrue(linkedList.Empty);
      Assert.AreEqual(expected: 1, actual: linkedList.RecycleBin.Count);
      Assert.AreEqual(expected: 5, actual: linkedList.RecycleBin.Top.Item);
    }

    /// <remarks><a href="http://unitydoc.marrington.net/Able#Node Disposal">Dispose of a Node</a></remarks>
    [Test]
    public void IDisposable() {
      var linkedList = new LinkedList<DisposableInt>();
      var node       = linkedList.Add(new DisposableInt {counter = 333});
      Assert.AreEqual(expected: 333, actual: node.Item.counter);
      node.Dispose();
      Assert.AreEqual(expected: -1, actual: node.Item.counter);
    }

    private class DisposableInt : IDisposable {
      internal int  counter;
      public   void Dispose() => counter = -1;
    }

    /// <remarks><a href="http://unitydoc.marrington.net/Able#Node Disposal">Dispose of a Node</a></remarks>
    [Test]
    public void DisposeAfterMove() {
      var list1 = new LinkedList<int>();
      var list2 = new LinkedList<int>();
      var node  = list1.Add(12);
      node.MoveTo(list2);
      Assert.AreEqual(expected: node, actual: list2.Top);
      node.Dispose();
      Assert.IsFalse(list1.RecycleBin.Empty);
      Assert.IsTrue(list1.Empty);
      Assert.IsTrue(list2.Empty);
      Assert.IsTrue(list2.RecycleBin.Empty);
    }

    /// <remarks><a href="http://unitydoc.marrington.net/Able#fifo">Fifo</a></remarks>
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
      Assert.AreEqual(node4, list2.RecycleBin.Top);
      Assert.AreEqual(node2, list1.Top);
      Assert.AreEqual(node1, list1.Next);
      list1.Push(567);
      Assert.AreEqual(3, list1.Count);
      var node6 = list1.Top;
      Assert.AreEqual(567, node6.Item);
    }

    /// <remarks><a href="http://unitydoc.marrington.net/Able#node-walking">Walking a list</a></remarks>
    [Test]
    public void WalkAll() {
      var list = new LinkedList<int>();
      list.Push(23);
      list.Push(14);
      list.Push(99);
      int   count    = 0;
      int[] expected = {99, 14, 23};

      var last = list.Walk((node, next) => {
        Assert.AreEqual(expected: expected[count], actual: node.Item);
        if (next != null) Assert.AreEqual(expected: expected[count + 1], actual: next.Item);
        count += 1;
        return true;
      });

      Assert.IsNull(last);
      Assert.AreEqual(expected: 3, actual: count);
    }

    /// <remarks><a href="http://unitydoc.marrington.net/Able#node-walking">Walking a list</a></remarks>
    [Test]
    public void WalkTerminated() {
      var list = new LinkedList<int>
        {CompareNodes = (node, cursor) => node.Item.CompareTo(cursor.Item)};

      list.Push(23);
      list.Push(14);
      list.Push(99);
      int   count    = 0;
      int[] expected = {14, 23, 99};

      var last = list.Walk((node, next) => {
        if (node.Item > 50) return false;

        Assert.AreEqual(expected: expected[count], actual: node.Item);
        if (next != null) Assert.AreEqual(expected: expected[count + 1], actual: next.Item);
        count += 1;
        return true;
      });

      Assert.IsNotNull(last);
      Assert.AreEqual(expected: 99, actual: last.Item);
      Assert.AreEqual(expected: 2,  actual: count);
      Assert.AreEqual(expected: 3,  actual: list.Count);
      Assert.AreEqual(expected: 99, actual: list.Bottom.Item);
    }

    /// <remarks><a href="http://unitydoc.marrington.net/Able#linkedlist-debugmode">Debugging</a></remarks>
    [Test]
    public void DebugMode() {
      LinkedList<int>.DebugMode = true;
      LogAssert.Expect(LogType.Log, new Regex(".... LinkedList: Add to Tommy"));
      LogAssert.Expect(LogType.Log, new Regex(".... LinkedList: move Tommy to Freddy"));

      LogAssert.Expect(LogType.Log,
                       new Regex(".... LinkedList: move Freddy to end of Tommy Recycling Bin"));

      var tommy  = new LinkedList<int> {Name = "Tommy"};
      var freddy = new LinkedList<int> {Name = "Freddy"};
      var node   = tommy.Add(1972);
      node.MoveTo(freddy);
      node.Dispose();
      LinkedList<int>.DebugMode = false;
    }

    /// <remarks><a href="http://unitydoc.marrington.net/Able#linkedlist-tostring">Debugging</a></remarks>
    [Test]
    public void ToStringExample() {
      var julias = new LinkedList<int> {Name = "Julias"};
      Assert.AreEqual(expected: "Julias", actual: julias.ToString());
    }

    /// <remarks><a href="http://unitydoc.marrington.net/Able#dump">Return list contents as a string</a></remarks>
    [Test]
    public void Dump() {
      var listA = new LinkedList<int>
        {Name = "A", CompareNodes = (node, cursor) => node.Item.CompareTo(cursor.Item)};

      var listB = new LinkedList<int>
        {Name = "B", CompareNodes = (node, cursor) => node.Item.CompareTo(cursor.Item)};

      listA.Push(23);
      listA.Push(14);
      var node99  = listA.Push(99);
      var expect1 = new Regex("1:	A // A:: 14\n2:	A // A:: 23\n");
      LogAssert.Expect(LogType.Log, expect1);
      Debug.Log(listA.Dump(2));
      node99.MoveTo(listB);
      var expect2 = new Regex("1:	A // B:: 99\n");
      LogAssert.Expect(LogType.Log, expect2);
      Debug.Log(listB.Dump());
    }
  }
}
#endif