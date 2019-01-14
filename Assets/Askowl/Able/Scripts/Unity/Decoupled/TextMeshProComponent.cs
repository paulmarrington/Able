// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

#if TextMeshPro
namespace Decoupled {
  using TMPro;

  public partial class Textual {
    // ReSharper disable once UnusedMember.Local
    private class TextMeshProUguiInterface : ComponentInterface, TextualInterface {
      private TextMeshProUGUI TmpText => Component as TextMeshProUGUI;

      public string text { get => TmpText.text; set => TmpText.text = value; }

      public TextMeshProUguiInterface() => Instantiate<TextMeshProUGUI>(primary: true);
    }

    // ReSharper disable once UnusedMember.Local
    private TextMeshProUguiInterface textMeshProUguiInterface = new TextMeshProUguiInterface();
  }
}
#endif