using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Collections;
using Unity.Profiling;
using UnityEngine;

namespace Script.Extensions {
    
    /// <summary>
    /// A extension which adds several methods for <see cref="Animator"/> and <see cref="AnimatorOverrideController"/>
    /// </summary>
    public static class AnimatorExtensions {

        private static Func<Animator, int, string> _getCurrentStateName;
        private static Func<Animator, int, string> _getNextStateName;
        private static Func<Animator, int, string> _resolveHash;

        /// <summary>
        ///     Gets an instance method with single argument of type
        ///     <typeparamref
        ///         name="TArg0" />
        ///     and return type of <typeparamref name="TReturn" /> from
        ///     <typeparamref
        ///         name="TThis" />
        ///     and compiles it into a fast open delegate.
        /// </summary>
        /// <typeparam name="TThis">Type of the class owning the instance method.</typeparam>
        /// <typeparam name="TArg0">
        ///     Type of the single parameter to the instance method to
        ///     find.
        /// </typeparam>
        /// <typeparam name="TReturn">Type of the return for the method</typeparam>
        /// <param name="methodName">The name of the method the compile.</param>
        /// <returns>
        ///     The compiled delegate, which should be about as fast as calling the function
        ///     directly on the instance.
        /// </returns>
        /// <exception cref="ArgumentException">
        ///     If the method can't be found, or it has an
        ///     unexpected return type (the return type must match exactly).
        /// </exception>
        /// <see href="https://codeblog.jonskeet.uk/2008/08/09/making-reflection-fly-and-exploring-delegates/" />
        private static Func<TThis, TArg0, TReturn> BuildFastOpenMemberDelegate<TThis, TArg0, TReturn>(string methodName) {
            var method = typeof(TThis).GetMethod(methodName,
                BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                CallingConventions.Any,
                new[] {typeof(TArg0)},
                null);

            if (method == null)
                throw new ArgumentException("Can't find method " + typeof(TThis).FullName + "." + methodName + "(" + typeof(TArg0).FullName + ")");
            if (method.ReturnType != typeof(TReturn))
                throw new ArgumentException("Expected " + typeof(TThis).FullName + "." + methodName + "(" + typeof(TArg0).FullName + ") to have return type of string but was " +
                                            method.ReturnType.FullName);
            return (Func<TThis, TArg0, TReturn>) Delegate.CreateDelegate(typeof(Func<TThis, TArg0, TReturn>), method);
        }

        /// <summary>
        ///     [FOR DEBUGGING OnLY] Calls an internal method on <see cref="Animator" /> that
        ///     returns the name of the current state for a layer. The internal method could be removed
        ///     or refactored at any time, and may not have good performance.
        /// </summary>
        /// <param name="animator">The animator to get the current state from.</param>
        /// <param name="layer">The layer to get the current state from.</param>
        /// <returns>The name of the currently running state.</returns>
        public static string GetCurrentStateName(this Animator animator, int layer) {
            if (_getCurrentStateName == null)
                _getCurrentStateName = BuildFastOpenMemberDelegate<Animator, int, string>("GetCurrentStateName");
            return _getCurrentStateName(animator, layer);
        }

        /// <summary>
        ///     [FOR DEBUGGING OnLY] Calls an internal method on <see cref="Animator" /> that
        ///     returns the name of the next state for a layer. The internal method could be removed or
        ///     refactored at any time, and may not have good performance.
        /// </summary>
        /// <param name="animator">The animator to get the next state from.</param>
        /// <param name="layer">The layer to get the next state from.</param>
        /// <returns>The name of the next running state.</returns>
        public static string GetNextStateName(this Animator animator, int layer) {
            if (_getNextStateName == null)
                _getNextStateName = BuildFastOpenMemberDelegate<Animator, int, string>("GetNextStateName");
            return _getNextStateName(animator, layer);
        }

        /// <summary>
        ///     [FOR DEBUGGING OnLY] Calls an internal method on <see cref="Animator" /> that
        ///     returns the string used to create a hash from
        ///     <see cref="Animator.StringToHash(string)" />. The internal method could be removed or
        ///     refactored at any time, and may not have good performance.
        /// </summary>
        /// <param name="animator">The animator to get the string from.</param>
        /// <param name="hash">The hash to get the original string for.</param>
        /// <returns>The name of the string for <paramref name="hash" />.</returns>
        public static string ResolveHash(this Animator animator, int hash) {
            if (_resolveHash == null)
                _resolveHash = BuildFastOpenMemberDelegate<Animator, int, string>("ResolveHash");
            return _resolveHash(animator, hash);
        }

        /// <summary>
        /// Searches a clip by its name and returns it.
        /// GC alloc heavy due to the unoptimized <see cref="AnimatorOverrideController"/>, allocats a new array each iteration
        /// </summary>
        /// <param name="animator"></param>
        /// <param name="animState"></param>
        /// <returns></returns>
        public static AnimationClip GetClipByName(this AnimatorOverrideController animator, FixedString32Bytes animState) {
            
            foreach(var clip in animator.animationClips) {
                var clipName = clip.name.ToLower();
                if (animState.EqualsStack(clipName)) return clip;
            }

            return null;
        }

        /// <summary>
        /// Tries to get an value from the animator in a gc friendly way.
        /// </summary>
        /// <param name="animator"></param>
        /// <param name="paramName">The param name</param>
        /// <param name="hash">The hash</param>
        /// <returns></returns>
        public static bool TryGetAnimatorParam(this Animator animator, string paramName, out int hash ) {
            
            // Cache params
            if (AnimatorParamCache.Count <= 0) {
                foreach (var param in animator.parameters) 
                    AnimatorParamCache[param.name] = param.nameHash;
            }

            if(AnimatorParamCache != null && AnimatorParamCache.TryGetValue( paramName, out hash)) return true;

            hash = 0;
            return false;
        }

        /// <summary>
        /// Sets a float in the animator if its really in there.
        /// </summary>
        /// <param name="animator"></param>
        /// <param name="paramName"></param>
        /// <param name="v"></param>
        public static void SetFloatParamWithCheck(this Animator animator, string paramName, float v ) {
            
            // animator defined elsewhere.
            if(TryGetAnimatorParam(animator, paramName, out var hash)) 
                animator.SetFloat( hash, v );
        }
        
        /// <summary>
        /// Sets a float in the animator if its really in there.
        /// </summary>
        /// <param name="animator"></param>
        /// <param name="paramName"></param>
        /// <param name="v"></param>
        public static void SetBoolParamWithCheck(this Animator animator, string paramName, bool v) {
            
            // animator defined elsewhere.
            if(TryGetAnimatorParam(animator, paramName, out var hash)) 
                animator.SetBool( hash, v );
        }

        /// <summary>
        /// Returns the name of an <see cref="AnimationClip"/> by using an cache to prevent allocation by <see cref="UnityEngine.Object.name"/> each call
        /// </summary>
        /// <param name="animationClip"></param>
        /// <returns></returns>
        public static string GetNameCached(this AnimationClip animationClip) {

            if (AnimationClipNameCache.ContainsKey(animationClip)) return AnimationClipNameCache[animationClip];

            var name = animationClip.name;
            AnimationClipNameCache[animationClip] = name;

            return name;
        }

        /// <summary>
        /// A cache for looking up the names of <see cref="AnimationClip"/>'s used in our system to prevent allocation by <see cref="UnityEngine.Object.name"/>
        /// </summary>
        private static IDictionary<AnimationClip, string> AnimationClipNameCache { get; set; } = new Dictionary<AnimationClip, string>();
        
        /// <summary>
        /// Acts as an cache for <see cref="Animator.parameters"/> because each get operation on them causes gc allocs.
        /// </summary>
        private static IDictionary<string,int> AnimatorParamCache { get; set; } = new Dictionary<string,int>( ); // <key=paramname,value=hash>
    }
}