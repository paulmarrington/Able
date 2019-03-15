// Copyright 2019 (C) paul@marrington.net http://www.askowl.net/unity-packages

using System;
using System.IO;
using UnityEditor;
using Object = UnityEngine.Object;

namespace Askowl {
  /// <a href=""></a> //#TBD#//
  public class AssetDb : IDisposable {
    /// <a href=""></a> //#TBD#//
    public static AssetDb Instance => Cache<AssetDb>.Instance;
    /// <a href=""></a> //#TBD#//
    public Boolean Error;

    private string currentLocation = "";

    /// <a href=""></a> //#TBD#//
    public AssetDb CurrentFolder(out string path) {
      path = "Assets";
      foreach (Object obj in Selection.GetFiltered(typeof(Object), SelectionMode.Assets)) {
        path = AssetDatabase.GetAssetPath(obj);
        if (!string.IsNullOrEmpty(path) && File.Exists(path)) {
          path = Path.GetDirectoryName(path);
          break;
        }
      }
      currentLocation = path;
      return this;
    }
    /// <a href=""></a> //#TBD#//
    public AssetDb Select(Object asset) {
      if (Error) return Reset();
      EditorUtility.FocusProjectWindow();
      Selection.activeObject = asset;
      EditorGUIUtility.PingObject(asset);
      return this;
    }
    private AssetDb Reset() { throw new NotImplementedException(); }
    /// <a href=""></a> //#TBD#//
    public AssetDb Find(string path, out Object asset) {
      asset = AssetDatabase.LoadAssetAtPath(path, typeof(Object));
      Error = asset != null;
      return this;
    }
    /// <a href=""></a> //#TBD#//
    public AssetDb Select(string path) => Find(path, out var asset).Select(asset);
    /// <a href=""></a> //#TBD#//
    public AssetDb CreateFolders(string path) { return this; }
    /// <a href=""></a> //#TBD#//
    public AssetDb DeleteFolder(string path) { return this; }

    public void Dispose() {
      currentLocation = "";
      Cache<AssetDb>.Dispose(this);
    }
  }
}