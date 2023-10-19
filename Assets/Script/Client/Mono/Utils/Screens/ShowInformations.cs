using System;
using RotaryHeart.Lib.SerializableDictionary;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

namespace Script.Client.Mono.Utils.Screens {
    [Serializable]
    public class InformationHolder : SerializableDictionaryBase<int, Information> { }

    /// <summary>
    ///     This class is used to display important informations regulary.
    ///     When playAnimation is true the assignment of the information is called by the animation clip.
    /// </summary>
    public class ShowInformations : MonoBehaviour {
        [SerializeField] private Text textField;
        [SerializeField] private Animator animator;

        [SerializeField] private string appearState;
        [SerializeField] private float repeatTime;
        [SerializeField] private bool playAnimations;
        [SerializeField] private int lastInformationKey;

        [SerializeField] public InformationHolder informations = new InformationHolder();

        private void Start() {
            textField.text = GetInformation();

            if (playAnimations) InvokeRepeating("PlayAnimation", 0f, repeatTime);
            else InvokeRepeating("CastInformationToField", 0f, repeatTime);
        }

        /// <summary>
        ///     Refreshes the shown text, with a new information
        /// </summary>
        protected void CastInformationToField() { textField.text = GetInformation(); }


        /// <summary>
        ///     Plays the animations, whichis used to fade in/ out a information
        /// </summary>
        protected void PlayAnimation() {
            if (playAnimations)
                animator.Play(appearState);
        }

        /// <summary>
        ///     This method returns an information based on their chance to appear
        /// </summary>
        /// <returns></returns>
        protected string GetInformation() {
            var weight = 0;

            foreach (var kvp in informations)
                weight += kvp.Value.weight;

            var random = new Random();
            var randomChance = random.Next(0, weight);
            var currentWeight = 0;

            foreach (var kvp in informations) {
                currentWeight += kvp.Value.weight;

                if (randomChance <= currentWeight) {
                    if (kvp.Key != lastInformationKey) {
                        lastInformationKey = kvp.Key;
                        return kvp.Value.text;
                    }

                    GetInformation();
                }
            }

            return "- If this message appears here... something is horrible wrong ! -";
        }
    }
}