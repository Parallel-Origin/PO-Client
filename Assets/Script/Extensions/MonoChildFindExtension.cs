using System;
using Script.Client.Mono.Entity.Components;

namespace Script.Extensions {
    
    /// <summary>
    /// A extension for the <see cref="Child"/> class that extends it for additional search and query methods
    /// </summary>
    public static class MonoChildFindExtension {

        /// <summary>
        /// Searches in a child parent realation upwards for a certain component and returns it
        /// </summary>
        /// <param name="child">The child we wanna search from</param>
        /// <param name="type">The component we search upwards in the hierarchie</param>
        /// <returns></returns>
        public static object Search(this Child child, Type type) {
            
            var owner = child.Parent;
            while (owner != null)
                
                if (!owner.GetComponent<Parent>()) {
                    
                    var parent = owner.GetComponent<Parent>();
                    var childs = parent.Childs;

                    foreach (var currentChild in childs) 
                        if (currentChild.GetComponent(type))
                            return currentChild.GetComponent(type);

                    owner = !owner.GetComponent<Parent>() ? null : owner.GetComponent<Parent>().gameObject;
                }
                else owner = null;

            return null;
        }
    }
}