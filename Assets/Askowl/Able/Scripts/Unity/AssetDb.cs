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
    public AssetDb ProjectFolder(out string path) {
      Error = false;
      path  = "Assets";
      EditorUtility.FocusProjectWindow();
      foreach (Object asset in Selection.GetFiltered(typeof(Object), SelectionMode.Assets))
        if (SetFolderFor(asset))
          break;
      path = CurrentFolder;
      return this;
    }
    /// <a href=""></a> //#TBD#//
    private bool SetFolderFor(Object asset) {
      var path = AssetDatabase.GetAssetPath(asset);
      if (!string.IsNullOrEmpty(path))
        if (Directory.Exists(path)) {
          CurrentFolder = path;
          return true;
        } else if (File.Exists(path)) {
          CurrentFolder = Path.GetDirectoryName(path);
          return true;
        }
      Error = true;
      return false;
    }
    /// <a href=""></a> //#TBD#//
    public AssetDb Select(Object asset) {
      if (Error) return this;
      EditorUtility.FocusProjectWindow();
      SetFolderFor(Selection.activeObject = asset);
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
      CurrentFolder = AbsoluteFolder(path);
      asset         = AssetDatabase.LoadAssetAtPath(CurrentFolder, typeof(Object));
      Error         = asset == null;
      return this;
    }
    /// <a href=""></a> //#TBD#//
    public AssetDb Select(string path) => Find(path, out var asset).Select(asset);

    /// <a href=""></a> //#TBD#//
    public AssetDb CreateFolders(string path) {
      if (Error) return this;
      if (path[0] == '/') {
        path          = path.Substring(1);
        CurrentFolder = "";
      }
      var folders = path.Split('/');
      for (int i = 0; i < folders.Length; i++) {
        Find(folders[i], out Object _);
        if (Error) {
          Debug.Log($"*** CreateFolders '{string.Join("/", folders, 0, i - 1)}'/'{folders[i]}'"); //#DM#//
          AssetDatabase.CreateFolder(string.Join("/", folders, 0, i - 1), folders[i]);
          CurrentFolder = AbsoluteFolder(folders[i]);
          Debug.Log($"*** CreateFolders '{CurrentFolder}'"); //#DM#//
          Error         = false;
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