// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

namespace Askowl {
  using System;

  /// <a href=""></a>
  public class Stack<T> : IDisposable {
    private T[] stack = new T[8];
    private int pointer;

    // ReSharper disable once UnusedMember.Local
    private void DeactivateItem() { pointer = 0; }

    /// <a href="">For subclasses that do their own caching</a>
    protected Stack() { stack[0] = default(T); }

    /// <a href=""></a>
    public static Stack<T> Instance => Cache<Stack<T>>.Instance;

    /// <a href="bit.ly/">Count</a>
    public int Count => pointer;

    /// <a href="bit.ly/">Push</a>
    public T Push(T entry) {
      if (++pointer >= stack.Length) {
        var old = stack;
        stack = new T[old.Length * 2];
        Array.Copy(old, stack, old.Length);
      }

      return stack[pointer] = entry;
    }

    /// <a href="bit.ly/">Push</a>
    public T Pop() {
      var value = stack[pointer];
      if (pointer > 0) pointer--;
      return value;
    }

    /// <a href=""></a>
    public T Top { get { return stack[pointer]; } set { stack[pointer] = value; } }

    /// <inheritdoc />
    public virtual void Dispose() { Cache<Stack<T>>.Dispose(this); }
  }
}