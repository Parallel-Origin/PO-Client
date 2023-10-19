using ParallelOrigin.Core.Base.Classes.Pattern.Registers;
using Script.Client.Mono.Entity;
using UnityEngine;

namespace Script.Client.Mono.User_Interface {
    
    /// <summary>
    ///     A interface for a object which represents a element that belongs to a <see cref="UIManagement" />
    /// </summary>
    public interface IUIElement {
        
        /// <summary>
        ///     The <see cref="UIManagement" /> this Element belongs to.
        /// </summary>
        UIManager UIManagement { get; set; }

        /// <summary>
        ///     The element of this UIElement
        /// </summary>
        IEntity AsEntity { get; set; }
    }
    
    /// <summary>
    ///  A UI-Element which purpose is to interact with the global <see cref="UIManagement" /> for receiving and sending UI-Events.
    ///  It is used to establish a UI-Hierarchie or Tree for tracking down important elements
    /// </summary>
    [RequireComponent(typeof(Entity.Entity))]
    public class UIElement : MonoBehaviour, IUIElement {
        
        [SerializeField] private IEntity _asEntity;
        [SerializeField] private UIManager uiManagement;

        private void Awake() {
            
            // Register this as a Element inside our UI-Management
            AsEntity = GetComponent<IEntity>() == null ? gameObject.AddComponent<Entity.Entity>() : GetComponent<IEntity>();
        }

        /// <summary>
        ///     Add this UI-Element to our UI-Management
        /// </summary>
        private void Start() {
            UIManagement = ServiceLocator.Get<UIManager>();
            UIManagement.Add(this);
        }

        /// <summary>
        ///     Destroy this UI-Element, remove it from the UI-Management and remove it from the owner as a child
        /// </summary>
        private void OnDestroy() {
            
            UIManagement.Remove(this);

            var parent = AsEntity.AsChild.Parent;
            if (parent == null || parent.GetComponent<IUIElement>() == null) return;
            
            var entity = parent.GetComponent<IUIElement>().AsEntity;
            entity.AsParent.Childs.Remove(gameObject);
        }

        /// <summary>
        ///     The element of this UIElement
        /// </summary>
        public IEntity AsEntity {
            get => _asEntity;
            set => _asEntity = value;
        }

        /// <summary>
        ///     The <see cref="UIManagement" /> this Element belongs to.
        /// </summary>
        public UIManager UIManagement {
            get => uiManagement;
            set => uiManagement = value;
        }
    }
}