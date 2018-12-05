// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

namespace Askowl {
  using System;

  /// <a href="http://bit.ly/2OzDM9D">Cached C# Action instances using the observer pattern</a>
  public class Emitter : IDisposable {
    /// <a href="http://bit.ly/2OzDM9D">Retrieve an emitter from recycling or new</a>
//    public static Emitter Instance => Cache<Emitter>.Instance;
    public static Emitter Instance => Cache<Emitter>.Instance;

    /// <a href=""></a> //#TBD#//
    private readonly Fifo<Action> listeners = new Fifo<Action>();

    /// <a href="http://bit.ly/2OzDM9D">The owner shoots and all the listeners hear</a>
    public void Fire() {
      Firings++;
      for (var idx = 0; idx < listeners.Count; idx++) listeners[idx]();
    }

    /// <a href=""></a> //#TBD#//
    public int Firings;

    /// <a href="http://bit.ly/2OzDM9D">Ask an emitter to tell me too</a>
    public void Subscribe(IObserver observer) { listeners.Push(observer.OnNext); }

    /// <a href="http://bit.ly/2OzDM9D">Ask an emitter to tell me too</a>
    public void Subscribe(Action action) { listeners.Push(action); }

    /// <a href=""></a> //#TBD#//
    public void RemoveAllListeners() {
      listeners.Count = 0;
      Firings         = 0;
    }

    /// <a href=""></a> //#TBD#//
    public bool Waiting => listeners.Count > 0;

    /// <a href="http://bit.ly/2OzDM9D">Call when we are done with this emitter.</a> <inheritdoc />
    public void Dispose() {
      listeners.Dispose();
      Cache<Emitter>.Dispose(this);
    }
  }

  /// <a href="http://bit.ly/2OzDM9D">Cached C# Action{T} instances using the observer pattern</a> <inheritdoc cref="IObservable{T}" />
  // ReSharper disable once ClassNeverInstantiated.Global
  public class Emitter<T> : Emitter, IObservable<T> {
    /// <a href="http://bit.ly/2OzDM9D">Retrieve an emitter from recycling or new</a>
    public new static Emitter<T> Instance => Cache<Emitter<T>>.Instance;

    private event Action<T> Listeners = delegate { };

    /// <a href="http://bit.ly/2OzDM9D">Convenient reference to the value sent with the last emission</a>
    public T LastValue { get; private set; }

    /// <a href="http://bit.ly/2OzDM9D">The owner shoots and all the listeners hear</a>
    public void Fire(T value) { Listeners(LastValue = value); }

    /// <a href="http://bit.ly/2OzDM9D">Ask an emitter to give me a reference to the value ejected</a>
    public IDisposable Subscribe(IObserver<T> observer) {
      Action<T> listener = observer.OnNext;
      Listeners += listener;
      var disposed = false;

      return new Disposable {
        Action = () => {
          if (disposed) return;

          disposed  =  true;
          Listeners -= listener;
          observer.OnCompleted();
        }
      };
    }
  }

  /// <a href="http://bit.ly/2OzDM9D">Observer pattern without a payload</a>
  public interface IObserver {
    /// <a href="http://bit.ly/2OzDM9D">Get the next listener</a>
    void OnNext();

    /// <a href="http://bit.ly/2OzDM9D">Called when the emitter is discarded</a>
    void OnCompleted();
  }
}