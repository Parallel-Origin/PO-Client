using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Script.Extensions {
    
    /// <summary>
    ///     A set of directions we are able to search.
    /// </summary>
    public enum Direction {
        Up,
        Down,
        Both
    }

    /// <summary>
    ///     Traverses upwards from the Transform of the Gameobject called, to find next parent with the Component given
    /// </summary>
    public static class GameObjectFindExtension {

        /// <summary>
        ///     A method that searches for a certain component in the parents and returns it once found.
        /// </summary>
        /// <param name="go">The gameobject we start with</param>
        /// <typeparam name="T">The type of component we search for</typeparam>
        /// <returns></returns>
        public static T GetParentWithComponent<T>(this GameObject go) {

            var t = go.transform;

            while (t.parent != null) {

                if (t.parent.GetComponent<T>() != null) return t.parent.GetComponent<T>();
                t = t.parent.transform;
            }

            return default;
        }
        
        /// <summary>
        ///     Searches in the hierarchie for the generic... triggers events once found.
        /// </summary>
        /// <param name="direction">The direciton in which we search the types</param>
        /// <param name="types">A list of types we wanna search for</param>
        /// <returns>A list of found objects from the passed trough and previous defined types</returns>
        public static IList<object> Search(this GameObject go, Direction direction, params Type[] types) {

            var searchFor = types.ToList();
            var found = new List<object>();
            
            foreach (var type in searchFor) {
                
                object foundComponent = null;
                switch (direction) {
                    
                    case Direction.Up:
                        foundComponent = GetFromUp(go.transform, type);
                        break;

                    case Direction.Down:
                        foundComponent = GetFromDown(go.transform, type);
                        break;

                    case Direction.Both:
                        foundComponent = GetFromUp(go.transform, type);
                        if (foundComponent == null) foundComponent = GetFromDown(go.transform, type);
                        break;
                }

                if (foundComponent != null) found.Add(foundComponent);
            }

            return found;
        }
        
        /// <summary>
        ///     Searches in the hierarchie upwards till the wanted component was found.
        /// </summary>
        /// <param name="start"></param>
        /// <returns></returns>
        public static Object GetFromUp(Transform start, Type type) {
            
            var current = start.parent;
            while (current != null) {
                if (current.GetComponent(type) != null) return current.GetComponent(type);
                current = current.parent;
            }

            return default;
        }

        /// <summary>
        ///     Searches the hierarchie downwards till the wanted component was found.
        /// </summary>
        /// <param name="start"></param>
        /// <returns></returns>
        public static Object GetFromDown(Transform start, Type type) { return start.GetComponentInChildren(type); }
    }
}