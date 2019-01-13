namespace Decoupled {
  using System.Collections.Generic;
  using Askowl;
  using UnityEngine;

  /// <a href="http://bit.ly/2PMWqeq">Parent class for decoupled services</a>
  public abstract class Service {
    /// <a href="http://bit.ly/2PMWpqS">Set on creation and used to select named instances</a>
    public string Name;

    /// <a href="http://bit.ly/2APZMoL">Set if mock instance to stop any other version overriding it</a>
    protected static bool HasMockInstance;

    /// <a href="http://bit.ly/2PMWqeq">Override to initialise concrete service instances</a>
    protected abstract void Initialise();

    /// <a href="http://bit.ly/2APZMoL">Called my Mock mono-behaviour</a>
    public static void RegisterAsMock(Service mock) => mock.RegisterAsMock();

    /// <a href="http://bit.ly/2APZMoL">Implemented by each mock type in <see cref="Service{T}"/></a>
    protected abstract void RegisterAsMock();
  }

  /// <a href="http://bit.ly/2PdQyf3">Parent class for any decoupled service</a> <inheritdoc />
  public abstract class Service<T> : Service where T : Service<T> {
    private static readonly Log.EventRecorder warning = Log.Warnings($"{typeof(T).Name} Decoupler Service");

    /// <a href="http://bit.ly/2PdyOAj">Use to send to some or all registered services</a>
    public static readonly List<T> InstanceList = new List<T>();

    private static T defaultInstance;

    private static bool Available => InstanceList.Count > 0;

    /// <a href="http://bit.ly/2APZMoL">Used by <see cref="RegisterAsMock"/> to clear other registrations</a>
    public static void Reset() {
      InstanceList.Clear();
      defaultInstance = default;
    }

    /// <a href="http://bit.ly/2PMWqLs">Retrieve a service instance based on the dominant registration</a>
    public static T Instance {
      get {
        if (Available) return InstanceList[0];
        if (defaultInstance != default(T)) return defaultInstance;

        defaultInstance = new T();

        if (defaultInstance == null
        ) { Debug.LogErrorFormat("Cannot instantiate default '{0}'", typeof(T).Name); } else {
          defaultInstance.Initialise();
        }

        return defaultInstance;
      }
    }

    /// <a href="http://bit.ly/2PMWpqS">Retrieve a registered instance by name (being the service class name)</a>
    public static T NamedInstance(string name) {
      for (var i = 0; i < InstanceList.Count; i++) {
        if (InstanceList[i].Name == name) return InstanceList[i];
      }

      return null;
    }

    /// <a href="http://bit.ly/2PMWjj0">The default service is usually the one defining the interface. It is only used if no others register</a>
    public static void RegisterDefault<Td>() where Td : T, new() {
      if (Available) return;

      defaultInstance = new Td {Name = typeof(Td).Name};
    }

    /// <a href="http://bit.ly/2PMWqeq">Register one or more implementations of a service</a>
    public static void Register<Td>() where Td : T, new() {
      if (Available) Log.Debug($"More than one implementation for service '{typeof(T).Name}'");

      if (!HasMockInstance) new Td {Name = typeof(Td).Name}.Register();
    }

    internal void Register() {
      InstanceList.Add((T) this);
      Initialise();
    }

    /// <a href="http://bit.ly/2APZMoL">Mock registration implementation forces itself to be prime</a><inheritdoc />
    protected override void RegisterAsMock() {
      Reset();
      HasMockInstance = true;
      Register();
      warning("*** Mock Active ***");
    }
  }
}