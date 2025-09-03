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

        public abstract UniTask Execute(CancellationToken ct);
    }
}
