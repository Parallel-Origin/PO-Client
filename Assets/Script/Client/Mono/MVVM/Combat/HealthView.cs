using UnityEngine;
using UnityEngine.UI;

namespace Script.Client.Mono.MVVM.Combat {
    
    /// <summary>
    /// A class storing all health related UI elements for the <see cref="HealthViewModel"/>
    /// </summary>
    public class HealthView : MonoBehaviour {


        [SerializeField] private Slider healthSlider;

        /// <summary>
        /// The health slider
        /// </summary>
        public Slider HealthSlider {
            get => healthSlider;
            set => healthSlider = value;
        }
    }
}