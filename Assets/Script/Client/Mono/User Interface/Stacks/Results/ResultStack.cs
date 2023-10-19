using System;
using ParallelOrigin.Core.Base.Classes;
using Script.Server;
using UnityEngine;

namespace Script.Client.Mono.User_Interface.Stacks.Results {
    
    /// <summary>
    ///     A class which is used to store UI-Results from dialogues, popups or similar in a stack on the very top of the hierarchie
    ///     Searches for another <see cref="ActionStack" /> inside our UI-Hierarchie and merges the instances
    ///     This prevents multiple single result stacks in the same hierarchie
    /// </summary>
    public class ResultStack : MonoBehaviour {
        
        [Tooltip("The UI-Element we use to identify the UI-Hierarchie for finding another Action-Stack")] 
        [SerializeField]
        private EventDictionary<string, string> _resultDictionary = new EventDictionary<string, string>();

        /// <summary>
        ///     The event stack we use to store our values
        /// </summary>
        public EventDictionary<string, string> EventDictionary {
            get => _resultDictionary;
            set => _resultDictionary = value;
        }
    }
}