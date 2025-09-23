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
    /// 오직 "원본 에셋"만 관리한다.
    /// 인스턴스 수명과 풀링은 다 ResourceManager가 담당.
    /// </summary>
    public class AddressableManager : SingletonBase<AddressableManager>
    {
        private bool initialized;
        private Dictionary<string, AsyncOperationHandle> assetHandles = new Dictionary<string, AsyncOperationHandle>();

        // 씬 핸들: key -> AsyncOperationHandle<SceneInstance>
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

            handle.WaitForCompletion(); // 동기 방식
            if (handle.Status != AsyncOperationStatus.Succeeded)
            {
                assetHandles.Remove(key);
                throw new Exception($"[AddressableManager] LoadAsset failed(sync): {key} ({handle.OperationException?.Message})");
            }
            return handle.Result;
        }


        /// 자산 로드(캐시/중복요청 병합)
        public async UniTask<T> LoadAssetAsync<T>(string key, CancellationToken ct = default) where T : UnityEngine.Object
        {
            if (!initialized) 
                await InitializeAsync();

            if (assetHandles.TryGetValue(key, out var existing))
            {
                // 이미 로드중 or 로드완료면 재사용
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

        /// <summary>개별 해제(자산/인스턴스 모두)</summary>
        public void ReleaseAsset(string key)
        {
            if (assetHandles.Remove(key, out var handle))
                Addressables.Release(handle);
        }
        
        /// <summary>전부 해제(씬 종료 시점 권장)</summary>
        public void ReleaseAllAssets()
        {
            assetHandles.Clear();
        }
    }
}
