// Copyright 2019 (C) paul@marrington.net http://www.askowl.net/unity-packages

using System;
using System.Linq;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Askowl {
  /// <a href=""></a> //#TBD#//
  public interface IOnScriptReload {
    /// <a href=""></a> //#TBD#//
    void OnScriptReload();
  }
  /// <a href=""></a> //#TBD#//
  public static class OnScriptReloaded {
    /// <a href=""></a> //#TBD#//
    public static void Register(ScriptableObject continuation) {
      var guid  = Guid.NewGuid().ToString();
      var guids = PlayerPrefs.GetString("Askowl.OnScriptReload") ?? "";
      PlayerPrefs.SetString("Askowl.OnScriptReload", $"{guid};{guids}");
      PlayerPrefs.SetString(guid,                    continuation.GetType().ToString());
      var json = EditorJsonUtility.ToJson(continuation, true);
      PlayerPrefs.SetString($"{guid}-Content", EditorJsonUtility.ToJson(continuation));
    }

    [DidReloadScripts] private static void Phase2() {
      var guids = PlayerPrefs.GetString("Askowl.OnScriptReload").Split(';').Reverse();
      PlayerPrefs.DeleteKey("Askowl.OnScriptReload");
      foreach (var guid in guids) {
        var type = Type.GetType(typeName: guid);
        (ScriptableObject.CreateInstance(type) as IOnScriptReload)?.OnScriptReload();
      }
    }
  }
}