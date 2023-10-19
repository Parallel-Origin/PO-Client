using ParallelOrigin.Core.ECS.Components;
using ParallelOrigin.Core.Network;
using Script.Client.Systems.Reactive;
using Unity.Entities;

namespace Script.Client.Systems.Command {

    /// <summary>
    /// A struct containing logic for changing the animations for a entity. 
    /// </summary>
    public struct AnimationCommand : IExecutable {

        public Statefull<AnimationParamCommand> ParamCommand;
        public AnimationTriggerCommand TriggerCommand;
        public EntityManager EntityManager;

        public bool Trigger;

        public void Execute() {

            // Process param
            if (!Trigger) 
            {
                
                var entity = ParamCommand.Item.EntityLink.Resolve(EntityManager);
                if (entity != Entity.Null) {

                    var anim = EntityManager.GetComponentData<Animation>(entity);

                    // Change bool params
                    ref var statefull = ref ParamCommand;
                    ref var boolParams = ref ParamCommand.Item;

                    var name = boolParams.BoolName;
                    var active = boolParams.Activated;

                    switch (statefull.State) {

                        case State.Added:
                            anim.BoolParams.Add(name, active);
                            break;

                        case State.Updated:
                            anim.BoolParams[name] = active;
                            break;

                        case State.Removed:
                            anim.BoolParams.Remove(name);
                            break;
                    }

                    EntityManager.SetComponentData(entity, anim);
                }
            }

            // Process trigger instead
            if (Trigger) 
            {
                var entity = TriggerCommand.EntityLink.Resolve(EntityManager);
                if (entity != Entity.Null) {

                    var anim = EntityManager.GetComponentData<Animation>(entity);

                    // Set trigger 
                    var triggerName = TriggerCommand.TriggerName;
                    anim.Triggers.Add(triggerName);

                    EntityManager.SetComponentData(entity, anim);
                }
            }
        }
    }

    /// <summary>
    /// A system buffering <see cref="AnimationCommand"/>'s to execute them during processing. 
    /// </summary>
    public class AnimationCommandSystem : ExecutableBufferSystem<AnimationCommand>{ }
}
