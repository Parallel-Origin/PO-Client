using ParallelOrigin.Core.ECS;
using ParallelOrigin.Core.ECS.Components;
using ParallelOrigin.Core.ECS.Components.Items;
using ParallelOrigin.Core.Network;
using Script.Client.Mono.MVVM.Inventory;
using Script.Client.Mono.MVVM.Recipe;
using Script.Client.Mono.User_Interface.Screens;
using Script.Client.Systems.Reactive;
using Script.Extensions;
using Unity.Entities;

namespace Script.Client.Systems.Command {

    /// <summary>
    /// A inventory command which can be buffered and will update the local players inventory by re-linking its entity references aswell as update some screens with inventory informations.
    /// TODO : Introduce Commands entities ( For executing logic ) and event entities ( For reacting ) instead of this here :) this way we can also use ReactiveSystems again to update view models 
    /// </summary>
    public struct InventoryCommand : IExecutable {

        public GameScreenManager GameScreenManager;
        public EntityManager EntityManager;
        public CollectionCommand<Identity, Inventory, EntityLink> Command;

        public void Execute() {
            
            var inventoryScreen = (GameScreen)GameScreenManager.Get("inventory");
            var inventoryViewModel = inventoryScreen.GetComponent<InventoryViewModel>();

            var buildScreen = (GameScreen)GameScreenManager.Get("build");
            var buildScreenViewModel = buildScreen.GetComponent<BuildViewModel>();
            
            var inventoryOwner = EntityManager.FindById(Command.Identifier.ID);
            var inventory = EntityManager.GetComponentData<Inventory>(inventoryOwner);
            
            // Loop over modified items and update them in the screens
            for (var index = 0; index < Command.Items.Length; index++) {

                ref var reference = ref Command.Items[index];
                switch (reference.State) {
                    
                    case State.Added:
                        
                        inventory.Items.Add(reference.Item);
                        EntityManager.SetComponentData(inventoryOwner, inventory);
                        
                        inventoryViewModel.Added(ref reference.Item);
                        buildScreenViewModel.Updated();
                        
                        break;
                    
                    case State.Updated:

                        inventoryViewModel.Updated(ref reference.Item);
                        buildScreenViewModel.Updated();
                        break;
                    
                    case State.Removed:

                        for (var i = inventory.Items.m_length-1; i >= 0; i--) {

                             var entityRef =  inventory.Items[i];
                             if (entityRef.UniqueID == reference.Item.UniqueID) 
                                 inventory.Items.RemoveAt(i);
                        }
                        EntityManager.SetComponentData(inventoryOwner, inventory);
                        
                        inventoryViewModel.Removed(ref reference.Item);
                        buildScreenViewModel.Updated();
                        break;
                }
            }
            
            // Set inventory, otherwhise it wont update properly
            EntityManager.SetComponentData(inventoryOwner, inventory);
        }
    }
    
    /// <summary>
    /// A system which buffers inventory commands which then will be executed during the game loop.
    /// </summary>
    public class InventoryCommandSystem : ExecutableBufferSystem<InventoryCommand> {}
}