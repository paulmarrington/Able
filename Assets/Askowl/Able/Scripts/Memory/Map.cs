// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

namespace Askowl {
  using System;
  using System.Collections;
  using System.Collections.Generic;

  /// <a href=""></a>
  public class Map : IDisposable {
    private readonly Dictionary<object, object> map = new Dictionary<object, object>();

    /// <a href=""></a>
    private readonly ArrayList keys = new ArrayList();

    /// <a href="bit.ly/">Map</a>
    public Map(params object[] keyValuePairs) { Add(keyValuePairs); }

    /// <a href=""></a>
    public Map Add(params object[] keyValuePairs) {
      for (var j = 0; j < keyValuePairs.Length; j += 2) {
        SetKvp(keyValuePairs[j], keyValuePairs[j + 1]);
        keys.Add(Key);
        map[Key] = Value;
      }

      return this;
    }

    /// <a href=""></a>
    public Map Set(params object[] newKeys) {
      for (var j = 0; j < newKeys.Length; j++) {
        SetKvp(newKeys[j], null);
        keys.Add(Key);
        map[Key] = Value;
      }

      return this;
    }

    /// <a href=""></a>
    public Map Remove(params object[] oldKeys) {
      for (var j = 0; j < oldKeys.Length; j++) {
        if (map.ContainsKey(oldKeys[j])) {
          keys.Remove(oldKeys[j]);
          (map[oldKeys[j]] as IDisposable)?.Dispose();
          map.Remove(oldKeys[j]);
        }
      }

      return this;
    }

    /// <a href="bit.ly/">First</a>
    public object First => Count > 0 ? keys[index.Start()] : null;

    /// <a href="bit.ly/">First</a>
    public object Next => index.Reached(Count - 1) ? null : keys[index.Next()];

    private CounterStack index = CounterStack.Instance;

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
        SetKvp(key, null);
        Found = map.TryGetValue(key, out Value);
        return this;
      }
      set => map[Key = key] = Value = value;
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

    private void SetKvp(object key, object value, bool found = true) {
      Key   = key;
      Value = value;
      Found = found;
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