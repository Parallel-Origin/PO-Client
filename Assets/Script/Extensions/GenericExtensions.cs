using System;
using System.Linq;
using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Entities.Data;
using Sfs2X.Requests;

namespace Script.Extensions {
    
    /// <summary>
    /// A extension containing methods for reflection and generics
    /// </summary>
    public static class GenericExtensions {

        /// <summary>
        /// Checks if the first type is fully assignable to the second one by its generics.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static bool IsAssignableGenerics(this Type type, Type second) {

            return second.GetInterfaces()
                .Where(i => i.IsGenericType)
                .Any(i => i.GetGenericTypeDefinition() == type);
        }
        
        /// <summary>
        /// Constructs a class instance from its path and returns it
        /// </summary>
        /// <param name="strFullyQualifiedName"></param>
        /// <returns></returns>
        public static object GetInstance(this string strFullyQualifiedName) {     
            
            var type = Type.GetType(strFullyQualifiedName);
            return GetInstance(type);
        }
        
        /// <summary>
        /// Constructs a class instance by its path with a certain generic type
        /// </summary>
        /// <param name="strFullyQualifiedName"></param>
        /// <param name="generic"></param>
        /// <returns></returns>
        public static object GetInstance(this string strFullyQualifiedName, Type generic) {     
            
            var type = Type.GetType(strFullyQualifiedName);
            return GetInstance(type, generic);
        }
        
        /// <summary>
        /// Constructs a class instance by its type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object GetInstance(this Type type) {
            return GetInstance(type, typeof(object));
        }
        
        /// <summary>
        /// Constructs a class instance by its type and generic 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="generic"></param>
        /// <returns></returns>
        public static object GetInstance(this Type type, Type generic) {     
            
            if (type.IsGenericType) {
                
                Type[] typeArgs = { generic };
                var genericType = type.MakeGenericType(typeArgs);
                return Activator.CreateInstance(genericType);
            }

            return Activator.CreateInstance(type);         
        }
        
        /// <summary>
        /// Constructs a class instance by its type and its defined generics.
        /// If the type is already a List<short> it will just instantiate that one and return the instance
        /// </summary>
        /// <param name="type"></param>
        /// <param name="generic"></param>
        /// <returns></returns>
        public static object GetDefinedInstance(this Type type) {
            return Activator.CreateInstance(type);   
        }
        
        /// <summary>
        /// Checks if a value is Boxed.
        /// </summary>
        /// <param name="value"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool IsBoxed<T>(ref T value) {
            
            return 
                (typeof(T).IsInterface || typeof(T) == typeof(object)) &&
                value != null &&
                value.GetType().IsValueType;
        }
    }
}