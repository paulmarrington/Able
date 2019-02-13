// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

#if AskowlTests
using System;
using NUnit.Framework;
using NUnit.Framework.Internal;
// ReSharper disable MissingXmlDoc

namespace Askowl.Able.Examples {
  using UnityEngine;

  public class CacheExamples {
    #region Working Data
    public sealed class AgnosticClass {
      public string State { get; set; } = "New";
    }

    [Test] public void Example() {
      var myClass = Cache<AgnosticClass>.Instance;
      using (Cache<AgnosticClass>.Disposable(myClass)) { myClass.State = "Used"; }
      Debug.Log(myClass.State);
    }

    public sealed class AgnosticClassProcessed {
      public string State { get; set; } = "New";
    }

    private struct MyStruct {
      public string State { get; private set; }

      // A struct is useless in a cache if not initialised
      private static MyStruct CreateItem() => new MyStruct { State = "StructCreated" };
    }

    private class Aware : IDisposable {
      private static Aware CreateItem()     => new Aware { State = "Created" };
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
    [Test] public void InstanceStatic() {
      Cache<MyStruct>.Entries.Destroy();

      var myStruct = Cache<MyStruct>.Instance;

      Assert.AreEqual("StructCreated", myStruct.State);
    }

    [Test] public void DisposeStatic() {
      Cache<Aware>.Entries.Destroy();

      var myClass = Cache<Aware>.Instance;

      Assert.AreEqual("Created", myClass.State);

      myClass.Dispose();

      Assert.AreEqual("Deactivated", myClass.State);
    }

    [Test] public void RecycleEverything() {
      var unused = Cache<Aware>.Instance;

      Cache<Aware>.RecycleEverything();

      Assert.IsNull(Cache<Aware>.Entries.First);
      Assert.IsNotNull(Cache<Aware>.Entries.RecycleBin.First);
    }

    [Test] public void CreateItemStatic() {
      Cache<AgnosticClassProcessed>.Entries.Destroy();

      // This would normally be in a static constructor. It only need be run once
      Cache<AgnosticClassProcessed>.CreateItem = () => new AgnosticClassProcessed { State = "CreateItemStatic" };

      var agnosticClass = Cache<AgnosticClassProcessed>.Instance;

      using (Cache<AgnosticClassProcessed>.Disposable(agnosticClass)) {
        Assert.AreEqual("CreateItemStatic", agnosticClass.State);
      }
    }

    [Test] public void DeactivateItemStatic() {
      Cache<AgnosticClassProcessed>.Entries.Destroy();
      // This would normally be in a static constructor. It only need be run once
      Cache<AgnosticClassProcessed>.CreateItem     = () => new AgnosticClassProcessed { State = "CreateStatic" };
      Cache<AgnosticClassProcessed>.DeactivateItem = (item) => item.State =  "AgnosticDeactivateItem";
      Cache<AgnosticClassProcessed>.ReactivateItem = (item) => item.State += " AgnosticReactivateItem";

      var agnosticClass = Cache<AgnosticClassProcessed>.Instance;

      using (Cache<AgnosticClassProcessed>.Disposable(agnosticClass)) {
        Assert.AreEqual("CreateStatic", agnosticClass.State);
      }

      Assert.AreEqual("AgnosticDeactivateItem", agnosticClass.State);
    }

    [Test] public void ReactivateItemStatic() {
      Cache<AgnosticClassProcessed>.Entries.Destroy();

      // This would normally be in a static constructor. It only need be run once
      Cache<AgnosticClassProcessed>.CreateItem =
        () => new AgnosticClassProcessed { State = "CreateStatic" };

      Cache<AgnosticClassProcessed>.DeactivateItem = (item) => item.State =  "SealedDeactivateItem";
      Cache<AgnosticClassProcessed>.ReactivateItem = (item) => item.State += " SealedReactivateItem";

      var sealedClass = Cache<AgnosticClassProcessed>.Instance;

      using (Cache<AgnosticClassProcessed>.Disposable(sealedClass)) {
        Assert.AreEqual("CreateStatic", sealedClass.State);
      }

      sealedClass = Cache<AgnosticClassProcessed>.Instance;

      Assert.AreEqual("SealedDeactivateItem SealedReactivateItem", sealedClass.State);
    }

    [Test] public void DisposableStatic() {
      Cache<Aware>.Entries.Destroy();
      var myClass = Cache<Aware>.Instance;

      using (Cache<Aware>.Disposable(myClass)) Assert.AreEqual("Created", myClass.State);

      Assert.AreEqual("Deactivated", myClass.State);
    }
    #endregion

    #region Instance Examples
    [Test] public void Dispose() {
      Cache<Aware>.Entries.Destroy();
      var myClass = Aware.Instance;

      Assert.AreEqual("Created", myClass.State);

      myClass.Dispose();

      Assert.AreEqual("Deactivated", myClass.State);
    }

    [Test] public void CacheDispose() {
      Cache<Aware>.Entries.Destroy();
      var myClass = Aware.Instance;

      using (myClass) Assert.AreEqual("Created", myClass.State);

      Assert.AreEqual("Deactivated", myClass.State);
    }

    [Test] public void CreateItem() {
      Cache<Aware>.Entries.Destroy();

      var myClass = Aware.Instance;

      Assert.AreEqual("Created", myClass.State);
    }

    [Test] public void DeactivateItem() {
      Cache<Aware>.Entries.Destroy();
      var myClass = Aware.Instance;

      myClass.Dispose();

      Assert.AreEqual("Deactivated", myClass.State);
    }

    [Test] public void ReactivateItem() {
      Cache<Aware>.Entries.Destroy();
      var myClass = Aware.Instance;

      myClass.Dispose();
      myClass = Aware.Instance;

      Assert.AreEqual("DeactivatedReactivated", myClass.State);
    }
    #endregion
  }
}
#endif