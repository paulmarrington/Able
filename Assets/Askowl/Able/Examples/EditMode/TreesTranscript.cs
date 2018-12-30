//- Trees is part of the free Able library available on the Unity store. Please support my efforts by joining my Patreon group from the link below.
//- A tree collection is effectively lists of lists. It is a popular way of representing data in a way that our minds can follow while limiting the information being reviewed at any one time. Think of directory folder structures or multi-level menu systems.
using NUnit.Framework;

// ReSharper disable MissingXmlDoc

namespace Askowl.Transcript {
  public class TreesTranscript {
    [Test] public void TreesTranscriptSimplePasses() {
      //- It is a policy in able to use a static Instance method to create new collections. Both trees and nodes are cached to reduce excessive garbage collection usage.
      var tree = Trees.Instance;

      //- A tree has state. As you build up or walk through a tree your location within the structure is kept.
      tree.Add("One off root");

      //- Use IsRoot to check for and Root() to return to the root
      Assert.IsFalse(tree.IsRoot);
      tree.Root();
      Assert.IsTrue(tree.IsRoot);

      //- Trees is designed to easily chain commands and paths for addition or walking can be built up with dots between levels.
      tree = Trees.Instance.Add("Australia.Queensland.Brisbane.The Gap");

      //- When walking a tree, Failed will see if we are lost
      Assert.IsFalse(tree.To("Australia.Brisbane").Failed);

      //- To always starts from the root. Use Next to continue a walk
      tree.To("Australia.Queensland").Next("Brisbane.The Gap");

      //- You can retrieve the name of the current location
      Assert.AreEqual("The Gap", tree.Name);

      //- The path can include movement backwards with empty locations (..)
      Assert.IsFalse(tree.To("Australia.Queensland.Brisbane...Queensland").Failed);
      tree.To("Australia.Queensland.Brisbane.The Gap");
      Assert.IsFalse(tree.Next(".The Gap").Failed);

      //- Just as Name names the current node, Key provides the full path from the root
      Assert.AreEqual("Australia.Queensland.Brisbane.The Gap", tree.Key);

      //- Unlike many tree implementation, every node can have a leaf
      tree.To("Australia.Queensland").Leaf = "The Sunshine State";

      //- Leaf can be any object reference, while Value is always a string. Note we are using boxing here
      tree.Leaf = 23;
      Assert.AreEqual(23,   tree.Leaf);
      Assert.AreEqual("23", tree.Value);

      //- Because tree leaves can be of any type, we provide checks and conversions
      Assert.IsTrue(tree.IsNumber);
      Assert.AreEqual(23,   tree.Long);
      Assert.AreEqual(23.0, tree.Double);
      Assert.IsFalse(tree.IsNull);

      //- Use array access where it is more convenient
      Assert.AreEqual(tree.To("Australia.Queensland").Leaf, tree.To("Australia")["Brisbane"]);

      //- Use Dispose to remove everything from and including the current node'
      tree.To("Australia.Queensland").Dispose();

      //- When traversing the contents of a tree it is useful to save amd restore locations
      tree = Trees.Instance.Add("a.b.c.d");
      tree.Add("a.b.c2.d2");
      using (tree.Anchor("a.b")) {
        tree.Next("c.d");
      }
      Assert.AreEqual("a.b.c2.d2", tree.Key);

      //- and, of course, we need to be able to traverse a tree
      object[] cs = tree.To("a.b").Children; // will be c and c2

      //- and to walk the structure. Use anchor to walk more than one level
      var c = "";

      for (var name = tree.Root().FirstChild; name != null; name = tree.NextChild) c += name;

      Assert.AreEqual("cc2", c);
    }
  }
}