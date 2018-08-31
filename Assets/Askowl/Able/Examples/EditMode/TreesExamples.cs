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
      var tree = Trees.Instance.To("OneOffRoot");
      Assert.IsFalse(tree.Has("OneOffRoot"));

      Assert.IsTrue(tree.Root().Has("OneOffRoot"));
    }

    /// Using <see cref="Trees.IsRoot"/>
    [Test]
    public void IsRoot() {
      var tree = Trees.Instance;

      Assert.IsTrue(tree.IsRoot);

      tree.To("A.B.C");

      Assert.IsFalse(tree.IsRoot);
    }

    /// Using <see cref="Trees.Parent"/>
    [Test]
    public void Parent() {
      var tree = Trees.Instance.To("OneOffRoot");
      Assert.IsFalse(tree.IsRoot);
      Assert.IsTrue(tree.Parent().IsRoot);
    }

    /// Using <see cref="Trees.To"/>
    [Test]
    public void To() {
      var tree = Trees.Instance.To("A.B.C");

      tree.To("A", "B");
      Assert.IsTrue(tree.Has("C"));

      tree.To("A.B");
      Assert.IsTrue(tree.Has("C"));

      tree.To("A").To(".B");
      Assert.IsTrue(tree.Has("C"));
    }

    /// Using <see cref="Trees.Next"/>
    [Test]
    public void Next() {
      var tree = Trees.Instance.To("A.B.C");

      tree.To("A").Next("B");
      Assert.IsTrue(tree.Has("C"));

      tree.To("A").To(".B");
      Assert.IsTrue(tree.Has("C"));
    }

    /// Using <see cref="Trees.Has"/>
    [Test]
    public void Has() {
      var tree = Trees.Instance.To("IamHere");

      Assert.IsTrue(tree.Root().Has("IamHere"));
      Assert.IsFalse(tree.To("IamHere").Has("IamHere"));
    }

    /// Using <see cref="Trees.Name"/>
    [Test]
    public void Name() {
      var tree = Trees.Instance.To("IamHere");

      Assert.AreEqual("IamHere", tree.Name);

      Assert.AreEqual("~ROOT~", tree.Root().Name);
    }

    /// Using <see cref="Trees.IsA{T}"/>
    [Test]
    public void IsA() {
      var tree = Trees.Instance.To("IamHere");
      tree.Leaf = "a string value";

      Assert.IsTrue(tree.IsA<string>());
    }

    /// Using <see cref="Trees.Leaf"/>
    [Test]
    public void Leaf() {
      var tree = Trees.Instance.To("IamHere");
      Assert.IsNull(tree.Leaf);
      tree.Leaf = "a string value";
      Assert.IsNotNull(tree.Leaf);
    }

    /// Using <see cref="Trees.As{T}"/>
    [Test]
    public void As() {
      var tree = Trees.Instance.To("IamHere");
      tree.Leaf = 222;

      Assert.AreEqual(222, tree.As<int>());
    }

    /// Using <see cref="Trees.Dispose"/>
    [Test]
    public void Dispose() {
      var tree = Trees.Instance.To("IamHere");
      tree.Dispose();
      Assert.IsTrue(tree.IsRoot);
      Assert.IsFalse(tree.Has("IamHere"));
    }

    /// Using <see cref="Trees.DisposeHere"/>
    [Test]
    public void DisposeHere() {
      var tree = Trees.Instance.To("A.B.C");
      tree.To("A.B");
      tree.DisposeHere();
      Assert.AreEqual("A", tree.Name);
      Assert.IsFalse(tree.Has("B"));
    }

    /// Using <see cref="Trees.Anchor"/>
    [Test]
    public void Anchor() {
      var tree = Trees.Instance.To("A.B.C");

      using (tree.Anchor()) {
        tree.To("A");
      }

      Assert.AreEqual("C", tree.Name);
    }

    /// Using <see cref="Trees.Count"/>
    [Test]
    public void Count() {
      var tree = Trees.Instance.To("A.B.4").Parent(2);
      Assert.AreEqual(5, tree.Root().To("A.B").Count);
    }

    /// Using <see cref="Trees.ToString"/>
    [Test]
    public new void ToString() {
      var tree = Trees.Instance.To("A", "B", "C");
      Assert.AreEqual("A.B.C", tree.ToString());
    }
  }
}