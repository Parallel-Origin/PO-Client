using RotaryHeart.Lib.SerializableDictionary;
using Script.Client.Internal_Database.Structure;
using UnityEngine;

namespace Script.Client.Internal_Database.Contents {
    /// <summary>
    ///     A simply dictionary which stores strings as keys to <see cref="Content" />'s
    ///     Mostly used for describing the referenced content
    /// </summary>
    public class StringContentDictionary : SerializableDictionaryBase<string, Content> { }

    /// <summary>
    ///     A <see cref="Content" /> which is used as a 1:n relation between a <see cref="ContentStorage" /> and multiple other <see cref="Content" />-additions
    /// </summary>
    [CreateAssetMenu(menuName = "ParallelOrigin/Internal Database/Content/ContentDictionary")]
    public class ContentDictionary : Content {
        
        [SerializeField] private StringContentDictionary _contents;

        /// <summary>
        ///     A dictionary storing keys as references to the <see cref="Content" />
        /// </summary>
        public StringContentDictionary Contents {
            get => _contents;
            set => _contents = value;
        }
    }
}