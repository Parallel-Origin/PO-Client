using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Script.Client.Mono.MVVM.Recipe {
    
    /// <summary>
    /// Represents a view of the recipe which stores references to all required ui elements.
    /// </summary>
    public class RecipeEntryView : MonoBehaviour {

        [SerializeField] private TextMeshProUGUI titleTextField;
        [SerializeField] private TextMeshProUGUI describtionTextField;
        
        [SerializeField] private Image icon;
        
        [SerializeField] private Button collapseButton;
        [SerializeField] private Button createButton;

        [SerializeField] private Transform ingredientsTransform;

        /// <summary>
        /// A textfield of the title of the recipe
        /// </summary>
        public TextMeshProUGUI TitleTextField {
            get => titleTextField;
            set => titleTextField = value;
        }

        /// <summary>
        /// A textfield of the describtion of the recipes purpose
        /// </summary>
        public TextMeshProUGUI DescribtionTextField {
            get => describtionTextField;
            set => describtionTextField = value;
        }

        /// <summary>
        /// A image reference for the recipes icon
        /// </summary>
        public Image Icon {
            get => icon;
            set => icon = value;
        }

        /// <summary>
        /// A button which should collapse the recipe on click
        /// </summary>
        public Button CollapseButton {
            get => collapseButton;
            set => collapseButton = value;
        }

        /// <summary>
        /// A button used to construct the recipe
        /// </summary>
        public Button CreateButton {
            get => createButton;
            set => createButton = value;
        }

        /// <summary>
        /// A transform in which the ingredients should spawn
        /// </summary>
        public Transform IngredientsTransform {
            get => ingredientsTransform;
            set => ingredientsTransform = value;
        }
    }
}