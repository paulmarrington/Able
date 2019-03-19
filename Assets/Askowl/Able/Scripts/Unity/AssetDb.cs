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

    private string AbsoluteFolder(string path) {
      if (path.StartsWith("Assets")) return path;
      return (path[0] == '/') ? path.Substring(1) : (CurrentFolder != "") ? $"{CurrentFolder}/{path}" : path;
    }

    public void Dispose() {
      CurrentFolder = "";
      Error         = false;
      Cache<AssetDb>.Dispose(this);
    }

    #region Asset Creation and Loading (Editor only)
    #if UNITY_EDITOR
    /// <a href=""></a> //#TBD#//
    public static Object Load(string path, Type type) {
      path = Objects.FindFile(path);
      return (path == null) ? null : AssetDatabase.LoadAssetAtPath(path, type);
    }

    /// <a href=""></a> //#TBD#//
    public static T Load<T>(string path) where T : Object => (T) Load(path, typeof(T));

    /// <a href=""></a> //#TBD#//
    public AssetDb Load(string path, out Object asset, Type type) {
      asset = Load(path, type);
      Error = asset == null;
      return this;
    }

    /// <a href=""></a> //#TBD#//
    public AssetDb Load<T>(string path, out T asset) where T : Object {
      asset = Load<T>(path);
      return this;
    }

    /// <a href=""></a> //#TBD#//
    /// Warning: Changes path in project window. Get old path first with
    ///   var activeObject = Selection.activeObject;
    ///   var selectedPathInProjectView = AssetDatabase.GetAssetPath(Selection.activeObject);
    /// afterwards you can return with
    ///   EditorGUIUtility.PingObject(activeObject);
    /// but it is not perfect. Left project pane returns correctly, but right pane still points elsewhere
    public static Object LoadOrCreate(string path, Type type) {
      var asset = Load(path, type);
      if (asset != null) return asset;
      AssetDatabase.CreateAsset(asset = ScriptableObject.CreateInstance(type), path);
      AssetDatabase.SaveAssets();
      AssetDatabase.Refresh();
      return asset;
    }

    public AssetDb LoadOrCreate(string path, out Object asset, Type type) {
      asset = LoadOrCreate(path, type);
      return this;
    }

    /// <a href=""></a> //#TBD#//
    public static T LoadOrCreate<T>(string path) where T : ScriptableObject => (T) LoadOrCreate(path, typeof(T));

    /// <a href=""></a> //#TBD#//
    public AssetDb LoadOrCreate<T>(string path, out T asset) where T : ScriptableObject {
      asset = LoadOrCreate<T>(path);
      return this;
    }
    #endif
    #endregion
  }
}