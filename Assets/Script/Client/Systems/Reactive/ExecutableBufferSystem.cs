using System.Collections.Generic;
using Unity.Entities;

namespace Script.Client.Systems.Reactive {

    /// <summary>
    /// An interface which 
    /// </summary>
    public interface IExecutable {
        void Execute();
    }
    
    /// <summary>
    /// A system which stores a bunch of <see cref="T"/>'s which get debuffered and executed at some point.
    /// Great for commands which we receive during the frame, but want to wait till the next frame to execute them.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [UpdateInGroup(typeof(InitializationSystemGroup), OrderLast = true)]
    [AlwaysUpdateSystem]
    public abstract partial class ExecutableBufferSystem<T> : SystemBase where T : struct, IExecutable {

        protected override void OnCreate() {
            base.OnCreate();

            BufferQueue = new Queue<T>(64);
        }

        protected override void OnUpdate() {

            // Dequeue queue and execute buffered items
            while (BufferQueue.Count > 0) {
                
                var bufferedItem = BufferQueue.Dequeue();
                bufferedItem.Execute();
            }
        }

        /// <summary>
        /// Buffers an element which will be executed on the next system update.
        /// </summary>
        /// <param name="element"></param>
        public void Buffer(ref T element) {
            BufferQueue.Enqueue(element);
        }

        /// <summary>
        /// A buffer which will be playbacked once the system was updated. 
        /// </summary>
        public Queue<T> BufferQueue { get; private set; }
    }
}