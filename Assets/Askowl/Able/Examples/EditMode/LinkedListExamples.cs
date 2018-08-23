// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

#if UNITY_EDITOR && Able

using System;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

// ReSharper disable StaticMemberInGenericType disable ClassNeverInstantiated.Global disable MissingXmlDoc disable UnusedMember.Local disable MemberHidesStaticFromOuterClass

namespace Askowl.Examples {
  /// <remarks><a href="http://unitydoc.marrington.net/Able#linkedlist-a-different-perspective">LinkedList - a different perspective</a></remarks>
  public class LinkedListExamples {
    #region Test Classes
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
    #endregion

    #region Node tests
    ///<a href=""></a>
    [Test]
    public void NodePrevious() {
      var list = new LinkedList<int>("NodePrevious");
      list.Add(1, 2, 3, 4, 5);

      int count = 0;

      for (var node = list.Bottom; node != null; node = node.Previous) {
        count++;
      }

      Assert.AreEqual(expected: 5, actual: count);
      Assert.AreEqual(expected: 2, actual: list.Bottom.Previous.Item);
    }

    ///<a href=""></a>
    [Test]
    public void NodeNext() {
      var list = new LinkedList<int>("NodeNext");
      list.Add(1, 2, 3, 4, 5);

      int count = 0;

      for (var node = list.Top; node != null; node = node.Next) {
        count++;
      }

      Assert.AreEqual(expected: 5, actual: count);
      Assert.AreEqual(expected: 4, actual: list.Top.Next.Item);
    }

    ///<a href=""></a>
    [Test]
    public void NodeOwner() {
      var list1 = new LinkedList<int>("NodeOwner1");
      var node  = list1.Add(1);
      var list2 = new LinkedList<int>("NodeOwner2");

      Assert.AreEqual(expected: list1, actual: node.Owner);

      node.MoveTo(list2);
      Assert.AreEqual(expected: list2, actual: node.Owner);
    }

    ///<a href=""></a>
    [Test]
    public void NodeHome() {
      var list1 = new LinkedList<int>("NodeHome1");
      var node  = list1.Add(1);
      var list2 = new LinkedList<int>("NodeHome2");

      node.MoveTo(list2);
      Assert.AreEqual(expected: list2, actual: node.Owner);
      Assert.AreEqual(expected: list1, actual: node.Home);
    }

    ///<a href=""></a>
    [Test]
    public void NodeItem() {
      var list = new LinkedList<int>("NodeItem");
      var node = list.Add(1);

      Assert.AreEqual(expected: 1, actual: node.Item);
    }

    ///<a href=""></a>
    [Test]
    public void NodeComparisons() {
      var list = new LinkedList<int>("NodeComparisons");
      list.Add(1, 4, 2);

      Assert.IsTrue(list.Top   < list.Bottom);
      Assert.IsFalse(list.Top  > list.Next);
      Assert.IsFalse(list.Next <= list.Bottom);
      Assert.IsTrue(list.Next  >= list.Bottom);
    }

    ///<a href=""></a>
    [Test]
    public void NodeMoveTo() {
      var list1 = new LinkedList<int>("NodeMoveTo1");
      var list2 = new LinkedList<int>("NodeMoveTo2");
      var node1 = list1.Add(21);
      var node2 = list1.Add(18);

      node1.MoveTo(list2);

      Assert.AreEqual(expected: node1, actual: list2.Top);
      Assert.AreEqual(expected: node2, actual: list1.Top);
    }

    ///<a href=""></a>
    [Test]
    public void NodeMoveToEnd() {
      var list  = new LinkedList<int>("NodeMoveToEnd");
      var node1 = list.Add(11);
      var node2 = list.Add(22);
      var node3 = list.Add(33);
      var node4 = list.Add(44); // 44 33 22 11

      node3.MoveToEndOf(list); // 44 22 11 33

      Assert.AreEqual(expected: node3, actual: list.Bottom);
      Assert.AreEqual(expected: 4,     actual: list.Count);
      Assert.AreEqual(expected: node4, actual: list.Top);
      Assert.AreEqual(expected: node2, actual: list.Next);
      Assert.AreEqual(expected: node4, actual: list.Pop());
      Assert.AreEqual(expected: node1, actual: list.Next);
      Assert.AreEqual(expected: node3, actual: list.Bottom);
    }

    ///<a href=""></a>
    [Test]
    public void NodeDestroy() {
      var list = new LinkedList<int>("NodeDestroy");
      var node = list.Add(123);
      Assert.IsFalse(list.Empty);
      Assert.IsTrue(list.RecycleBin.Empty);

      node.Destroy();

      Assert.AreEqual(expected: default(int), actual: node.Item);
      Assert.IsNull(node.Previous);
      Assert.IsNull(node.Next);
      Assert.IsNull(node.Owner);
      Assert.IsNull(node.Home);
      Assert.IsTrue(list.Empty);
      Assert.IsTrue(list.RecycleBin.Empty);
      // node can be reclaimed by garbage collector after we leave this function
    }

    ///<a href=""></a>
    [Test]
    public void NodeRecycle() {
      var list = new LinkedList<int>("NodeRecycle");
      var node = list.Add(123);
      Assert.IsFalse(list.Empty);
      Assert.IsTrue(list.RecycleBin.Empty);

      node.Recycle();

      Assert.IsTrue(list.Empty);
      Assert.IsFalse(list.RecycleBin.Empty);

      // recycling is placed at the end of the recycle list so oldest is reused.
      node = list.Add(456);
      node.Recycle();

      Assert.AreEqual(123, list.RecycleBin.Top);
      Assert.AreEqual(456, list.RecycleBin.Bottom);
    }

    ///<a href=""></a>
    [Test]
    public void NodeDispose() {
      var list = new LinkedList<int>("NodeDispose");

      using (var node = list.Add(123)) {
        Assert.IsFalse(list.Empty);
        Assert.IsTrue(list.RecycleBin.Empty);
      }

      Assert.IsTrue(list.Empty);
      Assert.IsFalse(list.RecycleBin.Empty);
    }

    ///<a href=""></a>
    [Test]
    public void NodeToString() {
      var list1 = new LinkedList<int>("NodeToString1");
      var list2 = new LinkedList<int>("NodeToString1");
      var node  = list1.Add(789);
      node.MoveTo(list2);

      var expected = "NodeToString2 // NodeToString1:: 789";
    }

    ///<a href=""></a>
    [Test]
    public void NodeUpdate() { }
    #endregion

    #region Public List Creation and Destruction
    ///<a href=""></a>
    [Test]
    public void New() { }

    ///<a href=""></a>
    [Test]
    public void Name() { }

    ///<a href=""></a>
    [Test]
    public void CreateItem() { }

    ///<a href=""></a>
    [Test]
    public void CreateItemStatic() { }

    ///<a href=""></a>
    [Test]
    public void CreateItemDefault() { }

    ///<a href=""></a>
    [Test]
    public void ReactivateItem() { }

    ///<a href=""></a>
    [Test]
    public void ReactivateItemStatic() { }

    ///<a href=""></a>
    [Test]
    public void ReactivateItemDefault() { }

    ///<a href=""></a>
    [Test]
    public void DeactivateItem() { }

    ///<a href=""></a>
    [Test]
    public void DeactivateItemStatic() { }

    ///<a href=""></a>
    [Test]
    public void DeactivateItemDefault() { }

    ///<a href=""></a>
    [Test]
    public void CompareItem() { }

    ///<a href=""></a>
    [Test]
    public void CompareItemStatic() { }

    ///<a href=""></a>
    [Test]
    public void CompareItemDefault() { }

    ///<a href=""></a>
    [Test]
    public void Destroy() { }

    ///<a href=""></a>
    [Test]
    public void Dispose() { }
    #endregion

    #region Node Creation and Movement
    ///<a href=""></a>
    [Test]
    public void Add() { }

    ///<a href=""></a>
    [Test]
    public void Fetch() { }

    ///<a href=""></a>
    [Test]
    public void RecycleBin() { }
    #endregion

    #region FiFo Stack
    ///<a href=""></a>
    [Test]
    public void Top() { }

    ///<a href=""></a>
    [Test]
    public void Bottom() { }

    ///<a href=""></a>
    [Test]
    public void Next() { }

    ///<a href=""></a>
    [Test]
    public void Empty() { }

    ///<a href=""></a>
    [Test]
    public void Count() { }

    ///<a href=""></a>
    [Test]
    public void PushItem() { }

    ///<a href=""></a>
    [Test]
    public void PushNode() { }

    ///<a href=""></a>
    [Test]
    public void Pop() { }
    #endregion

    #region Traversal
    ///<a href=""></a>
    [Test]
    public void Walk() { }
    #endregion

    #region Debugging
    ///<a href=""></a>
    [Test]
    public void DebugMode() { }

    ///<a href=""></a>
    [Test]
    public void Dump() { }

    ///<a href=""></a>
    [Test]
    public new void ToString() { }
    #endregion

    /////////////////////////////////////////////////////////////////////////////////

    /// <remarks><a href=""></a></remarks>
    [Test]
    public void ListName() {
      var linkedList = new LinkedList<int>("List Name");
      Assert.AreEqual("List Name", linkedList.Name);
      linkedList = new LinkedList<int>("");
      Assert.AreEqual("int-1", linkedList.Name);
      linkedList = new LinkedList<int>(null);
      Assert.AreEqual("int-2", linkedList.Name);
    }

    /// <remarks><a href="">Are there active entries in the list?</a></remarks>
    [Test]
    public void Empty() {
      var linkedList = new LinkedList<int>("Empty");
      Assert.IsTrue(linkedList.Empty);
      linkedList.Add(3);
      Assert.IsFalse(linkedList.Empty);
    }

    /// <remarks><a href="">Count number of items in a list</a></remarks>
    [Test]
    public void Count() {
      var linkedList = new LinkedList<int>("Count");
      linkedList.Add(1);
      linkedList.Add(3);
      Assert.AreEqual(expected: 2, actual: linkedList.Count);
    }

    /// <remarks><a href="">First item on the linked list</a></remarks>
    [Test]
    public void Top() {
      var linkedList = new LinkedList<int>("Top");
      Assert.IsNull(linkedList.Top);
      linkedList.Add(1);
      linkedList.Add(3);
      Assert.AreEqual(expected: 3, actual: linkedList.Top.Item);
    }

    /// <remarks><a href="">Second item on the linked list</a></remarks>
    [Test]
    public void Next() {
      var linkedList = new LinkedList<int>("Next");
      Assert.IsNull(linkedList.Next);
      linkedList.Add(1);
      Assert.IsNull(linkedList.Next);
      linkedList.Add(3);
      Assert.AreEqual(expected: 1, actual: linkedList.Next.Item);
    }

    /// <remarks><a href="">Add an Item</a></remarks>
    [Test]
    public void AddUnordered() {
      var linkedList = new LinkedList<int>("AddUnordered");
      linkedList.Add(1);
      linkedList.Add(3);
      Assert.AreEqual(expected: 3, actual: linkedList.Top.Item);
      Assert.AreEqual(expected: 1, actual: linkedList.Next.Item);
    }

    /// <remarks><a href="">Add an Item specifying order</a></remarks>
    [Test]
    public void AddOrderedSealed() {
      var linkedList = new LinkedList<int>("AddOrderedSealed");

      linkedList.CompareItem = (node, other) => node.Item.CompareTo(other.Item);

      linkedList.Add(1);
      linkedList.Add(3);

      Assert.AreEqual(expected: 1, actual: linkedList.Top.Item);
      Assert.AreEqual(expected: 3, actual: linkedList.Next.Item);
    }

    /// <remarks><a href="">Add an Item specifying order</a></remarks>
    [Test]
    public void AddOrderedUnsealed() {
      var linkedList = LinkedList<MyClassProcessed>.Instance("AddOrderedUnsealed");

      linkedList.Add(new MyClassProcessed {State = "First Added"});
      linkedList.Fetch().Item.State = "Second uses Fetch";

      Assert.AreEqual(expected: "Second uses Fetch", actual: linkedList.Top.Item.State);
      Assert.AreEqual(expected: "First Added",       actual: linkedList.Next.Item.State);
    }

    /// <remarks><a href="">Dispose by Recycling</a></remarks>
    [Test]
    public void RecycleWithAdd() {
      var linkedList = LinkedList<MyClassRaw>.Instance("RecycleWithAdd");
      var node       = linkedList.Add(new MyClassRaw {State = "First Added"});
      node.Dispose();
      Assert.IsTrue(linkedList.Empty);
      Assert.AreEqual(expected: 1, actual: linkedList.RecycleBin.Count);
      linkedList.Add(new MyClassRaw {State = "First Added"});
      Assert.IsTrue(linkedList.RecycleBin.Empty);
      Assert.AreEqual(expected: 1,             actual: linkedList.Count);
      Assert.AreEqual(expected: "First Added", actual: linkedList.Top.Item.State);
    }

    /// <remarks><a href="">Dispose by Recycling</a></remarks>
    [Test]
    public void RecycleWithCreateItem() {
      var linkedList      = LinkedList<MyClassProcessed>.Instance("AddOrderedUnsealed");
      var newFromRecycler = linkedList.Fetch();
      Assert.AreEqual(expected: 1,              actual: linkedList.Count);
      Assert.AreEqual(expected: "Created",      actual: newFromRecycler.Item.State);
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
      var list = LinkedList<MyClassProcessed>.Instance("RecycleWithActivator");
      list.Add(new MyClassProcessed {State = "RecycleWithActivator"}).Recycle();
      var reactivated = list.Fetch();
      Assert.AreEqual(expected: "DeactivateItem ReactivateItem", actual: reactivated.Item.State);
    }

    /// <remarks><a href="http://unitydoc.marrington.net/Able#move-items-between-lists">Move Node</a></remarks>
    [Test]
    public void UnorderedMoveTo() {
//      var list1 = new LinkedList<int>();
//      var list2 = new LinkedList<int>();
//      var node1 = list1.Add(21);
//      var node2 = list1.Add(18);
//      node1.MoveTo(list2);
//      Assert.AreEqual(expected: node1, actual: list2.Top);
//      Assert.AreEqual(expected: node2, actual: list1.Top);
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
//      var list  = new LinkedList<int>();
//      var node1 = list.Add(11);
//      var node2 = list.Add(22);
//      var node3 = list.Add(33);
//      var node4 = list.Add(44); // 44 33 22 11
//      node3.MoveToEndOf(list);  // 44 22 11 33
//      Assert.AreEqual(expected: node3, actual: list.Bottom);
//      Assert.AreEqual(expected: 4,     actual: list.Count);
//      Assert.AreEqual(expected: node4, actual: list.Top);
//      Assert.AreEqual(expected: node2, actual: list.Next);
//      Assert.AreEqual(expected: node4, actual: list.Pop());
//      Assert.AreEqual(expected: node1, actual: list.Next);
//      Assert.AreEqual(expected: node3, actual: list.Bottom);
    }

    /// <remarks><a href="http://unitydoc.marrington.net/Able#Node Disposal">Dispose of a Node</a></remarks>
    [Test]
    public void Dispose() {
      var linkedList = new LinkedList<int>
        {CompareItem = (node, cursor) => node.Item.CompareTo(cursor.Item)};

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