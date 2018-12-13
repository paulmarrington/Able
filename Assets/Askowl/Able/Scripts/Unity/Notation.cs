using UnityEngine;

/// <a href="">Simple Component to add multiline notes to the Unity inspector (use Commentary for single-line)</a> //#TBD#//
[AddComponentMenu("Comments/Multiline")]
public class Notation : MonoBehaviour {
  /// <a href="">Only for Unity inspector</a> //#TBD#//
  [SerializeField, Multiline] public string Note;
}