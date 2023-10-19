using UnityEngine;

namespace Script.Extensions {

    /// <summary>
    ///     A extension class which extends <see cref="GameObject" /> by some methods used for detecting if its a prefab or instacne
    /// </summary>
    public static class UnityPrefabExtension {

        /// <summary>
        ///     Checks if the given prefab is a instance or a prefab.
        /// </summary>
        /// <param name="a_Object"></param>
        /// <returns></returns>
        public static bool GetIsPrefab(this GameObject aObject) { return aObject.scene.rootCount == 0; }
    }
}