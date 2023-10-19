using System;
using System.Collections.Generic;
using System.Linq;
using RotaryHeart.Lib.SerializableDictionary;
using Script.Client.Internal_Database.Structure.Interfaces;
using UnityEngine;

namespace Script.Client.Internal_Database.Structure {

    /// <summary>
    ///     Represents a reference towards another InternalDatabase for recursive hierarchies.
    /// </summary>
    [Serializable]
    public class InternalContentReference : SerializableDictionaryBase<string, InternalDatabase> { }


    //////////////////////////////////
    /// Internal Database
    //////////////////////////////////


    /// <summary>
    ///     Represents a internal database with the ability to reference towards
    /// </summary>
    [CreateAssetMenu(menuName = "ParallelOrigin/Internal Database/Internal Database")]
    public class InternalDatabase : ScriptableObject, IInternalDatabase {

        [SerializeField] public InternalContentReference internalContentReference; // Recursion
        [SerializeField] public List<ContentStorage> content; // Content Storage, for the Database Assets

        ////////////////////////////////
        /// GameContent methods
        ////////////////////////////////


        /// <summary>
        ///     Used to sum up all game content from the recursive linked internal databases and returns the result.
        /// </summary>
        /// <returns></returns>
        public virtual IList<IContentStorage> GetContentStorages() {

            // Cache the storage and return that one to prevent unnesecary allocations. 
            if (CachedGetContentStorages.Count > 0) return CachedGetContentStorages;
            
            // Gather all recursive other internal content storages.
            var gameContent = new List<IContentStorage>(content);
            foreach (var internalDatabase in internalContentReference.Values) {

                if (internalDatabase == null) continue;
                gameContent.AddRange(internalDatabase.GetContentStorages());
            }

            CachedGetContentStorages = gameContent;
            return CachedGetContentStorages;
        }

        /// <summary>
        ///     Used to sum up all game content from the recursive linked interal databases and return it as a easy acesseable and fast dictionary.
        /// </summary>
        /// <returns></returns>
        public virtual IDictionary<long, IContentStorage> GetContentStoragesDictionary() {

            if (Cache.Count > 0) return Cache;

            var gameContent = new Dictionary<long, IContentStorage>();
            foreach (var contentStorage in GetContentStorages()) gameContent.Add(contentStorage.TypeId, contentStorage);

            Cache = gameContent;
            return Cache;
        }


        ////////////////////////////////
        /// InternalDatabase methods
        ////////////////////////////////


        /// <summary>
        ///     Searches for a certain internal database by its key or name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual IInternalDatabase GetDatabase(string name, bool assetName = false) {

            if (!assetName && internalContentReference.ContainsKey(name)) return internalContentReference[name];

            if (assetName)
                foreach (var existingDatabase in internalContentReference.Values)
                    if (existingDatabase.name.Equals(name))
                        return existingDatabase;

            foreach (var database in internalContentReference.Values)
                if (database.GetDatabase(name, assetName) != null)
                    return database.GetDatabase(name, assetName);

            return null;
        }


        ////////////////////////////////
        /// Simplify methods
        ////////////////////////////////

        /// <summary>
        ///     Searches for a type in the game contents and returns it.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual IContentStorage GetContentStorageByType(Type type) {

            foreach (var gameContent in GetContentStorages())
                if (gameContent.GetType() == type)
                    return gameContent;

            return null;
        }

        /// <summary>
        ///     Searches for a <see cref="ContentStorage" /> by its id and returns it promptly.
        /// </summary>
        /// <param name="type">The id of the content storage we search for</param>
        /// <returns></returns>
        public virtual IContentStorage GetContentStorage(long type) { return GetContentStoragesDictionary()[type]; }

        /// <summary>
        ///     Searches for a <see cref="ContentStorage" /> by its id and returns an attached component <see cref="T"/>
        /// </summary>
        /// <param name="type">The id of the content storage we search for</param>
        /// <returns></returns>
        public virtual T GetFromContentStorage<T>(long type) where T : Content{

            var contentStorage = GetContentStorage(type);
            return contentStorage.Get<T>();
        }

        /// <summary>
        ///     Searches for the certain database and returns the requested GameContent - ID from it.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual IContentStorage GetFromDatabase(string name, long id) {

            var database = GetDatabase(name);
            return database.GetContentStoragesDictionary()[id];
        }

        /// <summary>
        ///     Searches for the certain database and returns the requested type from its game contents.
        /// </summary>
        /// <param name="name"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual IContentStorage GetFromDatabase(string name, Type type) {

            var database = GetDatabase(name);
            return database.GetContentStorageByType(type);
        }

        /// <summary>
        /// Cached <see cref="IContentStorage"/> to prevent a new list allocatio each <see cref="GetContentStorage"/> operation
        /// </summary>
        private IList<IContentStorage> CachedGetContentStorages { get; set; } = new List<IContentStorage>();
        
        /// <summary>
        /// Cached <see cref="IContentStorage"/> to prevent a new dictionary allocation each <see cref="GetContentStoragesDictionary"/>
        /// </summary>
        private IDictionary<long, IContentStorage> Cache { get; set; } = new Dictionary<long, IContentStorage>();
    }
}