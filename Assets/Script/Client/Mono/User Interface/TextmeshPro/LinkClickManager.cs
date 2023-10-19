using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Script.Client.Mono.User_Interface.TextmeshPro {
    /// <summary>
    ///     This class is used to interact with embedded links in tmp-componentns.
    ///     Currently it only opens up the set link once a set id was called.
    ///     TODO: Adding some kind of directory mechanic
    /// </summary>
    public class LinkClickManager : MonoBehaviour, IPointerClickHandler {
        [SerializeField] private string id;
        [SerializeField] private string url;
        private Canvas _canvas;

        // Used by the TMP_TextUtilities package to determine
        // if the position of a click intersects with a link
        private Camera _gameCamera;

        // Used to look up link info
        private TMP_Text _textComponent;

        private void Awake() {
            // Get a reference to the text component.
            _textComponent = gameObject.GetComponent<TMP_Text>();

            // Get a reference to the camera rendering the text taking into consideration the text component type.
            if (_textComponent.GetType() == typeof(TextMeshProUGUI)) {
                _canvas = gameObject.GetComponentInParent<Canvas>();

                if (_canvas != null) {
                    if (_canvas.renderMode == RenderMode.ScreenSpaceOverlay)
                        _gameCamera = null;
                    else
                        _gameCamera = _canvas.worldCamera;
                }
            }
            else { _gameCamera = Camera.main; }
        }

        /// <summary>
        ///     Gets called, once the mouse/touch interacts with the tmp-component.
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerClick(PointerEventData eventData) {
            var clickPosition = new Vector3(eventData.position.x, eventData.position.y, 0);

            // Check if mouse intersects with any links.
            var linkIndex = TMP_TextUtilities.FindIntersectingLink(_textComponent, clickPosition, _gameCamera);

            // Handle new Link selection.
            if (linkIndex != -1) {
                // Get information about the link.
                var linkInfo = _textComponent.textInfo.linkInfo[linkIndex];

                // Send the event to any listeners.
                HandleLinkClick(linkInfo.GetLinkID(), linkInfo.GetLinkText(), linkIndex);
            }
        }

        /// <summary>
        ///     Gets called once a special link in the tmp-component was clicked.
        ///     It will open up the device browser and shows the link.
        /// </summary>
        /// <param name="inLinkID"></param>
        /// <param name="inLinkText"></param>
        /// <param name="inLinkIndex"></param>
        public void HandleLinkClick(string inLinkID, string inLinkText, int inLinkIndex) {
            if (inLinkID.Equals(id))
                Application.OpenURL(url);
        }
    }
}