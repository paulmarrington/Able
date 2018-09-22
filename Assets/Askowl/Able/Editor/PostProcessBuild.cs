using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.Callbacks;
using UnityEngine;
#if UNITY_IOS || UNITY_TVOS
using UnityEditor.iOS.Xcode;
#endif

// Default plist includes NSCalendarsUsageDescription = Not Required

/// <summary>
/// Do any modifications required by the different platforms to build without fiddling
/// </summary>
[CreateAssetMenu(menuName = "Build/PostProcessor", fileName = "PostProcessBuild")]
public sealed class PostProcessBuild : ScriptableObject {
  /// <summary>
  /// iOS plist entry types allowed
  /// </summary>
  [Serializable]
  public enum EntryTypes {
    // ReSharper disable MissingXmlDoc

    String,
    Boolean,
    Integer

    // ReSharper restore MissingXmlDoc
  }

  // ReSharper disable UnassignedField.Global, MemberCanBeInternal, ReSharper disable FieldCanBeMadeReadOnly.Global, InconsistentNaming
  /// <summary>
  /// iOS required data needed to work the magic
  /// </summary>
  [Serializable]
  // ReSharper disable once InconsistentNaming
  public sealed class IOS {
    /// <summary>
    /// This is what is needed to add a plist entry into an iOS build
    /// </summary>
    [Serializable]
    public sealed class PlistEntry {
      // ReSharper disable MissingXmlDoc

      public string     Key;
      public EntryTypes EntryType;
      public string     Value;

      // ReSharper restore MissingXmlDoc
    }

    /// <summary>
    /// set UNITY_USES_REMOTE_NOTIFICATIONS to 0
    /// </summary>
    public bool RemoveNotifications = true;

    /// <summary>
    /// Entries to add to the build plist file used by iOS
    /// </summary>
    public PlistEntry[] PlistEntries;
  }

  /// <summary>
  /// Android required data needed to work the magic
  /// </summary>
  [Serializable]
  public sealed class Android {
    /// <summary>
    /// Multidex allows for files with more than 64k entries in the linker name dictionary
    /// </summary>
    public bool EnableMultidex = true;
  }

  /// <summary>
  /// Instance of iOS build data so that it can be modified or added to from the Unity editor
  /// </summary>
  // ReSharper disable once InconsistentNaming
  public IOS iOS;

  /// <summary>
  /// Instance of Android build data so that it can be modified or added to from the Unity editor
  /// </summary>
  // ReSharper disable once InconsistentNaming
  public Android android;
  // ReSharper restore UnassignedField.Global, MemberCanBeInternal, FieldCanBeMadeReadOnly.Global, InconsistentNaming

  /// <summary>
  /// Called by unity during the build process after the platform specific code has been created and before the platform builder takes over
  /// </summary>
  /// <param name="buildTarget"></param>
  /// <param name="pathToBuiltProject"></param>
  [PostProcessBuild(callbackOrder: 900)]
  public static void OnPostProcessBuild(BuildTarget buildTarget,
                                        string      pathToBuiltProject) {
#if UNITY_IOS || UNITY_TVOS
    PostProcessBuild postProcessBuild = Load();
    string           plistPath        = pathToBuiltProject + "/Info.plist";
    PlistDocument    plist            = new PlistDocument();
    plist.ReadFromString(text: File.ReadAllText(path: plistPath));

    if (postProcessBuild.iOS.RemoveNotifications) {
      MissingPushNotificationEntitlementRemover(pathToBuiltProject: pathToBuiltProject);
    }

    AddPlistEntries(plist: plist, plistEntries: postProcessBuild.iOS.PlistEntries);

    File.WriteAllText(path: plistPath, contents: plist.WriteToString());
#endif
  }

  private static void
    MissingPushNotificationEntitlementRemover([NotNull] string pathToBuiltProject) {
    string classesDirectory = Path.Combine(path1: pathToBuiltProject, path2: "Classes");
    string headerPath       = Path.Combine(path1: classesDirectory,   path2: "Preprocessor.h");

    if (File.Exists(path: headerPath)) {
      string code = File.ReadAllText(path: headerPath);

      code = Regex.Replace(
        input: code,
        pattern: "define UNITY_USES_REMOTE_NOTIFICATIONS 1",
        replacement: "define UNITY_USES_REMOTE_NOTIFICATIONS 0");

      File.WriteAllText(path: headerPath, contents: code);
    } else {
      Debug.LogError(message: "Preprocessor file doesn't exist.");
    }
  }

#if UNITY_IOS || UNITY_TVOS
  private static void AddPlistEntries(PlistDocument                         plist,
                                      [NotNull] IEnumerable<IOS.PlistEntry> plistEntries) {
    foreach (IOS.PlistEntry plistEntry in plistEntries) {
      switch (plistEntry.EntryType) {
        case EntryTypes.String:
          plist.root.SetString(key: plistEntry.Key, val: plistEntry.Value);
          break;
        case EntryTypes.Boolean:

          plist.root.SetBoolean(
            key: plistEntry.Key,
            val: Convert.ToBoolean(value: plistEntry.Value));

          break;
        case EntryTypes.Integer:

          plist.root.SetInteger(
            key: plistEntry.Key,
            val: Convert.ToInt32(value: plistEntry.Value));

          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
    }
  }
#endif

  private static PostProcessBuild Load() {
    PostProcessBuild instance      = Resources.Load<PostProcessBuild>("PostProcessBuild");
    if (instance == null) instance = Resources.Load<PostProcessBuild>("PostProcessBuildDefault");
    return instance;
  }
}

internal sealed class MyCustomBuildProcessor : IPreprocessBuildWithReport {
  public int  callbackOrder                         { get { return 0; } }
  public void OnPreprocessBuild(BuildReport report) { }

  public void OnPreprocessBuild(BuildTarget target, string targetPath) {
#if UNITY_ANDROID
    PostProcessBuild build = PostProcessBuild.Load();
    string           path = "Assets/Plugins/Android/AndroidManifest.xml";
    string           tools = "xmlns:tools=\"http://schemas.android.com/tools\"";
    string           tag = "<manifest";

    try {
      bool   changed = false;
      string manifest = File.ReadAllText(path);

      if (manifest.IndexOf(tools) == -1) {
        int at = manifest.IndexOf(tag) + tag.Length;
        manifest = manifest.Insert(at, " " + tools);
        changed = true;
      }

      if (build.android.EnableMultidex) {
        string multidex = "android:name=\"android.support.multidex.MultiDexApplication\"";

        if (manifest.IndexOf(multidex) == -1) {
          int at = manifest.IndexOf(tag) + tag.Length;
          manifest = manifest.Insert(at, " " + multidex);
          changed = true;
        }

        // check enabled project mainTemplate.gradle or remove .DISABLED suffix
        // android.defaultConfig.multiDexEnabled = true
        // dependencies.compile 'com.android.support:multidex:1.0.1'
      }

      if (changed) {
        File.WriteAllText(path, manifest);
      }
    } catch { }
#endif
  }
}