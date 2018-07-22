// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

using System;

namespace Askowl {
  public class Emitter {
    private event Action listeners = delegate { };

    /// <summary>
    /// Tell all watchers we have changed
    /// </summary>
    public void Fire() { listeners(); }

    public IDisposable Subscribe(IObserver observer) {
      Action listener = observer.OnNext;
      listeners += listener;

      return new Disposable {
        action = () => {
          listeners -= listener;
          observer.OnCompleted();
        }
      };
    }
  }

  public class Emitter<T> : Emitter, IObservable<T> {
    private event Action<T> listeners = delegate { };

    /// <summary>
    /// Tell all watchers we have changed
    /// </summary>
    public void Fire(T t) { listeners(t); }

    public IDisposable Subscribe(IObserver<T> observer) {
      Action<T> listener = observer.OnNext;
      listeners += listener;

      return new Disposable {
        action = () => {
          listeners -= listener;
          observer.OnCompleted();
        }
      };
    }
  }

  public interface IObserver {
    void OnNext();
    void OnCompleted();
  }
}