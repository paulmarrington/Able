// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

#if AskowlTests
namespace Askowl.Able.Examples {
  using NUnit.Framework;

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
      Assert.IsTrue(tree.Next("OneOffRoot").Failed);

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
      tree.To("A.C");
      Assert.IsTrue(tree.Failed);
      Assert.IsNotNull(tree.To("A.B"));
      // Failed is always reset when we search from root
      Assert.IsFalse(tree.Failed);
    }

    /// Using <see cref="Trees.To"/>
    [Test]
    public void To() {
      var tree = Trees.Instance.Add("A.B.C");
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
      Assert.AreEqual("A.B.D", tree.Key);
      // Unless we have an explicit move to root
      Assert.AreEqual("A.B.C", tree.Root().Add("A.B.C").Key);
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

    /// Using <see cref="Trees.Leaf"/>
    [Test]
    public void Leaf() {
      var tree = Trees.Instance.Add("IamHere");
      Assert.IsNull(tree.Leaf);
      tree.Leaf = "a string value";
      Assert.IsNotNull(tree.Leaf);
    }

    /// Using <see cref="Trees.Value"/>
    [Test]
    public void Value() {
      var tree = Trees.Instance.Add("IamHere");
      Assert.IsNull(tree.Leaf);
      tree.Leaf = "a string value";
      Assert.IsNotNull(tree.Leaf);
    }

    /// Using <see cref="Trees.IsNumber"/>
    [Test]
    public void IsNumber() {
      var tree = Trees.Instance.Add("IamHere");
      tree.Leaf = "22";
      Assert.IsTrue(tree.IsNumber);
      tree.Leaf = "33.34";
      Assert.IsTrue(tree.IsNumber);
      Assert.IsFalse(tree.Failed);
      tree.Leaf = "abc";
      Assert.IsFalse(tree.IsNumber);
      Assert.IsTrue(tree.Failed);
      tree.Leaf = "44abc";
      Assert.IsFalse(tree.IsNumber);
    }

    /// Using <see cref="Trees.Long"/>
    [Test]
    public void Long() {
      var tree = Trees.Instance.Add("IamHere");
      tree.Leaf = "22";
      Assert.AreEqual(expected: 22, actual: tree.Long);
      tree.Leaf = "33.34";
      Assert.AreEqual(expected: 33, actual: tree.Long);
      tree.Leaf = "abc";
      Assert.AreEqual(expected: 0, actual: tree.Long);
      tree.Leaf = "44abc";
      Assert.AreEqual(expected: 0, actual: tree.Long);
    }

    /// Using <see cref="Trees.Double"/>
    [Test]
    public void Double() {
      var tree = Trees.Instance.Add("IamHere");
      tree.Leaf = "22";
      Assert.AreEqual(expected: 22, actual: tree.Double);
      tree.Leaf = "33.34";
      Assert.AreEqual(expected: 33.34, actual: tree.Double);
      tree.Leaf = "abc";
      Assert.AreEqual(expected: 0, actual: tree.Double);
    }

    /// Using <see cref="Trees.Boolean"/>
    [Test]
    public void Boolean() {
      var tree = Trees.Instance.Add("IamHere");
      tree.Leaf = "false";
      Assert.IsFalse(tree.Boolean);
      tree.Leaf = "true";
      Assert.IsTrue(tree.Boolean);
      tree.Leaf = "23";
      Assert.IsFalse(tree.Boolean);
    }

    /// Using <see cref="Trees.IsNull"/>
    [Test]
    public void IsNull() {
      var tree = Trees.Instance.Add("IamHere");
      Assert.IsTrue(tree.IsNull);
      tree.Leaf = "null";
      Assert.IsTrue(tree.IsNull);
      tree.Leaf = null;
      Assert.IsTrue(tree.IsNull);
      tree.Leaf = "abc";
      Assert.IsFalse(tree.IsNull);
      tree.Leaf = "23";
      Assert.IsFalse(tree.IsNull);
    }

    /// Using <see cref="Trees.this[object]"/>
    [Test]
    public void ArrayUpdate() {
      var tree = Trees.Instance.Add("A.B.C");
      // Update the current node leaf
      tree.Leaf = "Leaf Update";
      Assert.AreEqual("Leaf Update", tree.To("A.B")["C"]);

      tree["C"] = "Array Update";
      Assert.AreEqual("Array Update", tree.To("A.B.C").Leaf);
    }

    /// Using <see cref="Trees.Dispose"/>
    [Test]
    public void Dispose() {
      var tree = Trees.Instance.Add("A.B.C");
      // drop everything form and including the current Node
      tree.To("A.B")?.Dispose();
      Assert.AreEqual("A", tree.Name);
      tree.To("B");
      Assert.IsTrue(tree.Failed);
    }

    /// Using <see cref="Trees.Anchor"/>
    [Test]
    public void Anchor() {
      var tree = Trees.Instance.Add("A.B.C");

      // `using` with `Tree`
      using (tree.Anchor()) tree.To("A");

      // Restored to state before `using`
      Assert.AreEqual("C", tree.Name);
    }

    /// Using <see cref="Trees.Children"/>
    [Test]
    public void Children() {
      var tree = Trees.Instance.Add("A.B.C1..C2..C3").To("A.B");
      // Retrieve the names of branches, leaves and arrays under the current branch
      Assert.AreEqual("C1,C2,C3", Csv.ToString(tree.Children));
    }

    /// Using <see cref="Trees.FirstChild"/>
    [Test]
    public void FirstChild() {
      var tree = Trees.Instance.Add("A.B.C1..C2..C3").To("A.B");
      Assert.AreEqual("C1", tree.FirstChild);
    }

    /// Using <see cref="Trees.NextChild"/>
    [Test]
    public void NextChild() {
      var tree   = Trees.Instance.Add("A.B.C1..C2..C3").To("A.B");
      var actual = "";

      for (var key = tree.FirstChild; key != null; key = tree.NextChild) actual += key;

      Assert.AreEqual("C1C2C3", actual);
    }

    /// Using <see cref="Trees.Key"/>
    [Test]
    public void Key() {
      var tree = Trees.Instance.Add("A.B.C");
      // The string is the path from the root to the current node
      Assert.AreEqual("A.B.C", tree.Key);
    }

    /// Using <see cref="Trees.ToString"/>
    [Test]
    public new void ToString() {
      var tree = Trees.Instance.Add("A.B.C");
      tree.Leaf = 333;
      // Value of the current Leaf as a string
      Assert.AreEqual("333", tree.To("A.B.C")?.ToString());
    }
  }
}
#endif