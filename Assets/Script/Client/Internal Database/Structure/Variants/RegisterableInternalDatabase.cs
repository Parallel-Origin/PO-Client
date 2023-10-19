using UnityEngine;

namespace Script.Client.Internal_Database.Structure.Variants {
    /// <summary>
    ///     Represents a internal database scriptable object wich registers itself
    ///     to the global registers for easy acess without singletons.
    ///     Only one instance of this may exist for marking the root of the <see cref="InternalDatabase" />
    /// </summary>
    [CreateAssetMenu(menuName = "ParallelOrigin/Internal Database/Register Internal Database")]
    public class RegisterableInternalDatabase : InternalDatabase, IRegisterableInternalDatabase { }
}