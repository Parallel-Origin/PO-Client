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
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace BitBenderGames {

    [RequireComponent(typeof(MobileTouchCamera))]
    public class MobilePickingController : MonoBehaviour {

        public enum SelectionAction {
            Select,
            Deselect,
        }

        #region inspector
        [SerializeField]
        [Tooltip("When set to true, the position of dragged items snaps to discrete units.")]
        private bool snapToGrid = true;
        [SerializeField]
        [Tooltip("Size of the snap units when snapToGrid is enabled.")]
        private float snapUnitSize = 1;
        [SerializeField]
        [Tooltip("When snapping is enabled, this value defines a position offset that is added to the center of the object when dragging. When a top-down camera is used, these 2 values are applied to the X/Z position.")]
        private Vector2 snapOffset = Vector2.zero;
        [SerializeField]
        [Tooltip("When set to Straight, picked items will be snapped to a perfectly horizontal and vertical grid in world space. Diagonal snaps the items on a 45 degree grid.")]
        private SnapAngle snapAngle = SnapAngle.Straight_0_Degrees;
        [Header("Event Callbacks")]
        [SerializeField]
        [Tooltip("Here you can set up callbacks to be invoked when a pickable transform is selected.")]
        private UnityEventWithTransform OnPickableTransformSelected;
        [SerializeField]
        [Tooltip("Here you can set up callbacks to be invoked when a pickable transform is deselected.")]
        private UnityEventWithTransform OnPickableTransformDeselected;
        [SerializeField]
        [Tooltip("Here you can set up callbacks to be invoked when a pickable transform is moved.")]
        private UnityEventWithTransform OnPickableTransformMoved;
        #endregion

        private TouchInputController touchInputController;

        private MobileTouchCamera mobileTouchCam;

        private Component SelectedCollider { get; set; }

        private MobileTouchPickable CurrentlyDraggedPickable { get; set; }

        private Transform CurrentlyDraggedTransform {
            get {
                if (CurrentlyDraggedPickable != null) {
                    return (CurrentlyDraggedPickable.PickableTransform);
                }
                else {
                    return null;
                }
            }
        }

        private Vector3 DraggedTransformOffset { get; set; }

        public bool SnapToGrid {
            get { return snapToGrid; }
            set { snapToGrid = value; }
        }

        public SnapAngle SnapAngle {
            get { return (snapAngle); }
            set { snapAngle = value; }
        }

        public float SnapUnitSize { get { return (snapUnitSize); } }

        public Vector2 SnapOffset { get { return (snapOffset); } }

        public const float snapAngleDiagonal = 45 * Mathf.Deg2Rad;

        private Vector3 currentlyDraggedTransformPosition = Vector3.zero;

        private const float transformMovedDistanceThreshold = 0.001f;

        public void Awake() {
            mobileTouchCam = FindObjectOfType<MobileTouchCamera>();
            if (mobileTouchCam == null) {
                Debug.LogError("No MobileTouchCamera found in scene. This script will not work without this.");
            }
            touchInputController = mobileTouchCam.GetComponent<TouchInputController>();
            if (touchInputController == null) {
                Debug.LogError("No TouchInputController found in scene. Make sure this component exists and is attached to the MobileTouchCamera gameObject.");
            }
        }

        public void Start() {
            touchInputController.OnInputClick += InputControllerOnInputClick;
            touchInputController.OnFingerDown += InputControllerOnFingerDown;
            touchInputController.OnDragUpdate += InputControllerOnDragUpdate;
            touchInputController.OnDragStop += InputControllerOnDragStop;
        }

        public void OnDestroy() {
            touchInputController.OnInputClick -= InputControllerOnInputClick;
            touchInputController.OnFingerDown -= InputControllerOnFingerDown;
            touchInputController.OnDragUpdate -= InputControllerOnDragUpdate;
            touchInputController.OnDragStop -= InputControllerOnDragStop;
        }

        private void InputControllerOnInputClick(Vector3 clickPosition, bool isDoubleClick) {

            Component previouslySelectedCollider = SelectedCollider;
            SelectedCollider = GetClosestColliderAtScreenPoint(clickPosition);

            if (previouslySelectedCollider != null && previouslySelectedCollider != SelectedCollider) {
                OnSelectedColliderChanged(SelectionAction.Deselect, previouslySelectedCollider);
            }

            if (SelectedCollider != null) {
                OnSelectedColliderChanged(SelectionAction.Select, SelectedCollider);
            }
        }

        public void DeselectSelectedCollider() {
            if (SelectedCollider != null) {
                OnSelectedColliderChanged(SelectionAction.Deselect, SelectedCollider);
                SelectedCollider = null;
            }
        }

        private Component GetClosestColliderAtScreenPoint(Vector3 screenPoint) {

            Component hitCollider = null;
            float hitDistance = float.MaxValue;
            Ray camRay = mobileTouchCam.Cam.ScreenPointToRay(screenPoint);
            RaycastHit hitInfo;
            if (Physics.Raycast(camRay, out hitInfo) == true) {
                hitDistance = hitInfo.distance;
                hitCollider = hitInfo.collider;
            }
            RaycastHit2D hitInfo2D = Physics2D.Raycast(camRay.origin, camRay.direction);
            if (hitInfo2D == true) {
                if (hitInfo2D.distance < hitDistance) {
                    hitCollider = hitInfo2D.collider;
                }
            }
            return (hitCollider);
        }

        private void InputControllerOnFingerDown(Vector3 fingerDownPos) {
            CurrentlyDraggedPickable = null;
            bool isDragStartedOnSelection = SelectedCollider != null && GetClosestColliderAtScreenPoint(fingerDownPos) == SelectedCollider;
            if (isDragStartedOnSelection == true) {
                MobileTouchPickable mobileTouchPickable = SelectedCollider.GetComponent<MobileTouchPickable>();
                if (mobileTouchPickable != null) {
                    mobileTouchCam.OnDragSceneObject(); //Lock camera movement.
                    CurrentlyDraggedPickable = mobileTouchPickable;
                    currentlyDraggedTransformPosition = CurrentlyDraggedTransform.position;

                    DraggedTransformOffset = Vector3.zero;
                    if (mobileTouchCam.Cam.orthographic == true && snapToGrid == false) {
                        DraggedTransformOffset = CurrentlyDraggedTransform.position - ComputeDragPositionWorld(fingerDownPos);
                    }
                }
            }
        }

        private void InputControllerOnDragUpdate(Vector3 dragPosStart, Vector3 dragPosCurrent, Vector3 correctionOffset) {
            if (CurrentlyDraggedTransform != null) {
                Vector3 dragPosWorld = ComputeDragPositionWorld(dragPosCurrent);
                CurrentlyDraggedTransform.position = dragPosWorld + DraggedTransformOffset;
                if (Vector3.Distance(CurrentlyDraggedTransform.position, currentlyDraggedTransformPosition) > transformMovedDistanceThreshold) {
                    InvokeSelectionSafe(OnPickableTransformMoved, CurrentlyDraggedTransform);
                }
                currentlyDraggedTransformPosition = CurrentlyDraggedTransform.position;
            }
        }

        private Vector3 ComputeDragPositionWorld(Vector3 dragPositionScreen) {
            Vector3 dragPosWorld = Vector3.zero;
            Ray dragRay = mobileTouchCam.Cam.ScreenPointToRay(dragPositionScreen);
            Plane referencePlane = mobileTouchCam.RefPlane;
            float dragRayRefDistance = 0;
            if (referencePlane.Raycast(dragRay, out dragRayRefDistance)) {
                dragPosWorld = dragRay.origin + dragRay.direction * dragRayRefDistance;
            }
            dragPosWorld = ClampDragPosition(CurrentlyDraggedPickable, dragPosWorld);
            return (dragPosWorld);
        }

        private Vector3 ClampDragPosition(MobileTouchPickable draggedPickable, Vector3 position) {
            var draggedTransform = draggedPickable.PickableTransform;
            if (mobileTouchCam.CameraAxes == CameraPlaneAxes.XY_2D_SIDESCROLL) {
                if (snapAngle == SnapAngle.Diagonal_45_Degrees) {
                    RotateVector2(ref position.x, ref position.y, -snapAngleDiagonal);
                }
                position.x = GetPositionSnapped(position.x, draggedPickable.LocalSnapOffset.x + snapOffset.x);
                position.y = GetPositionSnapped(position.y, draggedPickable.LocalSnapOffset.y + snapOffset.y);
                position.z = draggedTransform.position.z;
                if (snapAngle == SnapAngle.Diagonal_45_Degrees) {
                    RotateVector2(ref position.x, ref position.y, snapAngleDiagonal);
                }
            }
            else {
                if (snapAngle == SnapAngle.Diagonal_45_Degrees) {
                    RotateVector2(ref position.x, ref position.z, -snapAngleDiagonal);
                }
                position.x = GetPositionSnapped(position.x, draggedPickable.LocalSnapOffset.x + snapOffset.x);
                position.y = draggedTransform.position.y;
                position.z = GetPositionSnapped(position.z, draggedPickable.LocalSnapOffset.y + snapOffset.y);
                if (snapAngle == SnapAngle.Diagonal_45_Degrees) {
                    RotateVector2(ref position.x, ref position.z, snapAngleDiagonal);
                }
            }
            return (position);
        }

        private void RotateVector2(ref float x, ref float y, float degrees) {
            if (Mathf.Approximately(degrees, 0)) {
                return;
            }
            float newX = x * Mathf.Cos(degrees) - y * Mathf.Sin(degrees);
            float newY = x * Mathf.Sin(degrees) + y * Mathf.Cos(degrees);
            x = newX;
            y = newY;
        }

        private float GetPositionSnapped(float position, float snapOffset) {
            if (snapToGrid == true) {
                return (Mathf.RoundToInt(position / snapUnitSize) * snapUnitSize) + snapOffset;
            }
            else {
                return (position);
            }
        }

        private void InputControllerOnDragStop(Vector3 dragStopPos, Vector3 dragFinalMomentum) {
            CurrentlyDraggedPickable = null;
        }

        private void OnSelectedColliderChanged(SelectionAction selectionAction, Component selectionCollider) {
            var mobileTouchPickable = selectionCollider.GetComponent<MobileTouchPickable>();
            if (mobileTouchPickable != null) {
                if (selectionAction == SelectionAction.Select) {
                    InvokeSelectionSafe(OnPickableTransformSelected, mobileTouchPickable.PickableTransform);
                }
                else if (selectionAction == SelectionAction.Deselect) {
                    InvokeSelectionSafe(OnPickableTransformDeselected, mobileTouchPickable.PickableTransform);
                }
            }
        }

        private void InvokeSelectionSafe(UnityEventWithTransform eventAction, Transform selectionTransform) {
            if (eventAction != null) {
                eventAction.Invoke(selectionTransform);
            }
        }
    }
}
