// Copyright 2019 (C) paul@marrington.net http://www.askowl.net/unity-packages

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Askowl {
  /// <a href=""></a> //#TBD#//
  public class Template : IDisposable {
    /// <a href=""></a> //#TBD#//
    protected string result = "";

    /// <a href=""></a> //#TBD#//
    public static Template Instance => Cache<Template>.Instance;

    /// <a href=""></a> //#TBD#//
    public struct Substitution {
      /// <a href=""></a> //#TBD#//
      public Regex Regex;
      /// <a href=""></a> //#TBD#//
      public string With;
    }

    /// <a href=""></a> //#TBD#//
    public List<Substitution> substitutions = new List<Substitution>();

    /// <a href=""></a> //#TBD#//
    public Template From(string text) {
      result = text;
      return this;
    }

    /// <a href=""></a> //#TBD#//
    public Template Substitute(string regex, string with) =>
      Substitute(new Regex(regex, RegexOptions.Singleline), with);

    /// <a href=""></a> //#TBD#//
    public Template Substitute(Regex regex, string with) {
      substitutions.Add(new Substitution {Regex = regex, With = with});
      return this;
    }

    /// <a href=""></a> //#TBD#//
    public Template Substitute(string regex, object with) => Substitute(regex, with.ToString());

    /// <a href=""></a> //#TBD#//
    public Template Substitute(Regex regex, object with) => Substitute(regex, with.ToString());

    /// <a href=""></a> //#TBD#//
    public Template And(string regex, string with) => Substitute(regex, with);

    /// <a href=""></a> //#TBD#//
    public Template And(string regex, object with) => Substitute(regex, with);

    /// <a href=""></a> //#TBD#//
    public Template And(Regex regex, string with) => Substitute(regex, with);

    /// <a href=""></a> //#TBD#//
    public Template And(Regex regex, object with) => Substitute(regex, with);

    /// <a href=""></a> //#TBD#//
    public string Result() {
      if (string.IsNullOrWhiteSpace(result)) return "";
      for (int i = 0; i < substitutions.Count; i++) {
        result = substitutions[i].Regex.Replace(result, substitutions[i].With);
      }
      return result;
    }

    /// <a href=""></a> //#TBD#//
    public virtual void Add() { }

    /// <a href=""></a> //#TBD#//
    public virtual bool More() => false;

    /// <inheritdoc />
    public virtual void Dispose() {
      substitutions.Clear();
      result = "";
      Cache<Template>.Dispose(this);
    }

    #region Inner Template
    private class InnerTemplate : Template {
      internal         Regex         regex;
      private          string        template;
      private readonly StringBuilder builder = new StringBuilder();
      internal         Template      parent;
      internal readonly List<(int left, int right, string value)> matches =
        new List<(int left, int right, string value)>();
      internal int match;

      public override void Add() {
        builder.Append(From(template).Result());
        substitutions.Clear();
      }

      public override bool More() {
        if (match < matches.Count) {
          var left  = parent.result.Substring(0, matches[match].left);
          var right = parent.result.Substring(matches[match].right);
          parent.result = $"{left}{builder}{right}";
          builder.Clear();
        }
        if (--match < 0) return false;
        template = matches[match].value;
        return true;
      }

      public override void Dispose() {
        if (!string.IsNullOrWhiteSpace(result)) parent.result = regex.Replace(parent.result, result);
        base.Dispose();
      }
    }
    /// <a href=""></a> //#TBD#//
    public Template Inner(Regex regex) {
      var inner = Cache<InnerTemplate>.Instance;
      inner.parent = this;
      inner.regex  = regex;

      inner.matches.Clear();
      for (var match = regex.Match(result); match.Success; match = match.NextMatch())
        if (match.Groups.Count >= 2)
          inner.matches.Add((match.Index, match.Index + match.Length, match.Groups[1].Value));
      inner.match = inner.matches.Count;
      return inner;
    }
    /// <a href=""></a> //#TBD#//
    public Template Inner(string regex) => Inner(new Regex(regex, RegexOptions.Singleline));
    #endregion
  }
}