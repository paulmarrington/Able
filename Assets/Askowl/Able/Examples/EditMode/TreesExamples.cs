// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

using NUnit.Framework;

namespace Askowl.Examples {
  /// Using <see cref="Trees"/>
  public class TreesExamples {
    /// Using <see cref="Trees"/>
    [Test]
    public void Instance() {
      var tree = Trees.Instance;
      Assert.IsNotNull(tree);
    }

    /// Using <see cref="Trees.Root"/>
    [Test]
    public void Root() {
      var tree = Trees.Instance.Add("OneOffRoot");
      Assert.IsNull(tree.Next("OneOffRoot"));

      // Would fail if we were not back at root
      Assert.IsNotNull(tree.Root().Next("OneOffRoot"));

      // to starts at root unless starts with '.'
      Assert.IsNotNull(tree.To("OneOffRoot"));
    }

    /// Using <see cref="Trees.IsRoot"/>
    [Test]
    public void IsRoot() {
      var tree = Trees.Instance;

      Assert.IsTrue(tree.IsRoot);

      tree.Add("A.B.C");

      Assert.IsFalse(tree.IsRoot);
    }

    /// Using <see cref="Trees.Failed"/>
    [Test]
    public void Failed() {
      var tree = Trees.Instance.Add("A.B.C");
      Assert.IsNull(tree.To("A.C"));
      Assert.IsTrue(tree.Failed);
      Assert.IsNotNull(tree.To("A.B"));
      // Failed is always reset when we search from root
      Assert.IsFalse(tree.Failed);
    }

    /// Using <see cref="Trees.Parent"/>
    [Test]
    public void Parent() {
      // "A" is one branch off root
      var tree = Trees.Instance.Add("A.B");
      Assert.IsFalse(tree.IsRoot);
      Assert.IsTrue(tree.Parent().Parent().IsRoot);
      // The root node has a generated name
      Assert.AreEqual("~ROOT~", tree.Name);
    }

    /// Using <see cref="Trees.To"/>
    [Test]
    public void To() {
      var tree = Trees.Instance.Add("A.B.C");
      // `To` can take separate keys - allowing for non-string
      Assert.IsNotNull(tree.To("A", "B", "C"));
      // `To` can split on `.`. This example starts from root
      Assert.AreEqual("C", tree.To("A.B.C").Name);
      // A -> B -> C -> <- B <- A -> B: Look at spaces between dots
      Assert.AreEqual("B", tree.To("A.B.C...B")?.Name);
    }

    /// Using <see cref="Trees.Next"/>
    [Test]
    public void Next() {
      var tree = Trees.Instance.Add("A.B.C");
      // Next is relative to the current tree location
      tree.To("A").Next("B");
      Assert.AreEqual("B", tree.Name);
    }

    /// Using <see cref="Trees.Add"/>
    [Test]
    public void Add() {
      var tree = Trees.Instance.Add("A.B");
      tree.Add("D");
      // `Add` is always relative to the current location
      Assert.AreEqual("A.B.D", tree.ToString());
      // Unless we have an explicit move to root
      Assert.AreEqual("A.B.C", tree.Root().Add("A.B.C").ToString());
    }

    /// Using <see cref="Trees.Name"/>
    [Test]
    public void Name() {
      var tree = Trees.Instance.Add("IamHere");
      // Tree name is the name of the current node
      Assert.AreEqual("IamHere", tree.Name);
      // Root has a special name
      Assert.AreEqual("~ROOT~", tree.Root().Name);
    }

    /// Using <see cref="Trees.IsA{T}"/>
    [Test]
    public void IsA() {
      var tree = Trees.Instance.Add("IamHere");
      tree.Leaf = "a string value";
      // See if the leaf is of an expected type
      Assert.IsTrue(tree.IsA<string>());
    }

    /// Using <see cref="Trees.Leaf"/>
    [Test]
    public void Leaf() {
      var tree = Trees.Instance.Add("IamHere");
      Assert.IsNull(tree.Leaf);
      tree.Leaf = "a string value";
      Assert.IsNotNull(tree.Leaf);
    }

    /// Using <see cref="Trees.As{T}"/>
    [Test]
    public void As() {
      var tree = Trees.Instance.Add("IamHere");
      tree.Leaf = 222;
      // Retrieve a leaf as a type, returning default(T) on failure
      Assert.AreEqual(222, tree.As<int>());
      // Sets Failure flag
      Assert.IsFalse(tree.Failed);
    }

    /// Using <see cref="Trees.Dispose"/>
    [Test]
    public void Dispose() {
      var tree = Trees.Instance.Add("IamHere");
      // Drop everything in the tree
      tree.Dispose();
      Assert.IsTrue(tree.IsRoot);
      Assert.IsNull(tree.To("IamHere"));
    }

    /// Using <see cref="Trees.DisposeHere"/>
    [Test]
    public void DisposeHere() {
      var tree = Trees.Instance.Add("A.B.C");
      tree.To("A.B");
      // drop everything form and including the current Node
      tree.DisposeHere();
      Assert.AreEqual("A", tree.Name);
      Assert.IsNull(tree.To("B"));
      Assert.IsTrue(tree.Failed);
    }

    /// Using <see cref="Trees.Anchor"/>
    [Test]
    public void Anchor() {
      var tree = Trees.Instance.Add("A.B.C");

      // `using` with `Tree`
      using (tree.Anchor()) {
        tree.To("A");
      }

      // Restored to state before `using`
      Assert.AreEqual("C", tree.Name);
    }

    /// Using <see cref="Trees.Count"/>
    [Test]
    public void Count() {
      var tree = Trees.Instance.Add("A.B.4..2");
      // Number of slots in array - 0 for anything else
      Assert.AreEqual(2x, tree.To("A.B").Count);
    }

    /// Using <see cref="Trees.Children"/>
    [Test]
    public void Children() {
      var tree = Trees.Instance.Add("A.B.C1..C2..C3").To("A.B");
      // Retrieve the names of branches, leaves and arrays under the current branch
      Assert.AreEqual("C1,C2,C3", Csv.ToString(tree.Children));
    }

    /// Using <see cref="Trees.ToString"/>
    [Test]
    public new void ToString() {
      var tree = Trees.Instance.Add("A", "B", "C");
      // The string is the path from the root to the current node
      Assert.AreEqual("A.B.C", tree.ToString());
    }
  }
}