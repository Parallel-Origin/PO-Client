using System.Collections.Generic;
using Script.Client.Mono.Entity.Components;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Script.Client.Mono.User_Interface.Components {
    
    /// <summary>
    ///     Checks if the attached gameobject is in focus, if a click outside the element occurs, it gets destroyed
    /// </summary>
    [RequireComponent(typeof(Callbacks))]
    public class UIFocusDestroyer : MonoBehaviour {
        
        [SerializeField] private Canvas canvas;
        private Callbacks _callbacks;

        private void Awake() { _callbacks = GetComponent<Callbacks>(); }

        private void Update() {

            // Only trigger if we have a input & the pointer is not above a Ui-Element
            if (!canvas.enabled) return;

            // Prevent the code from running when nothing was pressed
            var isPressed = false;
            if (Application.platform == RuntimePlatform.WindowsEditor) isPressed = Input.GetMouseButtonDown(0);
            else isPressed = Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended;
            
            if (!isPressed) return;
            
            // Check if this gameobject is hovered by pointer, if not... destroy it
            var isHovered = false;
            if (Application.platform == RuntimePlatform.WindowsEditor) {
                
                // On editor and pc 
                foreach (var go in ExtendedStandaloneInputModule.GetPointerEventData().hovered)
                    if (go == gameObject)
                        isHovered = true;
            }
            else {

                // On mobile, check if touch also touches this object here by using a raycast. 
                var touch = Input.GetTouch(0);
                var results = new List<RaycastResult>();
                var eventDataCurrentPosition = new PointerEventData(EventSystem.current) { position = new Vector2(touch.position.x, touch.position.y) };
                EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

                foreach (var result in results) {
                    if (result.gameObject == gameObject || result.gameObject.transform.IsChildOf(gameObject.transform))
                        isHovered = true;
                }
            }

            if (isHovered) return;
            
            _callbacks.OnDestroyEvent?.Invoke(this);
            _callbacks.OnDestroy(this);
            Destroy(gameObject);
        }
    }
}