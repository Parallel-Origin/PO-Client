using ParallelOrigin.Core.Base.Classes.Pattern.Registers;
using ParallelOrigin.Core.ECS.Components;
using ParallelOrigin.Core.ECS.Components.Items;
using Script.Client.Internal_Database.Contents;
using Script.Client.Internal_Database.Structure.Interfaces;
using Script.Client.Internal_Database.Structure.Variants;
using Script.Client.Mono.Entity.Components;
using Script.Client.Systems.Reactive;
using Script.Extensions;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using Entity = Unity.Entities.Entity;

namespace Script.Client.Systems.Graphical {
    
    /// <summary>
    /// A system which takes care of showing <see cref="Equipment"/> for an entity graphically on its <see cref="GameObject"/>
    /// </summary>
    public partial class EquipmentSystem : SystemBase{

        protected override void OnCreate() {
            base.OnCreate();
            
            ServiceLocator.Wait<IRegisterableInternalDatabase>(o => {
                Database = (IInternalDatabase) o;
                MeshDatabase = Database.GetDatabase("meshes");
            });
        }

        protected override void OnUpdate() {
            
            // Spawns in the equiped items as meshes into the right gameobject slots to make them appear visual
            // Only when the entity was marked as dirty, was created or spawned in their go ... otherwhise its not that great for performance
            Entities.ForEach((ref Entity entity, ref Equipment equipment, in GameObject go) => {
                
                var slots = go.GetComponent<Slots>();
                
                // Equip and update existing slots
                foreach (var kvp in equipment.Equiped) {

                    var slot = kvp.Key.ToStringCached();
                    var equipable = kvp.Value;

                    // Get equipable, search mesh and equip that mesh 
                    var meshContent = MeshDatabase.GetContentStorage(equipable.Mesh);
                    var mesh = meshContent.Get<GameObjectContent>();
                    
                    slots.Equip(slot, mesh.Representation);
                }

                // Unequip those which do not exist anymore
                foreach (var kvp in slots.Equiped) {

                    var slot = new FixedString32Bytes(kvp.Key);
                    if (!equipment.Equiped.ContainsKey(slot)) 
                        slots.Unequip(kvp.Key);
                }
            }).WithAny<Created, Dirty, GameObjectAdded>().WithoutBurst().Run();
        }
        
        /// <summary>
        /// The database being used
        /// </summary>
        public IInternalDatabase Database { get; set; }
        
        /// <summary>
        /// The mesh database
        /// </summary>
        public IInternalDatabase MeshDatabase { get; set; }
    }
}