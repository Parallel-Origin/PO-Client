using System;
using System.Text.RegularExpressions;
using ParallelOrigin.Core.Base.Classes.Pattern.Registers;
using ParallelOrigin.Core.ECS;
using ParallelOrigin.Core.ECS.Components;
using ParallelOrigin.Core.Network;
using Script.Client.Internal_Database.Contents;
using Script.Client.Internal_Database.Structure;
using Script.Client.Internal_Database.Structure.Interfaces;
using Script.Client.Internal_Database.Structure.Variants;
using Script.Client.Mono.Entity;
using Script.Client.Mono.User_Interface.Stacks.Results;
using Script.Extensions;
using Script.Network;
using Script.Server;
using TMPro;
using Unity.Entities;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace Script.Client.Mono.MVVM.PopUp {
    
    using Entity = Unity.Entities.Entity;
    
    /// <summary>
    /// The view model of the standard popups UI.
    /// Controlls the logic and when and how to update.
    /// </summary>
    [RequireComponent(typeof(AmountPopUpView), typeof(AmountPopUpViewModel), typeof(ResultStack))]
    public class AmountPopUpViewModel : MonoBehaviour {

        private static readonly string AmountKey = "amount";
        
        [SerializeField] private Color confirmColor;
        [SerializeField] private Color cancelColor;

        [SerializeField] private int confirmText;
        [SerializeField] private int cancelText;
        
        private void Awake() {

            Database = ServiceLocator.Get<InternalDatabase>();
            Localizations = Database.GetDatabase("localizations");
            
            EntityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            AmountPopUpView = GetComponent<AmountPopUpView>();
            AmountPopUpModel = GetComponent<EcsEntity>();

            if (AmountPopUpModel.EntityReference != Entity.Null) Initialize();
            else AmountPopUpModel.OnReference += entity => Initialize();
        }

        /// <summary>
        /// Gets called at start, when the data model was assigned
        /// </summary>
        public void Initialize() {
            Refresh(AmountPopUpModel.EntityReference);
        }
        
        /// <summary>
        /// Refreshes the ui and reassign data to its fields
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="dirty"></param>
        public void Refresh(Entity entity) {

            if (entity != AmountPopUpModel.EntityReference) return;
            
            // Get pairs
            var localizations = EntityManager.GetComponentData<Localizations>(entity);
            var locals = localizations.LocalizationsMap;

            // Receive localisation
            var pair = locals["title"];
            var titleLocalisation = Localizations.GetContentStorage(pair).Get<LocalisationContent>();
            var confirmLocalisation = Localizations.GetContentStorage(this.confirmText).Get<LocalisationContent>();
            var cancelLocalisation = Localizations.GetContentStorage(this.cancelText).Get<LocalisationContent>();
            var title = titleLocalisation.DefaultLocalisation;

            // Set title
            AmountPopUpView.Title.text = $"{title}";
            
            // Spawn in first option as confirm
            var confirmButton = AmountPopUpView.Confirm.GetComponentInChildren<Button>();
            var confirmText = AmountPopUpView.Confirm.GetComponentInChildren<TextMeshProUGUI>();
            
            confirmText.color = confirmColor;
            confirmText.text = confirmLocalisation.DefaultLocalisation;
            confirmButton.onClick.AddListener(Confirm);

            // Spawn in second option as cancel
            var cancelButton = AmountPopUpView.Cancel.GetComponent<Button>();
            var cancelText = AmountPopUpView.Cancel.GetComponentInChildren<TextMeshProUGUI>();
            
            cancelText.color = cancelColor;
            cancelText.text = cancelLocalisation.DefaultLocalisation;
            cancelButton.onClick.AddListener(Cancel);
        }

        /// <summary>
        /// Confirms and sends the server a pickup request. 
        /// </summary>
        public void Confirm()
        {
            var network = ServiceLocator.Get<ClientNetwork>();
            var identity = EntityManager.GetComponentData<Identity>(AmountPopUpModel.EntityReference);
            
            // Send click to server
            var clickCommand = new PickupCommand {
                Popup = new EntityLink(identity.ID),
                Amount = uint.Parse(AmountPopUpView.InputField.text)
            };
            
            // Destroy open popup
            var destroyPopup = new EntityCommand {
                Id = identity.ID,
                Opcode = EntityOpCode.Delete,
                Type = string.Empty
            };

            network.Send(ref clickCommand);
            network.Send(ref destroyPopup);
        }

        /// <summary>
        /// Cancels the pickup option. 
        /// </summary>
        public void Cancel()
        {
            var network = ServiceLocator.Get<ClientNetwork>();
            var identity = EntityManager.GetComponentData<Identity>(AmountPopUpModel.EntityReference);
            
            // Send click to server
            var destroyPopup = new EntityCommand {
                Id = identity.ID,
                Opcode = EntityOpCode.Delete,
                Type = string.Empty
            };

            network.Send(ref destroyPopup);
        }
        
        public IInternalDatabase Database { get; set; }
        public IInternalDatabase Localizations { get; set; }

        public EntityManager EntityManager { get; set; }

        public AmountPopUpView AmountPopUpView { get; set; }
        public EcsEntity AmountPopUpModel { get; set; }
    }
}