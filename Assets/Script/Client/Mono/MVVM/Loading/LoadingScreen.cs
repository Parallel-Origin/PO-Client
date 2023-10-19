using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Script.Client.Mono.MVVM.Loading {
    /// <summary>
    ///     Represents a loading screen and is used to show and hide certain parts of the screen during the loading process.
    /// </summary>
    public class LoadingScreen : MonoBehaviour {
        
        [SerializeField] private int waitDuration;
        [SerializeField] private UnityEvent onLoadStart;
        [SerializeField] private UnityEvent onLoadFinished;

        /// <summary>
        /// Starts the loading screen and shows the transistion
        /// </summary>
        public void Initialize() {
            StartCoroutine(Loading());
        }

        /// <summary>
        /// Starts the loading, calls the certain events
        /// </summary>
        /// <returns></returns>
        protected IEnumerator Loading() {
            
            onLoadStart.Invoke();
            yield return new WaitForSeconds(waitDuration);
            
            onLoadFinished.Invoke();
            yield return null;
        }
    }
}