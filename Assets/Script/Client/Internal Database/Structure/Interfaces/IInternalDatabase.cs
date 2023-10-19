using System;
using System.Collections.Generic;

namespace Script.Client.Internal_Database.Structure.Interfaces {
    
    /// <summary>
    ///     Provides the structure and methods for every internal database class.
    /// </summary>
    public interface IInternalDatabase {
        
        /// <summary>
        ///     Used to sum up all game content from the recursive linked internal databases and returns the result.
        /// </summary>
        /// <returns></returns>
        IList<IContentStorage> GetContentStorages();

        /// <summary>
        ///     Used to sum up all game content from the recursive linked interal databases and return it as a easy acesseable and fast dictionary.
        /// </summary>
        /// <returns></returns>
        IDictionary<long, IContentStorage> GetContentStoragesDictionary();

        /// <summary>
        ///     Searches for a type in the game contents and returns it.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IContentStorage GetContentStorageByType(Type type);

        ////////////////////////////////
        /// InternalDatabase methods
        ////////////////////////////////


        /// <summary>
        ///     Searches for a certain internal database by its key or name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        IInternalDatabase GetDatabase(string name, bool assetName = false);

        ////////////////////////////////
        /// Simplify methods
        ////////////////////////////////

        /// <summary>
        ///     Searches for the certain database and returns the requested GameContent - ID from it.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        IContentStorage GetFromDatabase(string name, long id);

        /// <summary>
        ///     Searches for the certain database and returns the requested type from its game contents.
        /// </summary>
        /// <param name="name"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IContentStorage GetFromDatabase(string name, Type type);

        /// <summary>
        ///     Searches for a <see cref="ContentStorage" /> by its id and returns it promptly.
        /// </summary>
        /// <param name="type">The id of the content storage we search for</param>
        /// <returns></returns>
        IContentStorage GetContentStorage(long type);
        
        /// <summary>
        ///     Searches for a <see cref="ContentStorage" /> by its id and returns an attached component <see cref="T"/>
        /// </summary>
        /// <param name="type">The id of the content storage we search for</param>
        /// <returns></returns>
        T GetFromContentStorage<T>(long type) where T : Content;
    }
}