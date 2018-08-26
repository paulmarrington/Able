// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

using System;

namespace Askowl {
  /// <a href=""></a>
  public class Map {
    private object[] keys, values;

    /// <a href=""></a>
    public int Count { get; private set; }

    /// <a href=""></a>
    public Map(params object[] keyValuePairs) {
      var length = (Count < 4) ? 4 : Count;
      keys   = new object[length];
      values = new object[length];
      Add(keyValuePairs: keyValuePairs);
    }

    /// <a href=""></a>
    public Map Set(params object[] newKeys) {
      var oldCount = Count;
      Count += newKeys.Length;

      if (keys.Length < Count) Resize(to: Count);

      for (int i = oldCount, j = 0; i < Count; i++) {
        keys[i]   = newKeys[j++];
        values[i] = null;
      }

      Array.Sort(keys: keys, items: values, index: 0, length: Count);
      return this;
    }

    /// <a href=""></a>
    public Map Add(params object[] keyValuePairs) {
      var oldCount = Count;
      Count += keyValuePairs.Length / 2;

      if (keys.Length < Count) Resize(to: Count);

      for (int i = oldCount, j = 0; i < Count; i++) {
        keys[i]   = keyValuePairs[j++];
        values[i] = keyValuePairs[j++];
      }

      Array.Sort(keys: keys, items: values, index: 0, length: Count);
      return this;
    }

    /// <a href=""></a>
    public bool Contains(object key) {
      closest = Array.BinarySearch(array: keys, value: Key = key);
      Found   = closest >= 0;

      if (Found) {
        Value = values[closest];
        return true;
      }

      closest = ~closest;
      Key     = ((closest >= 0) && (closest < keys.Length)) ? keys[closest] : "";
      Value   = null;
      return false;
    }

    /// <a href=""></a>
    public bool IsA<T>(object key) => Contains(key: key) && (Value is T);

    /// <a href=""></a>
    public T Get<T>(object key) => IsA<T>(key: key) ? ((T) Value) : default(T);

    /// <a href=""></a>
    private int closest;

    /// <a href=""></a>
    public bool Found { get; private set; }

    /// <a href=""></a>
    public object Key { get; private set; }

    /// <a href=""></a>
    public object Value { get; private set; }

    private void Resize(int to) {
      var length              = keys.Length * 2;
      if (length < to) length = to;
      keys   = Resize(from: keys,   length: length);
      values = Resize(from: values, length: length);
    }

    private object[] Resize(object[] from, int length) {
      var larger = new object[length];
      Array.Copy(sourceArray: from, destinationArray: larger, length: from.Length);
      return larger;
    }
  }
}