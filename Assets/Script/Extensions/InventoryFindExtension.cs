
using ParallelOrigin.Core.ECS.Components;
using ParallelOrigin.Core.ECS.Components.Items;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Script.Extensions {
    /// <summary>
    /// An extension which contains several methods for <see cref="Inventory"/> regarding findings
    /// </summary>
    public static class InventoryFindExtension {

        /// <summary>
        /// Searches in the <see cref="Inventory"/> for a certain entity with a type id and returns it
        /// </summary>
        /// <param name="inv">The inventory</param>
        /// <param name="typeID">The entity's type id we search for</param>
        /// <returns>The found entity with the type-id</returns>
        public static Entity GetItemByType(this ref Inventory inv, FixedString32Bytes typeID) {

            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            
            var items = inv.Items;
            for (var index = 0; index < items.Length; index++) {

                var itemRef = items[index];
                var item = itemRef.Resolve(ref entityManager);
                var identity = entityManager.GetComponentData<Identity>(item);

                if (identity.Type.Equals(typeID)) 
                    return item;
            }
            
            return Entity.Null;
        }
    }
}