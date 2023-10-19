using ParallelOrigin.Core.Base.Classes.Pattern.Registers;
using Script.Client.Mono.User_Interface;
using Script.Client.Mono.User_Interface.Screens;
using UnityEngine;

namespace Script.Client.Mono.Activities.Interaction {
    
    /// <summary>
    ///  A script that hooks into <see cref="ITouchListener" /> and prevents it from executing click events if the <see cref="IGameScreenManager" /> has game screens open.
    /// </summary>
    [RequireComponent(typeof(TouchListener))]
    public class NoTouchWithOpenScreen : MonoBehaviour {
        
        [SerializeField] private GameScreenManager gameScreenManager;
        [SerializeField] private FieldManager fieldManager;
        [SerializeField] private TouchListener touchListener;

        private void Awake() {
            touchListener = GetComponent<TouchListener>();
            ServiceLocator.Wait<GameScreenManager>(o => gameScreenManager = (GameScreenManager) o);
            ServiceLocator.Wait<FieldManager>(o => fieldManager = (FieldManager)o);
        }

        private void Start() {
            touchListener.TapConditions.Add(ray => gameScreenManager.UIStack.EventStack.IsEmpty() && fieldManager.Stack.EventStack.IsEmpty());
            touchListener.DoubleTapConditions.Add(ray => gameScreenManager.UIStack.EventStack.IsEmpty());
        }
    }
}