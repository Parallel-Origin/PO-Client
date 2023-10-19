using System.Collections.Generic;
using ParallelOrigin.Core.Base.Interfaces;

namespace Script.Client.Internal_Database.Structure.Interfaces {
    
    /// <summary>
    ///     Represents a interface for game content assets in databases.
    ///     Its purpose is to store <see cref="Content" /> for a dynamic configuration of ingame entitys.
    /// </summary>
    public interface IContentStorage : ITypeable {

        /// <summary>
        ///     Searches for a certain <see cref="Content" />-Type in the local <see cref="ContentStorage.Components" /> and returns it
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T Get<T>() where T : Content;

        /// <summary>
        ///     Returns the category this content belongs to
        /// </summary>
        string Category { get; set; } // The Category, to which this content belongs to... Mobs... Buildings... etc

        /// <summary>
        ///     Attached Content to this Database-Entity
        /// </summary>
        IList<Content> Components { get; set; }
    }
}