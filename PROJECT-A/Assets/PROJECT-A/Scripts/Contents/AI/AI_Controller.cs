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
            Current.Exit(); // �� ���� ��
            Current = next;
            //stateChangeEvent?.Invoke(); // Ȥ�ö� �������ڸ��� �̺�Ʈ�� �ʿ��ϴٸ� ����
            Current.Enter();
        }

        void Update()
        {
            Current?.Tick(Time.deltaTime);
        }
    }
}
