using System;
using System.Collections.Generic;
using UnityEngine.Events;

namespace Script.Extensions {
    
    /// <summary>
    /// Extension Methods for the <see cref="UnityEvent"/>
    /// </summary>
    public static class UnityEventExtensionMethods {

        ///
        /// Gets all the actions in the event as an array of UnityAction.
        ///
        public static Action[] GetAllActions(this UnityEvent @event) {
            
            // Make a list of the actions that will be returned at the end.
            var actions = new List<Action>();
            
            // Loop through all of the actions in the event.
            for (var i = 0; i < @event.GetPersistentEventCount(); i++) {
                
                // Get the information about the action.
                var actionInfo = UnityEventBase.GetValidMethodInfo(@event.GetPersistentTarget(i), @event.GetPersistentMethodName(i), new Type[0]);
                
                // Cast actionInfo into a UnityAction to get the listener.
                var index = i;
                var target = @event.GetPersistentTarget(index);
                
                actions.Add(() => {
                    actionInfo.Invoke(target, null);
                });
            }
            
            // Return the list as an array.
            return actions.ToArray();
        }

        ///
        /// Gets a specific action at the index of index.
        ///
        public static Action GetAction(this UnityEvent @event, int index) { return @event.GetAllActions()[index]; }
    }
}