using System;
using System.Collections.Generic;

namespace Script.Extensions {
    
    /// <summary>
    /// Extensions for functional programming patterns like func, action and similar delegates
    /// </summary>
    public static class FunctionalExtensions {

        /// <summary>
        /// Checks if all <see cref="Func{TResult}"/> are true, otherwhise it returns false. 
        /// </summary>
        /// <param name="conditions"></param>
        /// <param name="input"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool Evaluate<T>(this List<Func<T, bool>> conditions, ref T input) {

            for (var index = 0; index < conditions.Count; index++) {

                var condition = conditions[index];
                if (!condition(input)) return false;
            }

            return true;
        }
    }
}