// Copyright 2019 (C) paul@marrington.net http://www.askowl.net/unity-packages

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Askowl {
  public static class LogEntries {
    private static MethodInfo clear, getCount;

    static LogEntries() {
      Type logEntries = Type.GetType("UnityEditor.LogEntries, UnityEditor.dll");
      clear    = logEntries?.GetMethod("Clear");
      getCount = logEntries?.GetMethod("GetCount");
    }

    public static bool HasErrors() {
      Assembly assembly = Assembly.GetAssembly(typeof(SceneView));
      clear.Invoke(new object(), null);
      return (int) getCount.Invoke(new object(), null) > 0;
    }
  }
}