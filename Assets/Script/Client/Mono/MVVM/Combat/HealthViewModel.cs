using UnityEngine;

namespace Script.Client.Mono.MVVM.Combat {
    
    /// <summary>
    ///     A Mono-Component which stores a reference to the healthbar of a <see cref="MonoEntity" />
    ///     Provides several methods for interacting and changing that bar.
    ///     Gets controlled/updated by the data-structure and the ecs.
    /// </summary>
    [RequireComponent(typeof(HealthView))]
    public class HealthViewModel : MonoBehaviour {
        
        private void Awake() { View = GetComponent<HealthView>(); }
        
        /// <summary>
        ///     Sets the bar value to a fixed value
        /// </summary>
        /// <param name="value"></param>
        public void Set(float value) {
            View.HealthSlider.value = value;
        }

        /// <summary>
        ///     Sets the current and the max value of the healthbar.
        /// </summary>
        /// <param name="current">The current value</param>
        /// <param name="max">The maximum value</param>
        public void SetHealth(float current, float max) {
            View.HealthSlider.value = current;
            View.HealthSlider.maxValue = max;
        }

        public HealthView View { get; set; }
    }
}