using Mapbox.Unity.Map;
using Mapbox.Unity.Utilities;
using ParallelOrigin.Core.Base.Classes.Pattern.Registers;
using ParallelOrigin.Core.ECS.Components;
using Script.Client.Mono.Entity;
using Script.Client.Mono.Map;
using Script.Extensions;
using Unity.Entities;
using UnityEngine;

namespace Script.Client.Mono.Activities.Interaction {
    
    /// <summary>
    ///  A component which hooks itself into <see cref="_touchListener" />
    ///  Checks if the clicks are inside the latlng area... the touch events only get triggered when those are within the borders.
    /// </summary>
    [RequireComponent(typeof(TouchListener))]
    public class NoTapOnEntityOutsideBorder : MonoBehaviour {

        [SerializeField] private string tag;
        
        private AbstractMap _map;
        private TouchListener _touchListener;
        private GameObject _movementArea;

        private EntityManager _manager;

        private void Awake() {
            
            ServiceLocator.Wait<AbstractMap>(o => _map = (AbstractMap)o);
            _touchListener = GetComponent<TouchListener>();
            _movementArea = GameObject.Find("MovementArea");
            _manager = World.DefaultGameObjectInjectionWorld.EntityManager;
            
            _touchListener.TapConditions.Add(InBorders);
        }

        /// <summary>
        ///     Checks in a hit is inside the lat/lng for triggering the double click.
        /// </summary>
        /// <param name="hit"></param>
        /// <returns></returns>
        protected bool InBorders(RaycastHit hit) {
            
            if (hit.collider == null) return false;

            // Get hitted entity
            var go = hit.collider.gameObject;
            var monoEntity = go.GetComponent<EcsEntity>();

            // If theres no mono entity ignore and return that the tap can be executed
            if (!monoEntity) return true;
            
            var entity = monoEntity.EntityReference;
            if (!_manager.Exists(entity)) return false;
            var identity = _manager.GetComponentData<Identity>(entity);

            // If entity has not tag, return with true, we dont need to check logic
            if (!identity.Tag.EqualsStack(tag)) return true;
            
            // Check if entity is inside the movement area and make this tap true or false based on this. 
            var tapPosition = hit.point;
            var tapGeoPosition = new Vector3(tapPosition.x, 0, tapPosition.z).GetGeoPosition(_map.CenterMercator, _map.WorldRelativeScale);

            if (tapGeoPosition.x >= GeoBounds.MaximumAllowedLatitude
                || tapGeoPosition.x <= GeoBounds.MinimumAllowedLatitude
                || tapGeoPosition.y >= GeoBounds.MaximumAllowedLongitude
                || tapGeoPosition.y <= GeoBounds.MinimumAllowedLongitude)
                return false;
            
            var movementAreaPosition = _movementArea.transform.position;
            return hit.point.x >= movementAreaPosition.x && hit.point.x <= movementAreaPosition.x + 900f &&
                   hit.point.z <= movementAreaPosition.z && hit.point.z >= movementAreaPosition.z - 900f;
        }
    }
}