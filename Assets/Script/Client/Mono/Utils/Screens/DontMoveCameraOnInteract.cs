using BitBenderGames;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Script.Client.Mono.Utils.Screens {
    
    /// <summary>
    ///     This class is a util for preventing the camera to move while interacting with popups or other screens.
    /// </summary>
    [RequireComponent(typeof(EventTrigger))]
    public class DontMoveCameraOnInteract : MonoBehaviour {
        
        public EventTrigger trigger;

        private void Start() {
            
            if (!trigger) trigger = GetComponent<EventTrigger>();

            var entryPointerDown = new EventTrigger.Entry {eventID = EventTriggerType.PointerDown};

            entryPointerDown.callback.AddListener(data => { TouchInputController.instance.OnEventTriggerPointerDown(data); });
            trigger.triggers.Add(entryPointerDown);
        }
    }
}