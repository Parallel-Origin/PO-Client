using System.Reflection;

namespace Script.Extensions {
    
    /// <summary>
    /// An extension which adds some helpfull methods for reflections.
    /// </summary>
    public static class ReflectionExtensions {

        /// <summary>
        /// Returns the property instance, if its null it assigns/sets a newly created one.
        /// </summary>
        /// <param name="property"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static object GetOrCreateValue(this PropertyInfo property, object instance) {
            
            var propertyInstance = property.GetValue(instance, null);
            if (propertyInstance == null) {
                propertyInstance = property.PropertyType.GetDefinedInstance();
                property.SetValue(instance, propertyInstance);
            }

            return propertyInstance;
        }
        
        /// <summary>
        /// Returns the field instance, if its null it assigns/sets a newly created one.
        /// </summary>
        /// <param name="field"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static object GetOrCreateValue(this FieldInfo field, object instance) {
            
            // Get existing field instance or create a new one and deserialize it
            var fieldInstance = field.GetValue(instance);
            if (fieldInstance == null) {
                fieldInstance = field.FieldType.GetDefinedInstance();
                field.SetValue(instance, fieldInstance);
            }

            return fieldInstance;
        }
    }
}