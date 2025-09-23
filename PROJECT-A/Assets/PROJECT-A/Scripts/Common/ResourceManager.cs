using System;
using System.Collections.Generic;
using TST;
using UnityEngine;

namespace A
{
    /// GameObject 전용 스폰/디스폰 관리.
    /// - 프리로드는 AddressableManager.LoadAssetAsync/Sync<GameObject>로 수행
    /// - 여기서는 동기 Instantiate만 제공
    /// - 외부 Destroy도 AutoReleaseOnDestroy로 카운트 정리
    public class ResourceManagerEX : SingletonBase<ResourceManagerEX>
    {
        private Dictionary<string, int> liveCounts = new(); // 카운트용     
        private Dictionary<string, HashSet<GameObject>> liveSets = new(); // 실제 오브젝트들
        private Dictionary<GameObject, string> instanceKey = new();   // 오브젝트를 통해 키 등록

        public GameObject Instantiate(string key, Vector2 position = default, Quaternion rotation = default, Transform parent = null)
        {
            if (!AddressableManager.Singleton.TryGet<GameObject>(key, out var prefab))
            {
                // 프리로드 안 되어 있으면 동기 로드 (주의: 스파이크 가능 → 로딩 화면에서 미리 올려두는 것을 권장)
                prefab = AddressableManager.Singleton.LoadAssetSync<GameObject>(key);
            }

            var go = UnityEngine.Object.Instantiate(prefab, position, rotation, parent);

            // 등록
            liveCounts[key] = GetLiveCounts(liveCounts, key) + 1;

            if (!liveSets.TryGetValue(key, out var set))
            {
                set = new HashSet<GameObject>();
                liveSets[key] = set;
            }
            set.Add(go);
            instanceKey[go] = key;

            // 태그/안전핀
            var tag = go.GetComponent<ResourceTag>();

            if (tag == null)
                go.AddComponent<ResourceTag>();

            tag.key = key; 
            tag.notified = false;


            var guard = go.GetComponent<AutoReleaseOnDestroy>();
            if (guard == null)
                go.AddComponent<AutoReleaseOnDestroy>();

            guard.Bind(this);
            return go;
        }

        public GameObject Instantiate(string key, Transform parent = null)
        {
            return Instantiate(key, Vector2.zero, Quaternion.identity, parent);
        }

        public void Destroy(GameObject go)
        {
            if (go == null) 
                return;

            // 인스턴스 키 찾기 
            if (instanceKey.TryGetValue(go, out var key))
            {
                instanceKey.Remove(go);

                if (liveSets.TryGetValue(key, out var set))  
                {
                    set.Remove(go); 
                    if (set.Count == 0) 
                        liveSets.Remove(key);
                }

                liveCounts[key] = Math.Max(0, liveCounts[key] - 1);
                UnityEngine.Object.Destroy(go);

                TryReleaseAssetIfNoUse(key);
            }
            else
            {
                UnityEngine.Object.Destroy(go);
            }
        }

        public void DestroyAll(string key)
        {
            if (!liveSets.TryGetValue(key, out var set))
                return;

            var list = new List<GameObject>(set);
            foreach (var go in list)
            {
                if (!go) 
                    continue;

                instanceKey.Remove(go);
                UnityEngine.Object.Destroy(go);
            }

            set.Clear();
            liveSets.Remove(key);
            liveCounts[key] = 0;

            TryReleaseAssetIfNoUse(key);
        }

        // 외부 Destroy 안전 경로 
        internal void NotifyDestroyed(GameObject go, string keyFromTag = null)
        {
            if (go == null) 
                return;

            if (!instanceKey.TryGetValue(go, out var key))
                key = keyFromTag;

            if (key == null) 
                return;

            instanceKey.Remove(go);

            if (liveSets.TryGetValue(key, out var set))
            {
                set.Remove(go);
                if (set.Count == 0) 
                    liveSets.Remove(key);
            }

            liveCounts[key] = Math.Max(0, liveCounts[key] - 1);
            TryReleaseAssetIfNoUse(key);
        }

        public void ReleaseAll()
        {
            // 모든 인스턴스 파괴
            if (instanceKey.Count > 0)
            {
                var gameObjects = new List<GameObject>(instanceKey.Keys);
                foreach (var go in gameObjects)
                {
                    if (go) 
                        UnityEngine.Object.Destroy(go);
                }
            }

            instanceKey.Clear();
            liveSets.Clear();
            liveCounts.Clear();

            // 원본 에셋 해제는 AddressableManager에 위임
            AddressableManager.Singleton.ReleaseAllAssets();
        }

        private void TryReleaseAssetIfNoUse(string key)
        {
            // 인스턴스 0만 체크해서 해제 시도.
            // (SO/Scene/Sound 보유는 AddressableManager 측에서 따로 관리/정책 결정)
            if (GetLiveCounts(liveCounts, key) > 0) 
                return;
            AddressableManager.Singleton.ReleaseAsset(key);
        }

        private int GetLiveCounts(Dictionary<string, int> dict, string key)
        {
            if (dict.TryGetValue(key, out int result))
                return result;
            else
                return 0; 
        }

        // 디버그/조회용
        public void ShowLiveCount(string key)
        {
            int cnt = GetLiveCounts(liveCounts, key);
            Debug.Log(cnt);
        }
    }
}
