using BitBenderGames;
using UnityEngine;

namespace Script.Extensions {

    /// <summary>
    ///     An extension for the unity <see cref="Camera" />
    /// </summary>
    public static class CameraExtensions {

        /// <summary>
        ///     Focuses the camera on a certain position.
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="target"></param>
        public static void Focus(this Camera camera, float cameraDistance, Vector3 target) {

            var cameraView = 2.0f * Mathf.Tan(0.5f * Mathf.Deg2Rad * camera.fieldOfView); // Visible height 1 meter in front
            var distance = cameraDistance * 1 / cameraView; // Combined wanted distance from the object
            distance += 0.5f * 1; // Estimated offset from the center to the outside of the object

            camera.transform.position = target;
            camera.transform.position -= distance * camera.transform.forward;
        }

        /// <summary>
        ///     Focuses the camera on a certain position.
        /// </summary>
        /// <param name="mobileTouchCamera"></param>
        /// <param name="cameraDistance"></param>
        /// <param name="target"></param>
        public static void Focus(this MobileTouchCamera mobileTouchCamera, float cameraDistance, Vector3 target) {

            var camera = Camera.main;
            var zoom = mobileTouchCamera.CamZoom;

            camera.Focus(cameraDistance, target);

            mobileTouchCamera.Transform.position = camera.transform.position;
            mobileTouchCamera.CamZoom = zoom;
        }
    }
}