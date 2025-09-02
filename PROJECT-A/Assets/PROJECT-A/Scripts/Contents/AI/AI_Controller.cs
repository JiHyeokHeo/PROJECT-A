using Cysharp.Threading.Tasks.Triggers;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace A
{
    public class AI_Controller : MonoBehaviour
    {
        public IAIState Current { get; private set; }

        
        public void AIStateChange(IAIState next)
        {
            if (next == null || Current == next)
                return;
            Current.Exit(); // 씬 나간 후
            Current = next;
            //stateChangeEvent?.Invoke(); // 혹시라도 시작하자마자 이벤트가 필요하다면 실행
            Current.Enter();
        }

        void Update()
        {
            Current?.Tick(Time.deltaTime);
        }
    }
}
