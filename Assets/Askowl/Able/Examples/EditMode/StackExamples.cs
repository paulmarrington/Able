// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

#if AskowlAble
namespace Askowl.Examples {
  using System.Collections;
  using NUnit.Framework;

  public class StackExamples {
    /// Using <see cref="Stack{T}.Instance"/>
    [Test]
    public void Instance() {
      using (var stack = Stack<int>.Instance) Assert.IsNotNull(stack);
    }

    /// Using <see cref="Stack.Count"/>
    [Test]
    public void Count() {
      using (var stack = Stack<int>.Instance) Assert.AreEqual(expected: 0, actual: stack.Count);
    }

    /// Using <see cref="Stack.Push"/>
    [Test]
    public void Push() {
      using (var stack = Stack<int>.Instance) {
        stack.Push(22);
        Assert.AreEqual(expected: 1, actual: stack.Count);
      }
    }

    /// Using <see cref="Stack.Pop"/>
    [Test]
    public void Pop() {
      using (var stack = Stack<int>.Instance) {
        stack.Push(22);
        Assert.AreEqual(expected: 22, actual: stack.Pop());
        Assert.AreEqual(expected: 0,  actual: stack.Count);
      }
    }

    /// Using <see cref="Stack{T}.Top"/>
    [Test]
    public void Top() {
      using (var stack = Stack<int>.Instance) {
        stack.Push(22);
        Assert.AreEqual(expected: 22, actual: stack.Top);
        Assert.AreEqual(expected: 1,  actual: stack.Count);
      }
    }
  }
}
#endif