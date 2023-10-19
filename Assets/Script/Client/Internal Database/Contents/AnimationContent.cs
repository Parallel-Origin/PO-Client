using Script.Client.Internal_Database.Structure;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Script.Client.Internal_Database.Contents {
    
    /// <summary>
    ///     A content which stores <see cref="AnimationClipDictionary" /> for overriding our set <see cref="Motion" />'s in our <see cref="Animator" /> in our <see cref="GameObjectContent" />
    /// </summary>
    [CreateAssetMenu(menuName = "ParallelOrigin/Internal Database/Content/AnimationContent")]
    public class AnimationContent : Content {
        
        [SerializeField] private string clipName;
        [SerializeField] private AssetReference clip;

        /// <summary>
        ///     The name of the clip
        /// </summary>
        public string ClipName {
            get => clipName;
            set => clipName = value;
        }

        /// <summary>
        ///     The referenced animation
        /// </summary>
        public AssetReference Clip {
            get => clip;
            set => clip = value;
        }
    }
}