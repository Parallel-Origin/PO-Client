using System;
using System.Collections.Generic;
using RotaryHeart.Lib.SerializableDictionary;
using UnityEngine;
using UnityEngine.UI;

namespace Script.Client.Mono.Entity.Components {
    
    /// <summary>
    ///     A serializable dictionary that contains key value pairs of id's and their assigned color
    ///     Due to serializable it can get used in the inspector
    /// </summary>
    [Serializable]
    public class ColorDictionary : SerializableDictionaryBase<int, Color> { }

    /// <summary>
    ///     A Mono-Entity component which takes care of the visual combat score representation of that entity for showing
    ///     its dangerous level to the players.
    /// </summary>
    public class Score : MonoBehaviour {
        
        [SerializeField] private Transform scoreTransform;
        [SerializeField] private Image scoreIcon;
        [SerializeField] private ColorDictionary scoreColors;

        [SerializeField] private bool usesCombatScore;

        public void Awake() {
            if (!usesCombatScore) return;
            if (scoreTransform == null) return;

            if (!scoreTransform.gameObject.GetComponent<Image>())
                scoreTransform.gameObject.AddComponent<Image>();

            var image = scoreTransform.gameObject.GetComponent<Image>();
            scoreIcon = image;

            if (scoreIcon != null) image.sprite = scoreIcon.sprite;
            SetColor(0);
        }

        /// <summary>
        ///     This method sets the color of the scoreIcon;
        /// </summary>
        /// <param name="color"></param>
        public void SetColor(Color color) { scoreIcon.color = color; }


        /// <summary>
        ///     This method sets the scoreIcon-color to the color specified in the combat score colors...
        ///     <param name="score">The score we wanna change the indicator icon to</param>
        /// </summary>
        public void SetColor(int score) {
            if (ContainsScore(score)) {
                var combatScoreColor = GetColorFromScore(score);
                scoreIcon.color = combatScoreColor;
            }
        }

        /// <summary>
        ///     This method checks if the colors dictionary contains a color for the specific score.
        /// </summary>
        /// <param name="score">The score</param>
        /// <returns></returns>
        public bool ContainsScore(int score) { return scoreColors.ContainsKey(score); }


        /// <summary>
        ///     This method returns the color referenced to the score.
        /// </summary>
        /// <param name="score"></param>
        /// <returns></returns>
        public Color GetColorFromScore(int score) { return scoreColors.ContainsKey(score) ? scoreColors[score] : Color.red; }

        /// <summary>
        ///     The transform of the gameobject which contains a image component that represents the combat score
        /// </summary>
        public Transform ScoreTransform {
            get => scoreTransform;
            set => scoreTransform = value;
        }

        /// <summary>
        ///     The score icon
        /// </summary>
        public Image ScoreIcon {
            get => scoreIcon;
            set => scoreIcon = value;
        }

        /// <summary>
        ///     A Dictionary containing several colors for different combat scores
        ///     Those are used to color the <see cref="ScoreIcon" /> to show the "score" or danger level of that "entity"
        /// </summary>
        public IDictionary<int, Color> ScoreColors {
            get => scoreColors;
            set => scoreColors = (ColorDictionary) value;
        }

        /// <summary>
        ///     Can be set if the healthbar should start using the assigned score variables
        /// </summary>
        public bool UsesCombatScore {
            get => usesCombatScore;
            set => usesCombatScore = value;
        }
    }
}