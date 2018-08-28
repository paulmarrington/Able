// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

using System;

namespace Askowl {
  /// <a href="">The parent and worker for caching</a>
  public static class Cache<T> {
    /// <a href="http://bit.ly"></a>
    public static readonly LinkedList<T> Entries = new LinkedList<T>($"{typeof(T)} Cache");

    /// <a href="http://bit.ly"></a>
    public static T Instance => Entries.Fetch().Item;

    /// <a href="http://bit.ly"></a>
    public static void Dispose(T item) => Entries.Dispose(item);

    /// <a href="http://bit.ly">Destroy everything in cache - including recycling</a>
    public static void CleanCache() => Entries.Destroy();

    /// <a href="http://bit.ly">Anything you need to do before recycling?</a>
    public static Func<T> CreateItemStatic { set { LinkedList<T>.CreateItemStatic = value; } }

    /// <a href="http://bit.ly">To create an entry when none are available</a>
    public static Action<T> DeactivateItemStatic {
      set { LinkedList<T>.DeactivateItemStatic = (node) => value(node.Item); }
    }

    /// <a href="http://bit.ly">What to do when an old item is restored from recycling</a>
    public static Action<T> ReactivateItemStatic {
      set { LinkedList<T>.ReactivateItemStatic = (node) => value(node.Item); }
    }

    /// <a href="http://bit.ly"></a>
    public static IDisposable Disposable(T item) => new Disposable {Action = Entries.ReverseLookup(item).Dispose};
  }

  /// <a href="http://bit.ly"></a>
  public class Cached<T> where T : Cached<T> {
    /// <a href="http://bit.ly"></a>
    public static T Instance => Cache<T>.Instance;

    /// <a href="http://bit.ly"></a>
    public void Dispose() => Cache<T>.Dispose((T) this);

    /// <a href="http://bit.ly"></a>
    public IDisposable Disposable() => Cache<T>.Disposable((T) this);

    /// <a href="http://bit.ly">Anything you need to do before recycling?</a>
    public Func<T> CreateItem { set { Cache<T>.Entries.CreateItem = value; } }

    /// <a href="http://bit.ly">To create an entry when none are available</a>
    public Action<T> DeactivateItem { set { Cache<T>.Entries.DeactivateItem = (node) => value(node.Item); } }

    /// <a href="http://bit.ly">What to do when an old item is restored from recycling</a>
    public Action<T> ReactivateItem { set { Cache<T>.Entries.ReactivateItem = (node) => value(node.Item); } }
  }
}