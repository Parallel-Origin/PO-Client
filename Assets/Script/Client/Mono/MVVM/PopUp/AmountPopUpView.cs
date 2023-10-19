using TMPro;
using UnityEngine;

namespace Script.Client.Mono.MVVM.PopUp {
    
    /// <summary>
    /// A component which acts as a view and stores references to all important ui elements for a normal amount popup.
    /// </summary>
    public class AmountPopUpView : MonoBehaviour {

        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private RectTransform confirm;
        [SerializeField] private RectTransform cancel;

        /// <summary>
        /// The title of the amount popup
        /// </summary>
        public TextMeshProUGUI Title {
            get => title;
            set => title = value;
        }

        /// <summary>
        /// The input field for the amount
        /// </summary>
        public TMP_InputField InputField {
            get => inputField;
            set => inputField = value;
        }

        /// <summary>
        /// The place for the confirm button we wanna spawn in
        /// </summary>
        public RectTransform Confirm {
            get => confirm;
            set => confirm = value;
        }

        /// <summary>
        /// The place for the cancel button we wanna spawn in
        /// </summary>
        public RectTransform Cancel {
            get => cancel;
            set => cancel = value;
        }
    }
}