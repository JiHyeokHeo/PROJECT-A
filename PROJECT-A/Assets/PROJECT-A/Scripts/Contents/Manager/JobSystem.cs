using UnityEngine;
using Unity.Burst;
using Unity.Jobs;
using Unity.Collections;

// Job System�� �Բ� ���ǿ� C# �ڵ带 ���� ����Ƽ�� �ڵ�� ������ �Ͽ� ���� �ӵ� �ش�ȭ
[BurstCompile]
public struct SimpleJob : IJob
{
    public int a;
    public int b;
    public NativeArray<int> result;

    public void Execute()
    {
        result[0] = a + b;
    }
}

namespace A
{
    public class JobSystem : MonoBehaviour
    {
        void Start()
        {
            NativeArray<int> result = new NativeArray<int>(1, Allocator.TempJob);

            SimpleJob job = new SimpleJob
            {
                a = 5,
                b = 10,
                result = result
            };
            
            for (int i = 0; i < 1000; i ++)
            {
                JobHandle handle = job.Schedule();
                handle.Complete();
            }

            Debug.Log("Result : " + result[0]);

            result.Dispose();
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
