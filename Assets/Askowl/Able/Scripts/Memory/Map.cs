// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

namespace Askowl {
  using System;
  using System.Collections;
  using System.Collections.Generic;

  /// <a href=""></a>
  // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
  public class Map : IDisposable {
    private readonly Dictionary<object, object> map = new Dictionary<object, object>();

    /// <a href=""></a>
    private readonly ArrayList keys = new ArrayList();

    /// <a href=""></a>
    // ReSharper disable once UnusedMethodReturnValue.Global
    public Map Remove(object key, bool dispose = true) {
      if ((key == null) || !map.ContainsKey(key)) return this;

      keys.Remove(key);
      if (dispose) (map[key] as IDisposable)?.Dispose();
      map.Remove(key);

      return this;
    }

    /// <a href="bit.ly/">First</a>
    public object First => Count > 0 ? keys[index.Start()] : null;

    /// <a href="bit.ly/">First</a>
    public object Next => index.Reached(Count - 1) ? null : keys[index.Next()];

    private CounterFifo index = CounterFifo.Instance;

    /// <a href="bit.ly/">[</a>
    public object this[int i] => keys[i];

    /// <a href="bit.ly/">Keys</a>
    public object[] Keys => keys.ToArray();

    /// <a href="bit.ly/">Sort</a>
    public Map Sort() {
      keys.Sort();
      return this;
    }

    /// <a href="bit.ly/">Sort</a>
    public Map Sort(Comparison<object> comparison) {
      keys.Sort(Comparer<object>.Create(comparison));
      return this;
    }

    /// <a href="bit.ly/">[</a>
    public Map this[object key] {
      get {
        Found = map.TryGetValue(Key = key, out Value);
        return this;
      }
    }

    /// <a href=""></a>
    public Map Add(object key, object value = null) {
      if (key == null) return this;

      if (!map.ContainsKey(key)) keys.Add(key);
      map[Key = key] = Value = value;
      return this;
    }

    /// <a href="bit.ly/">Count</a>
    public int Count => keys.Count;

    /// <a href=""></a>
    public bool Found;

    /// <a href=""></a>
    public object Key;

    /// <a href=""></a>
    public object Value;

    /// <a href=""></a>
    public virtual void Dispose() {
      for (var i = 0; i < Count; i++) (map[keys[i]] as IDisposable)?.Dispose();
      keys.Clear();
      map.Clear();
      index.Dispose();
    }

    /// <inheritdoc />
    public override string ToString() {
      (builder ?? (builder = new List<string>())).Clear();

      for (var key = First; key != null; key = Next) builder.Add($"{key}={Value}");

      return string.Join(separator: ", ", value: builder.ToArray());
    }

    private List<string> builder;
  }
}