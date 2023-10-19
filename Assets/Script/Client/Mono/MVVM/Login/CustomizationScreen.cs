using System.Linq;
using Script.Client.Mono.Utils.Login;
using UnityEngine;
using UnityEngine.UI;

namespace Script.Client.Mono.MVVM.Login {
    
    /// <summary>
    /// This class manages the customization screen.
    /// </summary>
    public class CustomizationScreen : MonoBehaviour {

        [SerializeField] private ToggleGroup genders;
        [SerializeField] private ToggleGroup buildingColors;

        /// <summary>
        /// Proceeds to the following screen
        /// </summary>
        public void Proceed() {

            if (!genders.AnyTogglesOn()) {

                Debug.Log("<CustomizationScreen : No Gender selected>");
                return;
            }

            if (!buildingColors.AnyTogglesOn()) {

                Debug.Log("<CustomizationScreen : No Color selected>");
                return;
            }

            Toggle selectedGenderToogle = genders.ActiveToggles().SingleOrDefault();
            string gender = selectedGenderToogle.gameObject.transform.Find("ToggleName").GetComponent<Text>().text;

            Toggle selectedColor = buildingColors.ActiveToggles().SingleOrDefault();
            string color = "#" + ColorUtility.ToHtmlStringRGBA(selectedColor.targetGraphic.color);

            UserCredentials.SetGender(gender);
            UserCredentials.SetBuildingsColor(color);
        }
    }
}
