using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace A
{
    public class PatternScheduler
    {
        MonsterContext monsterContext;
        float total;
        WeightedPattern[] patternWeights; // 패턴 가중치
        List<WeightedPattern> usablePatterns = new List<WeightedPattern>();

        public void SetUp(MonsterContext monsterContext)
        {
            this.monsterContext = monsterContext;
            BuildFromConfig();
        }

        void BuildFromConfig()
        {
            var set = monsterContext?.MonsterConfig.PatternSO;
            patternWeights = set?.Patterns;

            if (patternWeights == null || patternWeights.Length == 0 ) 
            {
                total = 0;
                return;
            }

            for (int i = 0; i < patternWeights.Length; i++)
            {
                patternWeights[i].Pattern.OnSetup(monsterContext);
                
            }
            total = patternWeights.Sum(index => Mathf.Max(0.001f, index.Weight));
        }

        public async UniTask ExecuteNext(CancellationToken ct)
        {
            if (patternWeights == null || patternWeights.Length == 0)
                return;

            usablePatterns.Clear();
            for (int i = 0; i < patternWeights.Length; i++)
            {
                if (patternWeights[i].Pattern.IsReady(Time.time))
                {
                    usablePatterns.Add(patternWeights[i]);
                }
            }

            var pick = Pick();
            if (pick == null)
                return;

            await pick.Pattern.Execute(ct);
            pick.Pattern.ResetCooldown(Time.time);
        }

        WeightedPattern Pick()
        {
            float r = UnityEngine.Random.value * total; 
            float acc = 0f; 
            foreach (var w in usablePatterns)
            { 
                acc += Mathf.Max(0.0001f, w.Weight); 
                if (r <= acc) 
                    return w; 
            }

            return null;
        }

    }
}
