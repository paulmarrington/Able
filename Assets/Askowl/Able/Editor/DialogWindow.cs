// Copyright 2019 (C) paul@marrington.net http://www.askowl.net/unity-packages
using System;
using UnityEditor;
using UnityEngine;

namespace Askowl {
  /// <a href=""></a> //#TBD#//
  public class DialogWindow : EditorWindow {
    private string       titleBar, prompt, ok, cancel;
    private Action<bool> response;
    private bool         responded;

    /// <a href=""></a> //#TBD#//
    public static DialogWindow Open(string title, string prompt, string ok, string cancel, Action<bool> onResponse) {
      var win = GetWindow<DialogWindow>();
      (win.titleBar, win.prompt, win.ok, win.cancel, win.response) = (title, prompt, ok, cancel, onResponse);
      win.Show();
      return win;
    }

    private void OnEnable() => responded = false;

    private void OnDisable() {
      if (!responded) response(true);
    }

    private void OnGUI() {
      titleContent.text = titleBar;
      if (GUILayout.Button(cancel)) Respond(false);
      GUILayout.Space(pixels: 10);
      GUIStyle style = GUI.skin.GetStyle("HelpBox");
      style.richText  = true;
      style.fontSize  = 16;
      style.alignment = TextAnchor.MiddleCenter;
      EditorGUILayout.LabelField(prompt, style);
      GUILayout.Space(pixels: 10);
      if (GUILayout.Button(ok)) Respond(true);
    }

    private void Respond(bool ok) {
      responded = true;
      Close();
      response(ok);
    }
  }
}