using UnityEngine;

namespace Script.Client.Mono.Map.Features {
    /// <summary>
    ///     This class is attached to a "water" will on the map... it is used to determine the tile type and manage/controll it for further purposes.
    /// </summary>
    public class Water : MonoBehaviour {
        
        [SerializeField] private Collider waterCollider;
        [SerializeField] private bool initialized;

        private void Awake() { Start(); }

        private void Start() {
            
            /*if (!initialized) {
                DisableRendering();
                initialized = true;
            }

            Prepare();*/
        }

        private void OnEnable() { Start(); }


        /// <summary>
        ///     This method is called once a collider colliders with the water collider... it will stop the players movement.
        /// </summary>
        /// <param name="other"></param>
        private void OnTriggerEnter(Collider other) {
            /*
            if (other.GetComponent<Character>()) {

                Character character = other.GetComponent<Character>();
                character.stop();
            }
            else if (other.GetComponent<DistanceChecker>()) {

                Destroy(other.gameObject);
            }*/
        }


        /// <summary>
        ///     This method is called once a collider stays in the waters collider and it will stop the players movement.
        /// </summary>
        /// <param name="other"></param>
        private void OnTriggerStay(Collider other) {
            /*
            if (other.GetComponent<Character>()) {

                Character character = other.GetComponent<Character>();
                character.stop();
            }*/
        }


        /// <summary>
        ///     This method just repositions the gameobject to get rid of flicker effects and it also manages the collider.
        /// </summary>
        private void Prepare() {
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, 2.5f, gameObject.transform.position.z);

            if (GetComponent<Collider>())
                waterCollider = GetComponent<Collider>();
        }

        /// <summary>
        ///     This method disables the shader and rendering of the water tile...
        /// </summary>
        public void DisableRendering() {
            var renderer = GetComponent<MeshRenderer>();
            renderer.enabled = false;
        }
    }
}