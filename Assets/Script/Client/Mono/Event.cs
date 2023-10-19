using System;
using System.Collections.Generic;
using ParallelOrigin.Core.Base.Interfaces.Observer;
using Unity.Collections;
using Unity.Entities;

namespace Script.Client.Mono {

    /// <summary>
    ///     A basic event which can get fired
    /// </summary>
    public interface IEvent {

        /// <summary>
        ///     The message of the event
        /// </summary>
        string Message { get; set; }

        /// <summary>
        ///     Additional params
        /// </summary>
        IParams Params { get; set; }
    }
    
    // <summary>
    ///     A interface providing structure for the basic EventHandler mechanics.
    /// </summary>
    public interface IEventHandler {

        /// <summary>
        ///     Adds a new listener to the event handler for listening to a specific event type
        /// </summary>
        /// <param name="type">The Type we are listening to</param>
        /// <param name="action">The action getting called</param>
        /// <returns>The unique ID of the added action</returns>
        string Register(Type type, Action<object, EventArgs> action);

        /// <summary>
        ///     Posts a event and notifies all listeners
        /// </summary>
        /// <param name="sender">The Sender calling this event</param>
        /// <param name="eventArgs">The event itself</param>
        void Post(object sender, EventArgs eventArgs);

        /// <summary>
        ///     Removes a previous registered action from the <see cref="EventHandler.GlobalEventHandler" />
        /// </summary>
        /// <param name="type">The type we want to remove a action for</param>
        /// <param name="actionID">The unique id of the action we wanna unsubscribe</param>
        void Remove(Type type, string actionID);


        /// <summary>
        ///     All listeners, requiring each a certain type and a certain action getting called
        /// </summary>
        IDictionary<Type, EventHandler<EventArgs>> Listeners { get; set; }
    }
    
    /// <summary>
    ///     A struct, representing a <see cref="Event" />
    ///     Required for multi threading for <see cref="SystemBase" /> in the Unity ECS
    /// </summary>
    public struct EventStruct : IEvent {

        private FixedString32Bytes _message;
        private IParams _params;

        /// <summary>
        ///     The message of this event
        /// </summary>
        public string Message {
            get => _message.ToString();
            set => _message = value;
        }

        /// <summary>
        ///     The params of this event
        /// </summary>
        public IParams Params {
            get => _params;
            set => _params = value;
        }
    }


    /// <summary>
    ///     A basic implementation of a Event which can get fired and consumed.
    ///     Same as the <see cref="EventStruct" /> but as a class for being able to send trough a <see cref="EventHandler{TEventArgs}" />
    ///     Uses the <see cref="EventStruct" /> internal for storing the event informations.
    /// </summary>
    [Serializable]
    public class Event : EventArgs, IEvent {

        public EventStruct WrappedEvent;

        /// <summary>
        ///     Constructs a basic event with its most nessecary informations out of a <see cref="EventStruct" />
        /// </summary>
        /// <param name="wrappedEvent">The <see cref="EventStruct" /> we consturcts a class event out of</param>
        public Event(EventStruct wrappedEvent) { this.WrappedEvent = wrappedEvent; }

        /// <summary>
        ///     Construct a basic event with its most nessecary informations
        /// </summary>
        /// <param name="message">The message being passed</param>
        /// <param name="params">Additional parameters being passed</param>
        public Event(string message, IParams @params) {
            Message = message;
            Params = @params;
        }

        public string Message {
            get => WrappedEvent.Message;
            set => WrappedEvent.Message = value;
        }

        public IParams Params {
            get => WrappedEvent.Params;
            set => WrappedEvent.Params = value;
        }
    }
}