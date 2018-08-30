// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

#if UNITY_EDITOR && Able

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

// ReSharper disable MemberCanBePrivate.Local disable UnusedAutoPropertyAccessor.Local
// ReSharper disable StaticMemberInGenericType disable ClassNeverInstantiated.Global disable MissingXmlDoc disable UnusedMember.Local disable MemberHidesStaticFromOuterClass

namespace Askowl.Examples {
  /// <remarks><a href="http://unitydoc.marrington.net/Able#linkedlist-a-different-perspective">LinkedList - a different perspective</a></remarks>
  public class LinkedListExamples {
    #region Test Classes
    public sealed class SealedClassRaw {
      public          string State      { get; set; } = "InConstructor";
      public override string ToString() => State;
    }

    public sealed class SealedClassProcessed {
      public          string State      { get; set; } = "InConstructor";
      public override string ToString() => State;
    }

    private struct MyStruct {
      public string State { get; set; }

      // A struct is useless in a linked list if not initialised
      private static  MyStruct CreateItem() => new MyStruct {State = "StructCreated"};
      public override string   ToString()   => State;
    }

    private struct MyStructProcessed {
      public          string State      { get; set; }
      public override string ToString() => State;
    }

    private class MyClassRaw {
      public          string State      { get; set; }
      public override string ToString() => State;
    }

    private class MyClassProcessed {
      public          string State      { get; set; }
      public override string ToString() => State;
    }

    private class MyClassInherited {
      private static MyClassInherited CreateItem() => new MyClassInherited {State = "CreateInherited"};

      private void DeactivateItem() => State += " Deactivated";
      private void ReactivateItem() => State += " Reactivated";

      private int CompareItem(MyClassInherited node, MyClassInherited other) =>
        String.Compare(node.State, other.State, StringComparison.Ordinal);

      public          string State      { get; set; }
      public override string ToString() => State;
    }

    static LinkedListExamples() {
      LinkedList<SealedClassProcessed>.CreateItemStatic =
        () => new SealedClassProcessed {State = "CreateItem"};

      LinkedList<SealedClassProcessed>.DeactivateItemStatic = (seal) => seal.Item.State =  " DeactivateItem";
      LinkedList<SealedClassProcessed>.ReactivateItemStatic = (seal) => seal.Item.State += " ReactivateItem";
    }
    #endregion

    #region Node Tests
    ///<a href=""></a>
    [Test]
    public void NodePrevious() {
      var list = new LinkedList<int>("NodePrevious");
      list.Add(1, 2, 3, 4, 5);

      int count = 0;

      for (var node = list.Last; node != null; node = node.Previous) {
        count++;
      }

      Assert.AreEqual(expected: 5, actual: count);
      Assert.AreEqual(expected: 2, actual: list.Last.Previous.Item);
    }

    ///<a href=""></a>
    [Test]
    public void NodeNext() {
      var list = new LinkedList<int>("NodeNext");
      list.Add(1, 2, 3, 4, 5);

      int count = 0;

      for (var node = list.First; node != null; node = node.Next) {
        count++;
      }

      Assert.AreEqual(expected: 5, actual: count);
      Assert.AreEqual(expected: 4, actual: list.First.Next.Item);
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
      var list = new LinkedList<int>("NodeComparisons") {
        CompareItem = (a, b) => a.Item.CompareTo(b.Item)
      };

      list.Add(1, 4, 2); // becomes 4 2 1

      Assert.IsTrue(list.First   < list.Last);  // 1 < 4
      Assert.IsTrue(list.Second  > list.First); // 2 > 1
      Assert.IsTrue(list.Second  <= list.Last); // 2 <= 4
      Assert.IsFalse(list.Second >= list.Last); // 2 >= 4
    }

    ///<a href=""></a>
    [Test]
    public void NodeMoveTo() {
      var list1 = new LinkedList<int>("NodeMoveTo1");
      var list2 = new LinkedList<int>("NodeMoveTo2");
      var node1 = list1.Add(21);
      var node2 = list1.Add(18);

      node1.MoveTo(list2);

      Assert.AreEqual(expected: node1, actual: list2.First);
      Assert.AreEqual(expected: node2, actual: list1.First);
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

      Assert.AreEqual(expected: node3, actual: list.Last);
      Assert.AreEqual(expected: 4,     actual: list.Count);
      Assert.AreEqual(expected: node4, actual: list.First);
      Assert.AreEqual(expected: node2, actual: list.Second);
      Assert.AreEqual(expected: node4, actual: list.Pop());
      Assert.AreEqual(expected: node1, actual: list.Second);
      Assert.AreEqual(expected: node3, actual: list.Last);
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
      var list  = new LinkedList<int>("NodeRecycle");
      var node1 = list.Add(123);
      var node2 = list.Add(456);
      Assert.IsTrue(list.RecycleBin.Empty);

      node1.Recycle();
      node2.Recycle(); // recycling is placed at the end of the recycle list so oldest is reused.

      Assert.IsTrue(list.Empty);
      Assert.IsFalse(list.RecycleBin.Empty);
      Assert.AreEqual(2, list.RecycleBin.Count); // 123 456

      Assert.AreEqual(456, list.RecycleBin.Last.Item);

      list.Fetch();
      Assert.AreEqual(1,   list.RecycleBin.Count);
      Assert.AreEqual(1,   list.Count);
      Assert.AreEqual(456, list.RecycleBin.First.Item);
    }

    ///<a href=""></a>
    [Test]
    public void NodeDispose() {
      var list = new LinkedList<int>("NodeDispose");

      using (list.Add(123)) {
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
      var list2 = new LinkedList<int>("NodeToString2");
      var node  = list1.Add(789);
      node.MoveTo(list2);

      var expected = "   NodeToString2      (1)  <<  NodeToString1      (0)   ::  789";
      var actual   = node.ToString();
      Assert.AreEqual(expected, actual);
    }
//
//    ///<a href=""></a> //#TBD#
//    [Test]
//    public void NodeUpdate() {
//      var list = new LinkedList<int>("NodeUpdate");
//      list.Add(11, 12, 13);
//      var node = list.Add(14);
//      list.Add(15, 16, 17);
//
//      node.Item = 33; // will get overwritten in the next statement.
//      node.Update(99).MoveToEndOf(list);
//
//      Assert.AreEqual(expected: 99,   actual: list.Last.Item);
//      Assert.AreEqual(expected: node, actual: list.Last);
//      Assert.AreEqual(expected: 7,    actual: list.Count);
//    }

    ///<a href=""></a>
    [Test]
    public void NodeFetch() {
      var list1 = new LinkedList<int>("NodeFetch1");
      var list2 = new LinkedList<int>("NodeFetch2");
      var node  = list1.Add(123);

      node.MoveTo(list2);
      var newNode = node.Fetch();

      Assert.AreEqual(expected: 0,          actual: list1.Count);
      Assert.AreEqual(expected: 2,          actual: list2.Count);
      Assert.AreEqual(expected: node.Home,  actual: newNode.Home);
      Assert.AreEqual(expected: node.Owner, actual: newNode.Owner);
    }

    ///<a href=""></a>
    [Test]
    public void NodePushItem() {
      var list1 = new LinkedList<int>("NodePushItem1");
      var list2 = new LinkedList<int>("NodePushItem2");
      var node  = list1.Add(123);
      node.Push(444);
      Assert.AreEqual(expected: 444, actual: list1.First.Item);
      node.MoveTo(list2);

      // here the node is created on Home and moved to Owner
      node.Push(555);
      Assert.AreEqual(expected: 555, actual: list2.First.Item);
    }
    #endregion

    #region Public List Creation and Destruction
    ///<a href=""></a>
    [Test]
    public void NewList() {
      var list = new LinkedList<int>("NewList");
      Assert.NotNull(list);
    }

    ///<a href=""></a>
    [Test]
    public void Name() {
      var list = new LinkedList<int>("Name");
      Assert.AreEqual("Name", list.Name);
    }

    ///<a href=""></a>
    [Test]
    public void CreateItemRaw() {
      var list    = new LinkedList<SealedClassRaw>("CreateItemRaw");
      var fetched = list.Fetch();
      Assert.AreEqual("InConstructor", fetched.Item.State);
    }

    ///<a href=""></a>
    [Test]
    public void CreateItemStatic() {
      // Normally is static constructor
      LinkedList<MyStructProcessed>.CreateItemStatic = () => new MyStructProcessed {
        State = "Static"
      };

      var list    = new LinkedList<MyStructProcessed>("CreateItemStatic");
      var fetched = list.Fetch();
      Assert.AreEqual("Static", fetched.Item.State);
    }

    ///<a href=""></a>
    [Test]
    public void CreateItemInstance() {
      var list = new LinkedList<MyStructProcessed>("CreateItemInstance")
        {CreateItem = () => new MyStructProcessed {State = "Instance"}};

      var fetched = list.Fetch();
      Assert.AreEqual("Instance", fetched.Item.State);
    }

    ///<a href=""></a>
    [Test]
    public void CreateItemInherited() {
      var list    = new LinkedList<MyClassInherited>("CreateItemInherited");
      var fetched = list.Fetch();
      Assert.AreEqual("CreateInherited", fetched.Item.State);
    }

    ///<a href=""></a>
    [Test]
    public void ReactivateItemRaw() {
      var list = new LinkedList<SealedClassRaw>("ReactivateItemRaw");

      list.Add(new SealedClassRaw {
        State = "New Item Raw"
      }).Recycle();

      var reactivated = list.Fetch();
      Assert.AreEqual(expected: "New Item Raw", actual: reactivated.Item.State);
    }

    ///<a href=""></a>
    [Test]
    public void ReactivateItemStatic() {
      // Normally is static constructor
      LinkedList<MyClassProcessed>.ReactivateItemStatic = (node) =>
        node.Item.State = "Reactivate Static";

      var list = new LinkedList<MyClassProcessed>("ReactivateItemStatic");

      list.Add(new MyClassProcessed {
        State = "New Item Raw"
      }).Recycle();

      var reactivated = list.Fetch();
      Assert.AreEqual(expected: "Reactivate Static", actual: reactivated.Item.State);
    }

    ///<a href=""></a>
    [Test]
    public void ReactivateItemInstance() {
      var list = new LinkedList<MyClassProcessed>("ReactivateItemInstance")
        {ReactivateItem = (node) => node.Item.State = "Reactivate Instance"};

      list.Add(new MyClassProcessed {
        State = "New Item Raw"
      }).Recycle();

      var reactivated = list.Fetch();
      Assert.AreEqual(expected: "Reactivate Instance", actual: reactivated.Item.State);
    }

    ///<a href=""></a>
    [Test]
    public void ReactivateItemInherited() {
      var list = new LinkedList<MyClassInherited>("ReactivateItemInstance");
      list.Fetch().Recycle();
      var reactivated = list.Fetch();
      Assert.AreEqual(expected: "CreateInherited Deactivated Reactivated", actual: reactivated.Item.State);
    }

    ///<a href=""></a>
    [Test]
    public void DeactivateItemRaw() {
      var list        = new LinkedList<SealedClassRaw>("DeactivateItemRaw");
      var deactivated = list.Add(new SealedClassRaw {State = "New Item Raw"}).Recycle();
      Assert.AreEqual(expected: "New Item Raw", actual: deactivated.Item.State);
    }

    ///<a href=""></a>
    [Test]
    public void DeactivateItemStatic() { // Normally is static constructor
      LinkedList<MyClassProcessed>.DeactivateItemStatic = (node) =>
        node.Item.State = "Deactivate Static";

      var list        = new LinkedList<MyClassProcessed>("DeactivateItemStatic");
      var deactivated = list.Add(new MyClassProcessed {State = "New Item Raw"}).Recycle();
      Assert.AreEqual(expected: "Deactivate Static", actual: deactivated.Item.State);
    }

    ///<a href=""></a>
    [Test]
    public void DeactivateItemInstance() {
      var list = new LinkedList<MyClassProcessed>("DeactivateItemInstance")
        {DeactivateItem = (node) => node.Item.State = "Deactivate Instance"};

      var deactivated = list.Add(new MyClassProcessed {State = "New Item Raw"}).Recycle();
      Assert.AreEqual(expected: "Deactivate Instance", actual: deactivated.Item.State);
    }

    ///<a href=""></a>
    [Test]
    public void DeactivateItemInherited() {
      var list        = new LinkedList<MyClassInherited>("DeactivateItemInherited");
      var deactivated = list.Fetch().Recycle();
      Assert.AreEqual(expected: "CreateInherited Deactivated", actual: deactivated.Item.State);
    }

    ///<a href=""></a>
    [Test]
    public void CompareItemRaw() {
      var list = new LinkedList<int>("CompareItemRaw");
      list.Add(1);
      list.Add(3);
      Assert.AreEqual(expected: 3, actual: list.First.Item);
      Assert.AreEqual(expected: 1, actual: list.Second.Item);
    }

    ///<a href=""></a>
    [Test]
    public void CompareItemStatic() {
      LinkedList<MyClassProcessed>.CompareItemStatic = (left, right) =>
        string.Compare(left.Item.State, right.Item.State, StringComparison.Ordinal);

      var list = new LinkedList<MyClassProcessed>("CompareItemStatic");

      list.Add(new MyClassProcessed {
        State = "Item 1"
      }, new MyClassProcessed {
        State = "Item 2"
      });

      Assert.AreEqual(expected: "Item 1", actual: list.First.Item.State);
      Assert.AreEqual(expected: "Item 2", actual: list.Second.Item.State);
    }

    ///<a href=""></a>
    [Test]
    public void CompareItemInstance() {
      var list = new LinkedList<MyClassRaw>("CompareItemInstance") {
        CompareItem = (left, right) => string.Compare(left.Item.State, right.Item.State, StringComparison.Ordinal)
      };

      list.Add(new MyClassRaw {State = "Item 1"}, new MyClassRaw {State = "Item 2"});

      Assert.AreEqual(expected: "Item 1", actual: list.First.Item.State);
      Assert.AreEqual(expected: "Item 2", actual: list.Second.Item.State);

      // does not affect another instance
      var list2 = new LinkedList<MyClassRaw>("CompareItemInstance2");

      list2.Add(new MyClassRaw {State = "Item 1"}, new MyClassRaw {State = "Item 2"});

      Assert.AreEqual(expected: "Item 2", actual: list2.First.Item.State);
      Assert.AreEqual(expected: "Item 1", actual: list2.Second.Item.State);
    }

    ///<a href=""></a>
    [Test]
    public void CompareItemInherited() {
      var list = new LinkedList<MyClassInherited>("CompareItemInherited");

      list.Add(new MyClassInherited {
        State = "Item 1"
      }, new MyClassInherited {
        State = "Item 2"
      });

      Assert.AreEqual(expected: "Item 1", actual: list.First.Item.State);
      Assert.AreEqual(expected: "Item 2", actual: list.Second.Item.State);

      // can be overridden
      var list2 = new LinkedList<MyClassInherited>("CompareItemInherited2") {
        CompareItem = (left, right) =>
          string.Compare(right.Item.State, left.Item.State, StringComparison.Ordinal)
      };

      list2.Add(new MyClassInherited {
        State = "Item 1"
      }, new MyClassInherited {
        State = "Item 2"
      });

      Assert.AreEqual(expected: "Item 2", actual: list2.First.Item.State);
      Assert.AreEqual(expected: "Item 1", actual: list2.Second.Item.State);
    }

    ///<a href=""></a>
    [Test]
    public void Dispose() {
      var list = new LinkedList<int>("Dispose");

      using (list) {
        list.Add(1, 3, 4);

        Assert.AreEqual(expected: 3, actual: list.Count);
        Assert.AreEqual(expected: 0, actual: list.RecycleBin.Count);
      }

      Assert.AreEqual(expected: 0, actual: list.Count);
      Assert.AreEqual(expected: 3, actual: list.RecycleBin.Count);
    }

    ///<a href=""></a>
    [Test]
    public void Destroy() {
      var list = new LinkedList<int>("Destroy");
      list.Add(1, 3, 4);
      Assert.AreEqual(expected: 3, actual: list.Count);
      Assert.AreEqual(expected: 0, actual: list.RecycleBin.Count);
      list.Destroy();
      Assert.AreEqual(expected: 0, actual: list.Count);
      Assert.AreEqual(expected: 0, actual: list.RecycleBin.Count);
    }
    #endregion

    #region Node Creation and Movement
    ///<a href=""></a>
    [Test]
    public void Add() {
      var list = new LinkedList<int>("Add");
      list.Add(11, 12, 13);
      Assert.AreEqual(expected: 3, actual: list.Count);
    }

    ///<a href=""></a>
    [Test]
    public void Fetch() {
      var list = new LinkedList<int>("Fetch");
      var node = list.Fetch();
      Assert.AreEqual(expected: 0, actual: node.Item);
      Assert.AreEqual(expected: 1, actual: list.Count);
    }

    ///<a href=""></a>
    [Test]
    public void RecycleBin() {
      var list = new LinkedList<int>("RecycleBin");
      var node = list.Fetch().Recycle();
      Assert.AreEqual(node, list.RecycleBin.First);
    }

    /// <a href="">Using <see cref="LinkedList{T}.ReverseLookup"/></a>
    [Test]
    public void ReverseLookupT() {
      var list   = new LinkedList<string>("Add");
      var node11 = list.Add("11");
      var node12 = list.Add("12");
      // first access will generate a reverse lookup Map
      Assert.AreEqual(node11, list.ReverseLookup("11"));
      Assert.AreEqual(node12, list.ReverseLookup("12"));

      var node13 = list.Add("13");
      Assert.AreEqual(node13, list.ReverseLookup("13"));
    }

    /// <a href="">Using <see cref="LinkedList{T}.Dispose(T)"/></a>
    [Test]
    public void DisposeT() {
      var list = new LinkedList<string>("Add");
      list.Add("11");
      list.Add("12");
      list.Add("13");
      // Note the need to wrap integers or they will use the wrong []
      Assert.AreEqual(3, list.Count);
      Assert.AreEqual(0, list.RecycleBin.Count);
      list.Dispose("13");
      Assert.AreEqual(2, list.Count);
      Assert.AreEqual(1, list.RecycleBin.Count);
      list.Dispose("11");
      Assert.AreEqual(1, list.Count);
      Assert.AreEqual(2, list.RecycleBin.Count);
      list.Dispose("12");
      Assert.AreEqual(0, list.Count);
      Assert.AreEqual(3, list.RecycleBin.Count);
    }
    #endregion

    #region FiFo Stack
    ///<a href=""></a>
    [Test]
    public void Top() {
      var list = new LinkedList<int>("Top");
      Assert.IsNull(list.First);
      list.Add(1);
      list.Add(3);
      Assert.AreEqual(expected: 3, actual: list.First.Item);
    }

    ///<a href=""></a>
    [Test]
    public void Bottom() {
      var list = new LinkedList<int>("Bottom");
      Assert.IsNull(list.Last);
      list.Add(1);
      list.Add(3);
      Assert.AreEqual(expected: 1, actual: list.Last.Item);
    }

    ///<a href=""></a>
    [Test]
    public void Next() {
      var list = new LinkedList<int>("Next");
      Assert.IsNull(list.Second);
      list.Add(1);
      Assert.IsNull(list.Second);
      list.Add(3);
      Assert.AreEqual(expected: 1, actual: list.Second.Item);
    }

    ///<a href=""></a>
    [Test]
    public void Empty() {
      var list = new LinkedList<int>("Empty");
      Assert.IsTrue(list.Empty);
      list.Add(3);
      Assert.IsFalse(list.Empty);
    }

    ///<a href=""></a>
    [Test]
    public void Count() {
      var list = new LinkedList<int>("Count");
      list.Add(1);
      list.Add(3);
      Assert.AreEqual(expected: 2, actual: list.Count);
    }

    ///<a href=""></a>
    [Test]
    public void PushNode() {
      var list1 = new LinkedList<int>("NodePushNode1");
      var list2 = new LinkedList<int>("NodePushNode2");
      var node1 = list1.Add(123);
      list2.Push(node1);
      Assert.AreEqual(expected: 123, actual: list2.First.Item);
    }

    ///<a href=""></a>
    [Test]
    public void PushItem() {
      var list1 = new LinkedList<int>("NodePushItem1");
      var list2 = new LinkedList<int>("NodePushItem2");
      var node  = list1.Add(123);
      list2.Push(node);
      Assert.AreEqual(expected: 123, actual: list2.First.Item);
    }

    ///<a href=""></a>
    [Test]
    public void Pop() {
      var list  = new LinkedList<int>("NodePop");
      var node1 = list.Add(123);
      var node2 = list.Pop();
      Assert.AreEqual(node1.Item, node2.Item);
      Assert.IsNotNull(list.RecycleBin);
    }
    #endregion

    #region Debugging
    ///<a href=""></a>
    [Test]
    public void DebugMode() {
      LinkedList<int>.DebugMode = true;
      LogAssert.Expect(LogType.Log, new Regex(".... LinkedList: Add to Tommy.*0."));
      LogAssert.Expect(LogType.Log, new Regex(".... LinkedList: move Tommy.*1. to Freddy.*0."));
      LogAssert.Expect(LogType.Log, new Regex(".*: move Freddy.*1.*to end of Tommy Recycling Bin.*0."));
      var tommy  = new LinkedList<int>("Tommy");
      var freddy = new LinkedList<int>("Freddy");
      var node   = tommy.Add(1972);
      node.MoveTo(freddy);
      node.Dispose();
      LinkedList<int>.DebugMode = false;
    }

    ///<a href=""></a>
    [Test]
    public void ToStringExample() {
      var julias = new LinkedList<int>("Julias");
      julias.Add(1, 2, 3);
      julias.Pop();
      Assert.AreEqual(expected: "Julias    (2/1)", actual: julias.ToString());
    }

    ///<a href=""></a>
    [Test]
    public void Dump() {
      var listA = new LinkedList<int>("A")
        {CompareItem = (node, cursor) => node.Item.CompareTo(cursor.Item)};

      var listB = new LinkedList<int>("B")
        {CompareItem = (node, cursor) => node.Item.CompareTo(cursor.Item)};

      listA.Add(23, 14);
      var node99  = listA.Push(99);
      var expect1 = new Regex("1:.*A.*3.*<<.*A.*3.*::.*14\n2:.*A.*3.*<<.*A.*3.*::.*23.*");
      LogAssert.Expect(LogType.Log, expect1);
      Debug.Log(listA.Dump(2));
      node99.MoveTo(listB);
      var expect2 = new Regex("1:.*B.*1.*<<.*A.*2.*::.*99\n");
      LogAssert.Expect(LogType.Log, expect2);
      Debug.Log(listB.Dump());
    }
    #endregion
  }
}
#endif