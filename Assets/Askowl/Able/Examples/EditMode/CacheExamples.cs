// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

using NUnit.Framework;
using UnityEngine;

namespace Askowl.Examples {
  public class CacheExamples {
    #region Working Data
    public sealed class SealedClass {
      public string State { get; set; } = "New";
    }

    public sealed class SealedClassProcessed {
      public string State { get; set; } = "New";
    }

    private struct MyStruct {
      public string State { get; private set; }

      // A struct is useless in a cache if not initialised
      // ReSharper disable once UnusedMember.Local
      private static MyStruct CreateItem() => new MyStruct {State = "StructCreated"};
    }

    // ReSharper disable UnusedMember.Local
    private class MyClassProcessed : Cached<MyClassProcessed> {
      private new static MyClassProcessed CreateItem()     => new MyClassProcessed {State = "Created"};
      private new        void             DeactivateItem() => State = "Deactivated";
      private new        void             ReactivateItem() => State += "Reactivated";

      public string State { get; private set; }
    }
    #endregion

    #region Static Examples
    /// <a href="">Using <see cref="Cache{T}.Instance"/></a>
    [Test]
    public void InstanceStatic() {
      Cache<MyStruct>.CleanCache();

      var myStruct = Cache<MyStruct>.Instance;

      Assert.AreEqual("StructCreated", myStruct.State);
    }

    /// <a href="">Using <see cref="Cache{T}.Dispose(T)"/></a>
    [Test]
    public void DisposeStatic() {
      Cache<MyClassProcessed>.CleanCache();

      var myClass = Cache<MyClassProcessed>.Instance;

      Assert.AreEqual("Created", myClass.State);

      myClass.Dispose();

      Assert.AreEqual("Deactivated", myClass.State);
    }

    /// <a href="">Using <see cref="Cache{T}.CleanCache"/></a>
    [Test]
    public void CleanCache() {
      var unused = Cache<MyClassProcessed>.Instance;

      Cache<MyClassProcessed>.CleanCache();

      Assert.IsNull(Cache<MyClassProcessed>.Entries.First);
    }

    /// <a href="">Using <see cref="Cache{T}.CreateItemStatic"/></a>
    [Test]
    public void CreateItemStatic() {
      Cache<SealedClassProcessed>.CleanCache();

      // This would normally be in a static constructor. It only need be run once
      Cache<SealedClassProcessed>.CreateItemStatic = () => new SealedClassProcessed {State = "CreateItemStatic"};

      var sealedClass = Cache<SealedClassProcessed>.Instance;

      using (Cache<SealedClassProcessed>.Disposable(sealedClass)) {
        Assert.AreEqual("CreateItemStatic", sealedClass.State);
      }
    }

    /// <a href="">Using <see cref="Cache{T}.DeactivateItemStatic"/></a>
    [Test]
    public void DeactivateItemStatic() {
      Cache<SealedClassProcessed>.CleanCache();
      // This would normally be in a static constructor. It only need be run once
      LinkedList<SealedClassProcessed>.CreateItemStatic     = () => new SealedClassProcessed {State = "CreateStatic"};
      LinkedList<SealedClassProcessed>.DeactivateItemStatic = (seal) => seal.Item.State =  "SealedDeactivateItem";
      LinkedList<SealedClassProcessed>.ReactivateItemStatic = (seal) => seal.Item.State += " SealedReactivateItem";

      var sealedClass = Cache<SealedClassProcessed>.Instance;

      using (Cache<SealedClassProcessed>.Disposable(sealedClass)) {
        Assert.AreEqual("CreateStatic", sealedClass.State);
      }

      Assert.AreEqual("SealedDeactivateItem", sealedClass.State);
    }

    /// <a href="">Using <see cref="Cache{T}.ReactivateItemStatic"/></a>
    [Test]
    public void ReactivateItemStatic() {
      Cache<SealedClassProcessed>.CleanCache();
      // This would normally be in a static constructor. It only need be run once
      LinkedList<SealedClassProcessed>.CreateItemStatic     = () => new SealedClassProcessed {State = "CreateStatic"};
      LinkedList<SealedClassProcessed>.DeactivateItemStatic = (seal) => seal.Item.State =  "SealedDeactivateItem";
      LinkedList<SealedClassProcessed>.ReactivateItemStatic = (seal) => seal.Item.State += " SealedReactivateItem";

      var sealedClass = Cache<SealedClassProcessed>.Instance;

      using (Cache<SealedClassProcessed>.Disposable(sealedClass)) {
        Assert.AreEqual("CreateStatic", sealedClass.State);
      }

      sealedClass = Cache<SealedClassProcessed>.Instance;

      Assert.AreEqual("SealedDeactivateItem SealedReactivateItem", sealedClass.State);
    }

    /// <a href="">Using <see cref="Cache{T}.Disposable(T)"/></a>
    [Test]
    public void DisposableStatic() {
      Cache<MyClassProcessed>.CleanCache();
      var myClass = Cache<MyClassProcessed>.Instance;

      using (Cache<MyClassProcessed>.Disposable(myClass)) {
        Assert.AreEqual("Created", myClass.State);
      }

      Assert.AreEqual("Deactivated", myClass.State);
    }
    #endregion

    #region Instance Examples
    /// <a href="">Using <see cref="Cached{T}.Dispose()"/></a>
    [Test]
    public void Dispose() {
      Cache<MyClassProcessed>.CleanCache();
      var myClass = MyClassProcessed.Instance;

      Assert.AreEqual("Created", myClass.State);

      myClass.Dispose();

      Assert.AreEqual("Deactivated", myClass.State);
    }

    /// <a href="">Using <see cref="Cached{T}.Disposable()"/></a>
    [Test]
    public void Disposable() {
      Cache<MyClassProcessed>.CleanCache();
      var myClass = MyClassProcessed.Instance;

      using (myClass.Disposable()) {
        Assert.AreEqual("Created", myClass.State);
      }

      Assert.AreEqual("Deactivated", myClass.State);
    }

    /// <a href="">Using <see cref="Cached{T}.CreateItem"/></a>
    [Test]
    public void CreateItem() {
      Cache<MyClassProcessed>.CleanCache();

      var myClass = MyClassProcessed.Instance;

      Assert.AreEqual("Created", myClass.State);
    }

    /// <a href="">Using <see cref="Cached{T}.DeactivateItem"/></a>
    [Test]
    public void DeactivateItem() {
      Cache<MyClassProcessed>.CleanCache();
      var myClass = MyClassProcessed.Instance;

      myClass.Dispose();

      Assert.AreEqual("Deactivated", myClass.State);
    }

    /// <a href="">Using <see cref="Cached{T}.ReactivateItem"/></a>
    [Test]
    public void ReactivateItem() {
      Cache<MyClassProcessed>.CleanCache();
      var myClass = MyClassProcessed.Instance;

      myClass.Dispose();
      myClass = MyClassProcessed.Instance;

      Assert.AreEqual("DeactivatedReactivated", myClass.State);
    }
    #endregion
  }
}