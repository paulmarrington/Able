// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

using System.Collections.Generic;
using UnityEngine;

namespace Askowl {
  /// <summary>
  /// Helper library for dealing with Unity Objects
  /// </summary>
  /// <remarks><a href="http://customassets.marrington.net#objects-helpers">More...</a></remarks>
  public static class Objects {
    /// <summary>
    /// Find an GameObject that has already been loaded into memory given it's name in the hierarchy.
    /// If there are more than one of this type, only one is returned.
    /// </summary>
    /// <remarks><a href="http://customassets.marrington.net#findt">More...</a></remarks>
    /// <param name="name">Name of the asset within the project heirarchy</param>
    /// <returns>List of game objects with this name</returns>
    public static GameObject[] FindGameObjects(string name) { return Find<GameObject>(name); }

    /// <summary>
    /// Find an GameObject that has already been loaded into memory given it's name in the hierarchy.
    /// If there are more than one of this type, only one is returned.
    /// </summary>
    /// <remarks><a href="http://customassets.marrington.net#findt">More...</a></remarks>
    /// <param name="name">Name of the asset within the project heirarchy</param>
    /// <returns>GameObject if found, null if not</returns>
    public static GameObject FindGameObject(string name) {
      GameObject[] gameObjects = FindGameObjects(name);
      return (gameObjects.Length > 0) ? gameObjects[0] : null;
    }

    /// <summary>
    /// Search for a GameObject by name and return a component on it by type
    /// </summary>
    /// <remarks><a href="http://customassets.marrington.net#findt">More...</a></remarks>
    /// <param name="name">Name of the asset within the project heirarchy</param>
    /// <see cref="Resources.FindObjectsOfTypeAll"/>
    /// <typeparam name="T">Component type that must be in the object</typeparam>
    /// <returns>An array of matching objects found</returns>
    public static T[] Find<T>(string name) where T : Object {
      T[]     all     = Resources.FindObjectsOfTypeAll<T>();
      List<T> results = new List<T>();

      if (name == null) return all;

      for (int i = 0; i < all.Length; i++) {
        if (all[i].name == name) results.Add(all[i]);
      }

      return results.ToArray();
    }

    /// <summary>
    /// Glue together the path to the provided game object in the hierarchy
    /// </summary>
    /// <param name="gameObject">Path from the root to here</param>
    /// <returns>Path from the root separated by /</returns>
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
  }
}