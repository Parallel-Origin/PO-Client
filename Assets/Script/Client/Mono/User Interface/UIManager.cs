using System;
using System.Collections.Generic;
using RotaryHeart.Lib.SerializableDictionary;
using ParallelOrigin.Core.Base.Classes.Pattern.Registers;
using UnityEngine;

namespace Script.Client.Mono.User_Interface {
    
    //////////////////////////
    /// Internal structure
    //////////////////////////


    /// <summary>
    ///     A serialisable dictionary which stores a transform assigned to a key
    /// </summary>
    [Serializable]
    public class TransformDictionary : SerializableDictionaryBase<string, Transform> { }


    //////////////////////////
    /// UIManager
    //////////////////////////


    /// <summary>
    ///  A Component which stores active UI-Elements for easy querys
    ///  Furthermore its able to receive and redirect events to event-listeners.
    /// </summary>
    public class UIManager : MonoBehaviour {
        
        [SerializeField] private TransformDictionary spawns;
        
        [SerializeField] private List<IUIElement> _all = new List<IUIElement>();
        [SerializeField] private Dictionary<string, List<IUIElement>> _categorys = new Dictionary<string, List<IUIElement>>();

        [SerializeField] private IEventHandler _eventHandler;
        
        [SerializeField] private Action<IUIElement> _onNewCategory = element => { };
        [SerializeField] private Action<IUIElement> _onNewElement = element => { };
        [SerializeField] private Action<IUIElement> _onCategoryRemove = element => { };
        [SerializeField] private Action<IUIElement> _onElementRemoved = element => { };
        
        private void Awake() { ServiceLocator.Register(this); }

        private void Start() { _eventHandler = ServiceLocator.Get<IEventHandler>(); }


        ////////////////////////////
        /// Main
        ///////////////////////////


        /// <summary>
        ///     Adds a element to the UI-Management which groups and sorts them into the properties.
        /// </summary>
        /// <param name="element"></param>
        public void Add(IUIElement element) {
            
            All.Add(element);
            OnNewElement(element);

            if (Categorys.ContainsKey(element.AsEntity.ElementCategory))
                Categorys[element.AsEntity.ElementCategory].Add(element);
            else Categorys.Add(element.AsEntity.ElementCategory, new List<IUIElement> {element});
        }

        /// <summary>
        ///     Removes a element from the UI-Management, which results in a removal from the sortings and mappings.
        /// </summary>
        /// <param name="element"></param>
        public void Remove(IUIElement element) {
            
            All.Remove(element);
            OnElementRemoved(element);

            if (Categorys.ContainsKey(element.AsEntity.ElementCategory))
                Categorys[element.AsEntity.ElementCategory].Remove(element);
        }


        ////////////////////////////
        /// Properties
        ///////////////////////////


        /// <summary>
        ///     A reference to the global event handler
        /// </summary>
        public IEventHandler EventHandler {
            get => _eventHandler;
            set => _eventHandler = value;
        }

        /// <summary>
        ///     A list of all existing elements.
        /// </summary>
        public List<IUIElement> All {
            get => _all;
            set => _all = value;
        }

        /// <summary>
        ///     A list of all available spawn places for Elements
        /// </summary>
        public TransformDictionary Spawns {
            get => spawns;
            set => spawns = value;
        }

        /// <summary>
        ///     All active elements grouped by their category
        /// </summary>
        public Dictionary<string, List<IUIElement>> Categorys {
            get => _categorys;
            set => _categorys = value;
        }

        /// <summary>
        ///     Gets called, once a new element was added
        /// </summary>
        public Action<IUIElement> OnNewElement {
            get => _onNewElement;
            set => _onNewElement = value;
        }

        /// <summary>
        ///     Gets called, once a element was removed
        /// </summary>
        public Action<IUIElement> OnElementRemoved {
            get => _onElementRemoved;
            set => _onElementRemoved = value;
        }

        /// <summary>
        ///     Gets called, once a new category was established by a element
        /// </summary>
        public Action<IUIElement> OnNewCategory {
            get => _onNewCategory;
            set => _onNewCategory = value;
        }

        /// <summary>
        ///     Gets called, once a category was removed, there no elements of that category existant anymore
        /// </summary>
        public Action<IUIElement> OnCategoryRemove {
            get => _onCategoryRemove;
            set => _onCategoryRemove = value;
        }
    }
}