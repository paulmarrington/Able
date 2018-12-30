// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

namespace Askowl {
  using System;
  using System.Collections;
  using System.Collections.Generic;

  /// <a href="http://bit.ly/2NUH9Io">A dictionary wrapper</a>
  // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
  public class Map : IDisposable {
    private readonly Dictionary<object, object> map = new Dictionary<object, object>();

    private readonly ArrayList keys = new ArrayList();

    /// <a href=""></a> //#TBD#//
    public static Map Instance => Cache<Map>.Instance;

    internal Map CreateItem() => new Map {cached = true};

    /// <a href="http://bit.ly/2NUH9Io">Remove an entry, optionally calling Dispose()</a>
    // ReSharper disable once UnusedMethodReturnValue.Global
    public Map Remove(object key, bool dispose = true) {
      if ((key == null) || !map.ContainsKey(key)) return this;

      keys.Remove(key);
      if (dispose) (map[key] as IDisposable)?.Dispose();
      map.Remove(key);

      return this;
    }

    /// <a href="http://bit.ly/2OrPYc5">Retrieve the value for the first key entered/sorted</a>
    public object First => Count > 0 ? keys[index.Start()] : null;

    /// <a href="http://bit.ly/2OrPYc5">Retrieve the value for the second key entered/sorted</a>
    public object Next => index.Reached(Count - 1) ? null : keys[index.Next()];

    private readonly CounterFifo index = CounterFifo.Instance;

    /// <a href="http://bit.ly/2Oq9wOe">Get a key by index based where 0 is the oldest/sorted</a>
    public object this[int i] => keys[i];

    /// <a href="http://bit.ly/2Orh4QH">Array of keys in entry/sorted order</a>
    public object[] Keys => keys.ToArray();

    /// <a href="http://bit.ly/2RezFOy">Sort keys ascending alphabetic</a>
    public Map Sort() {
      keys.Sort();
      return this;
    }

    /// <a href="http://bit.ly/2RezFOy">Sort keys using a comparator</a>
    public Map Sort(Comparison<object> comparison) {
      keys.Sort(Comparer<object>.Create(comparison));
      return this;
    }

    /// <a href="http://bit.ly/2RhchzZ">Prepare retrieval given a known key</a>
    public Map this[object key] {
      get {
        Found = map.TryGetValue(Key = key, out Value);
        return this;
      }
    }

    /// <a href="http://bit.ly/2Oq9DcC">Add a map or set entry</a>
    public Map Add(object key, object value = null) {
      if (key == null) return this;

      if (!map.ContainsKey(key)) keys.Add(key);
      map[Key = key] = Value = value;
      return this;
    }

    /// <a href="http://bit.ly/2Rj0Rfg">Number of entries in the map</a>
    public int Count => keys.Count;

    /// <a href="http://bit.ly/2Oq9qWS">Was the last search successful?</a>
    public bool Found;

    /// <a href="http://bit.ly/2Rin6lu">Last key searched for</a>
    public object Key;

    /// <a href="http://bit.ly/2Os3Wed">Last value found (null for failure)</a>
    public object Value;

    /// <a href="http://bit.ly/2NUH9Io">Remove all entries - calling Dispose() on each one</a>
    public virtual void Dispose() {
      if (cached) { Cache<Map>.Dispose(this); } else { DeactivateItem(this); }
    }

    internal static void DeactivateItem(Map item) {
      for (var i = 0; i < item.Count; i++) (item.map[item.keys[i]] as IDisposable)?.Dispose();
      item.keys.Clear();
      item.map.Clear();
      item.index.Dispose();
    }

    public void Clear() {
      keys.Clear();
      map.Clear();
    }

    /// <a href=""></a> //#TBD#//
    public Dictionary<string, Tv> ToDictionary<Tv>() where Tv : class {
      using (this) {
        var dictionary = new Dictionary<string, Tv>();

        for (var key = First; key != null; key = Next) dictionary[key.ToString()] = this[key].Value as Tv;
        return dictionary;
      }
    }

    /// <inheritdoc />
    public override string ToString() {
      (builder ?? (builder = new List<string>())).Clear();

      for (var key = First; key != null; key = Next) builder.Add($"{key}={this[key].Value}");

      return string.Join(separator: ", ", value: builder.ToArray());
    }

    private List<string> builder;
    private bool         cached;
  }
}