using UnityEngine;

#if UNITY_EDITOR && AskowlBase
namespace Askowl.Samples {
  /// <inheritdoc />
  /// <summary>
  /// Exercise classes in Askowl-Base
  /// </summary>
  public class BaseExampleBehaviour : MonoBehaviour {
    /// <summary>
    /// Search for a GameObject by name and return a component on it by type
    /// </summary>
    public void FindDisabledObject() {
      MeshFilter[] cubes = Objects.Find<MeshFilter>("Disabled Object");

      Debug.LogFormat("Found {0} matching game objects. This is {1} correct",
                      cubes.Length, (cubes.Length == 1) ? "" : "NOT");

      if (cubes.Length == 0) return;

      cubes[0].gameObject.SetActive(!cubes[0].gameObject.activeInHierarchy);
      Debug.LogFormat("Cube is now {0}abled", cubes[0].gameObject.activeInHierarchy ? "En" : "Dis");
    }
  }
}
#endif