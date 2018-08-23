// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

using System;

namespace Askowl {
  /// <summary>
  /// Simplified creation of an action that happens at the end of a using statement
  /// no matter what else happens. The using is am implicit try/finally block
  /// with the IDisposable called as part of finally. It is a struct, so no added heap footprint.
  /// <code>IDisposable Ephemeral() => new Disposable {action = () => whatever};</code>
  /// </summary>
  /// <remarks><a href="http://unitydoc.marrington.net/Able#disposablecs-helper-for-idisposable.dispose">Use iDisposable for Greater Good</a></remarks>
  public struct Disposable : IDisposable {
    /// <remarks><a href="http://unitydoc.marrington.net/Able#disposablecs-helper-for-idisposable.dispose">Use iDisposable for Greater Good</a></remarks>
    public Action Action;

    /// <inheritdoc />
    public void Dispose() { Action?.Invoke(); }
  }

  /// <see cref="Disposable"/>, but carrying a payload
  /// <remarks><a href="http://unitydoc.marrington.net/Able#disposable-with-payload">Use iDisposable for Greater Good</a></remarks>
  public struct Disposable<T> : IDisposable {
    /// <remarks><a href="http://unitydoc.marrington.net/Able#disposable-with-payload">Use Disposable for Greater Good</a></remarks>
    public Action<T> Action;

    /// <remarks><a href="http://unitydoc.marrington.net/Able#disposable-with-payload">Use Disposable for Greater Good</a></remarks>
    public T Value;

    /// <inheritdoc />
    public void Dispose() {
      Action?.Invoke(Value);
      (Value as IDisposable)?.Dispose();
    }
  }
}