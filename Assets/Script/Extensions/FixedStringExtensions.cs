using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.Collections;
using UnityEngine.VFX;

namespace Script.Extensions {
    
    /// <summary>
    /// An extension with several utils for <see cref="FixedString32"/> and other fixedStrings
    /// </summary>
    public static class FixedStringExtensions {

        /// <summary>
        /// Compares a <see cref="FixedString32"/> with a <see cref="string"/> byte for byte to determine if they are equal or not in value. 
        /// </summary>
        /// <param name="fs"></param>
        /// <param name="toEqual"></param>
        /// <returns></returns>
        public static bool EqualsStack(this ref FixedString32Bytes fs, string toEqual) {

            if (fs.Length != toEqual.Length) return false;
            for (var index = 0; index < fs.Length; index++) {

                var fsByte = fs[index];
                var toEqualByte = (byte)toEqual[index];
                if (fsByte != toEqualByte) return false;
            }

            return true;
        }
        
        /// <summary>
        /// Compares a <see cref="FixedString32"/> with a <see cref="string"/> byte for byte to determine if they are equal or not in value. 
        /// </summary>
        /// <param name="fs"></param>
        /// <param name="toEqual"></param>
        /// <returns></returns>
        public static bool EqualsStack(this ref FixedString32Bytes fs, string toEqual, StringComparison comparison) {

            if (fs.Length != toEqual.Length) return false;
            for (var index = 0; index < fs.Length; index++) {

                var fsByte = Convert.ToChar(fs[index]);
                var toEqualByte = toEqual[index];

                // Check for lower case comparison if wanted
                if (comparison == StringComparison.CurrentCultureIgnoreCase) {
                    
                    var fsLower = char.ToLower(fsByte);
                    var toEqualLower = char.ToLower(toEqualByte);
                    
                    if (fsLower != toEqualLower) 
                        return false;
                }
                else if(fsByte != toEqualByte) return false;
            }
            
            return true;
        }

        /// <summary>
        /// Converts a <see cref="FixedString32"/> into a string by using a cache.
        /// Less GC and faster for lookups and stuff.
        /// </summary>
        /// <param name="fs"></param>
        /// <returns></returns>
        public static string ToStringCached(this in FixedString32Bytes fs) {

            if (FixedString32Cache.ContainsKey(fs)) 
                return FixedString32Cache[fs];
            
            var str = fs.ToString();
            FixedString32Cache[fs] = str;
            return str;
        }
        
        /// <summary>
        /// Acts as a Cache used when we convert a <see cref="FixedString32"/> into a string to prevent gc.
        /// </summary>
        public static Dictionary<FixedString32Bytes, string> FixedString32Cache { get; set; } = new Dictionary<FixedString32Bytes, string>(64);

    }
}