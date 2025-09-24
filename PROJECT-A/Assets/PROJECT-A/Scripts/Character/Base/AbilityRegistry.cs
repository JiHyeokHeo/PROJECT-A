using System.Collections;
using UnityEngine;

namespace Character
{
    [DefaultExecutionOrder(-60)] // CharacterBase보다 먼저 실행
    public class AbilityRegistry : MonoBehaviour
    {
        [Tooltip("ICapability를 구현한 컴포넌트만 넣으세요")]
        [SerializeField]
        MonoBehaviour[] abilities;

        private void Awake()
        {
            var hub = GetComponentInParent<CharacterBase>(true);
            if (hub == null)
            {
                Debug.LogError("Character Base 없음");
                return;
            }
            
            foreach (var a in abilities)
            {
                if (a == null)
                    continue;
                if (a is not ICapability)
                    continue;

                var ifaces = a.GetType().GetInterfaces();
                foreach (var itf in ifaces)
                {
                    if (itf == typeof(ICapability))
                        continue;
                    if (!typeof(ICapability).IsAssignableFrom(itf))
                        continue;
                    hub.RegisterCapability(itf, a);
                }
                hub.RegisterCapability(a.GetType(), a);
            }
        }
    }
}