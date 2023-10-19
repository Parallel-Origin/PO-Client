using ParallelOrigin.Core.ECS;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.UIElements;

namespace Script.Extensions {
    
    /// <summary>
    /// An extension for the <see cref="UnsafeList"/>
    /// </summary>
    public static class UnsafeListExtension {

        /// <summary>
        /// Checks if theres a certain <see cref="EntityLink"/> with a certain id in the list. 
        /// </summary>
        /// <param name="list"></param>
        /// <param name="uniqueID"></param>
        /// <returns></returns>
        public static bool Contains(this ref UnsafeList<EntityLink> list, long uniqueID)  {

            for (var index = 0; index < list.Length; index++) {

                var existingUniqueID = list[index].UniqueID;
                if (existingUniqueID == uniqueID) return true;
            }

            return false;
        }
    }
}