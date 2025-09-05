using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

namespace A
{
    public abstract class MonsterPattern
    {
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
