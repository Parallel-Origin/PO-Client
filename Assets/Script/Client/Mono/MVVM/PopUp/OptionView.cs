using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Script.Client.Mono.MVVM.PopUp {
    
    /// <summary>
    /// A script that represents the option view and contains references to the most important ui elements on that option.
    /// </summary>
    public class OptionView : MonoBehaviour {

        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private Button optionButton;

        /// <summary>
        /// The title of the option
        /// </summary>
        public TextMeshProUGUI Title {
            get => title;
            set => title = value;
        }

        /// <summary>
        /// The button of the option which can get pressed
        /// </summary>
        public Button OptionButton {
            get => optionButton;
            set => optionButton = value;
        }
    }
}