using TMPro;
using UnityEngine;

namespace Script.Client.Mono.MVVM.Character {
    
    /// <summary>
    /// A view for the username which stores the username of an entity.
    /// Does not have any viewmodel because well... there doesnt exist that much logic and its only being set once. 
    /// </summary>
    public class NameView : MonoBehaviour {

        [SerializeField] private TextMeshProUGUI usernameField;

        /// <summary>
        /// The username field
        /// </summary>
        public TextMeshProUGUI UsernameField => usernameField;
    }
}