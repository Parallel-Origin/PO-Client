using System.Collections;
using ParallelOrigin.Core.Base.Classes.Pattern.Registers;
using UnityEngine;
using UnityEngine.Events;

namespace Script.Client.Mono.Map {
    /// <summary>
    ///     This class manages the GPS location.
    ///     On start it will query the location once and store it as the current location. (Input.Location)
    /// </summary>
    public class GpsLocation : MonoBehaviour {
        
        [SerializeField] public UnityEvent gpsNotEnabled;

        private void Awake() { ServiceLocator.Register(this); }

        public void Start() { StartCoroutine(Locate()); }

        /// <summary>
        ///     Tries to locate the user by its GPS and loads the queried result into the Input.Location classes.
        /// </summary>
        /// <returns></returns>
        protected IEnumerator Locate() {
            // First, check if user has location service enabled
            if (!Input.location.isEnabledByUser) {
                Debug.Log("<GPSLocation : GPS not enabled by user>");
                gpsNotEnabled.Invoke();

                yield break;
            }

            // Start service before querying location
            Input.location.Start();

            // Wait until service initializes
            var maxWait = 20;

            while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0) {
                yield return new WaitForSeconds(1);
                maxWait--;
            }

            // Service didn't initialize in 20 seconds
            if (maxWait < 1) {
                Debug.Log("<GPSLocation : Timed out>");
                yield break;
            }

            // Connection has failed
            if (Input.location.status == LocationServiceStatus.Failed) {
                Debug.Log("<GPSLocation : Unable to determine device location>");
                yield break;
            }

            // Access granted and location value could be retrieved
            Debug.Log("<GPSLocation : Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + ">");
            Debug.Log("<GPSLocation : Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.altitude + " " +
                      Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp + ">");

            // Stop service if there is no need to query location updates continuously
            Input.location.Stop();
        }
    }
}