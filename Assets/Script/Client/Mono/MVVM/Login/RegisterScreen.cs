using Script.Client.Mono.Map;
using Script.Client.Mono.Utils.Login;
using Script.Client.Mono.Utils.Permissions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Script.Client.Mono.MVVM.Login {
    /// <summary>
    ///     This class is used in the registration screen. It manages the registration ( client side ) as well as the field checking.
    /// </summary>
    public class RegisterScreen : MonoBehaviour {
        [SerializeField] private RequestPermissions requestPermissions;
        [SerializeField] private GpsLocation gpsLocation;

        [SerializeField] private Button proceedToCustomizationButton;
        [SerializeField] private Button completeRegistrationButton;

        [SerializeField] private InputField username;
        [SerializeField] private InputField password;
        [SerializeField] private InputField email;
        [SerializeField] private Toggle acceptedGdpr;

        [SerializeField] private UnityEvent onRegistration;
        [SerializeField] private UnityEvent onNext;
        [SerializeField] private UnityEvent onUnfilledFields;
        [SerializeField] private UnityEvent noLocationPermissions;
        [SerializeField] private UnityEvent noGpsActivated;

        private void Start() {
            proceedToCustomizationButton.onClick.AddListener(() => { Proceed(); });
            completeRegistrationButton.onClick.AddListener(() => { Register(); });

            username.onEndEdit.AddListener(edited => { UserCredentials.SetUsername(edited.Trim()); });
            password.onEndEdit.AddListener(edited => { UserCredentials.SetPassword(edited); });
            email.onEndEdit.AddListener(edited => { UserCredentials.SetEmail(edited); });
        }

        /// <summary>
        ///     Completes the registration
        /// </summary>
        private void Register() {
            if (FieldsFilled()) {
/*
            if (!requestPermissions.locationGranted()) {

                Debug.Log("<RegisterScreen : No location permission found => cannot proceed to registration... meanwhile requesting permission...>");
                requestPermissions.requestLocation();
                noLocationPermissions.Invoke();

                return;
            }

            bool canRelocate = true;
            gpsLocation.gpsNotEnabled.AddListener(() => { canRelocate = false; });

            Debug.Log("<RegisterScreen : --Trying to receive location-- >");
            gpsLocation.Start();
            Debug.Log("<RegisterScreen : --Receiving GPSLocation-- >");


            if (canRelocate) {
                Debug.Log("<RegisterScreen : --Sucessfully received position-- >");
                onRegistration.Invoke();
            }
            else {
                noGPSActivated.Invoke();
                Debug.Log("<RegisterScreen : -- GPS not activated -- >");
            }
*/

                onRegistration.Invoke();
            }
            else {
                onUnfilledFields.Invoke();
                Debug.LogWarning("<RegisterScreen : Fields need to be filled>");
            }
        }

        /// <summary>
        ///     Redirects to the next screen
        /// </summary>
        private void Proceed() {
            if (FieldsFilled()) { onNext.Invoke(); }
            else {
                onUnfilledFields.Invoke();
                Debug.LogWarning("<RegisterScreen : Fields need to be filled>");
            }
        }

        /// <summary>
        ///     Checks if all fields are filled...
        /// </summary>
        /// <returns></returns>
        private bool FieldsFilled() {
            if (string.IsNullOrEmpty(username.text))
                return false;
            if (string.IsNullOrEmpty(password.text))
                return false;
            if (string.IsNullOrEmpty(email.text))
                return false;
            if (!acceptedGdpr.isOn)
                return false;

            return true;
        }
    }
}