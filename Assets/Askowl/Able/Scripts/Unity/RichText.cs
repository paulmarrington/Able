namespace Askowl.RichText {
  /// <a href=""></a>
  /// <inheritdoc />
  public class Color : Colour { }

  /// <a href=""></a>
  public class Colour {
    /// <a href=""></a>
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