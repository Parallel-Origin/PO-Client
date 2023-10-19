using System;
using System.Collections.Generic;
using RotaryHeart.Lib.SerializableDictionary;
using Script.Client.Internal_Database.Structure;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Script.Client.Internal_Database.Contents {
    /// <summary>
    ///     A dictionary mapping strings to gameobject - prefabs
    /// </summary>
    [Serializable]
    public class AssetReferenceDictionary : SerializableDictionaryBase<string, AssetReference> { }

    /// <summary>
    ///     A content class which stores gameobjects...
    /// </summary>
    [CreateAssetMenu(menuName = "ParallelOrigin/Internal Database/Content/GameObjectContent")]
    public class GameObjectContent : Content {
        
        [SerializeField] private AssetReference asset;
        [SerializeField] private AssetReferenceDictionary assetReferenceDictionary;

        /// <summary>
        ///     The default representation
        /// </summary>
        public AssetReference Representation {
            get => asset;
            set => asset = value;
        }

        /// <summary>
        ///     Alternative representations, skins, transparent... what ever.
        /// </summary>
        public IDictionary<string, AssetReference> Alternatives {
            get => assetReferenceDictionary;
            set => assetReferenceDictionary = (AssetReferenceDictionary) value;
        }
    }
}