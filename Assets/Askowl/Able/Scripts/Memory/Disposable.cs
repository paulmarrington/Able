// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

namespace Askowl {
  using System;

  /// <a href="http://bit.ly/2Oo7I8v">Simplified creation of an action that happens at the end of a using statement no matter what else happens. The using is am implicit try/finally block with the IDisposable called as part of finally. It is a struct, so no added heap footprint. `IDisposable Ephemeral() => new Disposable {action = () => whatever};`</a>
  public struct Disposable : IDisposable {
    /// <a href="http://bit.ly/2Oo7I8v">Use iDisposable for Greater Good</a>
    public Action Action;

    /// <inheritdoc />
    public void Dispose() { Action?.Invoke(); }
  }

  /// <a href="http://bit.ly/2Rj0TUq">`Disposable`, but carrying a payload</a>
  public struct Disposable<T> : IDisposable {
    /// <a href="http://bit.ly/2Rj0TUq">Use iDisposable for Greater Good</a>
    public Action<T> Action;

    /// <a href="http://bit.ly/2Rj0TUq">The payload</a>
    public T Value;

    /// <inheritdoc />
    public void Dispose() {
      Action?.Invoke(Value);
      (Value as IDisposable)?.Dispose();
    }
  }
}