// Copyright 2019 (C) paul@marrington.net http://www.askowl.net/unity-packages

using System;
using System.Reflection;

namespace Askowl {
  /// <a href=""></a> //#TBD#//
  public static class MethodCache {
    private static readonly Map cache = Map.Instance;

    /// <a href=""></a> //#TBD#//
    public static object Call(object instance, string method, object[] parameters) {
      var type = instance.GetType();
      if (!cache[type].Found) cache.Add(type, Map.Instance);
      var methods = cache.Value as Map;
      // ReSharper disable once PossibleNullReferenceException
      if (!methods[method].Found) {
        Type[] parameterTypes = new Type[parameters.Length];
        for (int i = 0; i < parameters.Length; i++) {
          parameterTypes[i] = parameters[i].GetType();
        }
        methods.Add(method, type.GetMethod(method, parameterTypes));
      }
      MethodInfo methodInfo = methods[method].Value as MethodInfo;
      return methodInfo?.Invoke(instance, parameters);
    }
  }
}