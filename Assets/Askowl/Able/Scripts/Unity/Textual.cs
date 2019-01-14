// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

namespace Decoupled {
  using UnityEngine.UI;

  /// <a href="http://bit.ly/2PLnhaQ">Change text component properties - Unity or TextMeshPro</a>
  public interface TextualInterface {
    // ReSharper disable once InconsistentNaming
    /// <a href="http://bit.ly/2PLnhaQ">Get or set the text to display</a>
    string text { get; set; }
  }

  /// <a href="http://bit.ly/2PLnhaQ">Decoupled text component concrete implementation</a> <inheritdoc cref="ComponentDecoupler{T}" />
  public partial class Textual : ComponentDecoupler<Textual>, TextualInterface {
    private TextualInterface Backer => Instance as TextualInterface;

    /// <inheritdoc />
    public string text { get => Backer.text; set => Backer.text = value; }
  }

  public partial class Textual {
    private class UnityTextInterface : ComponentInterface, TextualInterface {
      private Text UnityText => Component as Text;

      public string text { get => UnityText.text; set => UnityText.text = value; }

      public UnityTextInterface() => Instantiate<Text>(primary: false);
    }

    // ReSharper disable once UnusedMember.Local
    private UnityTextInterface unityTextInterface = new UnityTextInterface();
  }
}