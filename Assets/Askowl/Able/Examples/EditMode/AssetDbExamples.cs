using System;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.Internal;
using UnityEditor;
using UnityEngine;
// ReSharper disable MissingXmlDoc
#if !ExcludeAskowlTests
namespace Askowl.Able.Examples {
  public class AssetDbExamples {
    [Test] public void ProjectFolder() {
      using (var assetDb = AssetDb.Instance.ProjectFolder(out var path)) {
        Assert.IsFalse(assetDb.Error);
        Assert.IsTrue(path.StartsWith("Assets"));
      }
    }
    [Test] public void FindAbsolute() {
      using (var assetDb = AssetDb.Instance.Find("Assets/Askowl/Able", out var asset)) {
        Assert.IsFalse(assetDb.Error);
        Assert.IsNotNull(asset);
      }
    }
    [Test] public void FindRelative() {
      using (var assetDb = AssetDb.Instance.CurrentFolder("Assets/Askowl")) {
        assetDb.Find("./Able/Scripts", out var asset);
        Assert.IsFalse(assetDb.Error);
        Assert.IsNotNull(asset);
      }
    }
    [Test] public void CurrentFolder() {
      // ReSharper disable once NotAccessedVariable
      using (var assetDb = AssetDb.Instance.Find("Assets/Askowl/Able/Scripts/Unity/AssetDb.cs", out var asset)) {
        Assert.IsFalse(assetDb.Error);
        Assert.AreEqual("Assets/Askowl/Able/Scripts/Unity", assetDb.CurrentFolder());
      }
    }
    [Test] public void Error() {
      // ReSharper disable once NotAccessedVariable
      using (var assetDb = AssetDb.Instance.Find("Assets/Askowl", out var asset)) {
        assetDb.Find("Not-Able/Scripts", out asset);
        Assert.IsTrue(assetDb.Error);
      }
    }
    [Test] public void SelectFolderFromAsset() {
      using (var assetDb = AssetDb.Instance.Select("Assets/Askowl/Able/Scripts/Able.asmdef")) {
        assetDb.ProjectFolder(out var path);
        Assert.AreEqual("Assets/Askowl/Able/Scripts", path);
      }
    }
    [Test] public void SelectFolder() {
      using (var assetDb = AssetDb.Instance.Select("Assets/Askowl/Able/Scripts")) {
        assetDb.ProjectFolder(out var path);
        Assert.AreEqual("Assets/Askowl/Able/Scripts", path);
      }
    }
    [Test] public void Selected() {
      using (var assetDb = AssetDb.Instance.Select("Assets/Askowl/Able/Scripts/Able.asmdef")) {
        assetDb.Selected(out var asset);
        Assert.AreEqual("Able", asset.name);
      }
    }
    [Test] public void SelectAsset() {
      var assets        = AssetDatabase.FindAssets("Introduction to Able");
      var assetToSelect = AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GUIDToAssetPath(assets[0]));
      using (var assetDb = AssetDb.Instance.Select(assetToSelect)) {
        assetDb.Selected(out var assetSelected);
        Assert.AreEqual(assetToSelect,        assetSelected);
        Assert.AreEqual("Assets/Askowl/Able", assetDb.CurrentFolder());
      }
    }
    [Test] public void SubFolders() {
      using (var assetDb = AssetDb.Instance.Select("Assets/Askowl/Able/Scripts")) {
        assetDb.SubFolders(out var subFolders);
        Assert.GreaterOrEqual(4, subFolders.Length);
        Assert.IsTrue(subFolders.Contains("Assets/Askowl/Able/Scripts/Memory"));
      }
    }
    [Test] public void CreateFolders() {
      var path = $"Assets/Temp/{(long) Clock.EpochTimeNow}";
      using (var assetDb = AssetDb.Instance) {
        assetDb.CreateFolders(path);
        Assert.IsFalse(assetDb.Error);
        assetDb.SubFolders("Assets/Temp", out string[] subFolders);
        var j = string.Join(",", subFolders);
        Assert.IsTrue(subFolders.Contains(path));
        assetDb.Delete(path);
      }
    }
    [Test] public void Delete() {
      var guid = ((int) Clock.EpochTimeNow + 1).ToString();
      var path = $"Assets/Temp/{guid}";
      using (var assetDb = AssetDb.Instance) {
        assetDb.CreateFolders(path);
        Assert.IsFalse(assetDb.Delete(path).Error);
        assetDb.SubFolders("Assets/Temp", out string[] subFolders);
        Assert.IsFalse(subFolders.Contains(guid));
      }
    }
  }
}
#endif