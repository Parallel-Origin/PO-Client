// /************************************************************
// *                                                           *
// *   Mobile Touch Camera                                     *
// *                                                           *
// *   Created 2015 by BitBender Games                         *
// *                                                           *
// *   bitbendergames@gmail.com                                *
// *                                                           *
// ************************************************************/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace BitBenderGames {

    public class DemoController : MonoBehaviour {

        [SerializeField]
        private Text textInfo;

        [SerializeField]
        private Text textDetail;

        private TouchInputController touchInputController;

        private MobileTouchCamera mobileTouchCamera;

        private MobilePickingController mobilePickingController;

        private Camera cam;

        private Coroutine coroutineHideInfoText;

        private Transform selectedPickableTransform;

        public void Awake() {
            cam = FindObjectOfType<Camera>();
            mobileTouchCamera = cam.GetComponent<MobileTouchCamera>();
            touchInputController = cam.GetComponent<TouchInputController>();
            mobilePickingController = cam.GetComponent<MobilePickingController>();

            #region detail callbacks
            touchInputController.OnInputClick += new TouchInputController.InputClickDelegate(OnInputClick);
            touchInputController.OnDragStart += new TouchInputController.Input1PositionDelegate(OnDragStart);
            touchInputController.OnDragStop += new TouchInputController.DragStopDelegate(OnDragStop);
            touchInputController.OnDragUpdate += new TouchInputController.DragUpdateDelegate(OnDragUpdate);
            touchInputController.OnFingerDown += new TouchInputController.Input1PositionDelegate(OnFingerDown);
            touchInputController.OnPinchStart += new TouchInputController.PinchStartDelegate(OnPinchStart);
            touchInputController.OnPinchStop += new System.Action(OnPinchStop);
            touchInputController.OnPinchUpdate += new TouchInputController.PinchUpdateDelegate(OnPinchUpdate);
            #endregion

            ShowInfoText("Mobile Touch Camera Demo\nSwipe: Scroll\nPinch: Zoom\nTap: Pick Item", 5);
        }

        public void OnPickItem(RaycastHit hitInfo) {
            Debug.Log("Picked a collider: " + hitInfo.collider);
            ShowInfoText("" + hitInfo.collider, 2);
        }

        public void OnPickItem2D(RaycastHit2D hitInfo2D) {
            Debug.Log("Picked a 2D collider: " + hitInfo2D.collider);
            ShowInfoText("" + hitInfo2D.collider, 2);
        }

        public void OnPickableTransformSelected(Transform pickableTransform) {
            if (pickableTransform != selectedPickableTransform) {
                StartCoroutine(AnimateScaleForSelection(pickableTransform));
            }
            foreach (var itemRenderer in pickableTransform.GetComponentsInChildren<Renderer>()) {
                itemRenderer.material.color = Color.green;
            }
            selectedPickableTransform = pickableTransform;
        }

        public void OnPickableTransformDeselected(Transform transform) {
            transform.localScale = Vector3.one;
            foreach (var itemRenderer in transform.GetComponentsInChildren<Renderer>()) {
                itemRenderer.material.color = Color.white;
            }
            selectedPickableTransform = null;
        }

        private IEnumerator AnimateScaleForSelection(Transform pickableTransform) {
            float timeAtStart = Time.time;
            const float animationDuration = 0.25f;
            while (Time.time < timeAtStart + animationDuration) {
                float progress = (Time.time - timeAtStart) / animationDuration;
                float scaleFactor = 1.0f + Mathf.Sin(progress * Mathf.PI) * 0.2f;
                pickableTransform.localScale = Vector3.one * scaleFactor;
                yield return null;
            }
            pickableTransform.localScale = Vector3.one;
        }

        public void SetCameraModeOrtho() {
            cam.orthographic = true;
            mobileTouchCamera.CamZoomMin = 4;
            mobileTouchCamera.CamZoomMax = 20;
            mobileTouchCamera.CamZoom = 7;
            mobileTouchCamera.CamOverzoomMargin = 1;
        }

        public void SetCameraModePerspective() {
            mobileTouchCamera.PerspectiveZoomMode = PerspectiveZoomMode.FIELD_OF_VIEW;
            cam.orthographic = false;
            mobileTouchCamera.CamZoomMin = 30;
            mobileTouchCamera.CamZoomMax = 60;
            mobileTouchCamera.CamZoom = 60;
            mobileTouchCamera.CamOverzoomMargin = 10;
        }

        public void SetCameraModePerspectiveTranslation() {
            mobileTouchCamera.PerspectiveZoomMode = PerspectiveZoomMode.TRANSLATION;
            cam.orthographic = false;
            mobileTouchCamera.CamZoomMin = 5;
            mobileTouchCamera.CamZoomMax = 40;
            mobileTouchCamera.CamZoom = 10;
            mobileTouchCamera.CamOverzoomMargin = 2;
            cam.fieldOfView = 60;
        }

        public void SetSnapAngleStraight() {
            mobilePickingController.SnapAngle = SnapAngle.Straight_0_Degrees;
        }

        public void SetSnapAngleDiagonal() {
            mobilePickingController.SnapAngle = SnapAngle.Diagonal_45_Degrees;
        }

        public void ToggleGameObjectActive(GameObject go) {
            go.SetActive(!go.activeInHierarchy);
        }

        public void ToggleCamAngle(bool angle) {
            mobilePickingController.SnapAngle = angle == true ? SnapAngle.Straight_0_Degrees : SnapAngle.Diagonal_45_Degrees;
        }

        public void SetInputOnLockedArea() {
            touchInputController.IsInputOnLockedArea = true;
        }

        private void ShowInfoText(string message, int onScreenTime) {
            textInfo.text = message;
            if (coroutineHideInfoText != null) {
                StopCoroutine(coroutineHideInfoText);
            }
            textInfo.enabled = true;
            coroutineHideInfoText = StartCoroutine(HideInfoText(onScreenTime));
        }

        private IEnumerator HideInfoText(int delay) {
            yield return new WaitForSeconds(delay);
            textInfo.enabled = false;
        }

        #region detail messages
        private void SetTextDetail(string message) {
            textDetail.text = message;
        }

        private void OnInputClick(Vector3 clickScreenPosition, bool isDoubleClick) {
            SetTextDetail("OnInputClick(clickScreenPosition: " + clickScreenPosition + ", isDoubleClick: " + isDoubleClick + ")");
        }

        private void OnPinchUpdate(Vector3 pinchCenter, float pinchDistance, float pinchStartDistance) {
            SetTextDetail("OnPinchUpdate(pinchCenter: " + pinchCenter + ", pinchDistance: " + pinchDistance + ", pinchStartDistance: " + pinchStartDistance + ")");
        }

        private void OnPinchStop() {
            SetTextDetail("OnPinchStop()");
        }

        private void OnPinchStart(Vector3 pinchCenter, float pinchDistance) {
            SetTextDetail("OnPinchStart(pinchCenter: " + pinchCenter + ", pinchDistance: " + pinchDistance + ")");
        }

        private void OnFingerDown(Vector3 screenPosition) {
            SetTextDetail("OnFingerDown(screenPosition: " + screenPosition + ")");
        }

        private void OnDragUpdate(Vector3 dragPosStart, Vector3 dragPosCurrent, Vector3 correctionOffset) {
            SetTextDetail("OnDragUpdate(dragPosStart: " + dragPosStart + ", dragPosCurrent: " + dragPosCurrent + ")");
        }

        private void OnDragStop(Vector3 dragStopPos, Vector3 dragFinalMomentum) {
            SetTextDetail("OnDragStop(dragStopPos: " + dragStopPos + ", dragFinalMomentum: " + dragFinalMomentum + ")");
        }

        private void OnDragStart(Vector3 pos) {
            SetTextDetail("OnDragStart(pos: " + pos + ")");
        }
        #endregion
    }
}
