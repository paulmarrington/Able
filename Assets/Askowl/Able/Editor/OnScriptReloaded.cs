// Copyright 2019 (C) paul@marrington.net http://www.askowl.net/unity-packages

using System;
using System.Linq;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace Askowl {
  /// <a href=""></a> //#TBD#//
  public interface IOnScriptReload {
    /// <a href=""></a> //#TBD#//
    void OnScriptReload();
  }
  /// <a href=""></a> //#TBD#//
  public static class OnScriptReloaded {
    /// <a href=""></a> //#TBD#//
    public static void Register(object continuation) {
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
      foreach (var guid in guids)
        if (!string.IsNullOrEmpty(guid)) {
          var typeName = PlayerPrefs.GetString(guid);
          var json     = PlayerPrefs.GetString($"{guid}-Content");
          PlayerPrefs.DeleteKey(guid);
          var type = Type.GetType(typeName);
          if (type == null) throw new Exception($"Unknown type {typeName}'");
          var target = (type.IsAssignableFrom(typeof(ScriptableObject)))
                         ? ScriptableObject.CreateInstance(type)
                         : ObjectFactory.CreateInstance(type);
          if (target == null) throw new Exception($"Can't instantiate '{typeName}'");
          EditorJsonUtility.FromJsonOverwrite(json, target);
          (target as IOnScriptReload)?.OnScriptReload();
        }
    }
  }
}