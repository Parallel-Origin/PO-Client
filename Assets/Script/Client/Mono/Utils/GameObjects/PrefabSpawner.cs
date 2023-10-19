using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Script.Client.Mono.Utils.GameObjects {
    
    /// <summary>
    /// Simple class used to spawn prefabs.
    /// </summary>
    public class PrefabSpawner : MonoBehaviour {
        
        public Transform parent;
        public AssetReference asset;

        public void SpawnAsset() {
            asset.InstantiateAsync(parent, false);
        }
    }
}