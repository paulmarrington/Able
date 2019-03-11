// Copyright 2019 (C) paul@marrington.net http://www.askowl.net/unity-packages

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Askowl {
  /// <a href=""></a> //#TBD#//
  public class Template : IDisposable {
    protected string result;

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
    public Template From(string text) {
      result = text;
      return this;
    }

    /// <a href=""></a> //#TBD#//
    public Template Substitute(string regex, MatchEvaluator with) {
      substitutions.Add(new Substitution {Regex = new Regex(regex, RegexOptions.Multiline), With = with});
      return this;
    }

    /// <a href=""></a> //#TBD#//
    public Template Substitute(string regex, object with) => Substitute(regex, _ => with.ToString());

    /// <a href=""></a> //#TBD#//
    public Template And(string regex, MatchEvaluator with) => Substitute(regex, with);

    /// <a href=""></a> //#TBD#//
    public Template And(string regex, object with) => Substitute(regex, with);

    /// <a href=""></a> //#TBD#//
    public string Result() {
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
      Cache<Template>.Dispose(this);
    }

    #region Inner Template
    private class InnerTemplate : Template {
      internal         Regex         regex;
      internal         Match         match;
      private          string        template;
      private readonly StringBuilder builder = new StringBuilder();
      internal         Template      parent;

      public override void Add() {
        builder.Append(From(template).Result());
        substitutions.Clear();
        match.NextMatch();
      }

      public override bool More() {
        if (builder.Length > 0) {
          result = $"{result.Substring(0, match.Index)}{builder}{result.Substring(match.Index + match.Length)}";
          builder.Clear();
        }
        if (!match.Success || (match.Groups.Count < 2)) return false;
        template = match.Groups[1].Value;
        return true;
      }

      public override void Dispose() {
        parent.result = regex.Replace(parent.result, Result());
        base.Dispose();
      }
    }
    /// <a href=""></a> //#TBD#//
    public Template Inner(string regex) {
      var inner = Cache<InnerTemplate>.Instance;
      inner.parent = this;
      inner.regex  = new Regex(regex, RegexOptions.Multiline);
      inner.match  = inner.regex.Match(result);
      return inner;
    }
    #endregion
  }
}