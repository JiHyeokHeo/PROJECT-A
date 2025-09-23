using UnityEngine;

namespace A
{
    /// <summary>
    /// �� �ν��Ͻ��� � key(������)���� �Դ��� ǥ��.
    /// �ܺ� Destroy ��ο����� ResourceManager�� ȸ���� �� �ְ� ���´�.
    /// </summary>
    public sealed class ResourceTag : MonoBehaviour
    {
        [Tooltip("�� ������Ʈ�� ������ ��巹���� Ű(������ Ű)")]
        public string key;

        [HideInInspector] public bool notified; // �ߺ� ���� ����
    }

}