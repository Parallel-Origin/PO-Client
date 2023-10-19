using System.Collections.Generic;
using ParallelOrigin.Core.ECS.Components;
using Script.Client.Mono.Utils.Login;
using Unity.Collections;
using Unity.Entities;

namespace Script.Extensions {

    /// <summary>
    ///     An extension for the <see cref="EntityManager" /> that adds several search and filter methods for monobehaviour useage.
    /// </summary>
    public static class EntityManagerFindExtension {

        /// <summary>
        ///     Searches and returns an <see cref="Entity" /> by its <see cref="Identity" /> id from the server.
        /// </summary>
        /// <param name="em"></param>
        /// <param name="id">The id we search the entity for</param>
        /// <returns>The entity that has the <see cref="Identity" /> with the id attached </returns>
        public static Entity FindById(this ref EntityManager em, long id) {

            // Cache the identity query aswell as the found entity.
            if (IdentityQuery == default) 
                IdentityQuery = em.CreateEntityQuery(typeof(Identity));
            
            // Cache the entity found 
            if (CachedEntities.ContainsKey(id)) {
                
                var cachedEntity = CachedEntities[id];
                if (em.Exists(cachedEntity)) 
                    return cachedEntity;
                
                CachedEntities.Remove(id); // Remove cached it and entity because entity is not valid anymroe
            }
            
            // Search for the entity with the id
            var identities = IdentityQuery.ToEntityArray(Allocator.TempJob);
            foreach (var entity in identities) {
                
                var identity = em.GetComponentData<Identity>(entity);
                if (!identity.ID.Equals(id)) continue;
                
                identities.Dispose();
                CachedEntities.Add(id, entity);
                
                return entity;
            }

            identities.Dispose();
            return Entity.Null;
        }

        /// <summary>
        /// Searches and returns an <see cref="Entity" /> as a <see cref="Character"/> by its username
        /// </summary>
        /// <param name="em"></param>
        /// <param name="playerName">The players name</param>
        /// <returns></returns>
        public static Entity FindByName(this ref EntityManager em, string playerName) {
            
            if (PlayerQuery == default) PlayerQuery = em.CreateEntityQuery(typeof(Character));

            var player = Entity.Null;
            var identities = PlayerQuery.ToEntityArray(Allocator.TempJob);
            foreach (var entity in identities) {
                
                var playerCmp = em.GetComponentData<Character>(entity);
                if (playerCmp.Name.EqualsStack(playerName))
                    player = entity;
            }
            identities.Dispose();

            return player;
        }

        /// <summary>
        ///     Searches and returns an <see cref="Entity" /> that represents the <see cref="LocalPlayer" />
        /// </summary>
        /// <param name="em"></param>
        /// <returns>The entity that is the <see cref="LocalPlayer" /></returns>
        public static Entity FindLocalPlayer(this ref EntityManager em) {
            
            var possibleLocalPlayer = em.FindByName(UserCredentials.GetUsername());
            return possibleLocalPlayer;
        }

        /// <summary>
        ///     Finds all <see cref="Entity" />'s of a certain aspect and returns them in a list.
        /// </summary>
        /// <param name="em"></param>
        /// <param name="types">The aspect composition of entities we search for</param>
        /// <returns></returns>
        public static IEnumerable<Entity> Find(this ref EntityManager em, params ComponentType[] types) {

            EntityQuery query = default;

            if (CachedQueries == null) CachedQueries = new Dictionary<ComponentType[], EntityQuery> {{types, em.CreateEntityQuery(types)}};
            if (CachedQueries.ContainsKey(types)) query = CachedQueries[types];

            var entities = query.ToEntityArray(Allocator.TempJob);
            var entityList = new HashSet<Entity>(entities);
            entities.Dispose();

            return entityList;
        }
        
        /// <summary>
        /// Finds an prefab entity by its type and returns it. 
        /// </summary>
        /// <param name="em"></param>
        /// <param name="typeID"></param>
        /// <returns></returns>
        public static Entity FindPrefab(this ref EntityManager em, string typeID) {

            if(PrefabQuery == default) 
                PrefabQuery = em.CreateEntityQuery(typeof(Identity),typeof(Prefab));

            var entities = PrefabQuery.ToEntityArray(Allocator.TempJob);
            var identities = PrefabQuery.ToComponentDataArray<Identity>(Allocator.TempJob);
            
            for (var index = 0; index < entities.Length; index++) {

                var entity = entities[index];
                var identity = identities[index];

                if (identity.Type.Equals(typeID)) {

                    entities.Dispose();
                    identities.Dispose();
                    return entity;
                }
            }

            entities.Dispose();
            identities.Dispose();
            return Entity.Null;
        }
        
        
        /// <summary>
        /// A query to get all <see cref="Prefab"/> marked entities for lookups. 
        /// </summary>
        public static EntityQuery PrefabQuery { get; set; }

        /// <summary>
        ///     A cached <see cref="EntityQuery" /> that contains all <see cref="Identity" /> entities
        /// </summary>
        private static EntityQuery IdentityQuery { get; set; }


        /// <summary>
        ///     A cached <see cref="EntityQuery" /> that contains all <see cref="Identity" /> entities
        /// </summary>
        private static EntityQuery LocalPlayerQuery { get; set; }
        
        /// <summary>
        ///     A cached <see cref="EntityQuery" /> that contains all <see cref="Identity" /> entities
        /// </summary>
        private static EntityQuery PlayerQuery { get; set; }

        /// <summary>
        /// A cache for entities found by id... this way we dont need to iterate over all entities every find operation
        /// </summary>
        public static NativeParallelHashMap<long, Entity> CachedEntities { get; set; } = new NativeParallelHashMap<long, Entity>(512, Allocator.Persistent);
        
        /// <summary>
        /// A cache for buffered entities to find them by their id. 
        /// </summary>
        public static NativeParallelHashMap<long, Entity> BufferedEntity { get; set; } = new NativeParallelHashMap<long, Entity>(512, Allocator.Persistent);
        
        /// <summary>
        ///     A dictionary which stores cached queries for this extension.
        /// </summary>
        private static Dictionary<ComponentType[], EntityQuery> CachedQueries { get; set; }
    }
}