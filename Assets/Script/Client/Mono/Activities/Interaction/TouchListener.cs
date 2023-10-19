using System;
using System.Collections;
using System.Collections.Generic;
using ParallelOrigin.Core.Base.Classes.Pattern.Registers;
using Script.Extensions;
using UnityEngine;

namespace Script.Client.Mono.Activities.Interaction {

    /// <summary>
    /// A comparer for sorting <see cref="RaycastHit"/>'s based on their allowance and null value
    /// </summary>
    public struct HitComparer : IComparer<RaycastHit> {

        public List<Func<RaycastHit, bool>> TapConditions;

        public HitComparer(List<Func<RaycastHit, bool>> onTapAllowed) { this.TapConditions = onTapAllowed; }

        public int Compare(RaycastHit hit1, RaycastHit hit2) {
            
            // Hits with collider == null to the back
            if (hit1.collider == null && hit1.collider == null) {
                return 0;
            }
            if (hit1.collider == null) {
                return 1;
            }
            if (hit2.collider == null) {
                return -1;
            }

            // Actuall comparison
            return Convert.ToInt32(TapConditions.Evaluate(ref hit1)) - Convert.ToInt32(TapConditions.Evaluate(ref hit2));
        }
    }

    /// <summary>
    /// This class manages the user touch interaction. It listens for touches, taps or drags and calls a different set of method to make the player move or interact with his surrounding.
    /// </summary>
    public class TouchListener : MonoBehaviour {

        #region Variables
        
        [SerializeField] private float doubleInteractionDelay = 0.25f;
        [SerializeField] private bool hold = true;
        
        [SerializeField] private Action<RaycastHit> _onTap = hit => { };
        [SerializeField] private Action<int, RaycastHit[]> _onTaps = (size, hits) => { };
        [SerializeField] private Action<RaycastHit> _onDoubleTap = hit => { };

        // Buffers no non allocating memory each tap
        private RaycastHit[] _singleTapHits = new RaycastHit[50]; 
        private RaycastHit[] _doubleTapHits = new RaycastHit[50]; 
        
        #endregion

        #region Main Methods

        //----------------------------------------------------------
        // Main Methods
        //----------------------------------------------------------

        private void Awake() { ServiceLocator.Register(this); }

        private void Update() {
            
            if (hold)
                if (Input.GetMouseButtonUp(0))
                    DoubleTap();

            if (!Input.GetMouseButtonUp(0)) return;
            
            SingleTap();
            hold = true;
            StartCoroutine(Waiting(doubleInteractionDelay));
        }

        /// <summary>
        ///     Makes sure that theres a delay between each double tap.
        /// </summary>
        /// <param name="waitTime"></param>
        /// <returns></returns>
        protected IEnumerator Waiting(float waitTime) {
            yield return new WaitForSeconds(waitTime);
            hold = false;
        }

        #endregion
        
        #region Single Click/Touch Methods

        //----------------------------------------------------------
        // Single Click/Touch Methods
        //----------------------------------------------------------

        /// <summary>
        ///     This method gets called by the double/single tap process. It determines if a clicked object is of an certain type by calling "checkForObject()".
        /// </summary>
        public void SingleTap() {
            
            // Ray cast and sort array to first allowed hits, then non allowed hits
            var size = Physics.RaycastNonAlloc(Camera.main.ScreenPointToRay(Input.mousePosition), _singleTapHits, Mathf.Infinity);
            Array.Sort(_singleTapHits, new HitComparer(TapConditions));

            // Cound amount of allowed hits & invoke callbacks already 
            var allowedSize = 0;
            for (var index = 0; index < size; index++) {

                ref var hit = ref _singleTapHits[index];
                if (!TapConditions.Evaluate(ref hit)) break;
                
                OnTap(hit);
                allowedSize++;
            }

            if(allowedSize > 0)
                OnTaps(allowedSize, _singleTapHits);
        }

        #endregion

        #region Double Click/Touch Methods

        //----------------------------------------------------------
        // Double Click/Touch Methods
        //----------------------------------------------------------


        /// <summary>
        ///     Gets called by the local method "Update". It converts the screenpoint to a world point and makes the MovementManager move the character towards it. Futhermore it checks if the touched point is
        ///     within the MovementArea.
        /// </summary>
        public void DoubleTap() {
            
            var size = Physics.RaycastNonAlloc(Camera.main.ScreenPointToRay(Input.mousePosition), _doubleTapHits, Mathf.Infinity);
            for (var index = 0; index < size; index++) {

                ref var hit = ref _doubleTapHits[index];
                if (DoubleTapConditions.Evaluate(ref hit))
                    OnDoubleTap(hit);
            }
        }

        #endregion

        /// <summary>
        ///     The delay of two double touches after another
        /// </summary>
        public float DoubleInteractionDelay {
            get => doubleInteractionDelay;
            set => doubleInteractionDelay = value;
        }

        /// <summary>
        ///     True if we disable touches
        /// </summary>
        public bool Hold {
            get => hold;
            set => hold = value;
        }

        /// <summary>
        ///     Gets fired once a touch occurs to determine if that double touch was legitimate and should trigger <see cref="OnTap" />
        /// </summary>
        public List<Func<RaycastHit, bool>> TapConditions { get; } = new List<Func<RaycastHit, bool>>(8);

        /// <summary>
        ///     A event getting called on a single touch
        /// </summary>
        public Action<RaycastHit> OnTap {
            get => _onTap;
            set => _onTap = value;
        }
        
        /// <summary>
        ///     A event getting called on a single touch
        /// </summary>
        public Action<int, RaycastHit[]> OnTaps {
            get => _onTaps;
            set => _onTaps = value;
        }

        /// <summary>
        ///     Gets fired once a double touch occurs to determine if that double touch was legitimate and should trigger <see cref="OnDoubleTap" />
        /// </summary>
        public List<Func<RaycastHit, bool>> DoubleTapConditions { get; } = new List<Func<RaycastHit, bool>>(8);

        /// <summary>
        ///     A event getting called on a double touch.
        /// </summary>
        public Action<RaycastHit> OnDoubleTap {
            get => _onDoubleTap;
            set => _onDoubleTap = value;
        }
    }
}