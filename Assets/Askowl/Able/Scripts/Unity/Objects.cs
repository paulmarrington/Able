// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

using System.Collections.Generic;
using UnityEngine;

namespace Askowl {
  /// <a href=""></a>
  public static class Objects {
    /// <a href=""></a>
    public static GameObject[] FindGameObjects(string name) => Find<GameObject>(name);

    /// <a href=""></a>
    public static GameObject FindGameObject(string name) {
      GameObject[] gameObjects = FindGameObjects(name);
      return (gameObjects.Length > 0) ? gameObjects[0] : null;
    }

    /// <a href=""></a>
    public static T[] Find<T>(string name) where T : Object {
      T[]     all     = Resources.FindObjectsOfTypeAll<T>();
      List<T> results = new List<T>();

      if (name == null) return all;

      for (int i = 0; i < all.Length; i++) {
        if (all[i].name == name) results.Add(all[i]);
      }

      return results.ToArray();
    }

    /// <a href=""></a>
    public static string Path(GameObject gameObject) {
      List<string> path      = new List<string>();
      Transform    transform = gameObject.transform;

      while (transform != null) {
        path.Add(transform.name);
        transform = transform.parent;
      }

      path.Reverse();
      return string.Join(separator: "/", value: path.ToArray());
    }

    /// <a href=""></a>
    public static GameObject CreateGameObject(params string[] path) {
      if (path.Length == 1) path         = path[0].Split('/');
      var last                           = path.Length - 1;
      var gameObject                     = GameObject.Find($"/{path[0]}");
      if (gameObject == null) gameObject = new GameObject(path[0]);

      for (int i = 1; i <= last; i++) {
        var transform = gameObject.transform.Find(path[i]);

        if (transform == null) {
          transform        = new GameObject(path[i]).transform;
          transform.parent = gameObject.transform;
        }

        gameObject = transform.gameObject;
      }

      return gameObject;
    }
  }
}