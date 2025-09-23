using UnityEngine;

namespace A
{
    /// <summary>
    /// 이 인스턴스가 어떤 key(프리팹)에서 왔는지 표시.
    /// 외부 Destroy 경로에서도 ResourceManager가 회수할 수 있게 돕는다.
    /// </summary>
    public sealed class ResourceTag : MonoBehaviour
    {
        [Tooltip("이 오브젝트가 생성된 어드레서블 키(프리팹 키)")]
        public string key;

        [HideInInspector] public bool notified; // 중복 통지 방지
    }

}