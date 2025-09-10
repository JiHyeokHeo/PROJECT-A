using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

namespace A
{
    public abstract class MonsterPattern
    {
        // TODO : 인터페이스를 통한 CoolDown Casting 더 분리 가능, 추후 필요하다면 분리
        protected MonsterContext context;
        protected float cooldown;
        protected float castingTime;
        public float weight;
        protected float nextExecuteTime;

        public bool IsReadyToExecute(float now)
        {
            return now >= nextExecuteTime;
        }

        public void ResetCooldown(float now)
        {
            nextExecuteTime = now + cooldown;
        }

        //pattern.Init(context, definition.CoolDown, definition.Weight);
        public abstract void Init(MonsterContext context, MonsterPatternSetSO data);

        public abstract UniTask Execute(CancellationToken ct);
    }
}
