/*
 * Parts as provided by https://answers.unity.com/users/993732/empire-games958.html
 * on https://answers.unity.com/questions/1225189/how-can-i-change-scripting-define-symbols-before-a.html
 * and modified for generalisation. Usage:
 *
 * using UnityEditor;
 *
  [InitializeOnLoad]
  public class MyDefinitions : DefineSymbols {
    static MyDefinitions() {
      AddOrRemoveDefines(HasFolder("Appodeal"), "Adze;AdzeAppodeal");
    }
  }
 */

namespace Askowl {
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Text.RegularExpressions;
  using UnityEditor;
  using UnityEngine;

  /// <a href="http://bit.ly/2RhucXi">Used by Unity editor code to add or remove definitions based in rules such as the existence of directories</a> <inheritdoc />
  public class DefineSymbols : Editor {
    /// <a href="http://bit.ly/2OrbCgM">Add preprocessor definitions</a>
    public static void AddDefines(string defines) {
      var allDefines = Split();
      allDefines.AddRange(collection: defines.Split(';').Except(second: allDefines));
      Save(defines: allDefines);
    }

    /// <a href="http://bit.ly/2OrbCgM">Remove preprocessor definitions</a>
    public static void RemoveDefines(string defines) =>
      Save(defines: Split().Except(second: defines.Split(';')).ToList());

    /// <a href="http://bit.ly/2OrbCgM">Add or remove preprocessor definitions base on a boolean</a>
    public static void AddOrRemoveDefines(bool addDefines, string named) {
      if (addDefines) { AddDefines(defines: named); } else { RemoveDefines(defines: named); }
    }

    /// <a href="http://bit.ly/2Rk1o06">See if there is a folder under `Assets`</a>
    public static bool HasFolder(string folder) =>
      !string.IsNullOrEmpty(folder) && AssetDatabase.IsValidFolder(path: "Assets/" + folder);

    /// <a href="http://bit.ly/2Rk1o06">Check Packages/manifest.json for package installation</a>
    public static bool HasPackage(string packageName) {
      if (json == null) json = Json.Instance.Parse(File.ReadAllText("Packages/manifest.json"));
      return !string.IsNullOrEmpty(packageName) && !json.Node.To($"dependencies.{packageName}").Failed;
    }

    private static Json json;

    /// <a href="http://bit.ly/2RinksQ">See if the current build target is one listed</a>
    protected static bool Target(params BuildTarget[] targets) =>
      targets.ToList().Contains(item: EditorUserBuildSettings.activeBuildTarget);

    protected const BuildTarget
      // ReSharper disable MissingXmlDoc
      // ReSharper disable InconsistentNaming
      OSX            = BuildTarget.StandaloneOSX,
      Windows        = BuildTarget.StandaloneWindows,
      iOS            = BuildTarget.iOS,
      Android        = BuildTarget.Android,
      Linux          = BuildTarget.StandaloneLinux,
      Windows64      = BuildTarget.StandaloneWindows64,
      WebGL          = BuildTarget.WebGL,
      WSAPlayer      = BuildTarget.WSAPlayer,
      Linux64        = BuildTarget.StandaloneLinux64,
      LinuxUniversal = BuildTarget.StandaloneLinuxUniversal,
      PS4            = BuildTarget.PS4,
      XboxOne        = BuildTarget.XboxOne,
      tvOS           = BuildTarget.tvOS,
      Switch         = BuildTarget.Switch;
    // ReSharper restore InconsistentNaming
    // ReSharper restore MissingXmlDoc

    private static List<string> Split() =>
      PlayerSettings.GetScriptingDefineSymbolsForGroup(
                       targetGroup: EditorUserBuildSettings
                        .selectedBuildTargetGroup)
                    .Split(';').ToList();

    private static void Save(List<string> defines) {
      PlayerSettings.SetScriptingDefineSymbolsForGroup(
        targetGroup: EditorUserBuildSettings.selectedBuildTargetGroup,
        defines: string.Join(separator: ";", value: defines.ToArray()));
    }
  }

  /// <inheritdoc />
  [InitializeOnLoad] public sealed class SetProjectName : DefineSymbols {
    static SetProjectName() {
      var    dataPath    = Application.dataPath.Split('/');
      string projectName = dataPath[dataPath.Length - 2];
      AddDefines(Regex.Replace(projectName,                "[^A-Za-z0-9_]", ""));
      AddDefines(Regex.Replace(PlayerSettings.productName, "[^A-Za-z0-9_]", ""));
    }
  }
}