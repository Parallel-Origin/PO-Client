using UnityEngine;
using UnityEngine.UI;

namespace Script.Client.Mono.MVVM.Login {
    
    /// <summary>
    /// Represents an login model which stores references to the ui elements
    /// </summary>
    public class LoginView : MonoBehaviour {

        [SerializeField] private Button loginButton;
        [SerializeField] private InputField username;
        [SerializeField] private InputField password;

        public Button LoginButton {
            get => loginButton;
            set => loginButton = value;
        }

        public InputField Username {
            get => username;
            set => username = value;
        }

        public InputField Password {
            get => password;
            set => password = value;
        }
    }
}