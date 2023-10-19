using System;
using System.Collections.Generic;
using LiteNetLib;
using Mapbox.Map;
using Mapbox.Unity.Map;
using Mapbox.Utils;
using ParallelOrigin.Core.Base.Classes;
using ParallelOrigin.Core.Base.Classes.Pattern.Prototype;
using ParallelOrigin.Core.Base.Classes.Pattern.Registers;
using ParallelOrigin.Core.ECS;
using ParallelOrigin.Core.ECS.Components;
using ParallelOrigin.Core.ECS.Components.Combat;
using ParallelOrigin.Core.ECS.Components.Interactions;
using ParallelOrigin.Core.ECS.Components.Items;
using ParallelOrigin.Core.ECS.Components.Transform;
using ParallelOrigin.Core.Network;
using Script.Client.Internal_Database.Contents;
using Script.Client.Internal_Database.Structure;
using Script.Client.Internal_Database.Structure.Variants;
using Script.Client.Mono.Activities.Interaction;
using Script.Client.Mono.Map;
using Script.Client.Mono.MVVM.Chat;
using Script.Client.Mono.MVVM.Inventory;
using Script.Client.Mono.User_Interface;
using Script.Client.Mono.User_Interface.Screens;
using Script.Client.Mono.Utils.Login;
using Script.Client.Prototypers;
using Script.Client.Systems.Command;
using Script.Extensions;
using Script.Network;
using Script.Server;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Events;
using Unity.Entities;
using UnityEngine.AddressableAssets;
using UnityEngine.Profiling;
using UnityEngine.Serialization;
using UnityEngine.SocialPlatforms;
using Character = ParallelOrigin.Core.ECS.Components.Character;
using Animation = ParallelOrigin.Core.ECS.Components.Animation;
using Child = ParallelOrigin.Core.ECS.Components.Child;
using Mesh = ParallelOrigin.Core.ECS.Components.Mesh;
using Parent = ParallelOrigin.Core.ECS.Components.Parent;
using Sprite = ParallelOrigin.Core.ECS.Components.Sprite;
using Vector2d = Mapbox.Utils.Vector2d;

namespace Script {
    
    /// <summary>
    ///     The Main class of our game, gets called first and is used to prepare the scene.
    ///     Awake : Self initit
    ///     Start : Initialisation of and with other components
    /// </summary>
    public class Main : MonoBehaviour {
        
        [SerializeField] private string ipAdress;
        [SerializeField] private string port;
        
        [SerializeField] private InternalDatabase internalDatabase;
        [SerializeField] private AbstractMap map;
        [SerializeField] private GameScreenManager gameScreenManager;

        [FormerlySerializedAs("OnBadUsername")] [SerializeField] private UnityEvent onBadUsername;
        [FormerlySerializedAs("OnBadPassword")] [SerializeField] private UnityEvent onBadPassword;
        [FormerlySerializedAs("OnDisconnect")] [SerializeField] private UnityEvent onDisconnect;

        ///////////////
        /// World & Entities
        ///////////////
        
        private EntityManager _entityManager;
        private EntityPrototyperHierarchy _prototyperHierarchy;
        
        private CharacterPrototyper _characterPrototyper;
        private ItemPrototyper _itemPrototyper;
        private ItemOnGroundPrototyper _itemOnGroundPrototyper;
        private OptionPrototyper _optionPrototyper;
        private PopupPrototyper _popupPrototyper;
        private RecipePrototyper _recipePrototyper;
        private ResourcePrototyper _resourcePrototyper;
        private StructurePrototyper _structurePrototyper;
        private MobPrototyper _mobPrototyper;
        
        private ClientNetwork _network;

        private void Awake() {
            
            _network = new ClientNetwork {
                Ip = ipAdress, 
                Port = ushort.Parse(port)
            };
            
            ServiceLocator.Register(internalDatabase);
            ServiceLocator.Register(map);
            ServiceLocator.Register(_network);
        }

        private void Start() {
            
            Application.targetFrameRate = 60;
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            ///////////////////////////////////////
            /// Prototypers
            ///////////////////////////////////////

            // The main prototyper
            _prototyperHierarchy = new EntityPrototyperHierarchy();
            ServiceLocator.Register(_prototyperHierarchy);

            // Game
            _characterPrototyper = new CharacterPrototyper();
            _resourcePrototyper = new ResourcePrototyper();
            _itemPrototyper = new ItemPrototyper();
            _itemOnGroundPrototyper = new ItemOnGroundPrototyper();
            _structurePrototyper = new StructurePrototyper();
            _mobPrototyper = new MobPrototyper();
            _recipePrototyper = new RecipePrototyper();

            // UI
            _popupPrototyper = new PopupPrototyper();
            _optionPrototyper = new OptionPrototyper();

            // Creating paths to create entities by their path like .Clone("1:1);
            _prototyperHierarchy.Register("1", _characterPrototyper);
            _prototyperHierarchy.Register("2", _resourcePrototyper);
            _prototyperHierarchy.Register("3", _itemPrototyper);
            _prototyperHierarchy.Register("4", _structurePrototyper);
            _prototyperHierarchy.Register("5", _mobPrototyper);
            _prototyperHierarchy.Register("6", _itemOnGroundPrototyper);

            _prototyperHierarchy.Register("recipe", _recipePrototyper);

            _prototyperHierarchy.Register("ui_popup", _popupPrototyper);
            _prototyperHierarchy.Register("ui_option", _optionPrototyper);

            ///////////////////////////////////////
            /// ECS
            ///////////////////////////////////////

            // Setting up the ECSNetworkLayer, responsible for creating, updating and removing components
            EntityManagerCommanderExtensions.PrototyperHierarchy = _prototyperHierarchy;
            EntityManagerCommanderExtensions.OperationProcessor[Operation.Create] = new CreateOperationProcessor(_prototyperHierarchy);
            EntityManagerCommanderExtensions.OperationProcessor[Operation.Add] = new AddOperationProcessor();
            EntityManagerCommanderExtensions.OperationProcessor[Operation.Update] = new UpdateOperationProcessor();
            EntityManagerCommanderExtensions.OperationProcessor[Operation.Remove] = new RemoveOperationProcessor();
            EntityManagerCommanderExtensions.OperationProcessor[Operation.Delete] = new DeleteOperationProcessor();
            
            ///////////////////////////////////////
            /// Network
            ////C///////////////////////////////////
           
            _network.Start();
            _network.OnConnected += _ => Debug.Log( "Connected to server");
            _network.OnBadUsername += (_, _) => onBadUsername.Invoke();
            _network.OnBadPassword += (_, _) => onBadPassword.Invoke();
            _network.OnLogin += (NetPeer _, in Character _) => Debug.Log( "Loged in");
            _network.OnDisconnected += (_, _) => {
                onDisconnect.Invoke();
                Debug.Log("Disconnected from server");
            };

            // Single commands
            _network.OnReceive(WorldNetworkExtensions.CenterMap,() => new MapCommand());
            _network.OnReceive(WorldNetworkExtensions.Teleport, () => new TeleportationCommand());
            _network.OnReceive(WorldNetworkExtensions.ShowMessage, () => new ChatMessageCommand());
            
            _network.OnReceive((command, _) => _entityManager.ConvertToEcs(ref command), () => new EntityCommand());
            _network.OnReceive((command, _) => _entityManager.ConvertToEcs(ref command), () => new ComponentCommand<NetworkTransform>());
            _network.OnReceive((command, _) => _entityManager.ConvertToEcs(ref command), () => new ComponentCommand<NetworkRotation>());
       
            // Entity Commands ( Add, Set, Remove components )
            _network.OnReceive((command, _) => _entityManager.ConvertToEcs(ref command), () => new EntityCommand<Identity, Character, Inventory, Mesh, NetworkTransform, Movement, Health, BuildRecipes>());  // Player 
            _network.OnReceive((command, _) => _entityManager.ConvertToEcs(ref command), () => new EntityCommand<Identity, Mesh, Health, NetworkTransform, NetworkRotation>());  // AOI Entered
            _network.OnReceive((command, _) => _entityManager.ConvertToEcs(ref command), () => new EntityCommand<Identity, Item, Mesh, Sprite, NetworkTransform>());  // AOI Entered for Items on ground
            _network.OnReceive((command, _) => _entityManager.ConvertToEcs(ref command), () => new EntityCommand<Identity, Character, Mesh, Health, NetworkTransform, NetworkRotation, Movement>());  // AOI Charackter entered
            _network.OnReceive((command, _) => _entityManager.ConvertToEcs(ref command), () => new EntityCommand<NetworkTransform, NetworkRotation>());  // Movement
            _network.OnReceive((command, _) => _entityManager.ConvertToEcs(ref command), () => new EntityCommand<Health>());  // Health updates
            
            _network.OnReceive((command, _) => _entityManager.ConvertToEcs(ref command), () => new EntityCommand<Identity, Popup, Mesh, Localizations, Parent>());  // Popup 
            _network.OnReceive((command, _) => _entityManager.ConvertToEcs(ref command), () => new EntityCommand<Identity, Mesh, Localizations, Child>());  // Popup options
            
            _network.OnReceive((command, _) => _entityManager.ConvertToEcs(ref command), () => new EntityCommand<Identity, Item, Mesh, Sprite>()); // Inventory 
            _network.OnReceive((command, _) => _entityManager.ConvertToEcs(ref command), () => new EntityCommand<Item>()); // Inventory updates or item on ground updates 
            _network.OnReceive((command, _) => _entityManager.ConvertToEcs(ref command), () => new EntityCommand<Identity, Mesh, Sprite, Recipe, BuildingRecipe, Localizations>()); // Recipes

            // Anim command which references an entity and forces it to set a bool to play a anim
            _network.OnReceive((command, _) => {
                
                var inventoryCommand = new AnimationCommand{ EntityManager = _entityManager, ParamCommand = command, Trigger = false};
                
                var commandSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<AnimationCommandSystem>();
                commandSystem.Buffer(ref inventoryCommand);
            }, () => new Statefull<AnimationParamCommand>());
            
            // Entity commands which triggers an animation once. 
            _network.OnReceive((command, _) => {
                
                var inventoryCommand = new AnimationCommand{ EntityManager = _entityManager, TriggerCommand = command, Trigger = true};
                
                var commandSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<AnimationCommandSystem>();
                commandSystem.Buffer(ref inventoryCommand);
            }, () => new AnimationTriggerCommand());


            // List updates... couldnt find a nicer way yet
            _network.OnReceive((command, _) => {

                var inventoryCommand = new InventoryCommand { GameScreenManager = gameScreenManager, EntityManager = _entityManager, Command = command };
                
                var commandSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<InventoryCommandSystem>();
                commandSystem.Buffer(ref inventoryCommand);
            }, () => new CollectionCommand<Identity, Inventory, EntityLink>());
            
        }

        private void Update() { _network.Update(); }

        private void OnDestroy() { _network.Stop(); }
    }
}