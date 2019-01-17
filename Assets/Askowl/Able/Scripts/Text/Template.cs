// Copyright 2019 (C) paul@marrington.net http://www.askowl.net/unity-packages

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Askowl {
  /// <a href=""></a> //#TBD#//
  public class Template : IDisposable {
    /// <a href=""></a> //#TBD#//
    public static Template Instance => Cache<Template>.Instance;

    /// <a href=""></a> //#TBD#//
    public struct Substitution {
      /// <a href=""></a> //#TBD#//
      public Regex Regex;
      /// <a href=""></a> //#TBD#//
      public MatchEvaluator With;
    }

    /// <a href=""></a> //#TBD#//
    public List<Substitution> substitutions = new List<Substitution>();

    /// <a href=""></a> //#TBD#//
    public Template Substitute(string regex, object with) => Substitute(regex, _ => with.ToString());

    /// <a href=""></a> //#TBD#//
    public Template Substitute(string regex, MatchEvaluator with) {
      substitutions.Add(new Substitution {Regex = new Regex(regex, RegexOptions.Multiline), With = with});
      return this;
    }

    /// <a href=""></a> //#TBD#//
    public string Process(string text) {
      for (int i = 0; i < substitutions.Count; i++) {
        text = substitutions[i].Regex.Replace(text, substitutions[i].With);
      }
      return text;
    }

    /// <inheritdoc />
    public void Dispose() {
      substitutions.Clear();
      Cache<Template>.Dispose(this);
    }
  }
}