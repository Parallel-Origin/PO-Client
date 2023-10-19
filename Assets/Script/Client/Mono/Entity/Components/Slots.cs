using System.Collections.Generic;
using Script.Client.Mono.User_Interface;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Script.Client.Mono.Entity.Components {
    
    /// <summary>
    /// A component for an entity which stores a list of equipment slots. 
    /// </summary>
    public class Slots : MonoBehaviour {

        [SerializeField] private TransformDictionary equipmentSlots;
        
        /// <summary>
        /// Equips a mesh and places it into the fitting <see cref="EquipmentSlots"/>
        /// </summary>
        /// <param name="slotName"></param>
        /// <param name="mesh"></param>
        public void Equip(string slotName, AssetReference mesh) {

            var transform = EquipmentSlots[slotName];
            
            // Unequip old go
            if(Equiped.ContainsKey(slotName)) 
                Unequip(slotName);

            mesh.InstantiateAsync(transform).Completed += handle => {
                Equiped[slotName] = handle.Result;
            };
        }

        /// <summary>
        /// Unequips a mesh from a slot.
        /// </summary>
        /// <param name="slotname"></param>
        public void Unequip(string slotname) {

            var go = Equiped[slotname];
            
            Addressables.ReleaseInstance(go);
            Equiped.Remove(slotname);
        }

        /// <summary>
        /// The slots that exist on this gameobject for equipment
        /// </summary>
        public TransformDictionary EquipmentSlots {
            get => equipmentSlots;
            set => equipmentSlots = value;
        }

        /// <summary>
        /// The equiped slots which are in use
        /// </summary>
        public Dictionary<string, GameObject> Equiped { get; set; } = new Dictionary<string, GameObject>();
    }
}