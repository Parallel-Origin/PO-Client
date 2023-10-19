using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Script.Client.Mono.MVVM.Recipe {
    
    /// <summary>
    /// Represents the view of a recipe ingredient UI and all its important UI elements.
    /// </summary>
    public class RecipeIngredientView : MonoBehaviour{

        [SerializeField] private TextMeshProUGUI nameTextField;
        [SerializeField] private Image icon;

        /// <summary>
        /// The textfield of the name of the ingredient.
        /// </summary>
        public TextMeshProUGUI NameTextField {
            get => nameTextField;
            set => nameTextField = value;
        }

        /// <summary>
        /// A image which represents the ingredients icon.
        /// </summary>
        public Image Icon {
            get => icon;
            set => icon = value;
        }
    }
}