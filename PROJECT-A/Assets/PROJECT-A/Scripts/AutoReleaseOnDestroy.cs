using UnityEngine;

namespace A
{
    /// <summary>
    /// 매니저를 거치지 않은 외부 Destroy 경로에서도
    /// ResourceManager가 라이프사이클을 정리할 수 있도록 도와주는 안전핀.
    /// </summary>
    public sealed class AutoReleaseOnDestroy : MonoBehaviour
    {
        private ResourceManagerEX rm;

        public void Bind(ResourceManagerEX rm)
        {
            this.rm = rm;
        }

        private void OnDestroy()
        {
            if (rm == null) 
                return;

            var tag = GetComponent<ResourceTag>();
            if (tag != null && !tag.notified)
            {
                tag.notified = true;
                rm.NotifyDestroyed(gameObject, tag.key);
            }
            else
            {
                rm.NotifyDestroyed(gameObject, null);
            }
        }
    }
}
