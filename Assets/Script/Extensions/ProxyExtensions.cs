using Sfs2X.Entities;
using Sfs2X.Entities.Variables;

namespace Script.Extensions {
    
    /// <summary>
    /// An extension for the <see cref="IMMOItem"/>
    /// </summary>
    public static class ProxyExtensions {

        /// <summary>
        /// Extracts the id of an entity from a <see cref="IMMOItem"/> and return either that id or zero
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static long GetId(this IMMOItem item) {

            if (!item.ContainsVariable("id")) return -1;
            
            var idVariable = item.GetVariable("id");
            var id = idVariable.Type == VariableType.STRING ? long.Parse(idVariable.GetStringValue()) : (long)idVariable.GetDoubleValue();

            return id;
        }
        
        /// <summary>
        /// Extracts the id of an entity from a <see cref="IMMOItem"/> and return either that id or zero
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static long GetId(this User user) {

            if (!user.ContainsVariable("id")) return -1;
            
            var idVariable = user.GetVariable("id");
            var id = idVariable.Type == VariableType.STRING ? long.Parse(idVariable.GetStringValue()) : (long)idVariable.GetDoubleValue();

            return id;
        }
    }
}