// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

namespace Askowl {
  using UnityEngine;

  /// <summary>
  /// Static helper class to reduce scaffolding when working with components
  /// </summary>
  /// <remarks><a href="http://customassets.marrington.net#components">More...</a></remarks>
  public static class Components {
    /// <summary>
    /// Retrieve a reference to an active component by type from GameObject
    /// </summary>
    /// <remarks><a href="http://customassets.marrington.net#componentsfindtname">More...</a></remarks>
    /// <param name="path">Unique path to a GameObject in the scene. Doesn't need to be contiguous. Can be multiple strings or one with / as path separator</param>
    /// <typeparam name="T">Type of the Component in the GameObject</typeparam>
    /// <returns>Reference to the component</returns>
    public static T Find<T>(params string[] path) where T : Component {
      if (path.Length == 0) return default(T);

      if (path.Length == 1) path = path[0].Split('/');

      GameObject[] parentObjects = Objects.FindGameObjects(path[0]);

      for (int i = 0; i < parentObjects.Length; i++) {
        T found = Find<T>(parentObjects[i], path);
        if (found != default(T)) return found;
      }

      return default(T);
    }

    private static T Find<T>(GameObject parentObject, string[] path) where T : Component {
      T[] components = parentObject.GetComponentsInChildren<T>();
      if (components.Length == 0) return default(T);

      if (path.Length == 1) return components[0];

      for (int i = 0; i < components.Length; i++) {
        bool found = true;

        GameObject childObject = components[i].gameObject;

        for (int j = path.Length - 1; found && (j >= 0); j--) {
          while (found && (childObject.name != path[j])) {
            if ((childObject == parentObject)) {
              found = false;
            } else {
              childObject = childObject.transform.parent.gameObject;
            }
          }
        }

        if (found) return components[i];
      }

      return default(T);
    }

    /// <summary>
    /// Create a new GameObject, name it and add a component of the specified type.
    /// </summary>
    /// <remarks><a href="http://customassets.marrington.net#componentscreatetname">More...</a></remarks>
    /// <param name="name">The name given to both the game object and contained component</param>
    /// <typeparam name="T">Type of component we are creating</typeparam>
    /// <returns>a reference to the game object containing the newly minted component</returns>
    // ReSharper disable once UnusedMember.Global
    public static T Create<T>(string name = null) where T : Component {
      GameObject gameObject = new GameObject();

      T instance = Create<T>(gameObject, name);
      gameObject.name = instance.name;
      return instance;
    }

    /// <summary>
    /// Create a new GameObject, name it and add a component of the specified type.
    /// </summary>
    /// <remarks><a href="http://customassets.marrington.net#componentscreatetgameobjectname">More...</a></remarks>
    /// <param name="gameObject">The existing game object to which we are adding a new component</param>
    /// <param name="name">The name given to both the game object and contained component</param>
    /// <typeparam name="T">Type of component we are creating</typeparam>
    /// <returns>a reference to the game object containing the newly minted component</returns>
    // ReSharper disable once MemberCanBePrivate.Global
    public static T Create<T>(GameObject gameObject, string name = null)
      where T : Component {
      T instance = gameObject.AddComponent<T>();
      instance.name = name ?? typeof(T).ToString();
      return instance;
    }
  }
}