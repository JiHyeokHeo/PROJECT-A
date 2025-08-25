using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TST
{
    // ���ӿ���, Main�� �����ϴ� ���� ���� �������̽��� ������ �߻� �θ� Ŭ����
    public abstract class SceneBase : MonoBehaviour
    {
        public abstract IEnumerator OnStart();

        public abstract IEnumerator OnEnd();
    }
}
