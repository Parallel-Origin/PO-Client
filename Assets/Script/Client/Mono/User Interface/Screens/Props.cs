using UnityEngine;

namespace Script.Client.Mono.User_Interface.Screens {
    
    /// <summary>
    ///     Represents a Prop, a parameter which is used to get passed to <see cref="GameScreen" /> for initilisation
    /// </summary>
    public interface IProps {
        /// <summary>
        ///     The prop-value, getting put into a <see cref="IParams" /> for being transfered to the opened screen
        /// </summary>
        object Value { get; set; }
    }
    
    /// <summary>
    ///     Represents a parameter or value, which gets transfered by a <see cref="GameScreenTransistion" /> to the opened <see cref="GameScreen" />
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Props<T> : MonoBehaviour, IProps {
        
        [SerializeField] private T value;

        /// <summary>
        ///     The prop-value, getting put into a <see cref="IParams" /> for being transfered to the opened screen
        /// </summary>
        public object Value {
            get => value;
            set => this.value = (T) value;
        }
    }
}