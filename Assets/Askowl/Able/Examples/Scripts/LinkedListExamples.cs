// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

using NUnit.Framework;
using Assert = UnityEngine.Assertions.Assert;

namespace Askowl.Examples {
  /// <remarks><a href="http://unitydoc.marrington.net/Able#linkedlist"></a></remarks>
  public class LinkedListExamples {
    /// <remarks><a href="http://unitydoc.marrington.net/Able#linkedlist-new"></a></remarks>
    [Test]
    public LinkedList<int> New() {
      var linkedList = LinkedList<int>.New();
      Assert.IsTrue(!string.IsNullOrWhiteSpace(linkedList.Name));
      return linkedList;
    }

    /// <remarks><a href="http://unitydoc.marrington.net/Able#linkedlist-new"></a></remarks>
    [Test]
    public void NewWithName() {
      var linkedList = LinkedList<int>.New("Teddy");
      Assert.AreEqual("Teddy", linkedList.Name);
    }

    /// <remarks><a href="http://unitydoc.marrington.net/Able#linkedlist-new"></a></remarks>
    [Test]
    public LinkedList<int> NewWithNameAndInRange() {
      var linkedList = LinkedList<int>.New("Sorted Teddy");
      Assert.AreEqual("Sorted Teddy", linkedList.Name);
      return linkedList;
    }

    /// <remarks><a href="http://unitydoc.marrington.net/Able#linkedlist-add"></a></remarks>
    [Test]
    public void Add() {
      var linkedList = New();
      linkedList.Add(3);
      Assert.IsFalse(linkedList.Empty);
      Assert.AreEqual(1, linkedList.Count);
    }

    /// <remarks><a href="http://unitydoc.marrington.net/Able#linkedlist-moveto-inrange"></a></remarks>
    [Test]
    public void MoveToInRange() { }

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

    /// <remarks><a href="http://unitydoc.marrington.net/Able#linkedlist-dispose"></a></remarks>
    [Test]
    public void Dispose() { }

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