using System.Collections;
using Script.Client.Mono.Utils.GameObjects;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Script.Network {
    
    /// <summary>
    /// This class will check on start if a new version is avaible, if so it will spawn an popup to inform about it.
    ///  It downloads a txt file from the url to check the version.
    /// </summary>
    [RequireComponent(typeof(PrefabSpawner))]
    public class VersionReceiver : MonoBehaviour {
        
        [SerializeField] private string versionDownloadUrl;
 
        [FormerlySerializedAs("OnNewVersionAvailable")] [SerializeField] private UnityEvent onNewVersionAvailable;
        [FormerlySerializedAs("OnNoNewVersionAvailable")] [SerializeField] private UnityEvent onNoNewVersionAvailable;
        
        private void Start() { GetVersion(); }

        /// <summary>
        ///  Receives the newest version
        /// </summary>
        protected void GetVersion() {
            
            if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork || Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
                StartCoroutine(Download());
            else
                Debug.LogError("VersionReceiver : Network not reachable");
        }


        /// <summary>
        ///     Starts a IEnumerator to download the version txt file from the url.
        /// </summary>
        /// <returns></returns>
        protected IEnumerator Download() {
            
            var webRequest = new WWW(versionDownloadUrl);
            yield return webRequest;

            var newestVersion = webRequest.text;
            Debug.Log($"VersionReceiver : Received version.. [{newestVersion}], app version [{Application.version}]");

            if (IsNewVersion(newestVersion))
                onNewVersionAvailable.Invoke();
            else onNoNewVersionAvailable.Invoke();
        }

        /// <summary>
        ///     Checks if a new version is available and returns true if so
        /// </summary>
        /// <param name="newestVersion"></param>
        /// <returns></returns>
        protected bool IsNewVersion(string newestVersion) { return !newestVersion.Equals(Application.version); }
    }
}