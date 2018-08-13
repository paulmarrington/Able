// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

using System;

namespace Askowl {
  /// <summary>
  /// Simplified creation of an action that happens at the end of a using statement
  /// no matter what else happens. The using is am implicit try/finally block
  /// with the IDisposable called as part of finally.
  /// <code>IDisposable Ephemeral() => new Disposable {action = () => whatever};</code>
  /// </summary>
  /// <remarks><a href="http://unitydoc.marrington.net/Able#disposable">Use iDisposable for Greater Good</a></remarks>
  public struct Disposable : IDisposable {
    /// <summary>
    /// Action function to call when the using block is done.
    /// </summary>
    public Action Action;

    /// <inheritdoc />
    public void Dispose() { Action?.Invoke(); }
  }
}