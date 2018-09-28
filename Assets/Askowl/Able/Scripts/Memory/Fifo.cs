// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

namespace Askowl {
  using System;

  /// <a href=""></a>
  public class Fifo<T> : IDisposable {
    private T[] stack = new T[8];
    private int pointer;

    // ReSharper disable once UnusedMember.Local
    private void DeactivateItem() { pointer = 0; }

    /// <a href="">For subclasses that do their own caching</a>
    protected Fifo() { }

    /// <a href=""></a>
    public static Fifo<T> Instance => Cache<Fifo<T>>.Instance;

    /// <a href=""></a>
    public T this[int i] => stack[i];

    /// <a href="bit.ly/">Count</a>
    public int Count { get => pointer; set => pointer = value < pointer ? value : pointer; }

    /// <a href="bit.ly/">Push</a>
    public T Push(T entry) {
      if (pointer >= stack.Length) {
        var old = stack;
        stack = new T[old.Length * 2];
        Array.Copy(old, stack, old.Length);
      }

      return stack[pointer++] = entry;
    }

    /// <a href="bit.ly/">Pop</a>
    public virtual T Pop() => pointer == 0 ? default : stack[--pointer];

    /// <a href="bit.ly/">Pop</a>
    public Fifo<T> Swap() {
      if (pointer >= 2) {
        T top = Top;
        Top  = Next;
        Next = top;
      }
      return this;
    }

    /// <a href=""></a>
    public T Top { get => stack[pointer - 1]; set => stack[pointer - 1] = value; }

    /// <a href=""></a>
    public T Next { get => stack[pointer - 2]; set => stack[pointer - 2] = value; }

    /// <a href=""></a>
    public T Bottom { get => stack[0]; set => stack[0] = value; }

    /// <inheritdoc />
    public virtual void Dispose() {
      for (var i = 0; i < pointer; i++) (stack[i] as IDisposable)?.Dispose();
      pointer = 0;
      Cache<Fifo<T>>.Dispose(this);
    }
  }
}