// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

#if UNITY_EDITOR && Able
using System.Linq;
using Askowl;
using NUnit.Framework;
using UnityEditor;

/// <summary>
/// Check whether compiler symbols can be defined and used
/// </summary>
public sealed class DefineSymbolsExample {
  /// <summary>
  /// Make sure that the definitions created by MyDefinitions class have been recognised
  /// </summary>
  [Test]
  public void DefineSymbolsSimplePasses() {
    #if (!Askowl)
    Assert.Fail("Expected define 'Askowl' does not exist");
        #endif

    #if ThisWontHappen
    Assert.Fail("This define should not have happened");
        #endif

    Assert.IsTrue(condition: checkDefines(named: "Askowl"));

    DefineSymbols.RemoveDefines(defines: "Askowl");
    Assert.IsFalse(condition: checkDefines(named: "Askowl"));

    DefineSymbols.AddOrRemoveDefines(addDefines: true, named: "Askowl");
    Assert.IsTrue(condition: checkDefines(named: "Askowl"));

    DefineSymbols.AddOrRemoveDefines(addDefines: false, named: "Askowl");
    Assert.IsFalse(condition: checkDefines(named: "Askowl"));
  }

  private static readonly BuildTargetGroup BuildTargetGroup =
    EditorUserBuildSettings.selectedBuildTargetGroup;

  bool checkDefines(string named) {
    var definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup: BuildTargetGroup);

    var allDefines = definesString.Split(';').ToList();
    return allDefines.Contains(item: named);
  }
}

/// <a href="DefineSymbols"></a>
/// <inheritdoc />
[InitializeOnLoad]
public sealed class MyDefinitions : DefineSymbols {
  static MyDefinitions() {
    var update = HasFolder(folder: "Askowl/Build") && Target(iOS, Android, OSX, Windows);

    // add more than one by separating with ;
    AddOrRemoveDefines(addDefines: update, named: "Askowl");

    update = HasFolder(folder: "I-do-not-exist");
    AddOrRemoveDefines(addDefines: update, named: "ThisWontHappen");
  }
}
#endif