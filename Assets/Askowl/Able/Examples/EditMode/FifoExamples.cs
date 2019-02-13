// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

#if AskowlTests
namespace Askowl.Able.Examples {
  using NUnit.Framework;

  public class FifoExamples {
    /// Using <see cref="Fifo{T}.Instance"/>
    [Test]
    public void Instance() {
      using (var stack = Fifo<int>.Instance) Assert.IsNotNull(stack);
    }

    /// Using <see cref="Fifo{T}.Count"/>
    [Test]
    public void Count() {
      using (var stack = Fifo<int>.Instance) {
        Assert.AreEqual(expected: 0, actual: stack.Count);
        stack.Push(22);
        stack.Push(33);
        Assert.AreEqual(expected: 2, actual: stack.Count);
        stack.Count = 33;
        Assert.AreEqual(expected: 2, actual: stack.Count);
        stack.Count = 1;
        Assert.AreEqual(expected: 1, actual: stack.Count);
      }
    }

    /// Using <see cref="Fifo{T}.Push"/>
    [Test]
    public void Push() {
      using (var stack = Fifo<int>.Instance) {
        stack.Push(22);
        Assert.AreEqual(expected: 1, actual: stack.Count);
      }
    }

    /// Using <see cref="Fifo{T}.Pop"/>
    [Test]
    public void Pop() {
      using (var stack = Fifo<int>.Instance) {
        stack.Push(22);
        Assert.AreEqual(expected: 22, actual: stack.Pop());
        Assert.AreEqual(expected: 0,  actual: stack.Count);
      }
    }

    /// Using <see cref="Fifo{T}.Top"/>
    [Test]
    public void Top() {
      using (var stack = Fifo<int>.Instance) {
        stack.Push(22);
        Assert.AreEqual(expected: 22, actual: stack.Top);
        Assert.AreEqual(expected: 1,  actual: stack.Count);
      }
    }

    /// Using <see cref="Fifo{T}.Top"/>
    [Test]
    public void Next() {
      using (var stack = Fifo<int>.Instance) {
        stack.Push(22);
        stack.Push(33);
        Assert.AreEqual(expected: 22, actual: stack.Next);
      }
    }

    /// Using <see cref="Fifo{T}.Top"/>
    [Test]
    public void Swap() {
      using (var stack = Fifo<int>.Instance) {
        stack.Push(22);
        stack.Push(33);
        stack.Swap();
        Assert.AreEqual(expected: 22, actual: stack.Top);
        Assert.AreEqual(expected: 33, actual: stack.Next);
      }
    }
  }
}
#endif