using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using Object = UnityEngine.Object;

namespace Skywatch.AssetManagement
{
    /// <summary>
    /// Handles all loading, unloading, instantiating, and destroying of AssetReferences and their associated Objects.
    /// </summary>
    public static class AssetManager
    {
        const string _err = "<color=#ffa500>" + nameof(AssetManager) + " Error:</color> ";

        public delegate void DelegateAssetLoaded(object key, AsyncOperationHandle handle);
        public static event DelegateAssetLoaded OnAssetLoaded; // 加载成功委托

        public delegate void DelegateAssetUnloaded(object runtimeKey);
        public static event DelegateAssetUnloaded OnAssetUnloaded; // 卸载成功委托
        
        static readonly Dictionary<object, AsyncOperationHandle> loadingAssets = new Dictionary<object, AsyncOperationHandle>(10);
        static readonly Dictionary<object, AsyncOperationHandle> loadedAssets = new Dictionary<object, AsyncOperationHandle>(50);
        public static IReadOnlyList<object> LoadedAssets => loadedAssets.Values.Select(x => x.Result).ToList();
        static readonly Dictionary<object, List<GameObject>> instances = new Dictionary<object, List<GameObject>>(20);

        public static int loadedAssetsCount => loadedAssets.Count;
        public static int loadingAssetsCount => loadingAssets.Count;
        public static int instantiatedAssetsCount => instances.Values.SelectMany(x => x).Count();

        #region Get
        // 是否已加载
        public static bool IsLoaded(AssetReference aRef)
        {
            return loadedAssets.ContainsKey(aRef.RuntimeKey);
        }
        
        // 是否已加载
        static bool IsLoaded(object key)
        {
            return loadedAssets.ContainsKey(key);
        }
        
        // 是否正在加载
        public static bool IsLoading(AssetReference aRef)
        {
            return loadingAssets.ContainsKey(aRef.RuntimeKey);
        }
        
        // 是否正在加载
        static bool IsLoading(object key)
        {
            return loadingAssets.ContainsKey(key);
        }
        
        // 是否已实例化
        public static bool IsInstantiated(AssetReference aRef)
        {
            return instances.ContainsKey(aRef.RuntimeKey);
        }
        
        // 是否已实例化
        public static bool IsInstantiated(object key)
        {
            return instances.ContainsKey(key);
        }
        
        // 获取实例化数量
        public static int InstantiatedCount(AssetReference aRef)
        {
            return !IsInstantiated(aRef) ? 0 : instances[aRef.RuntimeKey].Count;
        }
        #endregion
        
        #region Load/Unload
        // 加载资源(异步)
        /// <summary>
        /// Tries to get an already loaded <see cref="UnityEngine.Object"/> of type <see cref="TObjectType"/>.
        /// Returns <value>true</value> if the object was loaded and sets <paramref name="handle"/> to the completed <see cref="AsyncOperationHandle{TObject}"/>
        /// If the object was not loaded returns <value>false</value>, loads the object and sets <paramref name="handle"/> to the un-completed <see cref="AsyncOperationHandle{TObject}"/>
        /// <param name="aRef">The <see cref="AssetReference"/> to load.</param>
        /// <param name="handle">The loading or completed <see cref="AsyncOperationHandle{TObject}"/></param>
        /// <typeparam name="TObjectType">The type of NON-COMPONENT object to load.</typeparam>
        /// <returns><value>true</value> if the object has already been loaded, false otherwise.</returns>
        /// Do not use for <see cref="Component"/>s. call <see cref="LoadComponentAsync{TComponentType}"/>
        /// </summary>
        public static bool LoadAssetAsync<TObjectType>(object key, out AsyncOperationHandle<TObjectType> handle) where TObjectType : Object
        {
            if (loadedAssets.ContainsKey(key))
            {
                try
                {
                    handle = loadedAssets[key].Convert<TObjectType>();
                }
                catch
                {
                    handle = Addressables.ResourceManager.CreateCompletedOperation(loadedAssets[key].Result as TObjectType, string.Empty);
                }
                return true;
            }
            if (loadingAssets.ContainsKey(key))
            {
                try
                {
                    handle = loadingAssets[key].Convert<TObjectType>();
                }
                catch
                {
                    handle = Addressables.ResourceManager.CreateChainOperation(loadingAssets[key], chainOp => Addressables.ResourceManager.CreateCompletedOperation(chainOp.Result as TObjectType, string.Empty));
                }
                return false;
            }
            handle = Addressables.LoadAssetAsync<TObjectType>(key);
            loadingAssets.Add(key, handle);
            handle.Completed += op2 =>
            {
                loadedAssets.Add(key, op2);
                loadingAssets.Remove(key);
                OnAssetLoaded?.Invoke(key, op2);
            };
            return false;
        }
        
        // 加载资源(异步)
        /// <summary>
        /// Tries to get an already loaded <see cref="UnityEngine.Object"/> of type <see cref="TObjectType"/>.
        /// Returns <value>true</value> if the object was loaded and sets <paramref name="handle"/> to the completed <see cref="AsyncOperationHandle{TObject}"/>
        /// If the object was not loaded returns <value>false</value>, loads the object and sets <paramref name="handle"/> to the un-completed <see cref="AsyncOperationHandle{TObject}"/>
        /// <param name="aRef">The <see cref="AssetReference"/> to load.</param>
        /// <param name="handle">The loading or completed <see cref="AsyncOperationHandle{TObject}"/></param>
        /// <typeparam name="TObjectType">The type of NON-COMPONENT object to load.</typeparam>
        /// <returns><value>true</value> if the object has already been loaded, false otherwise.</returns>
        /// Do not use for <see cref="Component"/>s. call <see cref="LoadComponentAsync{TComponentType}"/>
        /// </summary>
        public static bool LoadAssetAsync<TObjectType>(AssetReference aRef, out AsyncOperationHandle<TObjectType> handle) where TObjectType : Object
        {
            CheckRuntimeKey(aRef);
            return LoadAssetAsync(aRef.RuntimeKey, out handle);
        }
        
        // 加载资源(异步)
        /// <summary>
        /// DO NOT USE FOR <see cref="Component"/>s. Call <see cref="LoadComponentAsync{TComponentType}"/>
        ///
        /// Tries to get an already loaded <see cref="UnityEngine.Object"/> of type <see cref="TObjectType"/>.
        /// Returns <value>true</value> if the object was loaded and sets <paramref name="handle"/> to the completed <see cref="AsyncOperationHandle{TObject}"/>
        /// If the object was not loaded returns <value>false</value>, loads the object and sets <paramref name="handle"/> to the un-completed <see cref="AsyncOperationHandle{TObject}"/>
        /// </summary>
        /// <param name="aRef">The <see cref="AssetReferenceT{TObject}"/> to load.</param>
        /// <param name="handle">The loading or completed <see cref="AsyncOperationHandle{TObject}"/></param>
        /// <typeparam name="TObjectType">The type of NON-COMPONENT object to load.</typeparam>
        /// <returns><value>true</value> if the object has already been loaded, false otherwise.</returns>
        public static bool LoadAssetAsync<TObjectType>(AssetReferenceT<TObjectType> aRef, out AsyncOperationHandle<TObjectType> handle) where TObjectType : Object
        {
            CheckRuntimeKey(aRef);
            return LoadAssetAsync(aRef.RuntimeKey, out handle);
        }
        
        // 获取已加载的资源
        public static bool GetAssetSync<TObjectType>(object key, out TObjectType result) where TObjectType : Object
        {
            if (loadedAssets.ContainsKey(key))
            {
                result = loadedAssets[key].Convert<TObjectType>().Result;
                return true;
            }
            result = null;
            return false;
        }

        // 获取已加载的资源
        public static bool GetAssetSync<TObjectType>(AssetReference aRef, out TObjectType result) where TObjectType : Object
        {
            CheckRuntimeKey(aRef);
            return GetAssetSync(aRef.RuntimeKey, out result);
        }
        
        // 获取已加载的资源
        public static bool GetAssetSync<TObjectType>(AssetReferenceT<TObjectType> aRef, out TObjectType result) where TObjectType : Object
        {
            CheckRuntimeKey(aRef);
            return GetAssetSync(aRef.RuntimeKey, out result);
        }
        
        // 加载组件(异步)
        /// <summary>
        /// Tries to get an already loaded <see cref="Component"/> of type <see cref="TComponentType"/>.
        /// Returns <value>true</value> if the object was loaded and sets <paramref name="handle"/> to the completed <see cref="AsyncOperationHandle{TObject}"/>
        /// If the object was not loaded returns <value>false</value>, loads the object and sets <paramref name="handle"/> to the un-completed <see cref="AsyncOperationHandle{TObject}"/>
        /// <param name="aRef">The <see cref="AssetReference"/> to load.</param>
        /// <param name="handle">The loading or completed <see cref="AsyncOperationHandle{TObject}"/></param>
        /// <typeparam name="TComponentType">The type of Component to load.</typeparam>
        /// <returns><value>true</value> if the object has already been loaded, false otherwise.</returns>
        /// Do not use for <see cref="UnityEngine.Object"/>s. Call <see cref="LoadAssetAsync{TObjectType}"/>
        /// </summary>
        public static bool LoadComponentAsync<TComponentType>(object key, out AsyncOperationHandle<TComponentType> handle) where TComponentType : Component
        {
            if (loadedAssets.ContainsKey(key))
            {
                handle = ConvertHandleToComponent<TComponentType>(loadedAssets[key]);
                return true;
            }
            if (loadingAssets.ContainsKey(key))
            {
                handle = Addressables.ResourceManager.CreateChainOperation(loadingAssets[key], ConvertHandleToComponent<TComponentType>);
                return false;
            }
            var op = Addressables.LoadAssetAsync<GameObject>(key);
            loadingAssets.Add(key, op);
            op.Completed += op2 =>
            {
                loadedAssets.Add(key, op2);
                loadingAssets.Remove(key);
                OnAssetLoaded?.Invoke(key, op2);
            };
            handle = Addressables.ResourceManager.CreateChainOperation<TComponentType, GameObject>(op, chainOp =>
            {
                var go = chainOp.Result;
                var comp = go.GetComponent<TComponentType>();
                return Addressables.ResourceManager.CreateCompletedOperation(comp, string.Empty);
            });
            return false;
        }
        
        // 加载组件(异步)
        /// <summary>
        /// Tries to get an already loaded <see cref="Component"/> of type <see cref="TComponentType"/>.
        /// Returns <value>true</value> if the object was loaded and sets <paramref name="handle"/> to the completed <see cref="AsyncOperationHandle{TObject}"/>
        /// If the object was not loaded returns <value>false</value>, loads the object and sets <paramref name="handle"/> to the un-completed <see cref="AsyncOperationHandle{TObject}"/>
        /// <param name="aRef">The <see cref="AssetReference"/> to load.</param>
        /// <param name="handle">The loading or completed <see cref="AsyncOperationHandle{TObject}"/></param>
        /// <typeparam name="TComponentType">The type of Component to load.</typeparam>
        /// <returns><value>true</value> if the object has already been loaded, false otherwise.</returns>
        /// Do not use for <see cref="UnityEngine.Object"/>s. Call <see cref="LoadAssetAsync{TObjectType}"/>
        /// </summary>
        public static bool LoadComponentAsync<TComponentType>(AssetReference aRef, out AsyncOperationHandle<TComponentType> handle) where TComponentType : Component
        {
            CheckRuntimeKey(aRef);
            return LoadComponentAsync(aRef.RuntimeKey, out handle);
        }
        
        // 加载组件(异步)
        /// <summary>
        /// Tries to get an already loaded <see cref="Component"/> of type <see cref="TComponentType"/>.
        /// Returns <value>true</value> if the object was loaded and sets <paramref name="handle"/> to the completed <see cref="AsyncOperationHandle{TObject}"/>
        /// If the object was not loaded returns <value>false</value>, loads the object and sets <paramref name="handle"/> to the un-completed <see cref="AsyncOperationHandle{TObject}"/>
        /// <param name="aRef">The <see cref="AssetReference"/> to load.</param>
        /// <param name="handle">The loading or completed <see cref="AsyncOperationHandle{TObject}"/></param>
        /// <typeparam name="TComponentType">The type of Component to load.</typeparam>
        /// <returns><value>true</value> if the object has already been loaded, false otherwise.</returns>
        /// Do not use for <see cref="UnityEngine.Object"/>s. Call <see cref="LoadAssetAsync{TObjectType}"/>
        /// </summary>
        public static bool LoadComponentAsync<TComponentType>(AssetReferenceT<TComponentType> aRef, out AsyncOperationHandle<TComponentType> handle) where TComponentType : Component
        {
            CheckRuntimeKey(aRef);
            return LoadComponentAsync(aRef.RuntimeKey, out handle);
        }
        
        // 获取已加载的组件
        public static bool GetComponentSync<TComponentType>(object key, out TComponentType result) where TComponentType : Component
        {
            if (loadedAssets.ContainsKey(key))
            {
                var handle = loadedAssets[key];
                result = null;
                var go = handle.Result as GameObject;
                if (!go)
                    throw new ConversionException($"Cannot convert {nameof(handle.Result)} to {nameof(GameObject)}.");
                result = go.GetComponent<TComponentType>();
                if(!result)
                    throw new ConversionException($"Cannot {nameof(go.GetComponent)} of Type {typeof(TComponentType)}.");
                return true;
            }
            result = null;
            return false;
        }
        
        // 获取已加载的组件
        public static bool GetComponentSync<TComponentType>(AssetReference aRef, out TComponentType result) where TComponentType : Component
        {
            CheckRuntimeKey(aRef);
            return GetComponentSync(aRef.RuntimeKey, out result);
        }
        
        // 获取已加载的组件
        public static bool GetComponentSync<TComponentType>(AssetReferenceT<TComponentType> aRef, out TComponentType result) where TComponentType : Component
        {
            return GetComponentSync(aRef.RuntimeKey, out result);
        }
        
        // 根据标签加载资源(异步)
        public static AsyncOperationHandle<List<AsyncOperationHandle<Object>>> LoadAssetsByLabelAsync(string label)
        {
            return Addressables.ResourceManager.StartOperation(new LoadAssetsByLabelOperation(loadedAssets, loadingAssets, label, AssetLoadedCallback), default);
        }
        
        // 触发加载完成回调
        static void AssetLoadedCallback(object key, AsyncOperationHandle handle)
        {
            OnAssetLoaded?.Invoke(key, handle);
        }

        // 卸载资源
        /// <summary>
        /// Unloads the given <paramref name="aRef"/> and calls <see cref="DestroyAllInstances"/> if it was Instantiated.
        /// <param name="aRef"></param>
        /// <returns></returns>
        /// </summary>
        public static void Unload(object key)
        {
            CheckRuntimeKey(key);
            AsyncOperationHandle handle;
            if (loadingAssets.ContainsKey(key))
            {
                handle = loadingAssets[key];
                loadingAssets.Remove(key);
            }
            else if (loadedAssets.ContainsKey(key))
            {
                handle = loadedAssets[key];
                loadedAssets.Remove(key);
            }
            else
            {
                Debug.LogWarning($"{_err}Cannot {nameof(Unload)} RuntimeKey '{key}': It is not loading or loaded.");
                return;
            }
            if (IsInstantiated(key))
            {
                DestroyAllInstances(key);
            }
            Addressables.Release(handle);
            OnAssetUnloaded?.Invoke(key);
        }
        
        // 卸载资源
        /// <summary>
        /// Unloads the given <paramref name="aRef"/> and calls <see cref="DestroyAllInstances"/> if it was Instantiated.
        /// <param name="aRef"></param>
        /// <returns></returns>
        /// </summary>
        public static void Unload(AssetReference aRef)
        {
            CheckRuntimeKey(aRef);
            Unload(aRef.RuntimeKey);
        }
    
        // 根据标签卸载资源
        public static void UnloadByLabel(string label)
        {
            if (string.IsNullOrEmpty(label) || string.IsNullOrWhiteSpace(label))
            {
                Debug.LogError("Label cannot be empty.");
                return;
            }
            var locationsHandle = Addressables.LoadResourceLocationsAsync(label);
            locationsHandle.Completed += op =>
            {
                if (locationsHandle.Status != AsyncOperationStatus.Succeeded)
                {
                    Debug.LogError($"Cannot Unload by label '{label}'");
                    return;
                }
                if (op.Result.Count > 0)
                {
                    var keys = GetKeysFromLocations(op.Result);
                    foreach (var key in keys)
                    {
                        if (IsLoaded(key) || IsLoading(key))
                        {
                            Unload(key);
                        }
                    }
                }
            };
        }
        #endregion

        #region Instantiation
        // 加载资源并实例化(异步)
        public static bool InstantiateAsync(object key, Vector3 position, Quaternion rotation, Transform parent, out AsyncOperationHandle<GameObject> handle)
        {
            if (LoadAssetAsync(key, out AsyncOperationHandle<GameObject> loadHandle))
            {
                var instance = InstantiateInternal(key, loadHandle.Result, position, rotation, parent);
                handle = Addressables.ResourceManager.CreateCompletedOperation(instance, string.Empty);
                return true;
            }
            if (!loadHandle.IsValid())
            {
                Debug.LogError($"Load Operation was invalid: {loadHandle}.");
                handle = Addressables.ResourceManager.CreateCompletedOperation<GameObject>(null, $"Load Operation was invalid: {loadHandle}.");
                return false;
            }
            handle = Addressables.ResourceManager.CreateChainOperation(loadHandle, chainOp =>
            {
                var instance = InstantiateInternal(key, chainOp.Result, position, rotation, parent);
                return Addressables.ResourceManager.CreateCompletedOperation(instance, string.Empty);
            });
            return false;
        }
        
        // 加载资源并实例化(异步)
        public static bool InstantiateAsync(AssetReference aRef, Vector3 position, Quaternion rotation, Transform parent, out AsyncOperationHandle<GameObject> handle)
        {
            CheckRuntimeKey(aRef);
            return InstantiateAsync(aRef.RuntimeKey, position, rotation, parent, out handle);
        }
        
        // 加载组件并实例化(异步)
        public static bool InstantiateAsync<TComponentType>(object key, Vector3 position, Quaternion rotation, Transform parent, out AsyncOperationHandle<TComponentType> handle) where TComponentType : Component
        {
            if (LoadComponentAsync(key, out AsyncOperationHandle<TComponentType> loadHandle))
            {
                var instance = InstantiateInternal(key, loadHandle.Result, position, rotation, parent);
                handle = Addressables.ResourceManager.CreateCompletedOperation(instance, string.Empty);
                return true;
            }
            if (!loadHandle.IsValid())
            {
                Debug.LogError($"Load Operation was invalid: {loadHandle}.");
                handle = Addressables.ResourceManager.CreateCompletedOperation<TComponentType>(null, $"Load Operation was invalid: {loadHandle}.");
                return false;
            }
            //Create a chain that waits for loadHandle to finish, then instantiates and returns the instance GO.
            handle = Addressables.ResourceManager.CreateChainOperation(loadHandle, chainOp =>
            {
                var instance = InstantiateInternal(key, chainOp.Result, position, rotation, parent);
                return Addressables.ResourceManager.CreateCompletedOperation(instance, string.Empty);
            });
            return false;
        }
        
        // 加载组件并实例化(异步)
        public static bool InstantiateAsync<TComponentType>(AssetReference aRef, Vector3 position, Quaternion rotation, Transform parent, out AsyncOperationHandle<TComponentType> handle) where TComponentType : Component
        {
            CheckRuntimeKey(aRef);
            return InstantiateAsync<TComponentType>(aRef.RuntimeKey, position, rotation, parent, out handle);
        }
        
        // 加载组件并实例化(异步)
        public static bool InstantiateAsync<TComponentType>(AssetReferenceT<TComponentType> aRef, Vector3 position, Quaternion rotation, Transform parent, out AsyncOperationHandle<TComponentType> handle) where TComponentType : Component
        {
            CheckRuntimeKey(aRef);
            return InstantiateAsync(aRef.RuntimeKey, position, rotation, parent, out handle);
        }
    
        // 加载多个资源并实例化(异步)
        public static bool InstantiateMultiAsync(object key, int count, Vector3 position, Quaternion rotation, Transform parent, out AsyncOperationHandle<List<GameObject>> handle)
        {
            if (LoadAssetAsync(key, out AsyncOperationHandle<GameObject> loadHandle))
            {
                var list = new List<GameObject>(count);
                for (int i = 0; i < count; i++)
                {
                    var instance = InstantiateInternal(key, loadHandle.Result, position, rotation, parent);
                    list.Add(instance);
                }
                handle = Addressables.ResourceManager.CreateCompletedOperation(list, string.Empty);
                return true;
            }
            if (!loadHandle.IsValid())
            {
                Debug.LogError($"Load Operation was invalid: {loadHandle}.");
                handle = Addressables.ResourceManager.CreateCompletedOperation<List<GameObject>>(null, $"Load Operation was invalid: {loadHandle}.");
                return false;
            }
            handle = Addressables.ResourceManager.CreateChainOperation(loadHandle, chainOp =>
            {
                var list = new List<GameObject>(count);
                for (int i = 0; i < count; i++)
                {
                    var instance = InstantiateInternal(key, chainOp.Result, position, rotation, parent);
                    list.Add(instance);
                }
                return Addressables.ResourceManager.CreateCompletedOperation(list, string.Empty);
            });
            return false;
        }
        
        // 加载多个资源并实例化(异步)
        public static bool InstantiateMultiAsync(AssetReference aRef, int count, Vector3 position, Quaternion rotation, Transform parent, out AsyncOperationHandle<List<GameObject>> handle)
        {
            CheckRuntimeKey(aRef);
            return InstantiateMultiAsync(aRef.RuntimeKey, count, position, rotation, parent, out handle);
        }
        
        // 加载多个组件并实例化(异步)
        public static bool InstantiateMultiAsync<TComponentType>(object key, int count, Vector3 position, Quaternion rotation, Transform parent, out AsyncOperationHandle<List<TComponentType>> handle) where TComponentType : Component
        {
            if (LoadComponentAsync(key, out AsyncOperationHandle<TComponentType> loadHandle))
            {
                var list = new List<TComponentType>(count);
                for (int i = 0; i < count; i++)
                {
                    var instance = InstantiateInternal(key, loadHandle.Result, position, rotation, parent);
                    list.Add(instance);
                }
                handle = Addressables.ResourceManager.CreateCompletedOperation(list, string.Empty);
                return true;
            }
            if (!loadHandle.IsValid())
            {
                Debug.LogError($"Load Operation was invalid: {loadHandle}.");
                handle = Addressables.ResourceManager.CreateCompletedOperation<List<TComponentType>>(null, $"Load Operation was invalid: {loadHandle}.");
                return false;
            }
            handle = Addressables.ResourceManager.CreateChainOperation(loadHandle, chainOp =>
            {
                var list = new List<TComponentType>(count);
                for (int i = 0; i < count; i++)
                {
                    var instance = InstantiateInternal(key, chainOp.Result, position, rotation, parent);
                    list.Add(instance);
                }
                return Addressables.ResourceManager.CreateCompletedOperation(list, string.Empty);
            });
            return false;
        }
        
        // 加载多个组件并实例化(异步)
        public static bool InstantiateMultiAsync<TComponentType>(AssetReference aRef, int count, Vector3 position, Quaternion rotation, Transform parent, out AsyncOperationHandle<List<TComponentType>> handle) where TComponentType : Component
        {
            CheckRuntimeKey(aRef);
            return InstantiateMultiAsync<TComponentType>(aRef.RuntimeKey, count, position, rotation, parent, out handle);
        }
        
        // 加载多个组件并实例化(异步)
        public static bool InstantiateMultiAsync<TComponentType>(AssetReferenceT<TComponentType> aRef, int count, Vector3 position, Quaternion rotation, Transform parent, out AsyncOperationHandle<List<TComponentType>> handle) where TComponentType : Component
        {
            return InstantiateMultiAsync<TComponentType>(aRef.RuntimeKey, count, position, rotation, parent, out handle);
        }
        
        // 获取已加载的资源并实例化(异步)
        public static bool InstantiateExistSync(AssetReference aRef, Vector3 position, Quaternion rotation, Transform parent, out GameObject result)
        {
            if (!GetAssetSync(aRef, out GameObject loadResult))
            {
                result = null;
                return false;
            }
            result = InstantiateInternal(aRef, loadResult, position, rotation, parent);
            return true;
        }
        
        // 获取已加载的组件并实例化(异步)
        public static bool InstantiateExistSync<TComponentType>(AssetReference aRef, Vector3 position, Quaternion rotation, Transform parent, out TComponentType result) where TComponentType : Component
        {
            if (!GetComponentSync(aRef, out TComponentType loadResult))
            {
                result = null;
                return false;
            }
            result = InstantiateInternal(aRef, loadResult, position, rotation, parent);
            return true;
        }
        
        // 获取已加载的组件并实例化(异步)
        public static bool InstantiateExistSync<TComponentType>(AssetReferenceT<TComponentType> aRef, Vector3 position, Quaternion rotation, Transform parent, out TComponentType result) where TComponentType : Component
        {
            return InstantiateExistSync(aRef as AssetReference, position, rotation, parent, out result);
        }
    
        // 获取已加载的资源并实例化(异步)
        public static bool InstantiateExistMultiSync(AssetReference aRef, int count, Vector3 position, Quaternion rotation, Transform parent, out List<GameObject> result)
        {
            if (!GetAssetSync(aRef, out GameObject loadResult))
            {
                result = null;
                return false;
            }
            var list = new List<GameObject>(count);
            for (int i = 0; i < count; i++)
            {
                var instance = InstantiateInternal(aRef, loadResult, position, rotation, parent);
                list.Add(instance);
            }
            result = list;
            return true;
        }
        
        // 获取已加载的组件并实例化(异步)
        public static bool InstantiateExistMultiSync<TComponentType>(AssetReference aRef, int count, Vector3 position, Quaternion rotation, Transform parent, out List<TComponentType> result) where TComponentType : Component
        {
            if (!GetComponentSync(aRef, out TComponentType loadResult))
            {
                result = null;
                return false;
            }
            var list = new List<TComponentType>(count);
            for (int i = 0; i < count; i++)
            {
                var instance = InstantiateInternal(aRef, loadResult, position, rotation, parent);
                list.Add(instance);
            }
            result = list;
            return true;
        }
        
        // 获取已加载的组件并实例化(异步)
        public static bool InstantiateExistMultiSync<TComponentType>(AssetReferenceT<TComponentType> aRef, int count, Vector3 position, Quaternion rotation, Transform parent, out List<TComponentType> result) where TComponentType : Component
        {
            return InstantiateExistMultiSync(aRef as AssetReference, count, position, rotation, parent, out result);
        }
        
        // 资源实例化
        static TComponentType InstantiateInternal<TComponentType>(object key, TComponentType loadedAsset, Vector3 position, Quaternion rotation, Transform parent) where TComponentType : Component
        {
            var instance = Object.Instantiate(loadedAsset, position, rotation, parent);
            if (!instance)
                throw new NullReferenceException($"Instantiated Object of type '{typeof(TComponentType)}' is null.");
            var monoTracker = instance.gameObject.AddComponent<MonoTracker>();
            monoTracker.key = key;
            monoTracker.OnDestroyed += TrackerDestroyed;
            if (!instances.ContainsKey(key))
            {
                instances.Add(key, new List<GameObject>(10));
            }
            instances[key].Add(instance.gameObject);
            return instance;
        }
        
        // 实例化
        static TComponentType InstantiateInternal<TComponentType>(AssetReference aRef, TComponentType loadedAsset, Vector3 position, Quaternion rotation, Transform parent) where TComponentType : Component
        {
            return InstantiateInternal(aRef.RuntimeKey, loadedAsset, position, rotation, parent);
        }
        
        // 实例化
        static GameObject InstantiateInternal(object key, GameObject loadedAsset, Vector3 position, Quaternion rotation, Transform parent)
        {
            var instance = Object.Instantiate(loadedAsset, position, rotation, parent);
            if(!instance)
                throw new NullReferenceException($"Instantiated Object of type '{typeof(GameObject)}' is null.");
            var monoTracker = instance.gameObject.AddComponent<MonoTracker>();
            monoTracker.key = key;
            monoTracker.OnDestroyed += TrackerDestroyed;
            if (!instances.ContainsKey(key))
            {
                instances.Add(key, new List<GameObject>(8));
            }
            instances[key].Add(instance);
            return instance;
        }
        
        // 实例化
        static GameObject InstantiateInternal(AssetReference aRef, GameObject loadedAsset, Vector3 position, Quaternion rotation, Transform parent)
        {
            return InstantiateInternal(aRef.RuntimeKey, loadedAsset, position, rotation, parent);
        }
        
        //
        static void TrackerDestroyed(MonoTracker tracker)
        {
            if (instances.TryGetValue(tracker.key, out var list))
            {
                list.Remove(tracker.gameObject);
            }
        }
    
        // 销毁指定关联的所有实例化对象
        public static void DestroyAllInstances(AssetReference aRef)
        {
            CheckRuntimeKey(aRef);
            if (!instances.ContainsKey(aRef.RuntimeKey))
            {
                Debug.LogWarning($"{nameof(AssetReference)} '{aRef}' has not been instantiated. 0 Instances destroyed.");
                return;
            }
            DestroyAllInstances(aRef.RuntimeKey);
        }
        
        // 销毁指定key的所有实例化对象
        static void DestroyAllInstances(object key)
        {
            var list = instances[key];
            for (int i = list.Count - 1; i >= 0; i--)
            {
                DestroyInternal(list[i]);
            }
            instances[key].Clear();
            instances.Remove(key);
        }
        
        // 销毁实例化对象
        static void DestroyInternal(Object obj)
        {
            var c = obj as Component;
            if (c)
            {
                Object.Destroy(c.gameObject);
            }
            else
            {
                var go = obj as GameObject;
                if (go)
                {
                    Object.Destroy(go);
                }
            }
        }
        #endregion
        
        #region Utilities
        // 检查是否为key
        static void CheckRuntimeKey(AssetReference aRef)
        {
            if (!aRef.RuntimeKeyIsValid())
            {
                throw new InvalidKeyException($"{_err}{nameof(aRef.RuntimeKey)} is not valid for '{aRef}'.");
            }
        }
        
        // 检查是否为key
        static void CheckRuntimeKey(object key)
        {
            if (string.IsNullOrWhiteSpace(key.ToString()))
            {
                throw new InvalidKeyException($"{_err}{key} is not valid.");
            }
        }
        
        // 转换为组件handle
        static AsyncOperationHandle<TComponentType> ConvertHandleToComponent<TComponentType>(AsyncOperationHandle handle) where TComponentType : Component
        {
            GameObject go = handle.Result as GameObject;
            if (!go)
                throw new ConversionException($"Cannot convert {nameof(handle.Result)} to {nameof(GameObject)}.");
            TComponentType comp = go.GetComponent<TComponentType>();
            if (!comp)
                throw new ConversionException($"Cannot {nameof(go.GetComponent)} of Type {typeof(TComponentType)}.");
            var result = Addressables.ResourceManager.CreateCompletedOperation(comp, string.Empty);
            return result;
        }
        
        // 根据标签结果列表获取key列表
        static List<object> GetKeysFromLocations(IList<IResourceLocation> locations)
        {
            List<object> keys = new List<object>(locations.Count);
            foreach (var locator in Addressables.ResourceLocators)
            {
                foreach (var key in locator.Keys)
                {
                    bool isGUID = Guid.TryParse(key.ToString(), out var guid);
                    if (!isGUID)
                        continue;
                    if (!TryGetKeyLocationID(locator, key, out var keyLocationID))
                        continue;
                    var locationMatched = locations.Select(x => x.InternalId).ToList().Exists(x => x == keyLocationID);
                    if (!locationMatched)
                        continue;
                    keys.Add(key);
                }
            }
            return keys;
        }
        
        //
        static bool TryGetKeyLocationID(IResourceLocator locator, object key, out string internalID)
        {
            internalID = string.Empty;
            var hasLocation = locator.Locate(key, typeof(Object), out var keyLocations);
            if (!hasLocation)
                return false;
            if (keyLocations.Count == 0)
                return false;
            if (keyLocations.Count > 1)
                return false;
            internalID = keyLocations[0].InternalId;
            return true;
        }
        #endregion
    }
}

