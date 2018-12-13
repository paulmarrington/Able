using UnityEngine;

/// <a href="">Simple Component to add single line notes to the Unity inspector (use Notation for single-line)</a> //#TBD#//
[AddComponentMenu("Comments/Single Line")]
public class Commentary : MonoBehaviour {
  /// <a href="">Only for Unity inspector</a> //#TBD#//
  [SerializeField] public string Comment;
}