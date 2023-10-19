using TMPro;
using UnityEngine;

namespace Script.Client.Mono.MVVM.PopUp {
    
    /// <summary>
    /// A component which acts as a view and stores references to all important ui elements for a normal popup.
    /// </summary>
    public class PopUpView : MonoBehaviour {

        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private Transform optionTransform;

        /// <summary>
        /// A textfield acting as the title of the popup
        /// </summary>
        public TextMeshProUGUI Title {
            get => title;
            set => title = value;
        }

        /// <summary>
        /// A transform in which we spawn in the different transforms.
        /// </summary>
        public Transform OptionTransform {
            get => optionTransform;
            set => optionTransform = value;
        }
    }
}