// Copyright 2019 (C) paul@marrington.net http://www.askowl.net/unity-packages

using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Askowl {
  /// <a href=""></a> //#TBD#//
  public class AssetDb : IDisposable {
    /// <a href=""></a> //#TBD#//
    public static AssetDb Instance => Cache<AssetDb>.Instance;
    /// <a href=""></a> //#TBD#//
    public Boolean Error;
    /// <a href=""></a> //#TBD#//
    public string CurrentFolder = "";

    /// <a href=""></a> //#TBD#//
    public static string ProjectFolder() {
      EditorUtility.FocusProjectWindow();
      foreach (Object asset in Selection.GetFiltered(typeof(Object), SelectionMode.Assets)) {
        var path = SetFolderFor(asset);
        if (path != null) return path;
      }
      return "Assets";
    }

    /// <a href=""></a> //#TBD#//
    public AssetDb ProjectFolder(out string path) {
      path = ProjectFolder();
      return this;
    }
    /// <a href=""></a> //#TBD#//
    private static string SetFolderFor(Object asset) {
      var path = AssetDatabase.GetAssetPath(asset);
      if (!string.IsNullOrEmpty(path))
        if (Directory.Exists(path)) {
          return path;
        } else if (File.Exists(path)) {
          return Path.GetDirectoryName(path);
        }
      return null;
    }
    /// <a href=""></a> //#TBD#//
    public AssetDb Select(Object asset) {
      if (Error) return this;
      EditorUtility.FocusProjectWindow();
      CurrentFolder = SetFolderFor(Selection.activeObject = asset);
      EditorGUIUtility.PingObject(asset);
      return this;
    }
    /// <a href=""></a> //#TBD#//
    public AssetDb Selected(out Object asset) {
      asset = Selection.activeObject;
      return this;
    }
    /// <a href=""></a> //#TBD#//
    public AssetDb Find(string path, out Object asset) {
      path  = AbsoluteFolder(path);
      asset = AssetDatabase.LoadAssetAtPath(path, typeof(Object));
      Error = asset == null;
      if (!Error) CurrentFolder = path;
      return this;
    }
    /// <a href=""></a> //#TBD#//
    public AssetDb Select(string path = null) => Find(path ?? CurrentFolder, out var asset).Select(asset);

    /// <a href=""></a> //#TBD#//
    public AssetDb CreateFolders(string path) {
      if (Error) return this;
      if (path[0] == '/') CurrentFolder = "";

      var folders = path.Split('/');
      for (int i = 0; i < folders.Length; i++) {
        if (folders[i].Length != 0) {
          Find(folders[i], out Object _);
          if (Error) {
            Error = AssetDatabase.CreateFolder(CurrentFolder, folders[i]) == null;
            if (Error) return this;
            CurrentFolder = $"{CurrentFolder}/{folders[i]}";
          }
        }
      }
      return this;
    }
    /// <a href=""></a> //#TBD#//
    public AssetDb Delete(string path) {
      if (Error) return this;
      Error = AssetDatabase.MoveAssetToTrash(AbsoluteFolder(path));
      return this;
    }
    /// <a href=""></a> //#TBD#//
    public AssetDb SubFolders(out string[] subFolders) {
      subFolders = AssetDatabase.GetSubFolders(CurrentFolder);
      return this;
    }

    private string AbsoluteFolder(string path) =>
      (path[0] == '/') ? path.Substring(1) : (CurrentFolder != "") ? $"{CurrentFolder}/{path}" : path;

    public void Dispose() {
      CurrentFolder = "";
      Error         = false;
      Cache<AssetDb>.Dispose(this);
    }
  }
}