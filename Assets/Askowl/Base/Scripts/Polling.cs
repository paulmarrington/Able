using System;
using System.Collections;
using UnityEngine;

namespace Askowl {
  public interface IPolling {
    void Poll();
  }

  public class Polling : MonoBehaviour {
    [SerializeField] private float      updateIntervalInSeconds;
    [SerializeField] private IPolling[] componentsToPoll;

    private bool running;

    public bool Running {
      get { return running; }
      set {
        if (value && !running) StartPolling();
        running = value;
      }
    }

    private void OnEnable() { StartPolling(); }

    /// <summary>
    /// Start a coroutine to poll a component on the given MonoBehaviour.
    /// </summary>
    public virtual void StartPolling() {
      if (updateIntervalInSeconds > 0) StartCoroutine(StartPollingCoroutine());
    }

    private IEnumerator StartPollingCoroutine() {
      var interval = new WaitForSecondsRealtime(updateIntervalInSeconds);

      while (running) {
        foreach (IPolling component in componentsToPoll) component.Poll();
        yield return interval;
      }
    }
  }
}