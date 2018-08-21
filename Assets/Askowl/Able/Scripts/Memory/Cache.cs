// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

using System;

namespace Askowl {
  /// <inheritdoc cref="Askowl.Cache.Aware" />
  /// <remarks><a href="http://unitydoc.marrington.net/Able#make-a-class-cached">Wrapped class for caching</a></remarks>
  public abstract class Cached<T> : Cache<T>, Cache.Aware, IDisposable where T : Cache.Aware, IDisposable {
    /// <remarks><a href="http://unitydoc.marrington.net/Able#make-a-class-cached">Wrapped class for caching</a></remarks>
    public static T Instance {
      get {
        T instance = Cache.New<T>();
        return instance;
      }
    }

    /// <inheritdoc />
    public Action Recycle { get; set; } = () => { };

    /// <inheritdoc />
    public virtual void Dispose() => Recycle();
  }

  /// <remarks><a href="http://unitydoc.marrington.net/Able#caching-for-sealed-classes">Caching for sealed classes</a></remarks>
  public static class Cache {
    /// <remarks><a href="http://unitydoc.marrington.net/Able#Cache Awareness">Interface to make a class cache aware</a></remarks>
    public interface Aware {
      /// Called when an item is disposed of.
      Action Recycle { get; set; }
    }

    /// <remarks><a href="http://unitydoc.marrington.net/Able#fetch-a-new-cache-aware-Instance">Retrieve a new item from the cache</a></remarks>
    public static T New<T>() where T : Aware {
      var node = Cache<T>.Entries.Fetch();
      node.Item.Recycle = () => node.Dispose();
      return node.Item;
    }
  }

  /// <remarks><a href="http://unitydoc.marrington.net/Able#caching-objects">The parent and worker for caching</a></remarks>
  public class Cache<T> {
    internal static readonly LinkedList<T> Entries = new LinkedList<T>();

    /// <remarks><a href="http://unitydoc.marrington.net/Able#createitem">Anything you need to do before recycling?</a></remarks>
    public static Func<T> CreateItem { set { Entries.CreateItem = value; } }

    /// <remarks><a href="http://unitydoc.marrington.net/Able#deactivateitem">To create an entry when none are available</a></remarks>
    public static Action<T> DeactivateItem { set { Entries.DeactivateItem = (node) => value(node.Item); } }

    /// <remarks><a href="http://unitydoc.marrington.net/Able#reactivateitem">What to do when an old item is restored from recycling</a></remarks>
    public static Action<T> ReactivateItem { set { Entries.ReactivateItem = (node) => value(node.Item); } }

    /// <remarks><a href="http://unitydoc.marrington.net/Able#disposable-cache-item">Get entry when you will manually dispose later</a></remarks>
    public static Disposable<T> Disposable {
      get {
        var node = Entries.Fetch();

        return new Disposable<T> {
          Action  = (entry) => node.Dispose(),
          Payload = node.Item
        };
      }
    }

    /// <remarks><a href="http://unitydoc.marrington.net/Able#using-a-cached-item-safely">Get an entry, run some actions then send it back to recycling</a></remarks>
    public static void Use(Action<T> actions) {
      using (var instance = Disposable) {
        actions(instance.Payload);
      }
    }
  }
}