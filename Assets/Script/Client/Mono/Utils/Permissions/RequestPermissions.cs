using ParallelOrigin.Core.Base.Classes.Pattern.Registers;
using UnityEngine;
using UnityEngine.Android;

#if PLATFORM_ANDROID

#endif

namespace Script.Client.Mono.Utils.Permissions {
    /// <summary>
    ///     This class is used to request nessecary permissions on app startup...
    /// </summary>
    public class RequestPermissions : MonoBehaviour {
        private void Awake() { ServiceLocator.Register(this); }

        private void Start() { RequestLocation(); }

        /// <summary>
        ///     This method returns true when the user has authorized the Location permission, otherwise it returns false.
        /// </summary>
        /// <returns></returns>
        public bool LocationGranted() {
#if PLATFORM_ANDROID
            if (Permission.HasUserAuthorizedPermission(Permission.FineLocation))
                return true;
#endif
            return false;
        }

        /// <summary>
        ///     This method will request location permissions, if not already granted.
        /// </summary>
        public void RequestLocation() {
#if PLATFORM_ANDROID
            if (!LocationGranted())
                Permission.RequestUserPermission(Permission.FineLocation);
            else
                Debug.Log("<RequestPermissions : Location permission is granted >");
#endif
        }
    }
}