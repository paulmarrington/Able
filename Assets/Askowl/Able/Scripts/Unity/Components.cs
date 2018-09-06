// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

namespace Askowl {
  using UnityEngine;

  /// <a href=""></a>
  public static class Components {
    /// <a href=""></a>
    public static T Find<T>(params string[] path) where T : Component {
      if ((path = ParsePath(path)) == null) return default(T);

      GameObject[] parentObjects = Objects.FindGameObjects(path[0]);

      for (int i = 0; i < parentObjects.Length; i++) {
        T found = Find<T>(parentObjects[i], path);
        if (found != default(T)) return found;
      }

      return default(T);
    }

    /// <a href=""></a>
    public static T Find<T>(GameObject parentObject, params string[] path) where T : Component {
      path = ParsePath(path);
      T[] components = parentObject.GetComponentsInChildren<T>();
      if (components.Length == 0) return default(T);

      for (int i = 0; i < components.Length; i++) {
        bool found = true;

        GameObject childObject = components[i].gameObject;

        for (int j = path.Length - 1; found && (j >= 0); j--) {
          if (path[j].Length > 0) {
            while (found && (childObject.name != path[j])) {
              if ((childObject == parentObject)) {
                found = false;
              } else {
                childObject = childObject.transform.parent.gameObject;
              }
            }
          }
        }

        if (found) return components[i];
      }

      return default(T);
    }

    /// <a href=""></a>
    public static T Create<T>(params string[] path) where T : Component {
      GameObject gameObject = Objects.CreateGameObject(path);
      return gameObject.AddComponent<T>();
    }

    /// <a href=""></a>
    public static T Establish<T>(params string[] path) where T : Component =>
      Find<T>(path) ?? Create<T>(path);

    private static string[] ParsePath(string[] path) {
      if (path.Length == 0) return null;

      if (path.Length == 1) path = path[0].Split('/');
      return path;
    }
  }
}