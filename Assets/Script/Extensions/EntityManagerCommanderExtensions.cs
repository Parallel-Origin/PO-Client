using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ParallelOrigin.Core.Base.Classes.Pattern.Prototype;
using ParallelOrigin.Core.ECS.Components;
using Unity.Entities;

[assembly: InternalsVisibleTo("Unity.Entities")]
namespace Script.Extensions {

    /// <summary>
    /// Possible operations
    /// </summary>
    public enum Operation {
        Create,
        Add,
        Update,
        Remove,
        Delete
    }
    
    /// <summary>
    /// An ECS command getting applied to the ecs. 
    /// </summary>
    public ref struct EcsCommand<T> where T : struct, IComponentData {

        public Operation Opcode;
        public Entity Entity;
        public string TypeID;
        public T Data;
    }

    /// <summary>
    /// A struct which describes a ECS operation. <see cref="IOperationProcessor"/> are used to execute the described operation on an entity
    /// </summary>
    public ref struct EcsOperation<T> where T : struct, IComponentData{

        public EcsCommand<T> Command;
        public EntityManager EntityManager;
        public bool Exists;
    }
    

    /// <summary>
    /// An <see cref="IOperationProcessor"/> which processes incoming <see cref="EcsOperation{T}"/>'s to execute them.
    /// </summary>
    public interface IOperationProcessor {
        Entity Process<T>(ref EcsOperation<T> operation) where T : struct, IComponentData;
    }
    

    /// <summary>
    /// The implementation of the <see cref="Interfaces.IECSCommandProcessor" /> that is used to convert server packets into entity creation, deletion and modifications for the client side ecs.
    /// </summary>
    public static class EntityManagerCommanderExtensions {

        public static Entity Process<T>(this ref EntityManager manager, ref EcsCommand<T> command) where T : struct, IComponentData {
            
            ref var opcode = ref command.Opcode;
            ref var entity = ref command.Entity;
    
            // Create buffer and filter possible layers
            var operationProcessor = OperationProcessor[opcode];
            var exists = entity != Entity.Null && manager.HasComponent<T>(entity);
            
            // Prevent a null entity from add/update/remove operations
            if (entity == Entity.Null && opcode != Operation.Create) return Entity.Null;

            var operation = new EcsOperation<T> {
                Command = command,
                EntityManager = manager,
                Exists = exists
            };
            return operationProcessor.Process(ref operation);
        }
        
        /// <summary>
        /// The hierarchy used for cloning entities
        /// </summary>
        public static EntityPrototyperHierarchy PrototyperHierarchy { get; set; }

        /// <summary>
        /// All registered <see cref="IOperationProcessor"/>'s which are used to execute logic based on the incoming <see cref="EcsCommand"/> which gets converted into a <see cref="EcsOperation{T}"/>
        /// Those take care about the logic to apply the operations onto the entities themself
        /// </summary>
        public static IDictionary<Operation, IOperationProcessor> OperationProcessor { get; } = new Dictionary<Operation, IOperationProcessor>();
    }

    
    
    
    
    /// <summary>
    /// A <see cref="IOperationProcessor"/> which takes care of <see cref="Operation.Create"/> <see cref="EcsCommand"/>'s
    /// It creates entities based on the incoming data and adds/set the given component
    /// </summary>
    public class CreateOperationProcessor : IOperationProcessor {

        public CreateOperationProcessor(EntityPrototyperHierarchy prototyperHierarchy) { PrototyperHierarchy = prototyperHierarchy; }

        public Entity Process<T>(ref EcsOperation<T> operation) where T : struct, IComponentData {
            
            ref var data = ref operation.Command.Data;
            
            ref var manager = ref operation.EntityManager;
            ref var typeID = ref operation.Command.TypeID;
            ref var entity = ref operation.Command.Entity;
            ref var exists = ref operation.Exists;
            
            if (entity == Entity.Null)
            {
                entity = PrototyperHierarchy.Clone(typeID);
                exists = manager.HasComponent<T>(entity);
            }
            
            if(!exists) manager.AddComponentData(entity, data);
            else manager.SetComponentData(entity, data);

            return entity;
        }
        
       private EntityPrototyperHierarchy PrototyperHierarchy { get; set; }
    }

    /// <summary>
    /// A <see cref="IOperationProcessor"/> which takes care of <see cref="Operation.Add"/> <see cref="EcsCommand"/>'s
    /// It adds/sets entity components based on the incoming data
    /// </summary>
    public class AddOperationProcessor : IOperationProcessor {

        public Entity Process<T>(ref EcsOperation<T> operation) where T : struct, IComponentData {
            
            ref var data = ref operation.Command.Data;
            
            ref var manager = ref operation.EntityManager;
            ref var entity = ref operation.Command.Entity;
            ref var exists = ref operation.Exists;
            
            if(!exists) manager.AddComponentData(entity, data);
            else manager.SetComponentData(entity, data);
        
            return entity;
        }
    }

    /// <summary>
    /// A <see cref="IOperationProcessor"/> which takes care of <see cref="Operation.Add"/> <see cref="EcsCommand"/>'s
    /// It adds/sets entity components based on the incoming data
    /// </summary>
    public class UpdateOperationProcessor : IOperationProcessor {
        public Entity Process<T>(ref EcsOperation<T> operation) where T : struct, IComponentData {
            
            ref var data = ref operation.Command.Data;
            ref var ecb = ref operation.EntityManager;

            ref var entity = ref operation.Command.Entity;
            ref var exists = ref operation.Exists;
            
            if(!exists) ecb.AddComponentData(entity, data);
            else  ecb.AddComponentData(entity, data);
            
            return entity;
        }
    }
    
    /// <summary>
    /// A <see cref="IOperationProcessor"/> which takes care of <see cref="Operation.Delete"/> <see cref="EcsCommand"/>'s
    /// It destroys entities
    /// </summary>
    public class RemoveOperationProcessor : IOperationProcessor {

        public Entity Process<T>(ref EcsOperation<T> operation) where T : struct, IComponentData {
            
            ref var manager = ref operation.EntityManager;
            ref var entity = ref operation.Command.Entity;

            manager.RemoveComponent<T>(entity);
            return entity;
        }
    }

    /// <summary>
    /// A <see cref="IOperationProcessor"/> which takes care of <see cref="Operation.Delete"/> <see cref="EcsCommand"/>'s
    /// It destroys entities
    /// </summary>
    public class DeleteOperationProcessor : IOperationProcessor {
        
        public Entity Process<T>(ref EcsOperation<T> operation) where T : struct, IComponentData {
            
            ref var manager = ref operation.EntityManager;
            ref var entity = ref operation.Command.Entity;

            manager.AddComponentData(entity, new DestroyAfter{Ticks = 1});
            return entity;
        }
    }
}