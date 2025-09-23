using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using TST;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
namespace A
{
    /// <summary>
    /// ���� "���� ����"�� �����Ѵ�.
    /// �ν��Ͻ� ����� Ǯ���� �� ResourceManager�� ���.
    /// </summary>
    public class AddressableManager : SingletonBase<AddressableManager>
    {
        private bool initialized;
        private Dictionary<string, AsyncOperationHandle> assetHandles = new Dictionary<string, AsyncOperationHandle>();

        // �� �ڵ�: key -> AsyncOperationHandle<SceneInstance>
        private readonly Dictionary<string, AsyncOperationHandle<SceneInstance>> sceneHandles = new();

        public async UniTask InitializeAsync(CancellationToken ct = default)
        {
            if (initialized)
                return;
            await Addressables.InitializeAsync().ToUniTask(cancellationToken: ct);
            initialized = true;
        }

        public bool TryGet<T>(string key, out T asset) where T : UnityEngine.Object
        {
            if (assetHandles.TryGetValue(key, out var h) && h.IsDone && h.Status == AsyncOperationStatus.Succeeded && h.Result is T t)
            {
                asset = t;
                return true;
            }
            asset = null;
            return false;
        }

        public T LoadAssetSync<T>(string key) where T : UnityEngine.Object
        {
            if (assetHandles.TryGetValue(key, out var existing))
            {
                existing.WaitForCompletion();
                if (existing.Status != AsyncOperationStatus.Succeeded)
                    throw new Exception($"[AddressableManager] LoadAsset failed(cached sync): {key} ({existing.OperationException?.Message})");

                if (existing.Result is T t) 
                    return t;

                throw new InvalidOperationException(
                    $"Key '{key}' already loaded as '{existing.Result?.GetType().Name ?? "null"}', not '{typeof(T).Name}'.");
            }

            var handle = Addressables.LoadAssetAsync<T>(key);
            assetHandles[key] = handle;

            handle.WaitForCompletion(); // ���� ���
            if (handle.Status != AsyncOperationStatus.Succeeded)
            {
                assetHandles.Remove(key);
                throw new Exception($"[AddressableManager] LoadAsset failed(sync): {key} ({handle.OperationException?.Message})");
            }
            return handle.Result;
        }


        /// �ڻ� �ε�(ĳ��/�ߺ���û ����)
        public async UniTask<T> LoadAssetAsync<T>(string key, CancellationToken ct = default) where T : UnityEngine.Object
        {
            if (!initialized) 
                await InitializeAsync();

            if (assetHandles.TryGetValue(key, out var existing))
            {
                // �̹� �ε��� or �ε�Ϸ�� ����
                await existing.Task.AsUniTask().AttachExternalCancellation(ct);
                return (T)existing.Result;
            }

            var handle = Addressables.LoadAssetAsync<T>(key);
            assetHandles[key] = handle;

            await handle.Task.AsUniTask().AttachExternalCancellation(ct);

            if (handle.Status != AsyncOperationStatus.Succeeded)
            {
                assetHandles.Remove(key);
                throw new System.Exception($"Addressables Load failed: {key} ({handle.OperationException?.Message})");
            }
            return handle.Result;
        }

        /// <summary>���� ����(�ڻ�/�ν��Ͻ� ���)</summary>
        public void ReleaseAsset(string key)
        {
            if (assetHandles.Remove(key, out var handle))
                Addressables.Release(handle);
        }
        
        /// <summary>���� ����(�� ���� ���� ����)</summary>
        public void ReleaseAllAssets()
        {
            assetHandles.Clear();
        }
    }
}
