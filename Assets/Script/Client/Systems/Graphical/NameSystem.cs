using ParallelOrigin.Core.ECS.Components;
using Script.Client.Mono.MVVM.Character;
using Script.Extensions;
using Unity.Entities;
using UnityEngine;

namespace Script.Client.Systems.Graphical {
    
    /// <summary>
    /// A system taking care of Showing names for <see cref="Character"/>
    /// </summary>
    public partial class NameSystem : SystemBase{
        
        protected override void OnUpdate() {
            
            Entities.ForEach((ref Character charr, in GameObject go) => {
                
                // Updating visual health
                var nameView = go.GetComponent<NameView>();
                nameView.UsernameField.text = charr.Name.ToStringCached();
            }).WithoutBurst().Run();
        }
    }
}