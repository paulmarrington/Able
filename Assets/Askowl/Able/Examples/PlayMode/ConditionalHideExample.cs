﻿// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

using UnityEngine;

namespace Askowl {
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
    private GameObject text;

    [ConditionalHide("text"), SerializeField]
    private string backupContent = "blat";

    [ConditionalHide("text"), SerializeField]
    private int whatNow = 33;

    private void Start() {
      if (enableAll) {
        number = whatNow;
      } else {
        userName = backupContent;
      }

      if (text != null) {
        whatNow       = number;
        number        = hiThere;
        backupContent = userName;
      }
    }
  }
}