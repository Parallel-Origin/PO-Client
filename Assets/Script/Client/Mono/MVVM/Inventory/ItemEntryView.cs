using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Script.Client.Mono.MVVM.Inventory {
    
    /// <summary>
    /// A component that represents the ItemEntry view and stores references to all important UI elements.
    /// </summary>
    public class ItemEntryView : MonoBehaviour {

        [SerializeField] private TextMeshProUGUI nameField;
        [SerializeField] private Image icon;

        /// <summary>
        /// The Item-Name Field
        /// </summary>
        public TextMeshProUGUI NameField {
            get => nameField;
            set => nameField = value;
        }

        /// <summary>
        /// The item Icon
        /// </summary>
        public Image Icon {
            get => icon;
            set => icon = value;
        }
    }
}