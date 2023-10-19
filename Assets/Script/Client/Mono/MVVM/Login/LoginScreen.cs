using System;
using LiteNetLib;
using ParallelOrigin.Core.Base.Classes.Pattern.Registers;
using ParallelOrigin.Core.ECS.Components;
using Script.Client.Mono.Utils.Login;
using Script.Extensions;
using Script.Network;
using Script.Server;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Script.Client.Mono.MVVM.Login {
    
    using Entity = Unity.Entities.Entity;
    
    /// <summary>
    ///  Is used in the login screen to manage the login & loading mechanics when login is pressed.
    /// </summary>
    [RequireComponent(typeof(LoginView))]
    public class LoginScreen : MonoBehaviour {

        private static readonly string UsernameKey = "username";
        private static readonly string PasswordKey = "password";
        
        [SerializeField] private UnityEvent onLogin;
        [SerializeField] private UnityEvent onUnfilledFields;

        private void Awake() {
            
            ServiceLocator.Wait<ClientNetwork>(o => {
                
                // Invoke login event when login was sucessfull 
                ClientNetwork = (ClientNetwork)o;
                ClientNetwork.OnLogin += (NetPeer peer, in ParallelOrigin.Core.ECS.Components.Character character) => onLogin?.Invoke();
            });
            
            LoginView = GetComponent<LoginView>();
        }

        private void Start() {

            
            // Set new user credentials, those gonna get loaded at program start for a easier login
            onLogin.AddListener(() => {
                
                if (!PlayerPrefs.HasKey(UsernameKey) && !PlayerPrefs.HasKey(PasswordKey)) {
                    PlayerPrefs.SetString(UsernameKey, UserCredentials.GetUsername());
                    PlayerPrefs.SetString(PasswordKey, UserCredentials.GetPassword());
                }
                else {
                    PlayerPrefs.SetString(UsernameKey, UserCredentials.GetUsername());
                    PlayerPrefs.SetString(PasswordKey, UserCredentials.GetPassword());
                }
            });

            if (PlayerPrefs.HasKey(UsernameKey) && PlayerPrefs.HasKey(PasswordKey)) {
                
                LoginView.Username.text = PlayerPrefs.GetString(UsernameKey);
                LoginView.Password.text = PlayerPrefs.GetString(PasswordKey);

                UserCredentials.SetUsername(LoginView.Username.text);
                UserCredentials.SetPassword(LoginView.Password.text);
            }

            // Login, once the button was pressed and 
            LoginView.LoginButton.onClick.AddListener(Login);
            LoginView.Username.onEndEdit.AddListener(UserCredentials.SetUsername);
            LoginView.Password.onEndEdit.AddListener(UserCredentials.SetPassword);
        }

        /// <summary>
        ///     Gets called once we click on that assigned login button and communicates with the <see cref="IConnectionLogin" /> to process our user login
        /// </summary>
        public void Login() {
            
            if (FieldsFilled())
                ClientNetwork.Login(LoginView.Username.text, LoginView.Password.text);
            else {
                onUnfilledFields?.Invoke();
                Debug.LogWarning("<LoginScreen : Fields need to be filled>");
            }
        }

        /// <summary>
        ///     Checks if all required fields are filled in order to spawn a notification
        /// </summary>
        /// <returns></returns>
        private bool FieldsFilled() {

            if (string.IsNullOrEmpty(LoginView.Username.text)) return false;
            return !string.IsNullOrEmpty(LoginView.Password.text);
        }
        
        /// <summary>
        /// The client network used to communicate with the server
        /// </summary>
        public ClientNetwork ClientNetwork { get; set; }
        
        /// <summary>
        /// The login view which stores the ui 
        /// </summary>
        public LoginView LoginView { get; set; }
    }
}