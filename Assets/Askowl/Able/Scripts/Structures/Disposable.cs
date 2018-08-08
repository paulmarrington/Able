using System;

namespace Askowl {
  public struct Disposable : IDisposable {
    public Action action;
    public void   Dispose() { action?.Invoke(); }
  }
}