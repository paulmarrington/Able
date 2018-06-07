#if UNITY_EDITOR && AskowlBase
using System.Collections;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Askowl.Samples {
  /// <inheritdoc />
  /// <summary>
  /// Unity Test Runner PlayMode tests for Askowl-Base classes
  /// </summary>
  public class CustomAssetTests : PlayModeTests {
    private IEnumerator Setup() { yield return LoadScene("Askowl-Base-Examples"); }

    /// <summary>
    ///
    /// </summary>
    [UnityTest, Timeout(10000)]
    public IEnumerator Test() { yield return Setup(); }
  }
}
#endif