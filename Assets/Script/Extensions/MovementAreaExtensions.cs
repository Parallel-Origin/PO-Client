using BitBenderGames;
using Mapbox.Map;
using Mapbox.Unity.Map;
using Mapbox.Unity.Utilities;
using ParallelOrigin.Core.Base.Classes.Pattern.Registers;
using ParallelOrigin.Core.Base.Classes;
using Script.Client.Mono.Map;
using UnityEngine;

namespace Script.Extensions {
    
    /// <summary>
    /// An extension for the <see cref="MovementArea"/>
    /// </summary>
    public static class MovementAreaExtensions {

        public static void CenterAt(this MovementArea area, Vector2d pos) {

            var map = ServiceLocator.Get<AbstractMap>();
            var mobileTouchCamera = Camera.main.GetComponent<MobileTouchCamera>();

            var position = (Mapbox.Utils.Vector2d)pos;
            
            // Updating map & movement area
            if (!map.isInitialized) { map.Initialize(position, 15); }
            else {
                map.SetCenterLatitudeLongitude(position);
                map.UpdateMap();
            }

            // Focus camera on player
            var unityPos = position.AsUnityPosition(map.CenterMercator, map.WorldRelativeScale);
            mobileTouchCamera.Focus(5.0f, unityPos);

            // Set data and reposition the movement area
            area.Reposition(position);
        }
        
    }
}