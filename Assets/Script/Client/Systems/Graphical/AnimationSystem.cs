using System;
using System.Collections.Generic;
using ParallelOrigin.Core.Base.Classes.Pattern.Registers;
using Script.Client.Internal_Database.Contents;
using Script.Client.Internal_Database.Structure.Interfaces;
using Script.Client.Internal_Database.Structure.Variants;
using Script.Extensions;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using Animation = ParallelOrigin.Core.ECS.Components.Animation;

namespace Script.Client.Systems.Graphical {

    /// <summary>
    ///     A system which processes <see cref="AnimatorComponent" /> and <see cref="Components.Animation" /> for a set of <see cref="Entity" />
    ///     in order to update their visual animations.
    /// </summary>
    public partial class AnimationSystem : SystemBase {
        
        private IInternalDatabase _database;
        
        protected override void OnCreate() {
            base.OnCreate();
            
            ServiceLocator.Wait<IRegisterableInternalDatabase>(o => {
                _database = (IInternalDatabase) o;
                _database = _database.GetDatabase("animations");
            });
        }

        protected override void OnUpdate() {

            // Adding override controllers for the animations, once
            Entities.ForEach((ref Entity entity, in Animator animator) => {

                var overrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
                EntityManager.AddComponentObject(entity, overrideController);
                animator.runtimeAnimatorController = overrideController;
            }).WithNone<AnimatorOverrideController>().WithStructuralChanges().WithoutBurst().Run();
            
            // Override animation clips based on the animation state which represents the clip to play
            Entities.ForEach((ref Animation anim, in AnimatorOverrideController animator) => {

                // Loop over all overriden animation clips to apply the override ingame by exchaning the clips once during runtime
                foreach (var toOverwrite in anim.OverridenAnimationClips) {
                    
                    // Get animation from database
                    var clipID = toOverwrite.Value;
                    var stateName = toOverwrite.Key;
                    
                    var contentStorage = _database.GetContentStorage(clipID);
                    var replacementClip = contentStorage.Get<AnimationContent>();

                    // Receive a list of all overidden animations and cache them for preventing gc allocations each iteration
                    if (!OverrideCache.ContainsKey(animator)) {

                        var currentOverrides = new List<KeyValuePair<AnimationClip, AnimationClip>>();
                        animator.GetOverrides(currentOverrides);

                        // Caching
                        OverrideCache[animator] = currentOverrides;
                    }

                    // Check if replacement already happened for prevent it from doing this every frame
                    var overrides = OverrideCache[animator];
                    var clipToReplace = GetClipByName(overrides, stateName);
                    if (clipToReplace == null || Replaced(overrides, clipToReplace, replacementClip)) return;
                    
                    // Replace the animation of the certain state with our new animation we wanna play
                    /*var animatorr = animator;
                    replacementClip.Clip.LoadAssetAsyncIfValid<AnimationClip>(handle => {

                        animatorr[clipToReplace] = handle.Result;

                        // Replace the cache list 
                        var newOverrides = new List<KeyValuePair<AnimationClip, AnimationClip>>();
                        animatorr.GetOverrides(newOverrides);

                        // Caching
                        OverrideCache[animatorr] = newOverrides;
                    });*/
                }
            }).WithoutBurst().Run();

            // Play animations
            Entities.ForEach((ref Animation anim, in Animator animator) => {
                
                // Set state and override animation to the one from our animation component, for triggering transistions
                for (var index = 0; index < anim.Triggers.Length; index++) {

                    var trigger = anim.Triggers[index].ToStringCached();
                    animator.SetTrigger(trigger);
                }
                
                // Set state and override animation to the one from our animation component, for triggering transistions
                foreach (var kvp in anim.BoolParams) {

                    var parameterName = kvp.Key.ToStringCached();
                    animator.SetBoolParamWithCheck(parameterName, kvp.Value);
                }
                
                // Clear triggers, because they should only be used once on the animator
                if(anim.Triggers.Length > 0) anim.Triggers.Clear();
            }).WithoutBurst().Run();
        }
        
        
        /// <summary>
        /// Searches a clip by its name and returns it.
        /// GC alloc heavy due to the unoptimized <see cref="AnimatorOverrideController"/>, allocats a new array each iteration
        /// </summary>
        /// <param name="animator"></param>
        /// <param name="animState"></param>
        /// <returns></returns>
        public static AnimationClip GetClipByName(List<KeyValuePair<AnimationClip, AnimationClip>> overrides, FixedString32Bytes animState) {
            
            foreach(var clip in overrides) {

                var original = clip.Key;
                var clipName = original.GetNameCached();
                if (animState.EqualsStack(clipName, StringComparison.CurrentCultureIgnoreCase)) 
                    return original;
            }

            return null;
        }

        /// <summary>
        /// Checks if the given clip was already replaced with its replacement inside the passed list.
        /// </summary>
        /// <param name="overrides">A list of overriden clips</param>
        /// <param name="toReplace">The original clip we wanna check if it was overriden/replaced</param>
        /// <param name="replacement">Its replacement</param>
        /// <returns></returns>
        private static bool Replaced(List<KeyValuePair<AnimationClip, AnimationClip>> overrides, AnimationClip toReplace, AnimationContent replacement) {

            foreach (var kvp in overrides) {

                var original = kvp.Key;
                var replaced = kvp.Value;

                // Compare original and replaced and their names to check if they are equal and replaced
                if (original.Equals(toReplace) && replaced != null) {

                    var replacedClipName = replaced.GetNameCached();
                    if(replacedClipName == replacement.ClipName)
                        return true;
                }
            }

            return false;
        }

        /// <summary>
        /// A cache for looking up overrides and stuff pretty fast without new allocations
        /// </summary>
        private static IDictionary<AnimatorOverrideController, List<KeyValuePair<AnimationClip, AnimationClip>>> OverrideCache { get; set; } = new Dictionary<AnimatorOverrideController, List<KeyValuePair<AnimationClip, AnimationClip>>>();
    }
}