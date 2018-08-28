// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

using System;
using System.Collections;
using System.Collections.Generic;

namespace Askowl {
  /// <a href=""></a>
  public class Map {
    private readonly Dictionary<object, object> map = new Dictionary<object, object>();

    /// <a href=""></a>
    private readonly ArrayList keys = new ArrayList();

    /// <a href="bit.ly/">Map</a>
    public Map(params object[] keyValuePairs) { Add(keyValuePairs); }

    /// <a href=""></a>
    public Map Add(params object[] keyValuePairs) {
      for (int j = 0; j < keyValuePairs.Length; j += 2) {
        keys.Add(keyValuePairs[j]);
        map[keyValuePairs[j]] = keyValuePairs[j + 1];
      }

      return this;
    }

    /// <a href=""></a>
    public Map Set(params object[] newKeys) {
      for (int j = 0; j < newKeys.Length; j++) {
        keys.Add(newKeys[j]);
        map[newKeys[j]] = null;
      }

      return this;
    }

    /// <a href=""></a>
    public Map Remove(params object[] oldKeys) {
      for (int j = 0; j < oldKeys.Length; j++) {
        keys.Remove(oldKeys[j]);
        map.Remove(oldKeys[j]);
      }

      return this;
    }

    /// <a href=""></a>
    public bool IsA<T>() => Found && (Value is T);

    /// <a href="bit.ly/">First</a>
    public string First => (Count > 0) ? keys[index = 0] as string : null;

    /// <a href="bit.ly/">First</a>
    public string Next => (++index < Count) ? keys[index] as string : null;

    private int index;

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
        Value = null;
        Found = map.TryGetValue(Key = key, out Value);
        return this;
      }
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
    public T As<T>() => IsA<T>() ? ((T) Value) : default(T);

    /// <a href=""></a>
    public Type TypeOf => Value?.GetType();
  }
}