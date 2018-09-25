// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

using System.Collections.Generic;
using UnityEngine;

namespace Askowl {
  /// <a href=""></a>
  public static class Objects {
    /// <a href=""></a>
    public static T Find<T>(string name) where T : Object {
      var objects = FindAll<T>(name);
      return objects.Length > 0 ? objects[0] : null;
    }

    /// <a href=""></a>
    public static T[] FindAll<T>(string name) where T : Object {
      var all     = Resources.FindObjectsOfTypeAll<T>();
      var results = new List<T>();

      if (string.IsNullOrEmpty(name)) return all;

      for (var i = 0; i < all.Length; i++) {
        if (all[i].name == name) results.Add(all[i]);
      }

      return results.ToArray();
    }

    /// <a href=""></a>
    public static string Path(GameObject gameObject) {
      var       path      = new List<string>();
      Transform transform = gameObject.transform;

      while (transform != null) {
        path.Add(transform.name);
        transform = transform.parent;
      }

      path.Reverse();
      return string.Join(separator: "/", value: path.ToArray());
    }

    /// <a href=""></a>
    public static GameObject CreateGameObject(string path) {
      var split                          = path.Split('/');
      int last                           = split.Length - 1;
      var gameObject                     = GameObject.Find($"/{split[0]}");
      if (gameObject == null) gameObject = new GameObject(split[0]);

      for (var i = 1; i <= last; i++) {
        var transform = gameObject.transform.Find(split[i]);

        if (transform == null) {
          transform        = new GameObject(split[i]).transform;
          transform.parent = gameObject.transform;
        }

        gameObject = transform.gameObject;
      }

      return gameObject;
    }
  }
}