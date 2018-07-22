using System;

public struct Disposable : IDisposable {
  public Action action;
  public void   Dispose() { action?.Invoke(); }
}