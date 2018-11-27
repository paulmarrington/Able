// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

namespace Askowl {
  using UnityEngine;

  /// <a href="http://bit.ly/2NU54b0">Static helper for Unity Components</a>
  public static class Components {
    /// <a href="http://bit.ly/2RezHpE">Find a component on a given path</a>
    public static T Find<T>(string path = "") where T : Component {
      var split         = path.Split('/');
      var parentObjects = Objects.FindAll<GameObject>(split[0]);

      for (var i = 0; i < parentObjects.Length; i++) {
        var found = Find<T>(parentObjects[i], split);
        if (found != default(T)) return found;
      }

      return default;
    }

    /// <a href="http://bit.ly/2NX3NjD">Find a component of a specific type on a given path</a>
    public static T Find<T>(GameObject inParentObject, string path) where T : Component =>
      Find<T>(inParentObject, path.Split('/'));

    /// <a href="http://bit.ly/2NX3NjD">Find a component of a specific type on a given path</a>
    public static T Find<T>(GameObject inParentObject, string[] path) where T : Component {
      var components = inParentObject.GetComponentsInChildren<T>();
      if (components.Length == 0) return default;

      for (var i = 0; i < components.Length; i++) {
        var found = true;

        GameObject childObject = components[i].gameObject;

        for (int j = path.Length - 1; found && (j >= 0); j--) {
          if (path[j].Length > 0) {
            while (found && (childObject.name != path[j])) {
              if (childObject == inParentObject) { found = false; }
              else { childObject                         = childObject.transform.parent.gameObject; }
            }
          }
        }

        if (found) return components[i];
      }

      return default;
    }

    /// <a href="http://bit.ly/2Oo6eer">Create an instance of a component</a>
    public static T Create<T>(string path) where T : Component => Objects.CreateGameObject(path).AddComponent<T>();

    /// <a href="http://bit.ly/2RhMbN5">Find or create a component</a>
    public static T Establish<T>(string path) where T : Component => Find<T>(path) ?? Create<T>(path);
  }
}