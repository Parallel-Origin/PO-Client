using TMPro;
using UnityEngine;

namespace Script.Client.Mono.Utils.Inputfields {
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class ShowCharLimit : MonoBehaviour {
        public TextMeshProUGUI textField;
        public TMP_InputField inputField;

        private void Start() {
            textField = GetComponent<TextMeshProUGUI>();
            textField.SetText(inputField.characterLimit.ToString());

            inputField.onValueChanged.AddListener(value => { textField.SetText((inputField.characterLimit - value.Length).ToString()); });
        }
    }
}