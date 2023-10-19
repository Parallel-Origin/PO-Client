using System;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Script.Extensions {
    
    /// <summary>
    /// A extension class for extendign the <see cref="Addressables"/> because you can load two times the same asset ( not instan. )
    /// </summary>
    public static class AdressablesLoadExtension {

        /// <summary>
        /// Loads an asset in an async way and if it already was loaded, it simply returns the already loaded object and executes the callback.
        /// </summary>
        /// <param name="assetReference"></param>
        /// <param name="onLoaded">The callback getting executed on load</param>
        /// <typeparam name="TObject">The object asset we wanna load</typeparam>
        /// <returns></returns>
        public static AsyncOperationHandle<TObject> LoadAssetAsyncIfValid<TObject>(this AssetReference assetReference, Action<AsyncOperationHandle<TObject>> onLoaded = null) {
            
            var op = assetReference.OperationHandle;
            var handle = default(AsyncOperationHandle<TObject>);
            
            if (assetReference.IsValid() && op.IsValid()) {
                
                // Increase the usage counter & Convert.
                Addressables.ResourceManager.Acquire(op);
                handle = op.Convert<TObject>();
                if (handle.IsDone) {
                    onLoaded(handle);
                }
                else {
                    // Removed OnLoaded in-case it's already been added.
                    handle.Completed -= onLoaded;
                    handle.Completed += onLoaded;
                }
            }
            else {
                handle = assetReference.LoadAssetAsync<TObject>();
 
                // Removed OnLoaded in-case it's already been added.
                handle.Completed -= onLoaded;
                handle.Completed += onLoaded;
            }

            return handle;
        }
    }
}