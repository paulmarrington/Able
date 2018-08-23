// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

using System;

namespace Askowl {
  /// <a href="hhttp://bit.ly">Wrapped class for caching</a>
  /// <inheritdoc cref="Cache{T}" />
  public abstract class Cached<T> : Cache<T>, Cache.Aware, IDisposable where T : Cache.Aware, IDisposable {
    /// <remarks><a href="http://bit.ly">Wrapped class for caching</a></remarks>
    public static T Instance {
      get {
        var node = Entries.Fetch();
        node.Item.Recycle = () => node.Dispose();
        return node.Item;
      }
    }

    /// <inheritdoc />
    public Action Recycle { get; set; } = () => { };

    /// <inheritdoc />
    public virtual void Dispose() => Recycle();
  }

  /// <remarks><a href="">Caching for sealed classes</a></remarks>
  public static class Cache {
    /// <remarks><a href="">Interface to make a class cache aware</a></remarks>
    public interface Aware {
      /// Called when an item is disposed of.
      Action Recycle { get; set; }
    }
  }

  /// <remarks><a href="">The parent and worker for caching</a></remarks>
  public class Cache<T> {
    internal static readonly LinkedList<T> Entries = new LinkedList<T>();

    /// <remarks><a href="http://bit.ly">Destroy everything in cache - including recycling</a></remarks>
    public static void CleanCache() => Entries.Destroy();

    /// <remarks><a href="http://bit.ly">Anything you need to do before recycling?</a></remarks>
    public static Func<T> CreateItem { set { LinkedList<T>.CreateItem = value; } }

    /// <remarks><a href="http://bit.ly">To create an entry when none are available</a></remarks>
    public static Action<T> DeactivateItem { set { LinkedList<T>.DeactivateItem = (node) => value(node.Item); } }

    /// <remarks><a href="http://bit.ly">What to do when an old item is restored from recycling</a></remarks>
    public static Action<T> ReactivateItem { set { LinkedList<T>.ReactivateItem = (node) => value(node.Item); } }

    /// <remarks><a href="http://bit.ly">Get entry when you will manually dispose later</a></remarks>
    public static Disposable<T> Disposable {
      get {
        var node = Entries.Fetch();

        return new Disposable<T> {
          Action = (entry) => node.Dispose(),
          Value  = node.Item
        };
      }
    }

    /// <remarks><a href="http://bit.ly">Get an entry, run some actions then send it back to recycling</a></remarks>
    public static void Use(Action<T> actions) {
      using (var instance = Disposable) {
        actions(instance.Value);
      }
    }
  }
}