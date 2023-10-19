using UnityEngine;
using UnityEngine.UI;

namespace Script.Client.Mono.MVVM.PopUp {
    
    /// <summary>
    /// A view storing all views for the typical quit popup. 
    /// </summary>
    public class QuitPopupView : MonoBehaviour {

        [SerializeField] private Button cancelButton;
        [SerializeField] private Button confirmlButton;

        /// <summary>
        /// The cancel button
        /// </summary>
        public Button CancelButton {
            get => cancelButton;
            set => cancelButton = value;
        }

        /// <summary>
        /// The confirm button
        /// </summary>
        public Button ConfirmlButton {
            get => confirmlButton;
            set => confirmlButton = value;
        }
    }
}