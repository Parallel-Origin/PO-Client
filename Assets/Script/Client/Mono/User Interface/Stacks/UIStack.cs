using System;
using ParallelOrigin.Core.Base.Classes;
using UnityEngine;
using UnityEngine.Events;

namespace Script.Client.Mono.User_Interface.Stacks {
    
    /// <summary>
    ///  A class which acts as a stack for multiple layers of UI-Elements above each other
    ///  Automaticly takes care of hiding the previous elements and makes sure that each added element has a canvas added
    ///   Typical examples are screen flows...
    /// </summary>
    public class UIStack : MonoBehaviour {
        
        [SerializeField] private UnityEvent onNewVisible;
        [SerializeField] private UnityEvent allInvisible;

        [SerializeField] private EventStack<string, GameObject> _elementStack = new EventStack<string, GameObject>();
        
        [SerializeField] private Action<string, GameObject> _onInvisible = (key, gm) => { };
        [SerializeField] private Action<string, GameObject> _onVisible = (key, gm) => { };

        private void Awake() {
            
            // Setting up our event stack and hook up our inspector events
            _elementStack.OnFirst += OnNewVisible.Invoke;

            // When a new screen gets opened, toogle that screen and invoke certain events
            _elementStack.OnPush += (key, element) => {
                
                var canvas = element.GetComponent<Canvas>() == null ? element.AddComponent<Canvas>() : element.GetComponent<Canvas>();
                canvas.enabled = true;
                OnVisible.Invoke(key, element);
            };

            // When the current screen gets poped, close it and call certain events
            _elementStack.OnPop += (key, element) => {

                // In case the element was deleted
                if (!element) return;
                
                var canvas = element.GetComponent<Canvas>() == null ? element.AddComponent<Canvas>() : element.GetComponent<Canvas>();
                canvas.enabled = false;
                OnInvisible.Invoke(key, element);
            };

            _elementStack.OnEmpty += AllInvisible.Invoke;
        }

        /// <summary>
        ///     Shows a certain canvas and puts it on top of the stack
        /// </summary>
        /// <param name="id"></param>
        /// <param name="canvas"></param>
        public void Show(string id, GameObject canvas) { EventStack.Push(new Tuple<string, GameObject>(id, canvas)); }

        /// <summary>
        ///     Shows the previous canvas in the stack
        /// </summary>
        public void Back() { EventStack.Pop(); }

        /// <summary>
        ///     Hides all canvas and clears the stack
        /// </summary>
        public void HideAll() { EventStack.Clear(); }

        /// <summary>
        ///     Hides all canvas, except the first and clears all of them
        /// </summary>
        public void HideAllExceptFirst() {
            foreach (var canvas in EventStack.ElementStack) EventStack.Pop();
        }

        /// <summary>
        ///     The stack we are using, a event stack where we can register certain events
        /// </summary>
        public EventStack<string, GameObject> EventStack {
            get => _elementStack;
            set => _elementStack = value;
        }

        /// <summary>
        ///     Gets called when the stack was empty, and a new element gets added
        /// </summary>
        public UnityEvent OnNewVisible {
            get => onNewVisible;
            set => onNewVisible = value;
        }

        /// <summary>
        ///     Gets called, when a new element got visible
        /// </summary>
        public Action<string, GameObject> OnVisible {
            get => _onVisible;
            set => _onVisible = value;
        }

        /// <summary>
        ///     Gets called, when a element gets invisible
        /// </summary>
        public Action<string, GameObject> OnInvisible {
            get => _onInvisible;
            set => _onInvisible = value;
        }

        /// <summary>
        ///     Gets called, when all elements are invisible and the stack is empty
        /// </summary>
        public UnityEvent AllInvisible {
            get => allInvisible;
            set => allInvisible = value;
        }
    }
}