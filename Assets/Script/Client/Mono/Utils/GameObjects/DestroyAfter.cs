using UnityEngine;

namespace Script.Client.Mono.Utils.GameObjects {
    /// <summary>
    ///     This class will destroy a certain gameobject after a certain amount of seconds.
    /// </summary>
    public class DestroyAfter : MonoBehaviour {
        public float interval;

        private void Start() { Destroy(gameObject, interval); }
    }
}