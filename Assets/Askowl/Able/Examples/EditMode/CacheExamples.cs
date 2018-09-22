// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

#if AskowlAble
using System;
using NUnit.Framework;

// ReSharper disable UnusedMember.Local

namespace Askowl.Examples {
  public class CacheExamples {
    #region Working Data
    public sealed class AgnosticClass {
      public string State { get; set; } = "New";
    }

    public sealed class AgnosticClassProcessed {
      public string State { get; set; } = "New";
    }

    private struct MyStruct {
      public string State { get; private set; }

      // A struct is useless in a cache if not initialised
      private static MyStruct CreateItem() => new MyStruct {State = "StructCreated"};
    }

    private class Aware : IDisposable {
      private static Aware CreateItem()     => new Aware {State = "Created"};
      private        void  DeactivateItem() => State = "Deactivated";
      private        void  ReactivateItem() => State += "Reactivated";

      private Aware() { }
      public static Aware Instance => Cache<Aware>.Instance;

      public void Dispose() { Cache<Aware>.Dispose(this); }

      public string State { get; private set; }

      ~Aware() { Dispose(); }
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
      Cache<Aware>.CleanCache();

      var myClass = Cache<Aware>.Instance;

      Assert.AreEqual("Created", myClass.State);

      myClass.Dispose();

      Assert.AreEqual("Deactivated", myClass.State);
    }

    /// <a href="">Using <see cref="Cache{T}.CleanCache"/></a>
    [Test]
    public void CleanCache() {
      var unused = Cache<Aware>.Instance;

      Cache<Aware>.CleanCache();

      Assert.IsNull(Cache<Aware>.Entries.First);
      Assert.IsNull(Cache<Aware>.Entries.RecycleBin.First);
    }

    /// <a href="">Using <see cref="Cache{T}.CleanCache"/></a>
    [Test]
    public void ClearCache() {
      var unused = Cache<Aware>.Instance;

      Cache<Aware>.ClearCache();

      Assert.IsNull(Cache<Aware>.Entries.First);
      Assert.IsNotNull(Cache<Aware>.Entries.RecycleBin.First);
    }

    /// <a href="">Using <see cref="Cache{T}.CreateItem"/></a>
    [Test]
    public void CreateItemStatic() {
      Cache<AgnosticClassProcessed>.CleanCache();

      // This would normally be in a static constructor. It only need be run once
      Cache<AgnosticClassProcessed>.CreateItem = () => new AgnosticClassProcessed {State = "CreateItemStatic"};

      var agnosticClass = Cache<AgnosticClassProcessed>.Instance;

      using (Cache<AgnosticClassProcessed>.Disposable(agnosticClass)) {
        Assert.AreEqual("CreateItemStatic", agnosticClass.State);
      }
    }

    /// <a href="">Using <see cref="Cache{T}.DeactivateItem"/></a>
    [Test]
    public void DeactivateItemStatic() {
      Cache<AgnosticClassProcessed>.CleanCache();
      // This would normally be in a static constructor. It only need be run once
      Cache<AgnosticClassProcessed>.CreateItem = () => new AgnosticClassProcessed {State = "CreateStatic"};
      Cache<AgnosticClassProcessed>.DeactivateItem = (item) => item.State = "AgnosticDeactivateItem";
      Cache<AgnosticClassProcessed>.ReactivateItem = (item) => item.State += " AgnosticReactivateItem";

      var agnosticClass = Cache<AgnosticClassProcessed>.Instance;

      using (Cache<AgnosticClassProcessed>.Disposable(agnosticClass)) {
        Assert.AreEqual("CreateStatic", agnosticClass.State);
      }

      Assert.AreEqual("AgnosticedDeactivateItem", agnosticClass.State);
    }

    /// <a href="">Using <see cref="Cache{T}.ReactivateItem"/></a>
    [Test]
    public void ReactivateItemStatic() {
      Cache<AgnosticClassProcessed>.CleanCache();

      // This would normally be in a static constructor. It only need be run once
      Cache<AgnosticClassProcessed>.CreateItem =
        () => new AgnosticClassProcessed {State = "CreateStatic"};

      Cache<AgnosticClassProcessed>.DeactivateItem = (item) => item.State = "SealedDeactivateItem";
      Cache<AgnosticClassProcessed>.ReactivateItem = (item) => item.State += " SealedReactivateItem";

      var sealedClass = Cache<AgnosticClassProcessed>.Instance;

      using (Cache<AgnosticClassProcessed>.Disposable(sealedClass)) {
        Assert.AreEqual("CreateStatic", sealedClass.State);
      }

      sealedClass = Cache<AgnosticClassProcessed>.Instance;

      Assert.AreEqual("SealedDeactivateItem SealedReactivateItem", sealedClass.State);
    }

    /// <a href="">Using <see cref="Cache{T}.Disposable(T)"/></a>
    [Test]
    public void DisposableStatic() {
      Cache<Aware>.CleanCache();
      var myClass = Cache<Aware>.Instance;

      using (Cache<Aware>.Disposable(myClass)) {
        Assert.AreEqual("Created", myClass.State);
      }

      Assert.AreEqual("Deactivated", myClass.State);
    }
    #endregion

    #region Instance Examples
    /// <a href="">Using <see cref="Cache{T}.Dispose(T)"/></a>
    [Test]
    public void Dispose() {
      Cache<Aware>.CleanCache();
      var myClass = Aware.Instance;

      Assert.AreEqual("Created", myClass.State);

      myClass.Dispose();

      Assert.AreEqual("Deactivated", myClass.State);
    }

    /// <a href="">Using <see cref="Cache{T}.Dispose(T)"/></a>
    [Test]
    public void CacheDispose() {
      Cache<Aware>.CleanCache();
      var myClass = Aware.Instance;

      using (myClass) {
        Assert.AreEqual("Created", myClass.State);
      }

      Assert.AreEqual("Deactivated", myClass.State);
    }

    /// <a href="">Using <see cref="Cache{T}.CreateItem"/></a>
    [Test]
    public void CreateItem() {
      Cache<Aware>.CleanCache();

      var myClass = Aware.Instance;

      Assert.AreEqual("Created", myClass.State);
    }

    /// <a href="">Using <see cref="Cache{T}.DeactivateItem"/></a>
    [Test]
    public void DeactivateItem() {
      Cache<Aware>.CleanCache();
      var myClass = Aware.Instance;

      myClass.Dispose();

      Assert.AreEqual("Deactivated", myClass.State);
    }

    /// <a href="">Using <see cref="Cache{T}.ReactivateItem"/></a>
    [Test]
    public void ReactivateItem() {
      Cache<Aware>.CleanCache();
      var myClass = Aware.Instance;

      myClass.Dispose();
      myClass = Aware.Instance;

      Assert.AreEqual("DeactivatedReactivated", myClass.State);
    }
    #endregion
  }
}
#endif