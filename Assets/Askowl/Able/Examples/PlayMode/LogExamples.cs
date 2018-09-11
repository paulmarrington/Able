// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

using System.Collections;
using UnityEngine.TestTools;

namespace Askowl.Examples {
  /// Using <see cref="Log" />
  public class LogExamples : PlayModeTests {
    private string SceneName = "Askowl-Able-Examples";
    /// Using <see cref="Log.Messages"/>
    [UnityTest]
    public IEnumerator Messages() { yield return LoadScene(SceneName); }

    /// Using <see cref="Log.Events"/>
    [UnityTest]
    public IEnumerator Events() { yield return LoadScene(SceneName);  }

    /// Using <see cref="Log.Warnings"/>
    [UnityTest]
    public IEnumerator Warnings() { yield return LoadScene(SceneName);  }

    /// Using <see cref="Log.Errors"/>
    [UnityTest]
    public IEnumerator Errors() { yield return LoadScene(SceneName);  }

    /// Using <see cref="Log.More"/>
    [UnityTest]
    public IEnumerator More() { yield return LoadScene(SceneName);  }

    /// Using <see cref="Log.ToMap"/>
    [UnityTest]
    public IEnumerator ToMap() { yield return LoadScene(SceneName);  }

    /// Using <see cref="Log.ConsoleEnabled"/>
    [UnityTest]
    public IEnumerator ConsoleEnabled() { yield return LoadScene(SceneName);  }

    /// Using <see cref="Log.Actions"/>
    [UnityTest]
    public IEnumerator Actions() { yield return LoadScene(SceneName);  }

    /// Using <see cref="Log.NextAction"/>
    [UnityTest]
    public IEnumerator NextAction() { yield return LoadScene(SceneName);  }
  }
}