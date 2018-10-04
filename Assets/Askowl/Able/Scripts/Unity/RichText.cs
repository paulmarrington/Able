// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

namespace Askowl.RichText {
  /// <a href="http://bit.ly/2OpSm3i">Color class with American spelling</a><inheritdoc />
  public class Color : Colour { }

  /// <a href="http://bit.ly/2OpSm3i">Static helper for text colour</a>
  public class Colour {
    /// <a href="http://bit.ly/2OpSm3i">List of recognised colours. Use #rrggbb as an alternative</a>
    public static string
      Aqua      = "aqua",      Black  = " black", Blue     = "blue",
      Brown     = "brown",     Cyan   = "cyan",   DarkBlue = "darkblue",
      Fuchsia   = "fuchsia",   Green  = "green",  Grey     = "grey",
      LightBlue = "lightblue", Lime   = "lime",   Magenta  = "magenta",
      Maroon    = "maroon",    Navy   = "navy",   Olive    = "olive",
      Orange    = "orange",    Purple = "purple", Red      = "red",
      Silver    = "silver",    Teal   = "teal",   White    = "white",
      Yellow    = "yellow";

    /// <a href=""></a>
    public static string Tag(string colour, string text) => $"<color={colour}>{text}</color>";
  }
}