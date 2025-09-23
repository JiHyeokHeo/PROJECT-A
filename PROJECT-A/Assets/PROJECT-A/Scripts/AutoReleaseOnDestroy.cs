using UnityEngine;

namespace A
{
    /// <summary>
    /// �Ŵ����� ��ġ�� ���� �ܺ� Destroy ��ο�����
    /// ResourceManager�� ����������Ŭ�� ������ �� �ֵ��� �����ִ� ������.
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
