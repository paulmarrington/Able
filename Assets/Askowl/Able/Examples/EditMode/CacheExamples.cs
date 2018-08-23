// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

using NUnit.Framework;

namespace Askowl.Examples {
  public class CacheExamples {
//    // ReSharper disable once ClassNeverInstantiated.Global
//    public sealed class SealedClass  {
//      public string State { get; set; } = "New";
//    }
//
//    public sealed class SealedClassProcessed  {
//      public string State { get; set; } = "New";
//    }
//
//    private struct MyStruct  {
//      public string State { get; set; }
//
//      // A struct is useless in a cache if not initialised
//      // ReSharper disable once UnusedMember.Local
//      private static MyStruct CreateItem() => new MyStruct {State = "StructCreated"};
//    }
//
//    // ReSharper disable once ClassNeverInstantiated.Local
//    private class MyClassRaw : Cached<MyClassRaw> {
//      public string State { get; set; }
//    }
//
//    // ReSharper disable UnusedMember.Local
//    private class MyClassProcessed : Cached<MyClassProcessed> {
//      private new static MyClassProcessed CreateItem()     => new MyClassProcessed {State = "Created"};
//      private new        void             DeactivateItem() => State = "Deactivated";
//      private new        void             ReactivateItem() => State += "Reactivated";
//
//      public string State { get; set; }
//    }
//    // ReSharper restore UnusedMember.Local

    [Test]
    public void StructCreate() {
      LinkedList<MyStruct>.DebugMode = true;

      // can be used for value type and sealed classes.
      using (var myStruct = Cache<MyStruct>.Disposable) {
        Assert.AreEqual("StructCreated", myStruct.Value.State);
      }

      Cache<MyStruct>.CleanCache(); //#TBD# Not working
    }

    [Test]
    public void RawCreate() {
      // and as a base class for objects we want to cache
      using (var myClass = MyClassRaw.Instance) {
        Assert.IsNull(myClass.State);
        myClass.State = "using";
      }

      // will pull instance from recyle bin, so will have State set from last time
      using (var myClass = MyClassRaw.Instance) {
        Assert.AreEqual(myClass.State, "using");
      }

      MyClassRaw.CleanCache();
    }

    [Test]
    public void ProcessedCreateDeactivateReactivate() {
      using (var myClass = MyClassProcessed.Instance) {
        Assert.AreEqual("Created", myClass.State);
        myClass.State = "using";
      }

      // will pull instance from recyle bin, with it's processing
      using (var myClass = MyClassProcessed.Instance) {
        Assert.AreEqual(myClass.State, "DeactivatedReactivated");
      }

      MyClassProcessed.CleanCache();
    }

    [Test]
    public void Use() {
      Cache<MyStruct>.Use((myStruct) => Assert.AreEqual("StructCreated", myStruct.State));

      Cache<MyStruct>.CleanCache();
    }

    [Test]
    public void CachingForASealedClass() {
      Cache<SealedClass>.Use((sealedClassInstance) => Assert.AreEqual("New", sealedClassInstance.State));

      var sealedContainer = Cache<SealedClassProcessed>.Disposable;
      var sealedClass     = sealedContainer.Value;
      Assert.AreEqual("CreateItem", sealedClass.State);
      sealedContainer.Dispose();
      Assert.AreEqual("DeactivateItem ReactivateItem", sealedClass.State);

      Cache<SealedClass>.CleanCache();
      Cache<SealedClassProcessed>.CleanCache();
    }

//
//    static CacheExamples() {
//      Cache<SealedClassProcessed>.CreateItem     = () => new SealedClassProcessed {State = "CreateItem"};
//      Cache<SealedClassProcessed>.DeactivateItem = (seal) => seal.State =  "DeactivateItem";
//      Cache<SealedClassProcessed>.ReactivateItem = (seal) => seal.State += " ReactivateItem";
//    }
  }
}