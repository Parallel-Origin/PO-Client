using System;
using ParallelOrigin.Core.Base.Classes.Pattern.Registers;
using Script.Server;
using UnityEngine;

namespace Script.Client.Mono.MVVM.PopUp {
    
    /// <summary>
    /// A view model for the typical quit application popup
    /// </summary>
    [RequireComponent(typeof(QuitPopupView))]
    public class QuitPopupViewModel : MonoBehaviour {

        private void Start() {
            View = GetComponent<QuitPopupView>(); 
            View.CancelButton.onClick.AddListener(OnCancel);
            View.ConfirmlButton.onClick.AddListener(OnConfirm);
        }

        /// <summary>
        /// Gets called on cancel and destroyes the popup
        /// </summary>
        private void OnCancel() {
            Destroy(gameObject);
        }

        /// <summary>
        /// Gets called upon confirming the action and quits the application
        /// </summary>
        private void OnConfirm() {
            Application.Quit();
        }
        
        public QuitPopupView View { get; set; }
    }
}