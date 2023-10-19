using UnityEngine;

namespace Script.Client.Mono.MVVM.Item {
    
    /// <summary>
    /// A class which acts as a view for a item on the groun and stores the most important references to its UI elements. 
    /// </summary>
    public class ItemView : MonoBehaviour {

        [SerializeField] private SpriteRenderer spriteRenderer;

        public SpriteRenderer SpriteRenderer {
            get => spriteRenderer;
            set => spriteRenderer = value;
        }
    }
}