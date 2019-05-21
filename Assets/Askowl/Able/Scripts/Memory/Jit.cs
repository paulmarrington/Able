using System;

namespace Askowl {
  /// <a href=""></a> //#TBD#//
  public struct Jit<T> {
    private T             value;
    private bool          initialised;
    private Func<bool, T> factory;
    /// <a href=""></a> //#TBD#//
    public static Jit<T> Instance(Func<bool, T> factory) => new Jit<T> {factory = factory};
    /// <a href=""></a> //#TBD#//
    public T Value => (initialised) ? value : value = factory(initialised = true);
    /// <a href=""></a> //#TBD#//
    public static implicit operator T(Jit<T> jit) => jit.Value;
    /// <a href=""></a> //#TBD#//
    public override string ToString() => Value.ToString();
  }
}