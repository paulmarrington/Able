using System;

namespace Askowl {
  public struct ByReference<T> {
    public Func<T>   getter;
    public Action<T> setter;

    public T Value { get { return getter(); } set { setter(value); } }
  }
}