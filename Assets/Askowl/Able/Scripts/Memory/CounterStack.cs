// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

namespace Askowl {
  using System;

  /// <a href="http://bit.ly/2NU98YO">A stack of counters for garbage-free iterations</a> <inheritdoc />
  // ReSharper disable once ClassNeverInstantiated.Global
  public class CounterFifo : Fifo<int> {
    /// <a href="http://bit.ly/2NU98YO">Retrieve a new (cached) instance</a>
    public new static CounterFifo Instance => Cache<CounterFifo>.Instance;

    /// <a href="http://bit.ly/2NU98YO">Add a new counter and set it to the starting value</a>
    public int Start(int startingValue = 0) => Math.Abs(Push(startingValue));

    /// <a href="http://bit.ly/2NU98YO">Increment he top counter</a>
    public new int Next() => Math.Abs(++Top);

    /// <a href="http://bit.ly/2NU98YO">See if the top counter has reached the target value</a>
    public bool Reached(int bounds) {
      bool reached = Top >= bounds;
      if (reached) Pop();
      return reached;
    }

    /// <inheritdoc />
    public override void Dispose() { Cache<CounterFifo>.Dispose(this); }
  }
}