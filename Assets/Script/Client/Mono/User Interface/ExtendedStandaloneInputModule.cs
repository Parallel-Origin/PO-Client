using UnityEngine.EventSystems;

namespace Script.Client.Mono.User_Interface {
    
    /// <summary>
    ///     A better version of the <see cref="StandaloneInputModule" /> which features the possibility to receive the pointer events of clicks and touches
    /// </summary>
    public class ExtendedStandaloneInputModule : StandaloneInputModule {
        
        private static ExtendedStandaloneInputModule _instance;

        protected override void Awake() {
            base.Awake();
            _instance = this;
        }

        public static PointerEventData GetPointerEventData(int pointerId = -1) {
            PointerEventData eventData;
            _instance.GetPointerData(pointerId, out eventData, true);
            return eventData;
        }
    }
}