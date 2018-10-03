// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

#if AskowlAble
namespace Askowl {
  using UnityEngine;

  /// <a href=""></a>
  /// <inheritdoc />
  public class ConditionalHideExample : MonoBehaviour {
    [Header("Test Boolean Conditional"), SerializeField]
    private bool enableAll = true;

    [ConditionalHide("enableAll"), SerializeField]
    private int number;

    [ConditionalHide("enableAll"), SerializeField]
    private string userName = "User Name";

    [Header("Test String Conditional"), SerializeField]
    private string aString;

    [ConditionalHide("aString"), SerializeField]
    private int hiThere = 77;

    [Header("Test Object Reference Conditional"), SerializeField]
    private GameObject text = null;

    [ConditionalHide("text"), SerializeField]
    private string backupContent = "blat";

    [ConditionalHide("text"), SerializeField]
    private int whatNow = 33;
    /// <a href=""></a>
    public string AString { get => aString; set => aString = value; }

    private void Start() {
      if (enableAll) { number = whatNow; }
      else { userName         = backupContent; }

      if (text != null) {
        whatNow       = number;
        number        = hiThere;
        backupContent = userName;
      }
    }
  }
}
#endif