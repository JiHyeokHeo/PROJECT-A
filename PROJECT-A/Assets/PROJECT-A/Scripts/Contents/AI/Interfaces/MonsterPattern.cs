using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace A
{
    [Serializable]
    public abstract class MonsterPattern
    {
        public abstract float Cooldown { get; }

        protected MonsterContext MonsterContext { get; set; }

        public abstract void OnSetup(MonsterContext monsterContext);

        public float NextAvailableTime { get; protected set; }

        public void ResetCooldown(float now)
        {
            NextAvailableTime = now + Cooldown;
        }

        public bool IsReady(float now) => now >= NextAvailableTime;

        public abstract UniTask Execute(CancellationToken ct);
    }
}
