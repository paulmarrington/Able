/*
 * By Romejanic
 * https://forum.unity.com/threads/editor-utility-player-prefs-editor-edit-player-prefs-inside-the-unity-editor.370292/
 */

using System.Globalization;
using UnityEditor;
using UnityEngine;

/// <a href="http://bit.ly/2IvOrg1">Player Prefs Editor</a> <inheritdoc />
public sealed class PlayerPrefsEditor : EditorWindow {
  /// <a href="http://bit.ly/2IvOrg1">Player Prefs Editor</a>
  [MenuItem("Edit/Player Preferences")] public static void OpenWindow() {
    PlayerPrefsEditor window =
      (PlayerPrefsEditor) GetWindow(typeof(PlayerPrefsEditor));

    window.titleContent = new GUIContent("Player Prefs");
    window.Show();
  }

  /// <a href="http://bit.ly/2IvOrg1">Player Prefs Editor recognised fields</a>
  // ReSharper disable MissingXmlDoc
  public enum FieldType { String, Integer, Float }
  // ReSharper restore MissingXmlDoc

  private FieldType fieldType = FieldType.String;
  private string    setKey    = "";
  private string    setVal    = "";
  private string    error     = null;

  private void OnGUI() {
    EditorGUILayout.LabelField("Player Preferences Editor", EditorStyles.boldLabel);
    EditorGUILayout.LabelField("by RomejanicDev");
    EditorGUILayout.Separator();

    fieldType = (FieldType) EditorGUILayout.EnumPopup("Key Type", fieldType);
    setKey    = EditorGUILayout.TextField("Key to Set",   setKey);
    setVal    = EditorGUILayout.TextField("Value to Set", setVal);

    if (error != null) EditorGUILayout.HelpBox(error, MessageType.Error);

    if (GUILayout.Button("Set Key")) {
      if (fieldType == FieldType.Integer) {
        if (!int.TryParse(setVal, out int result)) {
          error = "Invalid input \"" + setVal + "\"";
          return;
        }

        PlayerPrefs.SetInt(setKey, result);
      }
      else if (fieldType == FieldType.Float) {
        if (!float.TryParse(setVal, out float result)) {
          error = "Invalid input \"" + setVal + "\"";
          return;
        }

        PlayerPrefs.SetFloat(setKey, result);
      }
      else { PlayerPrefs.SetString(setKey, setVal); }

      PlayerPrefs.Save();
      error = null;
    }

    if (GUILayout.Button("Get Key")) {
      if (fieldType      == FieldType.Integer) { setVal = PlayerPrefs.GetInt(setKey).ToString(); }
      else if (fieldType == FieldType.Float) {
        setVal = PlayerPrefs.GetFloat(setKey).ToString(CultureInfo.InvariantCulture);
      }
      else { setVal = PlayerPrefs.GetString(setKey); }
    }

    if (GUILayout.Button("Delete Key")) {
      PlayerPrefs.DeleteKey(setKey);
      PlayerPrefs.Save();
    }

    if (GUILayout.Button("Delete All Keys")) {
      PlayerPrefs.DeleteAll();
      PlayerPrefs.Save();
    }
  }
}