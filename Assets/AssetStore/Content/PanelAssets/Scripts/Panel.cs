using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Collections;
using System;

//[ExecuteInEditMode]
namespace SFS2XExamples.Panel {
    public class Panel : MonoBehaviour {

        //----------------------------------------------------------
        // Editor public properties
        //----------------------------------------------------------

        [Tooltip("IP address or domain name of the SmartFoxServer 2X instance")]
        public string Host = "127.0.0.1";

        [Tooltip("TCP port listened by the SmartFoxServer 2X instance; used for regular socket connection in all builds except WebGL")]
        public int TcpPort = 9933;

        [Tooltip("WebSocket port listened by the SmartFoxServer 2X instance; used for websocket connection in WebGL builds only")]
        public int WsPort = 8080;

        [Tooltip("Name of the SmartFoxServer 2X Zone to join (where needed)")]
        public string Zone = "BasicExamples";

        //----------------------------------------------------------
        // UI elements
        //----------------------------------------------------------

        public Animator cameraAnimator;
        public Button fpsLaunchButton;
        public Text fpsLaunchText;
        public UnityEngine.UI.InputField ipInput;
        public UnityEngine.UI.InputField portInput;

        //----------------------------------------------------------
        // Global Settings
        //----------------------------------------------------------

        private static Settings globalSettings;

        //----------------------------------------------------------
        // Unity calback methods
        //----------------------------------------------------------

        void Start() {
            Application.runInBackground = true;

#if UNITY_WEBGL
			fpsLaunchButton.interactable = false;
			fpsLaunchText.color = Color.gray;
#endif

            if (globalSettings == null) {
                // First time launching
                globalSettings = Settings.Instance;

                // Initialize UI
                ipInput.text = Settings.ipAddress = Host;

#if !UNITY_WEBGL
                portInput.text = (Settings.port = TcpPort).ToString();
#else
				portInput.text = (Settings.port = WsPort).ToString();
#endif
            }
            else {
                // Load IP & TCP Port configuration from global Settings
                ipInput.text = Settings.ipAddress;
                portInput.text = Settings.port.ToString();

                // Rotate camera to examples panel instantly
                cameraAnimator.SetBool("showExamples", true);
                cameraAnimator.speed = 2000;
            }
        }

        //----------------------------------------------------------
        // Public interface methods for UI
        //----------------------------------------------------------
        // Local Examples Panel
        public void OnDownloadSmartFoxServer2XButtonClick() {
            Application.OpenURL("http://smartfoxserver.com/download/sfs2x#p=installer");
        }
        public void OnDownloadLatestPatchButtonClick() {
            Application.OpenURL("http://smartfoxserver.com/download/sfs2x#p=updates");
        }
        public void OnShowExamplesButtonClick() {
            // Save IP & TCP Port configuration to global Settings
            SFS2XExamples.Panel.Settings.ipAddress = ipInput.text;
            SFS2XExamples.Panel.Settings.port = Int32.Parse(portInput.text);

            // Rotate camera to examples panel
            cameraAnimator.speed = 2;
            cameraAnimator.SetBool("showExamples", true);
        }

        // Live Examples Panel
        public void OnVisitTheLiveExamplesButtonClick() {
            Application.OpenURL("http://smartfoxserver.com/overview/demo#unity");
        }

        // Online Resources Panel
        public void OnIntroductionToSFS2XUnityButtonClick() {
            Application.OpenURL("http://docs2x.smartfoxserver.com/ExamplesUnity/introduction");
        }
        public void OnSmartFoxServer2XDocumentationButtonClick() {
            Application.OpenURL("http://docs2x.smartfoxserver.com/");
        }
        public void OnSFS2XUnityVideoTutorialsButtonClick() {
            Application.OpenURL("https://www.youtube.com/user/SmartFoxServer");
        }
        public void OnSFS2XLicensingOptionsButtonClick() {
            Application.OpenURL("http://www.smartfoxserver.com/products/sfs2x#p=licensing");
        }

        // Examples Panel
        public void OnGoBackButtonClick() {
            // Rotate camera back to main panel
            cameraAnimator.speed = 3;
            cameraAnimator.SetBool("showExamples", false);
        }

        public void OnExample01ConnectorButtonClick() {
            StartExample("01 Connector");
        }
        public void OnExample02LobbyButtonClick() {
            StartExample("02 Lobby");
        }
        public void OnExample03BuddyMessengerButtonClick() {
            StartExample("03 BuddyMessenger", Color.black, 1);
        }
        public void OnExample04TrisButtonClick() {
            StartExample("04 TrisLogin");
        }
        public void OnExample05ObjectMovementButtonClick() {
            StartExample("05 ObjectMovementConnection", Color.white, 0);
        }
        public void OnExample06FirstPersonShooterButtonClick() {
            StartExample("06 FPSLogin", Color.white, 0);
        }
        public void OnExample07MMORoomDemoButtonClick() {
            StartExample("07 MMORoomDemoConnection", Color.white, 0);
        }
        public void OnExample08SpaceWarButtonClick() {
            StartExample("08 SpaceWarGame", Color.white, 0);
        }
        public void OnExample09AdvancedConnectorButtonClick() {
            StartExample("09 AdvancedConnector");
        }

        public void StartExample(string sceneName) {
            StartExample(sceneName, Color.black, 0);
        }

        public void StartExample(string sceneName, Color escMessageColor, int escMessagePosition) {
            Settings.escMessageColor = escMessageColor;
            Settings.escMessagePosition = escMessagePosition;
            SceneManager.LoadScene(sceneName);
        }
    }
}
