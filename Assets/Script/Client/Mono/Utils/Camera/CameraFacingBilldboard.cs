using UnityEngine;

namespace Script.Client.Mono.Utils.Camera {
    
    /// <summary>
    /// A script which rotates the gameobject this component is assigned to towards the main camera.
    /// </summary>
    public class CameraFacingBilldboard : MonoBehaviour {
        
        [SerializeField] private UnityEngine.Camera main;

        private void Awake() {
            if (main == null) main = UnityEngine.Camera.main;
        }

        private void LateUpdate() {
            
            var cameraTransform = main.transform;
            var cameraRotation = cameraTransform.rotation;
            var position = transform.position;

            transform.LookAt(position + cameraRotation * Vector3.forward, cameraRotation * Vector3.up);
        }
    }
}