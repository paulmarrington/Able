// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

namespace Askowl {
  using System.Collections.Generic;

  /// <a href=""></a> //#TBD#//
  public static class Lists<T> {
    /// <a href=""></a> //#TBD#//
    public static List<T> Local {
      get {
        local.Clear();
        return local;
      }
    }
    private static List<T> local = new List<T>();
  }
}