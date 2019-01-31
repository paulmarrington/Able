// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

// ReSharper disable ClassNeverInstantiated.Global

using System;

namespace Askowl {
  /// <a href="http://bit.ly/2Rj0PEa">Instance caching</a>
  public class Cache {
    /// <a href="http://bit.ly/2Rj0Wj4">Boxed value items</a>
    public interface Boxed { }

    /// <a href="http://bit.ly/2Rj0Wj4">Caching boxed value items</a> <inheritdoc cref="LinkedList{T}" />
    public class Boxed<T> : LinkedList<T>.Node, Boxed {
      /// <a href="http://bit.ly/2Rj0Wj4">Unboxed value storage</a>
      // ReSharper disable once UnassignedField.Global
      public T Value;

      /// <a href="http://bit.ly/2Rj0Wj4">Add a cache entry containing a new value item</a>
      public static Boxed New(T item) => (Boxed) Cache<T>.Entries.Add(item);

      /// <a href="http://bit.ly/2Rj0Wj4">Create a new cache holding a copy of the value item.</a>
      public static Boxed Clone(Boxed<T> item) => (Boxed<T>) Cache<T>.Entries.Add(item.Value);

      /// <inheritdoc />
      public override string ToString() => Value.ToString();
    }
  }

  /// <a href="http://bit.ly/2Rj0PEa">The parent and worker for caching</a>
  // ReSharper disable once ClassNeverInstantiated.Global
  public class Cache<T> {
    /// <a href="http://bit.ly/2OrE4it">A Cache is managed as a linked list of entries</a>
    public static readonly LinkedList<T> Entries = new LinkedList<T>($"{typeof(T)} Cache");

    /// <a href="http://bit.ly/2NXgIlt">Get a node reference (from recycling if available)</a>
    public static T Instance => Entries.GetRecycledOrNew().Item;

    /// <a href="">Get a node reference (from recycling if available)</a>
    public static LinkedList<T>.Node NodeInstance => Entries.GetRecycledOrNew();

    /// <a href="http://bit.ly/2OrE4it">Get a new or recycled node and add an item reference</a>
    public static T Add(T item) => Entries.Add(item).Item;

    /// <a href=""></a> //#TBD#//
    public static T Value(object nodeReference) {
      var node = (nodeReference as LinkedList<T>.Node);
      return (node == null) ? default : node.Item;
    }

    /// <a href="http://bit.ly/2OrE4it">Dispose cleanly of a node and the contained item</a>
    public static void Dispose(T item) => Entries.Dispose(item);

    /// <a href="http://bit.ly/2Oq9upA">Dispose all active nodes - filling recycle bon</a>
    public static void RecycleEverything() => Entries.Dispose();

    /// <a href="http://bit.ly/2NV8Ssx">A lambda to be called to create an item</a>
    public static Func<T> CreateItem { set => Entries.CreateItem = value; }

    /// <a href="http://bit.ly/2Rh3gqG">A lambda to be called before sending an item to recycling for later use</a>
    public static Action<T> DeactivateItem { set => Entries.DeactivateItem = (node) => value(node.Item); }

    /// <a href="http://bit.ly/2Riz7Hj">A lambda to be called before reusing an item from the recycle bin</a>
    public static Action<T> ReactivateItem { set => Entries.ReactivateItem = (node) => value(node.Item); }

    /// <a href="http://bit.ly/2OrE4it">A disposable for `using` so items will be cleaned up</a>
    public static IDisposable Disposable(T item) => new Disposable {Action = Entries.ReverseLookup(item).Dispose};
  }

  /// <a href=""></a> //#TBD#//
  public class Cached<T> : IDisposable where T : Cached<T> {
    /// <a href=""></a> //#TBD#//
    public static T Instance => Cache<T>.Instance;
    protected Cached() { } // so we can't use new()

    /// <a href=""></a> //#TBD#//
    public void Dispose() => Cache<T>.Dispose(this as T);
  }
}