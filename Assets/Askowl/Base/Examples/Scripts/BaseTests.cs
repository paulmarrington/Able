#if UNITY_EDITOR && AskowlBase
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.TestTools;
using UnityEngine.UI;

// ReSharper disable MissingXmlDoc

namespace Askowl {
  public class BaseTests : PlayModeTests {
    private IEnumerator Setup() { yield return LoadScene("Askowl-Base-Examples"); }

    private IEnumerator Press(string buttonName) {
      yield return Setup();

      yield return PushButton(buttonName);
    }

    [UnityTest]
    public IEnumerator FindDisabledObject() { yield return Press("FindDisabledObject"); }

    [UnityTest]
    public IEnumerator FindGameObjectsAndPath() { yield return Press("FindGameObjectsAndPath"); }

    [UnityTest]
    public IEnumerator FindComponentGlobal() { yield return Press("FindComponentGlobal"); }

    [UnityTest]
    public IEnumerator FindComponentLocal() { yield return Press("FindComponentLocal"); }

    [UnityTest]
    public IEnumerator CreateComponent() { yield return Press("CreateComponent"); }

    [UnityTest]
    public IEnumerator PickExample() { yield return Press("PickExample"); }

    [UnityTest]
    public IEnumerator RangeExample() { yield return Press("RangeExample"); }

    [UnityTest]
    public IEnumerator SelectorExample() { yield return Press("SelectorExample"); }

    [UnityTest]
    public IEnumerator PlayModeControllerExample() {
      // LoadScene is derived from PlayModeTests and asserts that the scene was loaded
      // The current scene is kept in a protected field `Scene`
      yield return LoadScene("Askowl-Base-Examples");

      // PushButton(string[] path) is derived from PlayModeTests - so asserts button exists
      // The controller version will throw an exception if the button is not found
      yield return PushButton("FindDisabledObject");

      // Next is an override unique to the PlayModeController where we already have the button
      Button button = Components.Find<Button>("FindDisabledObject");
      yield return PushButton(button);
    }

    [UnityTest]
    public IEnumerator PlayModeTestsExample() {
      // LoadScene is derived from PlayModeTests and asserts that the scene was loaded
      // The current scene is kept in a protected field `Scene`
      yield return LoadScene("Askowl-Base-Examples");

      // PushButton(string[] path) asserts button exists before pushing it
      LogAssert.Expect(LogType.Log, new Regex(".*passed.*"));
      LogAssert.Expect(LogType.Log, new Regex("Cube.*abled$"));
      yield return PushButton("Canvas/FindDisabledObject");

      // Same as Components.Find<T>(path) except that it asserts that a component was found
      Button button1 = Components.Find<Button>("FindDisabledObject");
      Button button2 = Component<Button>("Canvas/FindDisabledObject");
      Assert.AreEqual(button1, button2);

      // Same as Objects.Find<T>(name) except that it asserts that a component was found
      Button   button3 = FindObject<Button>("FindDisabledObject");
      Button[] button4 = Objects.Find<Button>("FindDisabledObject");
      Assert.AreEqual(button3, button4[0]);

      // Same as Objects.FindGameObject(name) except asserts that one was found
      GameObject gameObject1 = FindGameObject("FindDisabledObject");
      GameObject gameObject2 = Objects.FindGameObject("FindDisabledObject");
      Assert.AreEqual(gameObject1, gameObject2);

      // To check that text from the scene matches a Regex...
      Text buttonText = gameObject1.transform.GetComponentInChildren<Text>();
      CheckPattern("^Objects.Find.* cube$", buttonText.text);

      // Unity allows us to check the log
      LogAssert.Expect(LogType.Log, new Regex(".*passed.*"));
      LogAssert.Expect(LogType.Log, new Regex("Cube.*abled$"));
      yield return PushButton("FindDisabledObject");

      // and we can check that no unexpected log messages were sent
//      LogAssert.NoUnexpectedReceived();

      // Normally an error log will immediately raise an assertion
      // When we want to check for error message, use:
      LogAssert.ignoreFailingMessages = true;
      Debug.LogError("Sample error to be ignored");
      LogAssert.ignoreFailingMessages = false;

      // Unity also lets us kick off a MonoBehaviour - as long as it is self-contained
      LogAssert.Expect(LogType.Log, new Regex("Awake"));
      LogAssert.Expect(LogType.Log, new Regex("OnEnable"));
      var test = new MonoBehaviourTest<MonoBehaviourTestable>();
      yield return test;

      MonoBehaviour.Destroy(test.gameObject);
    }
  }

  public class MonoBehaviourTestable : MonoBehaviour, IMonoBehaviourTest {
    public bool IsTestFinished { get; private set; }

    private void Awake() { Debug.Log("Awake"); }

    private void OnEnable() { Debug.Log("OnEnable"); }

    public void Start() { IsTestFinished = true; }
  }
}
#endif