// /************************************************************
// *                                                           *
// *   Mobile Touch Camera                                     *
// *                                                           *
// *   Created 2015 by BitBender Games                         *
// *                                                           *
// *   bitbendergames@gmail.com                                *
// *                                                           *
// ************************************************************/

using System.Collections.Generic;
using UnityEngine;
using System.Collections;

namespace BitBenderGames {

    [RequireComponent(typeof(TouchInputController))]
    [RequireComponent(typeof(Camera))]
    public class MobileTouchCamera : MonoBehaviourWrapped {

        #region inspector
        [SerializeField]
        private CameraPlaneAxes cameraAxes = CameraPlaneAxes.XY_2D_SIDESCROLL;
        [SerializeField]
        [Tooltip("For perspective cameras this value denotes the min field of view used for zooming. For orthographic cameras it denotes the min camera size.")]
        private float camZoomMin = 4;
        [SerializeField]
        [Tooltip("For perspective cameras this value denotes the max field of view used for zooming. For orthographic cameras it denotes the max camera size.")]
        private float camZoomMax = 12;
        [SerializeField]
        [Tooltip("The cam will overzoom the min/max values by this amount and spring back when the user releases the zoom.")]
        private float camOverzoomMargin = 1;
        [SerializeField]
        [Tooltip("These values define the scrolling borders for the camera. The camera will not scroll further than defined here. When a top-down camera is used, these 2 values are applied to the X/Z position.")]
        private Vector2 boundaryMin = new Vector2(float.MinValue, float.MinValue);
        [SerializeField]
        [Tooltip("These values define the scrolling borders for the camera. The camera will not scroll further than defined here. When a top-down camera is used, these 2 values are applied to the X/Z position.")]
        private Vector2 boundaryMax = new Vector2(float.MaxValue, float.MaxValue);
        [Header("Advanced")]
        [SerializeField]
        [Tooltip("The lower the value, the slower the camera will follow. The higher the value, the more direct the camera will follow movement updates. Necessary for keeping the camera smooth when the framerate is not in sync with the touch input update rate.")]
        private float camFollowFactor = 15.0f;
        [SerializeField]
        [Tooltip("When dragging quickly, the camera will keep autoscrolling in the last direction. The autoscrolling will slowly come to a halt. This value defines how fast the camera will come to a halt.")]
        private float autoScrollDamp = 300;
        [SerializeField]
        [Tooltip("This value only needs to be changed when your ground level is not at 0. E.g. In case your ground level is not at y = 0 for top-down cameras or at z = 0 for side-scrolling cameras you need to adjust this value to the proper ground level.")]
        private float groundLevelOffset = 0;
        [SerializeField]
        [Tooltip("When using a perspective camera, the zoom can either be performed by changing the field of view, or by moving the camera closer to the scene.")]
        private PerspectiveZoomMode perspectiveZoomMode = PerspectiveZoomMode.FIELD_OF_VIEW;
        [Header("Event Callbacks")]
        [SerializeField]
        [Tooltip("Here you can set up callbacks to be invoked when an item with Collider is tapped on.")]
        private UnityEventWithRaycastHit OnPickItem;
        [SerializeField]
        [Tooltip("Here you can set up callbacks to be invoked when an item with Collider2D is tapped on.")]
        private UnityEventWithRaycastHit2D OnPickItem2D;
        [SerializeField]
        [Tooltip("Here you can set up callbacks to be invoked when an item with Collider is double-tapped on.")]
        private UnityEventWithRaycastHit OnPickItemDoubleClick;
        [SerializeField]
        [Tooltip("Here you can set up callbacks to be invoked when an item with Collider2D is double-tapped on.")]
        private UnityEventWithRaycastHit2D OnPickItem2DDoubleClick;
        #endregion

        public CameraPlaneAxes CameraAxes {
            get { return (cameraAxes); }
            set { cameraAxes = value; }
        }

        private TouchInputController touchInputController;

        private Vector3 dragStartCamPos;
        private Vector3 cameraScrollVelocity;

        private float pinchStartCamZoomSize;
        private Vector3 pinchStartIntersectionCenter;
        private Vector3 pinchCenterCurrent;
        private float pinchDistanceCurrent;
        private float pinchDistanceStart;
        private Vector3 pinchCenterCurrentLerp;
        private float pinchDistanceCurrentLerp;

        private float timeRealDragStop;

        public bool IsAutoScrolling { get { return (cameraScrollVelocity.sqrMagnitude > float.Epsilon); } }

        public bool IsPinching { get; private set; }
        public bool IsDragging { get; private set; }

        private const float zoomBackSpringFactor = 20;
        private const float autoScrollVelocityMax = 60;

        private bool isStarted = false;

        public Camera Cam { get; private set; }

        private bool IsTranslationZoom { get { return (Cam.orthographic == false && perspectiveZoomMode == PerspectiveZoomMode.TRANSLATION); } }

        public float CamZoom {
            get {
                if (Cam.orthographic == true) {
                    return Cam.orthographicSize;
                }
                else {
                    if (IsTranslationZoom == true) {
                        Vector3 camCenterIntersection = GetIntersectionPoint(Cam.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0)));
                        return (Vector3.Distance(camCenterIntersection, Transform.position));
                    }
                    else {
                        return Cam.fieldOfView;
                    }
                }
            }
            set {
                if (Cam.orthographic == true) {
                    Cam.orthographicSize = value;
                }
                else {
                    if (IsTranslationZoom == true) {
                        Vector3 camCenterIntersection = GetIntersectionPoint(Cam.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0)));
                        Transform.position = camCenterIntersection - Transform.forward * value;
                    }
                    else {
                        Cam.fieldOfView = value;
                    }
                }
                ComputeCamBoundaries();
            }
        }

        public float CamZoomMin {
            get { return (camZoomMin); }
            set { camZoomMin = value; }
        }
        public float CamZoomMax {
            get { return (camZoomMax); }
            set { camZoomMax = value; }
        }
        public float CamOverzoomMargin {
            get { return (camOverzoomMargin); }
            set { camOverzoomMargin = value; }
        }
        public float CamFollowFactor {
            get { return (camFollowFactor); }
            set { camFollowFactor = value; }
        }
        public float AutoScrollDamp {
            get { return autoScrollDamp; }
            set { autoScrollDamp = value; }
        }
        public Vector2 BoundaryMin {
            get { return boundaryMin; }
            set { boundaryMin = value; }
        }
        public Vector2 BoundaryMax {
            get { return boundaryMax; }
            set { boundaryMax = value; }
        }
        public PerspectiveZoomMode PerspectiveZoomMode {
            get { return (perspectiveZoomMode); }
            set { perspectiveZoomMode = value; }
        }

        private bool isDraggingSceneObject;

        private Plane refPlaneXY = new Plane(new Vector3(0, 0, -1), 0);
        private Plane refPlaneXZ = new Plane(new Vector3(0, 1, 0), 0);
        public Plane RefPlane {
            get {
                if (CameraAxes == CameraPlaneAxes.XZ_TOP_DOWN) {
                    return refPlaneXZ;
                }
                else {
                    return refPlaneXY;
                }
            }
        }

        private List<Vector3> DragCameraMoveVector { get; set; }
        private const int momentumSamplesCount = 5;

        private Vector3 targetPositionClamped = Vector3.zero;

        public bool IsSmoothingEnabled { get; set; }

        private float ScreenRatio { get; set; }

        private Vector2 CamPosMin { get; set; }
        private Vector2 CamPosMax { get; set; }

        public void Awake() {
            Cam = GetComponent<Camera>();

            IsSmoothingEnabled = true;
            touchInputController = GetComponent<TouchInputController>();
            dragStartCamPos = Vector3.zero;
            cameraScrollVelocity = Vector3.zero;
            timeRealDragStop = 0;
            pinchStartCamZoomSize = 0;
            IsPinching = false;
            IsDragging = false;
            DragCameraMoveVector = new List<Vector3>();
            refPlaneXY = new Plane(new Vector3(0, 0, -1), groundLevelOffset);
            refPlaneXZ = new Plane(new Vector3(0, 1, 0), -groundLevelOffset);
            ScreenRatio = GetScreenRatio();
            ComputeCamBoundaries();

            if (CamZoomMax < CamZoomMin) {
                Debug.LogWarning("The defined max camera zoom (" + CamZoomMax + ") is smaller than the defined min (" + CamZoomMin + "). Automatically switching the values.");
                float camZoomMinBackup = CamZoomMin;
                CamZoomMin = CamZoomMax;
                CamZoomMax = camZoomMinBackup;
            }

            //Errors for certain incorrect settings.
            if (transform.forward == Vector3.down && cameraAxes != CameraPlaneAxes.XZ_TOP_DOWN) {
                Debug.LogError("Camera is pointing down but the cameraAxes is not set to TOP_DOWN. Make sure to set the cameraAxes variable properly.");
            }
            if (transform.forward == Vector3.forward && cameraAxes != CameraPlaneAxes.XY_2D_SIDESCROLL) {
                Debug.LogError("Camera is pointing sidewards but the cameraAxes is not set to 2D_SIDESCROLL. Make sure to set the cameraAxes variable properly.");
            }
        }

        public void Start() {
            touchInputController.OnInputClick += InputControllerOnInputClick;
            touchInputController.OnDragStart += InputControllerOnDragStart;
            touchInputController.OnDragUpdate += InputControllerOnDragUpdate;
            touchInputController.OnDragStop += InputControllerOnDragStop;
            touchInputController.OnFingerDown += InputControllerOnFingerDown;
            touchInputController.OnFingerUp += InputControllerOnFingerUp;
            touchInputController.OnPinchStart += InputControllerOnPinchStart;
            touchInputController.OnPinchUpdate += InputControllerOnPinchUpdate;
            touchInputController.OnPinchStop += InputControllerOnPinchStop;
            isStarted = true;
        }

        public void OnDestroy() {
            if (isStarted) {
                touchInputController.OnInputClick -= InputControllerOnInputClick;
                touchInputController.OnDragStart -= InputControllerOnDragStart;
                touchInputController.OnDragUpdate -= InputControllerOnDragUpdate;
                touchInputController.OnDragStop -= InputControllerOnDragStop;
                touchInputController.OnFingerDown -= InputControllerOnFingerDown;
                touchInputController.OnFingerUp -= InputControllerOnFingerUp;
                touchInputController.OnPinchStart -= InputControllerOnPinchStart;
                touchInputController.OnPinchUpdate -= InputControllerOnPinchUpdate;
                touchInputController.OnPinchStop -= InputControllerOnPinchStop;
            }
        }

        public Vector3 GetIntersectionPoint(Ray ray) {
            float distance = 0;
            bool success = RefPlane.Raycast(ray, out distance);
            if (success == false) {
                Debug.LogError("Failed to compute intersection between camera ray and reference plane. Make sure the camera Axes are set up correctly.");
            }
            return (ray.origin + ray.direction * distance);
        }

        /// <summary>
        /// Custom planet intersection method that doesn't take into account rays parallel to the plane or rays shooting in the wrong direction and thus never hitting.
        /// May yield slightly better performance however and should be safe for use when the camera setup is correct (e.g. axes set correctly in this script, and camera actually pointing towards floor).
        /// </summary>
        public Vector3 GetIntersectionPointUnsafe(Ray ray) {
            float distance = Vector3.Dot(RefPlane.normal, Vector3.zero - ray.origin) / Vector3.Dot(RefPlane.normal, (ray.origin + ray.direction) - ray.origin);
            return (ray.origin + ray.direction * distance);
        }

        private void UpdateZoom(float deltaTime) {

            if (IsPinching == true) {

                if (IsSmoothingEnabled == true) {
                    pinchDistanceCurrentLerp = Mathf.Lerp(pinchDistanceCurrentLerp, pinchDistanceCurrent, Mathf.Clamp01(Time.deltaTime * camFollowFactor));
                    pinchCenterCurrentLerp = Vector3.Lerp(pinchCenterCurrentLerp, pinchCenterCurrent, Mathf.Clamp01(Time.deltaTime * camFollowFactor));
                }
                else {
                    pinchDistanceCurrentLerp = pinchDistanceCurrent;
                    pinchCenterCurrentLerp = pinchCenterCurrent;
                }

                float cameraSize = pinchStartCamZoomSize * (pinchDistanceStart / Mathf.Max(pinchDistanceCurrentLerp, 0.0001f));
                cameraSize = Mathf.Clamp(cameraSize, camZoomMin - camOverzoomMargin, camZoomMax + camOverzoomMargin);
                CamZoom = cameraSize;

                //Position update.
                Vector3 intersectionDragCurrent = GetIntersectionPoint(Cam.ScreenPointToRay(pinchCenterCurrentLerp));
                Vector3 dragUpdateVector = intersectionDragCurrent - pinchStartIntersectionCenter;
                Vector3 targetPos = GetClampToBoundaries(Transform.position - dragUpdateVector);

                Transform.position = targetPos; //Disable smooth follow for the pinch-move update to prevent oscillation during the zoom phase.
                SetTargetPosition(targetPos);
            }
        }

        private void UpdatePosition(float deltaTime) {

            if (IsDragging == true || IsPinching == true) {
                Vector3 posOld = Transform.position;
                if (IsSmoothingEnabled == true) {
                    Transform.position = Vector3.Lerp(Transform.position, targetPositionClamped, Mathf.Clamp01(Time.deltaTime * camFollowFactor));
                }
                else {
                    Transform.position = targetPositionClamped;
                }
                DragCameraMoveVector.Add((posOld - Transform.position) / Time.deltaTime);
                if (DragCameraMoveVector.Count > momentumSamplesCount) {
                    DragCameraMoveVector.RemoveAt(0);
                }
            }

            Vector2 autoScrollVector = -cameraScrollVelocity * deltaTime;
            Vector3 camPos = Transform.position;
            switch (cameraAxes) {
                case CameraPlaneAxes.XY_2D_SIDESCROLL:
                    camPos.x += autoScrollVector.x;
                    camPos.y += autoScrollVector.y;
                    break;
                case CameraPlaneAxes.XZ_TOP_DOWN:
                    camPos.x += autoScrollVector.x;
                    camPos.z += autoScrollVector.y;
                    break;
            }
            Transform.position = GetClampToBoundaries(camPos);
        }

        private void SetTargetPosition(Vector3 newPositionClamped) {
            targetPositionClamped = newPositionClamped;
        }

        private bool GetIsBoundaryPosition(Vector3 testPosition) {

            bool isBoundaryPosition = false;
            switch (cameraAxes) {
                case CameraPlaneAxes.XY_2D_SIDESCROLL:
                    isBoundaryPosition = testPosition.x <= CamPosMin.x;
                    isBoundaryPosition |= testPosition.x >= CamPosMax.x;
                    isBoundaryPosition |= testPosition.y <= CamPosMin.y;
                    isBoundaryPosition |= testPosition.y >= CamPosMax.y;
                    break;
                case CameraPlaneAxes.XZ_TOP_DOWN:
                    isBoundaryPosition = testPosition.x <= CamPosMin.x;
                    isBoundaryPosition |= testPosition.x >= CamPosMax.x;
                    isBoundaryPosition |= testPosition.z <= CamPosMin.y;
                    isBoundaryPosition |= testPosition.z >= CamPosMax.y;
                    break;
            }
            return (isBoundaryPosition);
        }

        private Vector3 GetClampToBoundaries(Vector3 newPosition) {

            switch (cameraAxes) {
                case CameraPlaneAxes.XY_2D_SIDESCROLL:
                    newPosition.x = Mathf.Clamp(newPosition.x, CamPosMin.x, CamPosMax.x);
                    newPosition.y = Mathf.Clamp(newPosition.y, CamPosMin.y, CamPosMax.y);
                    break;
                case CameraPlaneAxes.XZ_TOP_DOWN:
                    newPosition.x = Mathf.Clamp(newPosition.x, CamPosMin.x, CamPosMax.x);
                    newPosition.z = Mathf.Clamp(newPosition.z, CamPosMin.y, CamPosMax.y);
                    break;
            }
            return (newPosition);
        }

        private void ComputeCamBoundaries() {
            if (Cam.orthographic == true) {
                float camHalfHeight = Cam.orthographicSize;
                float camHalfWidth = Cam.orthographicSize * ScreenRatio;
                CamPosMin = new Vector2(boundaryMin.x + camHalfWidth, boundaryMin.y + camHalfHeight);
                CamPosMax = new Vector2(boundaryMax.x - camHalfWidth, boundaryMax.y - camHalfHeight);
            }
            else {
                Vector2 camProjectedMin = Vector2.zero;
                Vector2 camProjectedMax = Vector2.zero;
                Vector2 camProjectedCenter = Vector2.zero;
                camProjectedMin.x = GetIntersectionPoint(Cam.ScreenPointToRay(new Vector3(0, Screen.height * 0.5f, 0))).x;
                camProjectedMax.x = GetIntersectionPoint(Cam.ScreenPointToRay(new Vector3(Screen.width, Screen.height * 0.5f, 0))).x;
                camProjectedCenter.x = GetIntersectionPoint(Cam.ScreenPointToRay(new Vector3(0.5f * (float)Screen.width, 0.5f * (float)Screen.height, 0))).x;
                switch (cameraAxes) {
                    case CameraPlaneAxes.XY_2D_SIDESCROLL:
                        camProjectedMin.y = GetIntersectionPoint(Cam.ScreenPointToRay(new Vector3(Screen.width * 0.5f, 0, 0))).y;
                        camProjectedMax.y = GetIntersectionPoint(Cam.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height, 0))).y;
                        camProjectedCenter.y = GetIntersectionPoint(Cam.ScreenPointToRay(new Vector3(0.5f * (float)Screen.width, 0.5f * (float)Screen.height, 0))).y;
                        break;
                    case CameraPlaneAxes.XZ_TOP_DOWN:
                        camProjectedMin.y = GetIntersectionPoint(Cam.ScreenPointToRay(new Vector3(Screen.width * 0.5f, 0, 0))).z;
                        camProjectedMax.y = GetIntersectionPoint(Cam.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height, 0))).z;
                        camProjectedCenter.y = GetIntersectionPoint(Cam.ScreenPointToRay(new Vector3(0.5f * (float)Screen.width, 0.5f * (float)Screen.height, 0))).z;
                        break;
                }
                CamPosMin = new Vector2(boundaryMin.x + (camProjectedCenter.x - camProjectedMin.x), boundaryMin.y + (camProjectedCenter.y - camProjectedMin.y));
                CamPosMax = new Vector2(boundaryMax.x - (camProjectedMax.x - camProjectedCenter.x), boundaryMax.y - (camProjectedMax.y - camProjectedCenter.y));
            }

            if (CamPosMax.x < CamPosMin.x) {
                float midPoint = (CamPosMax.x + CamPosMin.x) * 0.5f;
                CamPosMax = new Vector2(midPoint, CamPosMax.y);
                CamPosMin = new Vector2(midPoint, CamPosMin.y);
            }
            if (CamPosMax.y < CamPosMin.y) {
                float midPoint = (CamPosMax.y + CamPosMin.y) * 0.5f;
                CamPosMax = new Vector2(CamPosMax.x, midPoint);
                CamPosMin = new Vector2(CamPosMin.x, midPoint);
            }
        }

        private void SetClampToBoundaries() {
            Transform.position = GetClampToBoundaries(Transform.position);
        }

        public void LateUpdate() {

            //Zoom.
            UpdateZoom(Time.deltaTime);

            //Translation.
            UpdatePosition(Time.deltaTime);

            #region editor codepath
#if UNITY_EDITOR
            //Allow to use the middle mouse wheel in editor to be able to zoom without touch device during development.
            float mouseScrollDelta = Input.GetAxis("Mouse ScrollWheel");
            if (Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftControl)) {
                if (Input.GetKeyDown(KeyCode.KeypadPlus)) {
                    mouseScrollDelta = 0.25f;
                }
                else if (Input.GetKeyDown(KeyCode.KeypadMinus)) {
                    mouseScrollDelta = -0.25f;
                }
            }
            if (Mathf.Approximately(mouseScrollDelta, 0) == false) {
                float editorZoomFactor = 15;
                if (Cam.orthographic) {
                    editorZoomFactor = 15;
                }
                else {
                    if (IsTranslationZoom) {
                        editorZoomFactor = 30;
                    }
                    else {
                        editorZoomFactor = 100;
                    }
                }
                float zoomAmount = mouseScrollDelta * editorZoomFactor;
                float camSizeDiff = DoEditorCameraZoom(zoomAmount);
                Vector3 intersectionScreenCenter = GetIntersectionPoint(Cam.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0)));
                Vector3 pinchFocusVector = GetIntersectionPoint(Cam.ScreenPointToRay(Input.mousePosition)) - intersectionScreenCenter;
                float multiplier = (1.0f / CamZoom * camSizeDiff);
                Transform.position += pinchFocusVector * multiplier;
            }
            for (int i = 0; i < 3; ++i) {
                if (Input.GetKeyDown((KeyCode)((int)KeyCode.Alpha1 + i))) {
                    StartCoroutine(ZoomToTargetValueCoroutine(Mathf.Lerp(CamZoomMin, CamZoomMax, (float)i / 2.0f)));
                }
            }
#endif
            #endregion

            //When the camera is zoomed in further than the defined normal value, it will snap back to normal using the code below.
            if (IsPinching == false && IsDragging == false) {
                float camZoomDeltaToNormal = 0;
                if (CamZoom > camZoomMax) {
                    camZoomDeltaToNormal = CamZoom - camZoomMax;
                }
                else if (CamZoom < camZoomMin) {
                    camZoomDeltaToNormal = CamZoom - camZoomMin;
                }

                if (Mathf.Approximately(camZoomDeltaToNormal, 0) == false) {
                    float cameraSizeCorrection = Mathf.Lerp(0, camZoomDeltaToNormal, zoomBackSpringFactor * Time.deltaTime);
                    if (Mathf.Abs(cameraSizeCorrection) > Mathf.Abs(camZoomDeltaToNormal)) {
                        cameraSizeCorrection = camZoomDeltaToNormal;
                    }
                    CamZoom -= cameraSizeCorrection;
                }
            }

            var v3 = new Vector3(Input.GetAxis("Vertical"), Input.GetAxis("Horizontal"), 0);
            Camera.main.transform.Rotate(v3 * 25f * Time.deltaTime);

        }

        private float DoEditorCameraZoom(float amount) {
            float newCamZoom = CamZoom - amount;
            newCamZoom = Mathf.Clamp(newCamZoom, camZoomMin, camZoomMax);
            float camSizeDiff = CamZoom - newCamZoom;
            CamZoom = newCamZoom;
            return (camSizeDiff);
        }

        public void FixedUpdate() {

            ScreenRatio = GetScreenRatio();

            if (cameraScrollVelocity.sqrMagnitude > float.Epsilon) {
                float timeSinceDragStop = Time.realtimeSinceStartup - timeRealDragStop;
                float dampFactor = Mathf.Clamp01(timeSinceDragStop * 2);
                Vector3 camVelDamp = dampFactor * cameraScrollVelocity.normalized * autoScrollDamp * Time.fixedDeltaTime;
                if (camVelDamp.sqrMagnitude >= cameraScrollVelocity.sqrMagnitude) {
                    cameraScrollVelocity = Vector3.zero;
                }
                else {
                    cameraScrollVelocity -= camVelDamp;
                }
            }
        }

        private void InputControllerOnFingerDown(Vector3 pos) {
            cameraScrollVelocity = Vector3.zero;
        }

        private void InputControllerOnFingerUp() {
            isDraggingSceneObject = false;
        }

        private Vector3 GetDragVector(Vector3 dragPosStart, Vector3 dragPosCurrent) {
            Vector3 intersectionDragStart = GetIntersectionPoint(Cam.ScreenPointToRay(dragPosStart));
            Vector3 intersectionDragCurrent = GetIntersectionPoint(Cam.ScreenPointToRay(dragPosCurrent));
            return (intersectionDragCurrent - intersectionDragStart);
        }

        private Vector3 GetVelocityFromMoveHistory() {
            Vector3 momentum = Vector3.zero;
            if (DragCameraMoveVector.Count > 0) {
                for (int i = 0; i < DragCameraMoveVector.Count; ++i) {
                    momentum += DragCameraMoveVector[i];
                }
                momentum /= DragCameraMoveVector.Count;
            }
            if (CameraAxes == CameraPlaneAxes.XZ_TOP_DOWN) {
                momentum.y = momentum.z;
                momentum.z = 0;
            }
            return (momentum);
        }

        private void InputControllerOnDragStart(Vector3 dragPosStart) {
            if (isDraggingSceneObject == false) {
                cameraScrollVelocity = Vector3.zero;
                dragStartCamPos = Transform.position;
                IsDragging = true;
                DragCameraMoveVector.Clear();
                SetTargetPosition(Transform.position);
            }
        }

        private void InputControllerOnDragUpdate(Vector3 dragPosStart, Vector3 dragPosCurrent, Vector3 correctionOffset) {
            if (isDraggingSceneObject == false) {
                Vector3 dragVector = GetDragVector(dragPosStart, dragPosCurrent + correctionOffset);
                Vector3 posNewClamped = GetClampToBoundaries(dragStartCamPos - dragVector);
                SetTargetPosition(posNewClamped);
            }
        }

        private void InputControllerOnDragStop(Vector3 dragStopPos, Vector3 dragFinalMomentum) {
            if (isDraggingSceneObject == false) {
                cameraScrollVelocity = GetVelocityFromMoveHistory();
                if (cameraScrollVelocity.sqrMagnitude >= autoScrollVelocityMax * autoScrollVelocityMax) {
                    cameraScrollVelocity = cameraScrollVelocity.normalized * autoScrollVelocityMax;
                }
                timeRealDragStop = Time.realtimeSinceStartup;
                IsDragging = false;
                DragCameraMoveVector.Clear();
            }
        }

        private void InputControllerOnPinchStart(Vector3 pinchCenter, float pinchDistance) {

            pinchStartCamZoomSize = CamZoom;
            pinchStartIntersectionCenter = GetIntersectionPoint(Cam.ScreenPointToRay(pinchCenter));

            pinchCenterCurrent = pinchCenter;
            pinchDistanceCurrent = pinchDistance;
            pinchDistanceStart = pinchDistance;

            pinchCenterCurrentLerp = pinchCenter;
            pinchDistanceCurrentLerp = pinchDistance;

            SetTargetPosition(Transform.position);
            IsPinching = true;
        }

        private void InputControllerOnPinchUpdate(Vector3 pinchCenter, float pinchDistance, float pinchStartDistance) {
            pinchCenterCurrent = pinchCenter;
            pinchDistanceCurrent = pinchDistance;
            //Actual zoom computation is located in the LateUpdate method.
        }

        private void InputControllerOnPinchStop() {
            IsPinching = false;
            DragCameraMoveVector.Clear();
        }

        private void InputControllerOnInputClick(Vector3 clickPosition, bool isDoubleClick) {
            Ray camRay = Cam.ScreenPointToRay(clickPosition);
            if (OnPickItem != null || OnPickItemDoubleClick != null) {
                RaycastHit hitInfo;
                if (Physics.Raycast(camRay, out hitInfo) == true) {
                    if (OnPickItem != null) {
                        OnPickItem.Invoke(hitInfo);
                    }
                    if (isDoubleClick == true) {
                        if (OnPickItemDoubleClick != null) {
                            OnPickItemDoubleClick.Invoke(hitInfo);
                        }
                    }
                }
            }
            if (OnPickItem2D != null || OnPickItem2DDoubleClick != null) {
                RaycastHit2D hitInfo2D = Physics2D.Raycast(camRay.origin, camRay.direction);
                if (hitInfo2D == true) {
                    if (OnPickItem2D != null) {
                        OnPickItem2D.Invoke(hitInfo2D);
                    }
                    if (isDoubleClick == true) {
                        if (OnPickItem2DDoubleClick != null) {
                            OnPickItem2DDoubleClick.Invoke(hitInfo2D);
                        }
                    }
                }
            }
        }

        public void OnDragSceneObject() {
            isDraggingSceneObject = true;
        }

        private float GetScreenRatio() {
            return ((float)Screen.width / (float)Screen.height);
        }

        private IEnumerator ZoomToTargetValueCoroutine(float target) {

            if (Mathf.Approximately(target, CamZoom) == false) {
                float startValue = CamZoom;
                const float duration = 0.3f;
                float timeStart = Time.time;
                while (Time.time < timeStart + duration) {
                    float progress = (Time.time - timeStart) / duration;
                    CamZoom = Mathf.Lerp(startValue, target, Mathf.Sin(-Mathf.PI * 0.5f + progress * Mathf.PI) * 0.5f + 0.5f);
                    yield return null;
                }
                CamZoom = target;
            }
        }

    }
}
