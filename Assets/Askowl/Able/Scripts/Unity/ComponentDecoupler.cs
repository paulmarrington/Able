// Copyright 2018 (C) paul@marrington.net http://www.askowl.net/unity-packages

using Askowl;

namespace Decoupled {
  using System;
  using System.Collections.Generic;
  using UnityEngine;

  /// <a href="http://bit.ly/2AMWG4P">Creating Decoupled Components</a> <inheritdoc />
  public abstract class ComponentDecoupler<T> : MonoBehaviour where T : ComponentDecoupler<T> {
    /// <a href="http://bit.ly/2AO0T8m">List of component initialisers (used internally)</a>
    protected static event Action<T> Initialisers = delegate { };

    // ReSharper disable once StaticMemberInGenericType
    private static readonly Map interfaces = new Map();

    /// <a href="http://bit.ly/2AO0T8m">Parent class for component interfaces</a>
    public class ComponentInterface {
      internal Component Component;

      /// <a href="http://bit.ly/2AO0T8m">Called in concrete class constructor</a>
      protected void Instantiate<Tc>(bool primary) where Tc : Component {
        Type type = typeof(Tc);
        if (interfaces[type].Found) return;

        interfaces.Add(type);

        void initialiser(T t) {
          if (t.Instantiated) return; // someone else got in first

          if (primary || (t.defaultComponent == null)) t.defaultComponent = type;

          var component = t.GetComponent<Tc>();
          if (component != null) {
            Component            = component;
            t.componentInterface = (ComponentInterface) MemberwiseClone();
          }
        }

        Initialisers += initialiser;
      }

      /// <a href="http://bit.ly/2PHS606">Returns game object name as the most useful</a>
      public override string ToString() => Component == null ? "null" : Component.gameObject.name;
    }

    private ComponentInterface componentInterface;

    private Type defaultComponent;

    /// <a href="http://bit.ly/2AO0T8m">Set when the component is first instantiated (used internally)</a>
    protected bool Instantiated => componentInterface != null;

    /// <a href="http://bit.ly/2AO0T8m">Called in concrete component instances to get the correct backing component</a>
    protected ComponentInterface Instance {
      get {
        if (componentInterface != null) return componentInterface;

        Initialisers(this as T);
        if ((componentInterface != null) || (defaultComponent == null)) return componentInterface;

        gameObject.AddComponent(defaultComponent);
        Initialisers(this as T);
        return componentInterface;
      }
    }
  }
}