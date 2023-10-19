using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using LiteNetLib.Utils;
using ParallelOrigin.Core.ECS.Components;
using ParallelOrigin.Core.Network;
using Unity.Entities;
using UnityEngine;

[assembly: InternalsVisibleTo("Unity.Entities")]
namespace Script.Extensions {
    
    /// <summary>
    /// A extension for the <see cref="EntityManager"/> which contains several methods for converting <see cref="Entity"/>'s or components. 
    /// </summary>
    public static class EntityManagerConvertExtension {
        
        /// <summary>
        /// Converts a network <see cref="EntityCommand"/> to an <see cref="EcsCommand"/> and aplies it to the local ecs. 
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="command"></param>
        public static void ConvertToEcs(this ref EntityManager manager, ref EntityCommand command) {

            // Create identity and run the ecs command
            var entity = manager.FindById(command.Id); 
            
            var identity = new Identity { ID = command.Id, Tag = "", Type = command.Type };
            var ecsCommand = new EcsCommand<Identity>{Opcode = (Operation)command.Opcode, TypeID = command.Type, Entity = entity, Data = identity};
            entity = manager.Process(ref ecsCommand);
            
            try {
                if (entity.Index <= 0)
                    EntityManagerFindExtension.BufferedEntity.Add(command.Id, entity);
            }
            catch (Exception e) {
                Debug.Log($"ERROR {command.Id}/{command.Type}: "+e);
            }
        }
        
        /// <summary>
        /// Converts a network <see cref="EntityCommand"/> to an <see cref="EcsCommand"/> and aplies it to the local ecs. 
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="command"></param>
        public static void ConvertToEcs<T1>(this ref EntityManager manager, ref EntityCommand<T1> command) where T1 : struct, IComponentData, INetSerializable {

            // Unpack command
            ref var com = ref command.Command;
            ref var t1 = ref command.T1Component;

            var entity = manager.FindById(com.Id); 
            
            // Run command
            var ecsCommand = new EcsCommand<T1>{Opcode = (Operation)com.Opcode, TypeID = com.Type, Entity = entity,Data = t1};
            manager.Process(ref ecsCommand);
        }
        
        /// <summary>
        /// Converts a network <see cref="EntityCommand"/> to an <see cref="EcsCommand"/> and aplies it to the local ecs. 
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="command"></param>
        public static void ConvertToEcs<T1, T2>(this ref EntityManager manager, ref EntityCommand<T1,T2> command) where T1 : struct, IComponentData, INetSerializable where T2 : struct, IComponentData, INetSerializable {

            // Unpack command
            ref var com = ref command.Command;
            ref var t1 = ref command.T1Component;
            ref var t2 = ref command.T2Component;

            var entity = manager.FindById(com.Id); 
            
            // Run command
            var t1EcsCommand = new EcsCommand<T1>{Opcode = (Operation)com.Opcode, TypeID = com.Type, Entity = entity, Data = t1};
            entity = manager.Process(ref t1EcsCommand);
            
            var t2EcsCommand = new EcsCommand<T2>{Opcode = (Operation)com.Opcode, TypeID = com.Type, Entity = entity, Data = t2};
            manager.Process(ref t2EcsCommand);
        }
        
        /// <summary>
        /// Converts a network <see cref="EntityCommand"/> to an <see cref="EcsCommand"/> and aplies it to the local ecs. 
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="command"></param>
        public static void ConvertToEcs<T1, T2, T3>(this ref EntityManager manager, ref EntityCommand<T1,T2,T3> command) where T1 : struct, IComponentData, INetSerializable where T2 : struct, IComponentData, INetSerializable where T3 : struct, IComponentData, INetSerializable {

            // Unpack command
            ref var com = ref command.Command;
            ref var t1 = ref command.T1Component;
            ref var t2 = ref command.T2Component;
            ref var t3 = ref command.T3Component;
            
            var entity = manager.FindById(com.Id); 
            
            // Run command
            var t1EcsCommand = new EcsCommand<T1>{Opcode = (Operation)com.Opcode, TypeID = com.Type, Entity = entity,Data= t1};
            entity = manager.Process(ref t1EcsCommand);
            
            var t2EcsCommand = new EcsCommand<T2>{Opcode = (Operation)com.Opcode, TypeID = com.Type, Entity = entity,Data= t2};
            var t3EcsCommand = new EcsCommand<T3>{Opcode = (Operation)com.Opcode, TypeID = com.Type, Entity = entity,Data= t3};
            
            manager.Process(ref t2EcsCommand);
            manager.Process(ref t3EcsCommand);
        }
        
        /// <summary>
        /// Converts a network <see cref="EntityCommand"/> to an <see cref="EcsCommand"/> and aplies it to the local ecs. 
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="command"></param>
        public static void ConvertToEcs<T1, T2, T3, T4>(this ref EntityManager manager, ref EntityCommand<T1,T2,T3,T4> command) where T1 : struct, IComponentData, INetSerializable where T2 : struct, IComponentData, INetSerializable where T3 : struct, IComponentData, INetSerializable where T4 : struct, IComponentData, INetSerializable {

            // Unpack command
            ref var com = ref command.Command;
            ref var t1 = ref command.T1Component;
            ref var t2 = ref command.T2Component;
            ref var t3 = ref command.T3Component;
            ref var t4 = ref command.T4Component;
            
            var entity = manager.FindById(com.Id); 
            
            // Run command once ( May create an entity if it does not exist yet... so we save that entity  and run the others later... otherwhise each component command spawns one new entity )
            var t1EcsCommand = new EcsCommand<T1>{Opcode = (Operation)com.Opcode, TypeID = com.Type, Entity = entity,Data= t1};
            entity = manager.Process(ref t1EcsCommand);
            
            var t2EcsCommand = new EcsCommand<T2>{Opcode = (Operation)com.Opcode, TypeID = com.Type, Entity = entity,Data= t2};
            var t3EcsCommand = new EcsCommand<T3>{Opcode = (Operation)com.Opcode, TypeID = com.Type, Entity = entity,Data= t3};
            var t4EcsCommand = new EcsCommand<T4>{Opcode = (Operation)com.Opcode, TypeID = com.Type, Entity = entity,Data= t4};
            
            manager.Process(ref t2EcsCommand);
            manager.Process(ref t3EcsCommand);
            manager.Process(ref t4EcsCommand);
        }

        /// <summary>
        /// Converts a network <see cref="EntityCommand"/> to an <see cref="EcsCommand"/> and aplies it to the local ecs. 
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="command"></param>
        public static void ConvertToEcs<T1, T2, T3, T4, T5>(this ref EntityManager manager, ref EntityCommand<T1,T2,T3,T4,T5> command) where T1 : struct, IComponentData, INetSerializable where T2 : struct, IComponentData, INetSerializable where T3 : struct, IComponentData, INetSerializable where T4 : struct, IComponentData, INetSerializable where T5 : struct, IComponentData, INetSerializable {

            // Unpack command
            ref var com = ref command.Command;
            ref var t1 = ref command.T1Component;
            ref var t2 = ref command.T2Component;
            ref var t3 = ref command.T3Component;
            ref var t4 = ref command.T4Component;
            ref var t5 = ref command.T5Component;
            
            var entity = manager.FindById(com.Id); 
            
            // Run command once ( May create an entity if it does not exist yet... so we save that entity  and run the others later... otherwhise each component command spawns one new entity )
            var t1EcsCommand = new EcsCommand<T1>{Opcode = (Operation)com.Opcode, TypeID = com.Type, Entity = entity,Data= t1};
            entity = manager.Process(ref t1EcsCommand);
            
            var t2EcsCommand = new EcsCommand<T2>{Opcode = (Operation)com.Opcode, TypeID = com.Type, Entity = entity,Data= t2};
            var t3EcsCommand = new EcsCommand<T3>{Opcode = (Operation)com.Opcode, TypeID = com.Type, Entity = entity,Data= t3};
            var t4EcsCommand = new EcsCommand<T4>{Opcode = (Operation)com.Opcode, TypeID = com.Type, Entity = entity,Data= t4};
            var t5EcsCommand = new EcsCommand<T5>{Opcode = (Operation)com.Opcode, TypeID = com.Type, Entity = entity,Data= t5};
            
            manager.Process(ref t2EcsCommand);
            manager.Process(ref t3EcsCommand);
            manager.Process(ref t4EcsCommand);
            manager.Process(ref t5EcsCommand);
        }
        
        /// <summary>
        /// Converts a network <see cref="EntityCommand"/> to an <see cref="EcsCommand"/> and aplies it to the local ecs. 
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="command"></param>
        public static void ConvertToEcs<T1, T2, T3, T4, T5, T6>(this ref EntityManager manager, ref EntityCommand<T1,T2,T3,T4,T5,T6> command) where T1 : struct, IComponentData, INetSerializable where T2 : struct, IComponentData, INetSerializable where T3 : struct, IComponentData, INetSerializable where T4 : struct, IComponentData, INetSerializable where T5 : struct, IComponentData, INetSerializable where T6 : struct, IComponentData, INetSerializable {

            // Unpack command
            ref var com = ref command.Command;
            ref var t1 = ref command.T1Component;
            ref var t2 = ref command.T2Component;
            ref var t3 = ref command.T3Component;
            ref var t4 = ref command.T4Component;
            ref var t5 = ref command.T5Component;
            ref var t6 = ref command.T6Component;
            
            var entity = manager.FindById(com.Id); 
            
            // Run command once ( May create an entity if it does not exist yet... so we save that entity  and run the others later... otherwhise each component command spawns one new entity )
            var t1EcsCommand = new EcsCommand<T1>{Opcode = (Operation)com.Opcode, TypeID = com.Type, Entity = entity,Data= t1};
            entity = manager.Process(ref t1EcsCommand);
            
            var t2EcsCommand = new EcsCommand<T2>{Opcode = (Operation)com.Opcode, TypeID = com.Type, Entity = entity,Data= t2};
            var t3EcsCommand = new EcsCommand<T3>{Opcode = (Operation)com.Opcode, TypeID = com.Type, Entity = entity,Data= t3};
            var t4EcsCommand = new EcsCommand<T4>{Opcode = (Operation)com.Opcode, TypeID = com.Type, Entity = entity,Data= t4};
            var t5EcsCommand = new EcsCommand<T5>{Opcode = (Operation)com.Opcode, TypeID = com.Type, Entity = entity,Data= t5};
            var t6EcsCommand = new EcsCommand<T6>{Opcode = (Operation)com.Opcode, TypeID = com.Type, Entity = entity,Data= t6};
            
            manager.Process(ref t2EcsCommand);
            manager.Process(ref t3EcsCommand);
            manager.Process(ref t4EcsCommand);
            manager.Process(ref t5EcsCommand);
            manager.Process(ref t6EcsCommand);
        }
        
         /// <summary>
        /// Converts a network <see cref="EntityCommand"/> to an <see cref="EcsCommand"/> and aplies it to the local ecs. 
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="command"></param>
        public static void ConvertToEcs<T1, T2, T3, T4, T5, T6, T7>(this ref EntityManager manager, ref EntityCommand<T1,T2,T3,T4,T5,T6,T7> command) where T1 : struct, IComponentData, INetSerializable where T2 : struct, IComponentData, INetSerializable where T3 : struct, IComponentData, INetSerializable where T4 : struct, IComponentData, INetSerializable where T5 : struct, IComponentData, INetSerializable where T6 : struct, IComponentData, INetSerializable where T7 : struct, IComponentData, INetSerializable {

            // Unpack command
            ref var com = ref command.Command;
            ref var t1 = ref command.T1Component;
            ref var t2 = ref command.T2Component;
            ref var t3 = ref command.T3Component;
            ref var t4 = ref command.T4Component;
            ref var t5 = ref command.T5Component;
            ref var t6 = ref command.T6Component;
            ref var t7 = ref command.T7Component;
            
            var entity = manager.FindById(com.Id); 
            
            // Run command once ( May create an entity if it does not exist yet... so we save that entity  and run the others later... otherwhise each component command spawns one new entity )
            var t1EcsCommand = new EcsCommand<T1>{Opcode = (Operation)com.Opcode, TypeID = com.Type, Entity = entity,Data= t1};
            entity = manager.Process(ref t1EcsCommand);
            
            var t2EcsCommand = new EcsCommand<T2>{Opcode = (Operation)com.Opcode, TypeID = com.Type, Entity = entity,Data= t2};
            var t3EcsCommand = new EcsCommand<T3>{Opcode = (Operation)com.Opcode, TypeID = com.Type, Entity = entity,Data= t3};
            var t4EcsCommand = new EcsCommand<T4>{Opcode = (Operation)com.Opcode, TypeID = com.Type, Entity = entity,Data= t4};
            var t5EcsCommand = new EcsCommand<T5>{Opcode = (Operation)com.Opcode, TypeID = com.Type, Entity = entity,Data= t5};
            var t6EcsCommand = new EcsCommand<T6>{Opcode = (Operation)com.Opcode, TypeID = com.Type, Entity = entity,Data= t6};
            var t7EcsCommand = new EcsCommand<T7>{Opcode = (Operation)com.Opcode, TypeID = com.Type, Entity = entity,Data= t7};
            
            manager.Process(ref t2EcsCommand);
            manager.Process(ref t3EcsCommand);
            manager.Process(ref t4EcsCommand);
            manager.Process(ref t5EcsCommand);
            manager.Process(ref t6EcsCommand);
            manager.Process(ref t7EcsCommand);
        }
        
                  /// <summary>
        /// Converts a network <see cref="EntityCommand"/> to an <see cref="EcsCommand"/> and aplies it to the local ecs. 
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="command"></param>
        public static void ConvertToEcs<T1, T2, T3, T4, T5, T6, T7, T8>(this ref EntityManager manager, ref EntityCommand<T1,T2,T3,T4,T5,T6,T7,T8> command) where T1 : struct, IComponentData, INetSerializable where T2 : struct, IComponentData, INetSerializable where T3 : struct, IComponentData, INetSerializable where T4 : struct, IComponentData, INetSerializable where T5 : struct, IComponentData, INetSerializable where T6 : struct, IComponentData, INetSerializable where T7 : struct, IComponentData, INetSerializable where T8 : struct, IComponentData, INetSerializable{

            // Unpack command
            ref var com = ref command.Command;
            ref var t1 = ref command.T1Component;
            ref var t2 = ref command.T2Component;
            ref var t3 = ref command.T3Component;
            ref var t4 = ref command.T4Component;
            ref var t5 = ref command.T5Component;
            ref var t6 = ref command.T6Component;
            ref var t7 = ref command.T7Component;
            ref var t8 = ref command.T8Component;
            
            var entity = manager.FindById(com.Id); 
            
            // Run command once ( May create an entity if it does not exist yet... so we save that entity  and run the others later... otherwhise each component command spawns one new entity )
            var t1EcsCommand = new EcsCommand<T1>{Opcode = (Operation)com.Opcode, TypeID = com.Type, Entity = entity,Data= t1};
            entity = manager.Process(ref t1EcsCommand);
            
            var t2EcsCommand = new EcsCommand<T2>{Opcode = (Operation)com.Opcode, TypeID = com.Type, Entity = entity,Data= t2};
            var t3EcsCommand = new EcsCommand<T3>{Opcode = (Operation)com.Opcode, TypeID = com.Type, Entity = entity,Data= t3};
            var t4EcsCommand = new EcsCommand<T4>{Opcode = (Operation)com.Opcode, TypeID = com.Type, Entity = entity,Data= t4};
            var t5EcsCommand = new EcsCommand<T5>{Opcode = (Operation)com.Opcode, TypeID = com.Type, Entity = entity,Data= t5};
            var t6EcsCommand = new EcsCommand<T6>{Opcode = (Operation)com.Opcode, TypeID = com.Type, Entity = entity,Data= t6};
            var t7EcsCommand = new EcsCommand<T7>{Opcode = (Operation)com.Opcode, TypeID = com.Type, Entity = entity,Data= t7};
            var t8EcsCommand = new EcsCommand<T8>{Opcode = (Operation)com.Opcode, TypeID = com.Type, Entity = entity,Data= t8}; 
            
            manager.Process(ref t2EcsCommand);
            manager.Process(ref t3EcsCommand);
            manager.Process(ref t4EcsCommand);
            manager.Process(ref t5EcsCommand);
            manager.Process(ref t6EcsCommand);
            manager.Process(ref t7EcsCommand);
            manager.Process(ref t8EcsCommand);
        }
         
        /// <summary>
        /// Converts an <see cref="ComponentCommand{T}"/> to an ecs operation. 
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="command"></param>
        /// <typeparam name="T"></typeparam>
        public static void ConvertToEcs<T>(this ref EntityManager manager, ref ComponentCommand<T> command) where T : struct, IComponentData, INetSerializable {

            // Either find existing entity, otherwhise use buffer to find the buffered entities. 
            var entity = manager.FindById(command.Id);
            if (entity == Entity.Null) EntityManagerFindExtension.BufferedEntity.TryGetValue(command.Id, out entity);
            
            var ecsCommand = new EcsCommand<T>{Opcode = (Operation)command.Opcode, TypeID = null, Entity = entity, Data= command.Component};
            manager.Process(ref ecsCommand);
        }
        
        /// <summary>
        /// Converts a network <see cref="EntityCommand"/> to an <see cref="EcsCommand"/> and aplies it to the local ecs. 
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="command"></param>
        public static void ConvertToEcs(this ref EntityManager manager, ref BatchCommand<EntityCommand> command) {

            // Unpack command
            for (var index = 0; index < command.Data.Length; index++) {

                ref var entityCommand = ref command.Data[index];
                manager.ConvertToEcs(ref entityCommand);
            }
        }
        
        /// <summary>
        /// Converts a network <see cref="EntityCommand"/> to an <see cref="EcsCommand"/> and aplies it to the local ecs. 
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="command"></param>
        public static void ConvertToEcs<T1>(this ref EntityManager manager, ref BatchCommand<EntityCommand<T1>> command) where T1 : struct, IComponentData, INetSerializable {

            // Unpack command
            for (var index = 0; index < command.Data.Length; index++) {

                ref var entityCommand = ref command.Data[index];
                manager.ConvertToEcs(ref entityCommand);
            }
        }
        
        /// <summary>
        /// Converts a network <see cref="EntityCommand"/> to an <see cref="EcsCommand"/> and aplies it to the local ecs. 
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="command"></param>
        public static void ConvertToEcs<T1, T2>(this ref EntityManager manager, ref BatchCommand<EntityCommand<T1, T2>> command) where T1 : struct, IComponentData, INetSerializable where T2 : struct, IComponentData, INetSerializable {

            // Unpack command
            for (var index = 0; index < command.Data.Length; index++) {

                ref var entityCommand = ref command.Data[index];
                manager.ConvertToEcs(ref entityCommand);
            }
        }
        
        /// <summary>
        /// Converts a network <see cref="EntityCommand"/> to an <see cref="EcsCommand"/> and aplies it to the local ecs. 
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="command"></param>
        public static void ConvertToEcs<T1, T2, T3, T4>(this ref EntityManager manager, ref BatchCommand<EntityCommand<T1,T2,T3, T4>> command) where T1 : struct, IComponentData, INetSerializable where T2 : struct, IComponentData, INetSerializable where T3 : struct, IComponentData, INetSerializable where T4 : struct, IComponentData, INetSerializable {

            // Unpack command
            for (var index = 0; index < command.Data.Length; index++) {

                ref var entityCommand = ref command.Data[index];
                manager.ConvertToEcs(ref entityCommand);
            }
        }
        
        /// <summary>
        /// Converts a network <see cref="EntityCommand"/> to an <see cref="EcsCommand"/> and aplies it to the local ecs. 
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="command"></param>
        public static void ConvertToEcs<T1, T2, T3, T4, T5>(this ref EntityManager manager, ref BatchCommand<EntityCommand<T1,T2,T3, T4, T5>> command) where T1 : struct, IComponentData, INetSerializable where T2 : struct, IComponentData, INetSerializable where T3 : struct, IComponentData, INetSerializable where T4 : struct, IComponentData, INetSerializable where T5 : struct, IComponentData, INetSerializable{

            // Unpack command
            for (var index = 0; index < command.Data.Length; index++) {

                ref var entityCommand = ref command.Data[index];
                manager.ConvertToEcs(ref entityCommand);
            }
        }
        
        /// <summary>
        /// Converts a network <see cref="EntityCommand"/> to an <see cref="EcsCommand"/> and aplies it to the local ecs. 
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="command"></param>
        public static void ConvertToEcs<T1, T2, T3, T4, T5, T6>(this ref EntityManager manager, ref BatchCommand<EntityCommand<T1,T2,T3, T4, T5, T6>> command) where T1 : struct, IComponentData, INetSerializable where T2 : struct, IComponentData, INetSerializable where T3 : struct, IComponentData, INetSerializable where T4 : struct, IComponentData, INetSerializable where T5 : struct, IComponentData, INetSerializable where T6 : struct, IComponentData, INetSerializable{

            // Unpack command
            for (var index = 0; index < command.Data.Length; index++) {

                ref var entityCommand = ref command.Data[index];
                manager.ConvertToEcs(ref entityCommand);
            }
        }
        
        /// <summary>
        /// Converts a network <see cref="EntityCommand"/> to an <see cref="EcsCommand"/> and aplies it to the local ecs. 
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="command"></param>
        public static void ConvertToEcs<T1, T2, T3, T4, T5, T6, T7>(this ref EntityManager manager, ref BatchCommand<EntityCommand<T1,T2,T3, T4, T5, T6, T7>> command) where T1 : struct, IComponentData, INetSerializable where T2 : struct, IComponentData, INetSerializable where T3 : struct, IComponentData, INetSerializable where T4 : struct, IComponentData, INetSerializable where T5 : struct, IComponentData, INetSerializable where T6 : struct, IComponentData, INetSerializable where T7 : struct, IComponentData, INetSerializable{

            // Unpack command
            for (var index = 0; index < command.Data.Length; index++) {

                ref var entityCommand = ref command.Data[index];
                manager.ConvertToEcs(ref entityCommand);
            }
        }
    }
}