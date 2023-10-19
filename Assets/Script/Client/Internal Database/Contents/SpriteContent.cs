using Script.Client.Internal_Database.Structure;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Script.Client.Internal_Database.Contents {
    /// <summary>
    ///     Stores a reference to a sprite.
    /// </summary>
    [CreateAssetMenu(menuName = "ParallelOrigin/Internal Database/Content/SpriteContent")]
    public class SpriteContent : Content {
        [SerializeField] private AssetReference sprite;

        /// <summary>
        ///     A <see cref="AssetReference" /> to the icon we are using.
        /// </summary>
        public AssetReference Sprite {
            get => sprite;
            set => sprite = value;
        }
    }
}