// Copyright 2019 (C) paul@marrington.net http://www.askowl.net/unity-packages

using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Askowl {
  public class AssetDb : IDisposable {
    #if UNITY_EDITOR
    public static AssetDb Instance => Cache<AssetDb>.Instance;
    public        bool    Error;

    #region Find
    public AssetDb Find(string path, out Object asset) {
      path  = AbsoluteFolder(path);
      asset = AssetDatabase.LoadAssetAtPath(path, typeof(Object));
      Error = asset == null;
      if (!Error) currentFolder = Path.GetDirectoryName(path);
      return this;
    }

    public static string[] FindByFilter(string filter) {
      string[] list = AssetDatabase.FindAssets(filter);

      for (int i = 0; i < list.Length; i++) list[i] = (AssetDatabase.GUIDToAssetPath(list[i]));
      return list;
    }

    public static string[] FindByType<T>() where T : Object => FindByFilter($"t:{typeof(T)}");
    public static string[] FindByLabel(string label)        => FindByFilter($"l:{label}");
    public static string[] FindByName(string  name)         => FindByFilter($"name");
    #endregion

    public void Dispose() {
      currentFolder = "";
      Error         = false;
      Cache<AssetDb>.Dispose(this);
    }

    #region Selection
    private string currentFolder = "";

    public AssetDb CurrentFolder(string folder) {
      currentFolder = AbsoluteFolder(folder);
      return this;
    }

    public string CurrentFolder() => currentFolder;

    public static string ProjectFolder() {
      EditorUtility.FocusProjectWindow();
      foreach (Object asset in Selection.GetFiltered(typeof(Object), SelectionMode.Assets)) {
        var path = SetFolderFor(asset);
        if (path != null) return path;
      }
      return "Assets";
    }

    public AssetDb ProjectFolder(out string path) {
      path = ProjectFolder();
      return this;
    }
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
    public AssetDb Select(Object asset) {
      if (Error) return this;
      EditorUtility.FocusProjectWindow();
      currentFolder = SetFolderFor(Selection.activeObject = asset);
      EditorGUIUtility.PingObject(asset);
      return this;
    }
    public AssetDb Selected(out Object asset) {
      asset = Selection.activeObject;
      return this;
    }
    public AssetDb Select(string path = null) => Find(path ?? currentFolder, out var asset).Select(asset);
    #endregion

    #region Folders
    public AssetDb CreateFolders(string path) {
      if (Error) return this;
      path          = AbsoluteFolder(path);
      currentFolder = "";

      var folders = path.Split('/');
      for (int i = 0; i < folders.Length; i++) {
        if ((folders[i].Length != 0) && (folders[i] != ".")) {
          Find($"./{folders[i]}", out Object _);
          if (Error) {
            Error = AssetDatabase.CreateFolder(currentFolder, folders[i]) == null;
            if (Error) return this;
          }
          currentFolder = currentFolder.Length == 0 ? folders[i] : $"{currentFolder}/{folders[i]}";
        }
      }
      return this;
    }
    public AssetDb Delete(string path) {
      if (Error) return this;
      Error = !AssetDatabase.MoveAssetToTrash(AbsoluteFolder(path));
      return this;
    }
    public AssetDb SubFolders(out string[] subFolders) {
      subFolders = AssetDatabase.GetSubFolders(currentFolder);
      return this;
    }
    public AssetDb SubFolders(string parent, out string[] subFolders) {
      subFolders = AssetDatabase.GetSubFolders(parent);
      return this;
    }

    private string AbsoluteFolder(string path) {
      if (path.StartsWith("./")) {
        path = path.Substring(2);
        return (currentFolder == "") ? path : $"{currentFolder}/{path}";
      }
      return (path == "Assets") || path.StartsWith("Assets/") ? path : $"Assets/{path}";
    }
    #endregion

    #region Labels
    public static void     Labels(Object asset, params string[] labels) => AssetDatabase.SetLabels(asset, labels);
    public static string[] Labels(Object asset) => AssetDatabase.GetLabels(asset);
    #endregion

    #region Asset Creation and Loading (Editor only)
    public static Object Load(string path, Type type) {
      if (!path.Contains(".")) path += ".asset";
      path = Objects.FindFile(path);
      return (path == null) ? null : AssetDatabase.LoadAssetAtPath(path, type);
    }

    public static T Load<T>(string path) where T : Object => (T) Load(path, typeof(T));

    public AssetDb Load(string path, out Object asset, Type type) {
      asset = Load(path, type);
      Error = asset == null;
      return this;
    }

    public AssetDb Load<T>(string path, out T asset) where T : Object {
      asset = Load<T>(path);
      return this;
    }

    /// Warning: Changes path in project window. Get old path first with
    ///   var activeObject = Selection.activeObject;
    ///   var selectedPathInProjectView = AssetDatabase.GetAssetPath(Selection.activeObject);
    /// afterwards you can return with
    ///   EditorGUIUtility.PingObject(activeObject);
    /// but it is not perfect. Left project pane returns correctly, but right pane still points elsewhere
    public static Object LoadOrCreate(string path, Type type) {
      var asset = Load(path, type);
      if (asset != null) return asset;
      AssetDatabase.CreateAsset(asset = ScriptableObject.CreateInstance(type), $"Assets/{path}");
      AssetDatabase.SaveAssets();
      AssetDatabase.Refresh();
      return asset;
    }

    public AssetDb LoadOrCreate(string path, out Object asset, Type type) {
      asset = LoadOrCreate(path, type);
      return this;
    }

    public static T LoadOrCreate<T>(string path) where T : ScriptableObject => (T) LoadOrCreate(path, typeof(T));

    public AssetDb LoadOrCreate<T>(string path, out T asset) where T : ScriptableObject {
      asset = LoadOrCreate<T>(path);
      return this;
    }
    #endregion

    #endif
  }
}