using UnityEngine;
using UnityEngine.EventSystems;

namespace Script.Client.Mono.Utils.Screens {
    /// <summary>
    ///     This class is used to solve the UI responsivity issue... it basically just scales the drag threshold up for a better tapping experience.
    /// </summary>
    public class DragThresholdUtil : MonoBehaviour {
        private void Start() {
            var defaultValue = EventSystem.current.pixelDragThreshold;
            EventSystem.current.pixelDragThreshold = Mathf.Max(defaultValue, (int) (defaultValue * Screen.dpi / 160f));
            Debug.Log("<DragThresholdUtil : Changed drag to [" + Mathf.Max(defaultValue, (int) (defaultValue * Screen.dpi / 160f)) + "]>");
        }
    }
}