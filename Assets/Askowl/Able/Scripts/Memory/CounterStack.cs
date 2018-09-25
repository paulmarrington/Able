// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

namespace Askowl {
  using System;

  /// <a href=""></a>
  /// <inheritdoc />
  // ReSharper disable once ClassNeverInstantiated.Global
  public class CounterFifo : Fifo<int> {
    /// <a href="bit.ly/">Instance</a>
    public new static CounterFifo Instance => Cache<CounterFifo>.Instance;

    /// <a href=""></a>
    public int Start(int startingValue = 0) => Math.Abs(Push(startingValue));

    /// <a href="bit.ly/">Next</a>
    public new int Next() => Math.Abs(++Top);

    /// <a href="bit.ly/">Next</a>
    public bool Reached(int bounds) {
      var reached = (Top >= bounds);
      if (reached) Pop();
      return reached;
    }

    /// <inheritdoc />
    public override void Dispose() { Cache<CounterFifo>.Dispose(this); }
  }
}