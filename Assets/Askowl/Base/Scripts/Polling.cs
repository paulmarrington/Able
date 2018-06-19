using System;
using System.Collections;
using UnityEngine;

namespace Askowl {
  public interface IPolling {
    void Update();
  }

  public class Polling : MonoBehaviour {
    [SerializeField] private float    updateIntervalInSeconds;
    [SerializeField] private IPolling toPoll;
    [SerializeField] private bool     running;

    public bool Running {
      get { return running; }
      set {
        if (value && !running) StartPolling();
        running = value;
      }
    }

    private void OnEnable() { Running = running; }

    /// <summary>
    /// Start a coroutine to poll the gyroscope on the given MonoBehaviour.
    /// </summary>
    /// <param name="monoBehaviour">The MonoBehaviour that owns the polling coroutine</param>
    public virtual void StartPolling() {
      if (updateIntervalInSeconds > 0) StartCoroutine(StartPollingCoroutine());
    }

    private IEnumerator StartPollingCoroutine() {
      var interval = new WaitForSecondsRealtime(updateIntervalInSeconds);

      while (running) {
        toPoll.Update();
        yield return interval;
      }
    }
  }
}