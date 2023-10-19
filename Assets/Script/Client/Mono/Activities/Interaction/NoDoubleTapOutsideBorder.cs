using Mapbox.Unity.Map;
using Mapbox.Unity.Utilities;
using ParallelOrigin.Core.Base.Classes.Pattern.Registers;
using Script.Client.Mono.Map;
using UnityEngine;

namespace Script.Client.Mono.Activities.Interaction {
    
    /// <summary>
    ///  A component which hooks itself into <see cref="_touchListener" />
    ///  Checks if the clicks are inside the latlng area... the touch events only get triggered when those are within the borders.
    /// </summary>
    [RequireComponent(typeof(TouchListener))]
    public class NoDoubleTapOutsideBorder : MonoBehaviour {
        
        private AbstractMap _map;
        private TouchListener _touchListener;
        private GameObject _movementArea;

        private void Awake() {
            
            ServiceLocator.Wait<AbstractMap>(o => _map = (AbstractMap)o);
            _touchListener = GetComponent<TouchListener>();
            _movementArea = GameObject.Find("MovementArea");
            
            _touchListener.DoubleTapConditions.Add(InBorders);
        }

        /// <summary>
        ///     Checks in a hit is inside the lat/lng for triggering the double click.
        /// </summary>
        /// <param name="hit"></param>
        /// <returns></returns>
        protected bool InBorders(RaycastHit hit) {
            
            if (hit.collider == null) return false;

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