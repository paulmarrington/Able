// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

using JetBrains.Annotations;

namespace Askowl {
  /// <summary>
  /// Interface so that code can use a picker without know more about the source. A picker returns a value using source specific rules.
  /// </summary>
  /// <typeparam name="T">Type of result returned by the picker.</typeparam>
  /// <remarks><a href="http://customassets.marrington.net#pickt">More...</a></remarks>
  public interface Pick<out T> {
    /// <summary>
    /// Method to call to return the selection
    /// </summary>
    /// <remarks><a href="http://customassets.marrington.net#pickt">More...</a></remarks>
    T Pick();
  }
}