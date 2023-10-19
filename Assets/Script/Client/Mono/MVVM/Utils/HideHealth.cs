using Script.Client.Mono.MVVM.Combat;
using UnityEngine;
using UnityEngine.Events;

namespace Script.Client.Mono.MVVM.Utils {

    /// <summary>
    /// A component which works hand in hand with the <see cref="HealthViewModel"/> to hide the health ui for certain conditions. 
    /// </summary>
    [RequireComponent(typeof(HealthViewModel))]
    public class HideHealth : MonoBehaviour {

        [SerializeField] private bool hideWhenFull;
        [SerializeField] private CanvasGroup canvasGroup;

        [SerializeField] private UnityEvent onHide;
        [SerializeField] private UnityEvent onShow;

        private bool _hidden = true;

        private void Awake() { HealthViewModel = GetComponent<HealthViewModel>(); }

        private void LateUpdate() {

            var slider = HealthViewModel.View.HealthSlider;
            if (hideWhenFull && slider.maxValue - slider.value <= 0.1f) {
                
                canvasGroup.alpha = 0;
                if(!_hidden) onHide.Invoke();
                _hidden = true;
            }
            else {
                canvasGroup.alpha = 1;
                if(_hidden) onShow.Invoke();
                _hidden = false;
            }
        }

        public HealthViewModel HealthViewModel { get; set; }
    }
}