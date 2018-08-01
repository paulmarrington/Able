using System;
using UnityEngine;

namespace Askowl {
  public class Log {
    public event Action<string> Message = (message) => { Debug.Log(message); };
    public event Action<string> Warning = (message) => { Debug.LogWarning(message); };
    public event Action<string> Error   = (message) => { Debug.LogError(message); };

    static Log() {
#if !UNITY_EDITOR
    Application.SetStackTraceLogType(logType:LogType.Log,stackTraceType:StackTraceLogType.None);
#endif
    }
  }
}