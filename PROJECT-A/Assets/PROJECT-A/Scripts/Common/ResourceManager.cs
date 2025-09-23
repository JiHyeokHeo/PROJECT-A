using System;
using System.Collections.Generic;
using TST;
using UnityEngine;

namespace A
{
    /// GameObject ���� ����/���� ����.
    /// - �����ε�� AddressableManager.LoadAssetAsync/Sync<GameObject>�� ����
    /// - ���⼭�� ���� Instantiate�� ����
    /// - �ܺ� Destroy�� AutoReleaseOnDestroy�� ī��Ʈ ����
    public class ResourceManagerEX : SingletonBase<ResourceManagerEX>
    {
        private Dictionary<string, int> liveCounts = new(); // ī��Ʈ��     
        private Dictionary<string, HashSet<GameObject>> liveSets = new(); // ���� ������Ʈ��
        private Dictionary<GameObject, string> instanceKey = new();   // ������Ʈ�� ���� Ű ���

        public GameObject Instantiate(string key, Vector2 position = default, Quaternion rotation = default, Transform parent = null)
        {
            if (!AddressableManager.Singleton.TryGet<GameObject>(key, out var prefab))
            {
                // �����ε� �� �Ǿ� ������ ���� �ε� (����: ������ũ ���� �� �ε� ȭ�鿡�� �̸� �÷��δ� ���� ����)
                prefab = AddressableManager.Singleton.LoadAssetSync<GameObject>(key);
            }

            var go = UnityEngine.Object.Instantiate(prefab, position, rotation, parent);

            // ���
            liveCounts[key] = GetLiveCounts(liveCounts, key) + 1;

            if (!liveSets.TryGetValue(key, out var set))
            {
                set = new HashSet<GameObject>();
                liveSets[key] = set;
            }
            set.Add(go);
            instanceKey[go] = key;

            // �±�/������
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

            // �ν��Ͻ� Ű ã�� 
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

        // �ܺ� Destroy ���� ��� 
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
            // ��� �ν��Ͻ� �ı�
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

            // ���� ���� ������ AddressableManager�� ����
            AddressableManager.Singleton.ReleaseAllAssets();
        }

        private void TryReleaseAssetIfNoUse(string key)
        {
            // �ν��Ͻ� 0�� üũ�ؼ� ���� �õ�.
            // (SO/Scene/Sound ������ AddressableManager ������ ���� ����/��å ����)
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

        // �����/��ȸ��
        public void ShowLiveCount(string key)
        {
            int cnt = GetLiveCounts(liveCounts, key);
            Debug.Log(cnt);
        }
    }
}
