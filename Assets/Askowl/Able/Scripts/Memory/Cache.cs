// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

using System;

namespace Askowl {
  /// <a href=""></a>
  /// <inheritdoc cref="Cache{T}" />
  public abstract class Cached<T> : Cache<T>, Cache.Aware, IDisposable where T : Cache.Aware, IDisposable {
    /// <a href=""></a>
    public static T Instance {
      get {
        var node = Entries.Fetch();

        if (node.Item is Cache.Aware) node.Item.Recycle = () => node.Dispose();

        return node.Item;
      }
    }

    /// <inheritdoc />
    public Action Recycle { get; set; } = () => { };

    /// <inheritdoc />
    public virtual void Dispose() => Recycle();
  }

  /// <a href="">Caching for sealed classes</a>
  public static class Cache {
    /// <a href="">Interface to make a class cache aware</a>
    public interface Aware {
      /// Called when an item is disposed of.
      Action Recycle { get; set; }
    }
  }

  /// <a href="">The parent and worker for caching</a>
  public class Cache<T> {
    internal static readonly LinkedList<T> Entries = new LinkedList<T>($"{typeof(T)} Cache");

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

    /// <a href="http://bit.ly">Get entry when you will manually dispose later</a>
    public static Disposable<T> Disposable {
      get {
        var node = Entries.Fetch();

        return new Disposable<T> {
          Action = (entry) => node.Dispose(),
          Value  = node.Item
        };
      }
    }

    /// <a href="http://bit.ly">Get an entry, run some actions then send it back to recycling</a>
    public static void Use(Action<T> actions) {
      using (var instance = Disposable) {
        actions(instance.Value);
      }
    }
  }
}