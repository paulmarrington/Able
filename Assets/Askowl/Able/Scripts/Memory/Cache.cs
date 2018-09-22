// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

// ReSharper disable ClassNeverInstantiated.Global

namespace Askowl {
  using System;

  public class Cache {
    /// <a href="http://bit.ly"></a>
    public interface Boxed { }

    /// <a href="http://bit.ly"></a>
    /// <inheritdoc cref="LinkedList{T}" />
    public class Boxed<T> : LinkedList<T>.Node, Boxed {
      /// <a href="http://bit.ly"></a>
      public T Value;

      /// <a href="http://bit.ly"></a>
      public static Boxed New(T item) => (Boxed) Cache<T>.Entries.Add(item);

      /// <a href="http://bit.ly"></a>
      public static Boxed Clone(Boxed<T> item) => (Boxed<T>) Cache<T>.Entries.Add(item.Value);
    }
  }

  /// <a href=""></a>
  /// <a href="">The parent and worker for caching</a>
  // ReSharper disable once ClassNeverInstantiated.Global
  public class Cache<T> {
    /// <a href="http://bit.ly"></a>
    public static readonly LinkedList<T> Entries = new LinkedList<T>($"{typeof(T)} Cache");

    /// <a href="http://bit.ly"></a>
    public static T Instance => Entries.Fetch().Item;

    /// <a href="http://bit.ly"></a>
    public static T Add(T item) => Entries.Add(item).Item;

    /// <a href="http://bit.ly"></a>
    public static void Dispose(T item) => Entries.Dispose(item);

    /// <a href="http://bit.ly">Destroy everything in cache - including recycling</a>
    public static void ClearCache() => Entries.Destroy();

    /// <a href="http://bit.ly">Destroy everything in cache - including recycling</a>
    public static void CleanCache() => Entries.Dispose();

    /// <a href="http://bit.ly">Anything you need to do before recycling?</a>
    public static Func<T> CreateItem { set => Entries.CreateItem = value; }

    /// <a href="http://bit.ly">To create an entry when none are available</a>
    public static Action<T> DeactivateItem { set { Entries.DeactivateItem = (node) => value(node.Item); } }

    /// <a href="http://bit.ly">What to do when an old item is restored from recycling</a>
    public static Action<T> ReactivateItem { set { Entries.ReactivateItem = (node) => value(node.Item); } }

    /// <a href="http://bit.ly"></a>
    public static IDisposable Disposable(T item) => new Disposable { Action = Entries.ReverseLookup(item).Dispose };
  }
}