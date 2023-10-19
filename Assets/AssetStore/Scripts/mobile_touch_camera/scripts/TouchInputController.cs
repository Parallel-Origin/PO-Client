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

    public class TouchInputController : MonoBehaviour {

        public static TouchInputController instance;

        private float lastFingerDownTimeReal;
        private float lastClickTimeReal;
        private bool wasFingerDownLastFrame;
        private Vector3 lastFinger0DownPos;

        public const float clickDurationThreshold = 0.35f;
        public const float doubleclickDurationThreshold = 0.5f;
        private const float dragDurationThreshold = 0.01f;

        private const float dragStartDistanceThresholdRelative = 0.05f;
        public bool isDragging;
        private Vector3 dragStartPos;
        private Vector3 dragStartOffset;

        private List<Vector3> DragFinalMomentumVector { get; set; }
        private const int momentumSamplesCount = 5;

        private float pinchStartDistance;
        private List<Vector3> PinchStartPositions { get; set; }

        private bool wasDraggingLastFrame;
        private bool wasPinchingLastFrame;

        private bool isPinching;

        private bool isInputOnLockedArea = false;
        private bool isLocked = false;

        private float timeSinceDragStart = 0;

        public delegate void Input1PositionDelegate(Vector3 pos);

        public event Input1PositionDelegate OnDragStart;
        public event Input1PositionDelegate OnFingerDown;
        public event System.Action OnFingerUp;

        public delegate void DragUpdateDelegate(Vector3 dragPosStart, Vector3 dragPosCurrent, Vector3 correctionOffset);
        public event DragUpdateDelegate OnDragUpdate;

        public delegate void DragStopDelegate(Vector3 dragStopPos, Vector3 dragFinalMomentum);
        public event DragStopDelegate OnDragStop;

        public delegate void PinchStartDelegate(Vector3 pinchCenter, float pinchDistance);
        public event PinchStartDelegate OnPinchStart;

        public delegate void PinchUpdateDelegate(Vector3 pinchCenter, float pinchDistance, float pinchStartDistance);
        public event PinchUpdateDelegate OnPinchUpdate;

        public event System.Action OnPinchStop;

        private bool isClickPrevented;

        public delegate void InputClickDelegate(Vector3 clickPosition, bool isDoubleClick);
        public event InputClickDelegate OnInputClick;

        private bool isFingerDown;

        public bool IsInputOnLockedArea {
            get { return isInputOnLockedArea; }
            set { isInputOnLockedArea = value; }
        }

        public void Awake() {
            lastFingerDownTimeReal = 0;
            lastClickTimeReal = 0;
            lastFinger0DownPos = Vector3.zero;
            dragStartPos = Vector3.zero;
            isDragging = false;
            wasFingerDownLastFrame = false;
            DragFinalMomentumVector = new List<Vector3>();
            PinchStartPositions = new List<Vector3>();
            PinchStartPositions.Add(Vector3.zero);
            PinchStartPositions.Add(Vector3.zero);
            pinchStartDistance = 1;
            isPinching = false;
            isClickPrevented = false;

            instance = this;
        }

        public void OnEventTriggerPointerDown(BaseEventData baseEventData) {
            isInputOnLockedArea = true;
        }

        public void OnEventLock(bool locked) {
            isLocked = locked;
        }

        public void Update() {

            if (TouchWrapper.IsFingerDown == false) {
                isInputOnLockedArea = false;
            }

            bool pinchToDragCurrentFrame = false;

            if (isLocked)
                return;

            if (isInputOnLockedArea == false) {

                #region pinch
                if (isPinching == false) {
                    if (TouchWrapper.TouchCount == 2) {
                        StartPinch();
                        isPinching = true;
                    }
                }
                else {
                    if (TouchWrapper.TouchCount < 2) {
                        StopPinch();
                        isPinching = false;
                    }
                    else if (TouchWrapper.TouchCount == 2) {
                        UpdatePinch();
                    }
                }
                #endregion

                #region drag
                if (isPinching == false) {
                    if (wasPinchingLastFrame == false) {
                        if (wasFingerDownLastFrame == true && TouchWrapper.IsFingerDown) {
                            if (isDragging == false) {
                                Vector2 dragVector = TouchWrapper.Touch0.Position - dragStartPos;
                                float dragDistance = new Vector2(dragVector.x / Screen.width, dragVector.y / Screen.height).magnitude;
                                float dragTime = Time.realtimeSinceStartup - lastFingerDownTimeReal;
                                if ((dragDistance >= dragStartDistanceThresholdRelative && dragTime >= dragDurationThreshold)
                                 || dragTime > doubleclickDurationThreshold) {
                                    isDragging = true;
                                    dragStartOffset = lastFinger0DownPos - dragStartPos;
                                    dragStartPos = lastFinger0DownPos;
                                    DragStart(dragStartPos);
                                }
                            }
                        }
                    }
                    else {
                        if (TouchWrapper.IsFingerDown == true) {
                            isDragging = true;
                            dragStartPos = TouchWrapper.Touch0.Position;
                            DragStart(dragStartPos);
                            pinchToDragCurrentFrame = true;
                        }
                    }

                    if (isDragging == true && TouchWrapper.IsFingerDown == true) {
                        DragUpdate(TouchWrapper.Touch0.Position);
                    }

                    if (isDragging == true && TouchWrapper.IsFingerDown == false) {
                        isDragging = false;
                        DragStop(lastFinger0DownPos);
                    }
                }
                #endregion

                #region click
                if (isPinching == false && isDragging == false && wasPinchingLastFrame == false && wasDraggingLastFrame == false && isClickPrevented == false) {
                    if (wasFingerDownLastFrame == false && TouchWrapper.IsFingerDown) {
                        lastFingerDownTimeReal = Time.realtimeSinceStartup;
                        dragStartPos = TouchWrapper.Touch0.Position;
                        FingerDown(TouchWrapper.AverageTouchPos);
                    }

                    if (wasFingerDownLastFrame == true && TouchWrapper.IsFingerDown == false) {
                        float fingerDownUpDuration = Time.realtimeSinceStartup - lastFingerDownTimeReal;

                        if (fingerDownUpDuration < clickDurationThreshold) {
                            if (wasDraggingLastFrame == false && wasPinchingLastFrame == false) {
                                float clickDuration = Time.realtimeSinceStartup - lastClickTimeReal;

                                bool isDoubleClick = clickDuration < doubleclickDurationThreshold;

                                if (OnInputClick != null) {
                                    OnInputClick.Invoke(lastFinger0DownPos, isDoubleClick);
                                }

                                lastClickTimeReal = Time.realtimeSinceStartup;
                            }
                        }
                    }
                }
                #endregion

            }

            if (isDragging && TouchWrapper.IsFingerDown && pinchToDragCurrentFrame == false) {
                DragFinalMomentumVector.Add(TouchWrapper.Touch0.Position - lastFinger0DownPos);
                if (DragFinalMomentumVector.Count > momentumSamplesCount) {
                    DragFinalMomentumVector.RemoveAt(0);
                }
            }

            wasFingerDownLastFrame = TouchWrapper.IsFingerDown;
            if (wasFingerDownLastFrame == true) {
                lastFinger0DownPos = TouchWrapper.Touch0.Position;
            }

            wasDraggingLastFrame = isDragging;
            wasPinchingLastFrame = isPinching;

            if (TouchWrapper.TouchCount == 0) {
                isClickPrevented = false;
                if (isFingerDown == true) {
                    FingerUp();
                }
            }
        }

        private void StartPinch() {
            PinchStartPositions[0] = TouchWrapper.Touches[0].Position;
            PinchStartPositions[1] = TouchWrapper.Touches[1].Position;
            pinchStartDistance = GetPinchDistance(PinchStartPositions[0], PinchStartPositions[1]);
            if (OnPinchStart != null) {
                OnPinchStart.Invoke((PinchStartPositions[0] + PinchStartPositions[1]) * 0.5f, pinchStartDistance);
            }
            isClickPrevented = true;
        }

        private void UpdatePinch() {
            float pinchDistance = GetPinchDistance(TouchWrapper.Touches[0].Position, TouchWrapper.Touches[1].Position);
            if (OnPinchUpdate != null) {
                OnPinchUpdate.Invoke((TouchWrapper.Touches[0].Position + TouchWrapper.Touches[1].Position) * 0.5f, pinchDistance, pinchStartDistance);
            }
        }

        private float GetPinchDistance(Vector3 pos0, Vector3 pos1) {
            float distanceX = Mathf.Abs(TouchWrapper.Touches[0].Position.x - TouchWrapper.Touches[1].Position.x) / Screen.width;
            float distanceY = Mathf.Abs(TouchWrapper.Touches[0].Position.y - TouchWrapper.Touches[1].Position.y) / Screen.height;
            return (Mathf.Sqrt(distanceX * distanceX + distanceY * distanceY));
        }

        private void StopPinch() {
            dragStartOffset = Vector3.zero;
            if (OnPinchStop != null) {
                OnPinchStop.Invoke();
            }
        }

        private void DragStart(Vector3 pos) {
            if (OnDragStart != null) {
                OnDragStart(pos);
            }
            isClickPrevented = true;
            timeSinceDragStart = 0;
            DragFinalMomentumVector.Clear();
        }

        private void DragUpdate(Vector3 pos) {
            if (OnDragUpdate != null) {
                timeSinceDragStart += Time.deltaTime;
                Vector3 offset = Vector3.Lerp(Vector3.zero, dragStartOffset, Mathf.Clamp01(timeSinceDragStart * 10.0f));
                OnDragUpdate(dragStartPos, pos, offset);
            }
        }

        private void DragStop(Vector3 pos) {

            if (OnDragStop != null) {
                Vector3 momentum = Vector3.zero;
                if (DragFinalMomentumVector.Count > 0) {
                    for (int i = 0; i < DragFinalMomentumVector.Count; ++i) {
                        momentum += DragFinalMomentumVector[i];
                    }
                    momentum /= DragFinalMomentumVector.Count;
                }
                OnDragStop(pos, momentum);
            }

            DragFinalMomentumVector.Clear();
        }

        private void FingerDown(Vector3 pos) {
            isFingerDown = true;
            if (OnFingerDown != null) {
                OnFingerDown(pos);
            }
        }

        private void FingerUp() {
            isFingerDown = false;
            if (OnFingerUp != null) {
                OnFingerUp();
            }
        }
    }
}
