// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

using System;
using System.Text;

namespace Askowl {
  public class Map {
    public struct Node {
      private object key, value;
    }

    private Node[] nodes;
    private int    count;

    private void Add(object key, object value) {
      if (count)
    }

    public static Map Set(params object[] keys) { }

    public Map(params object[] keyValuePairs) {
      count = keyValuePairs.Length / 2;
      nodes = new Node[(count < 4) ? 4 : count];

      for (int i = 0; i < keyValuePairs.Length; i += 2) { }
    }
  }
}