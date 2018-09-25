﻿// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

// ReSharper disable HeuristicUnreachableCode

#if UNITY_EDITOR && Able
namespace Askowl.Examples {
  using System.Linq;
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

    private static readonly BuildTargetGroup buildTargetGroup =
      EditorUserBuildSettings.selectedBuildTargetGroup;

    bool checkDefines(string named) {
      string definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup: buildTargetGroup);

      var allDefines = definesString.Split(';').ToList();
      return allDefines.Contains(item: named);
    }
  }

  /// <a href="DefineSymbols"></a>
  /// <inheritdoc />
  [InitializeOnLoad]
  public sealed class MyDefinitions : DefineSymbols {
    static MyDefinitions() {
      bool update = HasFolder(folder: "Askowl/Able") && Target(iOS, Android, OSX, Windows);

      // add more than one by separating with ;
      AddOrRemoveDefines(addDefines: update, named: "Askowl");

      update = HasFolder(folder: "I-do-not-exist");
      AddOrRemoveDefines(addDefines: update, named: "ThisWontHappen");
    }
  }
}
#endif