using UnityEngine;
using UnityEngine.UI;

namespace Script.Client.Mono.Utils.GameObjects {
    [RequireComponent(typeof(Button))]
    public class DestroyOnButtonClick : MonoBehaviour {
        public Button button;
        public GameObject reference;

        private void Start() {
            button = GetComponent<Button>();

            button.onClick.AddListener(() => { Destroy(reference); });
        }
    }
}