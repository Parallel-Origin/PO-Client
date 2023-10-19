using Script.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace Script.Client.Mono.User_Interface.Components {
    
    /// <summary>
    ///     A component used to simulate the collapsing of UI-Elements.
    /// </summary>
    public class UICollapse : MonoBehaviour {

        [SerializeField] private Transform parent;
        [SerializeField] private Transform searchFrom;

        /// <summary>
        ///     Collapses the referenced parent item, sets its child to inactive and updates the <see cref="VerticalLayoutGroup" />
        /// </summary>
        public void Collapse() {
            
            foreach (Transform child in parent) {
                var childGo = child.gameObject;
                childGo.SetActive(!child.gameObject.activeSelf);
            }

            //Update size
            Canvas.ForceUpdateCanvases();

            var entry = parent.gameObject.GetParentWithComponent<VerticalLayoutGroup>();
            if (entry == null) return;

            entry.enabled = false;
            entry.enabled = true;

            // Update content size fitter
            var contentSizeFitter = searchFrom.gameObject.GetParentWithComponent<ContentSizeFitter>();
            if (contentSizeFitter == null) return;
            
            var rectTransformOfEntry = contentSizeFitter.GetComponent<RectTransform>();
            //LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransformOfEntry);
        }
    }
}