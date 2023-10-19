using ParallelOrigin.Core.Base.Classes.Pattern.Registers;
using Script.Client.Mono.User_Interface.Stacks;
using UnityEngine;

namespace Script.Client.Mono.User_Interface {
    /// <summary>
    ///     Works hand in hand with the <see cref="UIManager" /> for listening to events that spawn in <see cref="UIField" />
    ///     Notifies its <see cref="UIStack" /> for showing and hiding fields properly
    /// </summary>
    [RequireComponent(typeof(UIStack))]
    public class FieldManager : MonoBehaviour {
        
        [SerializeField] private UIStack stack;
        [SerializeField] private UIManager uiManagement;

        private void Awake() {
            ServiceLocator.Register(this);
            stack = GetComponent<UIStack>();
        }

        private void Start() {
            
            uiManagement = ServiceLocator.Get<UIManager>();
            uiManagement.OnNewElement += element => {
                if (element.AsEntity.ElementCategory.Equals("field"))
                    OnFieldSpawn(element);
            };

            uiManagement.OnElementRemoved += element => {
                if (element.AsEntity.ElementCategory.Equals("field"))
                    OnFieldDestroy(element);
            };
        }

        /// <summary>
        ///     Gets called, once a <see cref="UIField" /> was spawned via the <see cref="UIManager" />
        /// </summary>
        /// <param name="field"></param>
        protected void OnFieldSpawn(IUIElement field) { stack.Show("", field.AsEntity.GameObject); }

        /// <summary>
        ///     Gets called, once a <see cref="UIField" /> was destroyed via the <see cref="UIManager" />
        /// </summary>
        /// <param name="field"></param>
        protected void OnFieldDestroy(IUIElement field) { stack.Back(); }

        public UIStack Stack {
            get => stack;
            set => stack = value;
        }

        public UIManager UIManagement {
            get => uiManagement;
            set => uiManagement = value;
        }
    }
}