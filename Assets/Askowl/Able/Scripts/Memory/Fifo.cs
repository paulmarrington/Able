﻿// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

using System;
using System.Linq;

namespace Askowl {
  /// <a href="http://bit.ly/2NTA5Ml">Intentionally simple stack implementation</a>
  public class Fifo<T> : IDisposable {
    private T[] stack = new T[8];
    private int pointer;

    // ReSharper disable once UnusedMember.Local
    private void DeactivateItem() => pointer = 0;

    /// <a href="http://bit.ly/2NTA5Ml">Raw creation for subclasses that do their own caching</a>
    // ReSharper disable once EmptyConstructor
    public Fifo() { }

    /// <a href="http://bit.ly/2NTA5Ml">Fetch Fifo Stack from recycling</a>
    public static Fifo<T> Instance => Cache<Fifo<T>>.Instance;

    /// <a href="http://bit.ly/2NTA5Ml">Array-like indexing into the stack</a>
    public T this[int i] => stack[i];

    /// <a href="http://bit.ly/2NTA5Ml">Number of items on the stack</a>
    public int Count { get => pointer; set => pointer = value < pointer ? value : pointer; }

    /// <a href=""></a> //#TBD#//
    public bool Empty => Count == 0;

    /// <a href="http://bit.ly/2NTA5Ml">Push a new entry onto the top of the stack</a>
    // ReSharper disable once VirtualMemberNeverOverridden.Global
    public virtual T Push(T entry) {
      if (pointer >= stack.Length) Array.Resize(ref stack, stack.Length * 2);
      stack[pointer++] = entry;
      return entry;
    }

    /// <a href="http://bit.ly/2NTA5Ml">Pop an entry from the top of the stack</a>
    // ReSharper disable once VirtualMemberNeverOverridden.Global
//    public virtual T Pop() => pointer == 0 ? default : stack[--pointer];
    public virtual T Pop() => pointer == 0 ? default : stack[--pointer];

    private static   int nextId;
    private readonly int id = ++nextId;

    /// <a href="http://bit.ly/2NTA5Ml">Swap the top and second entries on the stack</a>
    public Fifo<T> Swap() {
      if (pointer >= 2) {
        T top = Top;
        Top  = Next;
        Next = top;
      }

      return this;
    }

    /// <a href="http://bit.ly/2NTA5Ml">Get/set the value at the top of the stack.</a>
    public T Top { get => Empty ? default : stack[pointer - 1]; set => stack[pointer - 1] = value; }

    /// <a href="http://bit.ly/2NTA5Ml">Get/set the value at second to top of the stack.</a>
    public T Next { get => stack[pointer - 2]; set => stack[pointer - 2] = value; }

    /// <a href="http://bit.ly/2NTA5Ml">Get/set the value at the bottom of the stack.</a>
    public T Bottom { get => Empty ? default : stack[0]; set => stack[0] = value; }

    /// <a href="http://bit.ly/2NTA5Ml">Send back to recycling</a><inheritdoc />
    public virtual void Dispose() {
      Clear();
      Cache<Fifo<T>>.Dispose(this);
    }

    /// <a href=""></a> //#TBD#//
    public void Clear() {
      for (var i = 0; i < pointer; i++) (stack[i] as IDisposable)?.Dispose();
      pointer = 0;
    }

    /// <a href=""></a> //#TBD#//
    public T[] ToArray() {
      Array.Resize(ref stack, pointer);
      return stack;
    }

    /// <a href=""></a> //#TBD#//
    public override string ToString() => $"{typeof(T)}-{id}:{pointer}";
  }
}