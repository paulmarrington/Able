// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

using System;

namespace Askowl {
  /// <summary>
  /// Emit a signal where observers may listen for events. No data is exchanged.
  /// </summary>
  /// <remarks><a href="http://unitydoc.marrington.net/Able#emitter">The Observer Pattern</a></remarks>
  public class Emitter {
    private event Action listeners = delegate { };

    /// <summary>
    /// Tell all watchers we have changed
    /// </summary>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#emitter">The Observer Pattern</a></remarks>
    public void Fire() { listeners(); }

    /// <summary>
    /// Called by an observer to show interest in an event. To cancel monitoring,
    /// call Dispose() on the return value or have it as the focus of a using
    /// statement.
    /// </summary>
    /// <param name="observer">instance that implements OnNext() and OnCompleted()</param>
    /// <returns>Am object with a Dispose() method.</returns>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#emitter">The Observer Pattern</a></remarks>
    public IDisposable Subscribe(IObserver observer) {
      Action listener = observer.OnNext;
      listeners += listener;
      bool disposed = false;

      return new Disposable {
        Action = () => {
          if (disposed) return;

          disposed  =  true;
          listeners -= listener;
          observer.OnCompleted();
        }
      };
    }
  }

  /// <inheritdoc cref="IObservable{T}" />
  /// <summary>
  /// Emit a signal where observers may listen for events. The signal includes
  /// a data object.
  /// </summary>
  /// <typeparam name="T">Object passed from emitter to observer as a transfer of information</typeparam>
  /// <remarks><a href="http://unitydoc.marrington.net/Able#emitter">The Observer Pattern</a></remarks>
  public class Emitter<T> : Emitter, IObservable<T> {
    private event Action<T> listeners = delegate { };

    /// <summary>
    /// Read-only reference to the last value sent to the listeners.
    /// </summary>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#emitter">The Observer Pattern</a></remarks>
    public T LastValue { get; private set; }

    /// <summary>
    /// Tell all watchers we have changed
    /// </summary>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#emitter">The Observer Pattern</a></remarks>
    public void Fire(T value) { listeners(LastValue = value); }

    /// <summary>
    /// Called by an observer to show interest in an event. To cancel monitoring,
    /// call Dispose() on the return value or have it as the focus of a using
    /// statement.
    /// </summary>
    /// <param name="observer">instance that implements IObserver{T} methods</param>
    /// <returns>Am object with a Dispose() method.</returns>
    /// <remarks><a href="http://unitydoc.marrington.net/Able#emitter">The Observer Pattern</a></remarks>
    public IDisposable Subscribe(IObserver<T> observer) {
      Action<T> listener = observer.OnNext;
      listeners += listener;
      bool disposed = false;

      return new Disposable {
        Action = () => {
          if (disposed) return;

          disposed  =  true;
          listeners -= listener;
          observer.OnCompleted();
        }
      };
    }
  }

  /// <summary>
  /// An Observable{T} equivalent when there is no data to share
  /// </summary>
  /// <remarks><a href="http://unitydoc.marrington.net/Able#emitter">The Observer Pattern</a></remarks>
  public interface IObserver {
    /// <summary>
    /// Called when an emitter fires
    /// </summary>
    void OnNext();

    /// <summary>
    /// Called when an emitter is done - and not expected to provide any more data
    /// </summary>
    void OnCompleted();
  }
}