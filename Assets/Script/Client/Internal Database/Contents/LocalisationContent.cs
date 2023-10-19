using Script.Client.Internal_Database.Structure;
using UnityEngine;

namespace Script.Client.Internal_Database.Contents {
    /// <summary>
    ///     A content which stores localisation data for certain texts and strings.
    /// </summary>
    [CreateAssetMenu(menuName = "ParallelOrigin/Internal Database/Content/LocalizationContent")]
    public class LocalisationContent : Content {
        
        [SerializeField] private string key;
        [SerializeField] private string defaultLocalisation;

        /// <summary>
        ///     The key of the localisation - string
        /// </summary>
        public string Key {
            get => key;
            set => key = value;
        }

        /// <summary>
        ///     The fallback value... in case that the key does not exists in the used localisation.
        /// </summary>
        public string DefaultLocalisation {
            get => defaultLocalisation;
            set => defaultLocalisation = value;
        }
    }
}