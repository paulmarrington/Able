#if UNITY_EDITOR && AskowlDecoupler
namespace Decoupled {
  public class FirstDecouplerInterface : Service<FirstDecouplerInterface> {
    protected int Number;

    internal virtual void Entry1(int number) { Number = number; }

    internal int Entry2() => Number;
  }
}
#endif