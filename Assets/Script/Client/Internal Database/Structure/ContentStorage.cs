using System;
using System.Collections.Generic;
using System.Linq;
using Script.Client.Internal_Database.Structure.Interfaces;
using UnityEngine;

namespace Script.Client.Internal_Database.Structure {
    
    /// <summary>
    ///     The basic implementation of a <see cref="InternalDatabase" /> asset for representing a ingame entity with dynamic content additions.
    /// </summary>
    [CreateAssetMenu(menuName = "ParallelOrigin/Internal Database/ContentStorage")]
    public class ContentStorage : ScriptableObject, IContentStorage {
        
        [SerializeField] private string category; // The Category, to which this content belongs to... Mobs... Buildings... etc
        [SerializeField] private short typeID; // The Asset-TypeID
        [SerializeField] private List<Content> components; // The Content and their components for storing data

        /// <summary>
        ///     Searches for a certain <see cref="Content" />-Type in the local <see cref="Components" /> and returns it
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Get<T>() where T : Content {

            var type = typeof(T);
            if (Cache.ContainsKey(type)) return Cache[type] as T;
            
            var result = components.First(content => content is T);
            Cache[type] = result;
            return Cache[type] as T;
        }

        /// <summary>
        /// The category of the content
        /// </summary>
        public string Category {
            get => category;
            set => category = value;
        }

        /// <summary>
        /// The type id of the content acting as an identifiier
        /// </summary>
        public short TypeId {
            get => typeID;
            set => typeID = value;
        }

        /// <summary>
        /// The components of the content item
        /// </summary>
        public IList<Content> Components {
            get => components;
            set => components = (List<Content>) value;
        }

        /// <summary>
        /// A cache for fast <see cref="Get{T}"/> operation acess
        /// </summary>
        private IDictionary<Type, Content> Cache { get; set; } = new Dictionary<Type, Content>();
    }
}