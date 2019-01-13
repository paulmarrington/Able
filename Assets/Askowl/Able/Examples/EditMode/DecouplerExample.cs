#if UNITY_EDITOR && AskowlDecoupler
using Decoupled;
using NUnit.Framework;

public sealed class TestDecoupler {
  [Test] public void DefaultServiceTest() {
    FirstDecouplerInterface.Reset();

    var firstDecoupler = FirstDecouplerInterface.Instance;

    firstDecoupler.Entry1(number: 12);
    Assert.AreEqual(expected: firstDecoupler.Entry2(), actual: 12);
  }

  [Test] public void LoadImplementedServiceTest() {
    FirstDecouplerInterface.Reset();

    FirstDecouplerInterface.Register<FirstDecouplerService>();

    var firstDecoupler = FirstDecouplerInterface.Instance;

    firstDecoupler.Entry1(number: 12);
    Assert.AreEqual(expected: 24, actual: firstDecoupler.Entry2());
  }
}
#endif