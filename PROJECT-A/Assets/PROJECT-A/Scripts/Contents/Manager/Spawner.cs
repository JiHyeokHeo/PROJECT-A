using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;


namespace A
{
    public class Spawner : MonoBehaviour
    {
        public static Spawner instance;
        [SerializeField] GameObject spawnObject;

        [Header("Spawn Settings")]
        [SerializeField, Min(0f)] private float spawnIntervalSeconds = 1f;
        [SerializeField] private Vector2 xRange = new Vector2(-300, 300);
        [SerializeField] private Vector2 yRange = new Vector2(-300, 300);
        [SerializeField] private bool autoStart = true;

        private CancellationTokenSource cancellationTokenSource;

        void Awake()
        {
            if (instance == null)
                instance = this;

            if (autoStart)
            {
                StartSpawning();
            }
        }

        void Start()
        {
            
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.V))
            {
                cancellationTokenSource.Cancel();
            }
        }

        private async UniTask Spawn(float seconds, CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                Instantiate(spawnObject, GetRandomPosition(), Quaternion.identity);

                try
                {
                    await UniTask.Delay(TimeSpan.FromSeconds(seconds),
                                        DelayType.DeltaTime,
                                        PlayerLoopTiming.Update,
                                        token);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        }

        public void StartSpawning(float? intervalOverride = null)
        {
            if (cancellationTokenSource != null && !cancellationTokenSource.IsCancellationRequested) return;
            cancellationTokenSource = new CancellationTokenSource();

            float interval = intervalOverride ?? spawnIntervalSeconds;
            Spawn(interval, cancellationTokenSource.Token).Forget();
        }

        public void StopSpawning()
        {
            if (cancellationTokenSource == null) return;
            cancellationTokenSource.Cancel();
            cancellationTokenSource.Dispose();
            cancellationTokenSource = null;
        }

        Vector3 GetRandomPosition()
        {
            float x = UnityEngine.Random.Range(xRange.x, xRange.y);
            float y = UnityEngine.Random.Range(yRange.x, yRange.y);
            return new Vector3(x, y, 0f);
        }

#if UNITY_EDITOR
        void OnDrawGizmosSelected()
        {
            // 스폰 영역 가시화 (XY 평면)
            Gizmos.color = Color.cyan;
            var min = new Vector3(Mathf.Min(xRange.x, xRange.y), Mathf.Min(yRange.x, yRange.y), 0f);
            var max = new Vector3(Mathf.Max(xRange.x, xRange.y), Mathf.Max(yRange.x, yRange.y), 0f);
            var center = (min + max) * 0.5f;
            var size = (max - min);
            Gizmos.DrawWireCube(center, size);
        }
#endif
    }
}
