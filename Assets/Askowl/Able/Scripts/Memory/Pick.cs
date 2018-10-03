// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

namespace Askowl {
  /// <a href="http://bit.ly/2Opnmk1">Interface so that code can use a picker without know more about the source. A picker returns a value using source specific rules.</a>
  public interface Pick<out T> {
    /// <a href="http://bit.ly/2Opnmk1">Method to call to return the selection</a>
    T Pick();
  }
}