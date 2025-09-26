using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TST;
using Unity.VisualScripting;
using UnityEngine;

namespace A
{
    public class CooldownGroup
    {
        private float cooldown;
        private float nextAvailableTime;

        public CooldownGroup(float cooldown)
        {
            this.cooldown = cooldown;
            nextAvailableTime = 0f;
        }

        public bool IsReady(float now) => now >= nextAvailableTime;

        public void Trigger(float now) => nextAvailableTime = now + cooldown;
    }

    public class PatternScheduler
    {
        MonsterContext monsterContext;
        float total; // 가중치
        List<MonsterPattern> skillCoolPatterns = new List<MonsterPattern>();
        List<MonsterPattern> transitionPatterns = new List<MonsterPattern>(); // 특정 조건에 의해 전환되는 패턴
        List<MonsterPattern> usablePatterns = new List<MonsterPattern>();

        MonsterPattern currentPattern;
        Dictionary<int, CooldownGroup> cooldownGroups = new Dictionary<int, CooldownGroup>();

        private float tickTime = 0.1f;
        private float lastTick = 0f;

        // 만약 어떠한 조건이 발생한다면 싹다 무시하고 패턴을 날린 후 시작
        public void UpdateTick()
        {
            if (Time.time < lastTick + tickTime)
                return;

            lastTick += tickTime;

            if (monsterContext == null || transitionPatterns.Count <= 0)
                return;

            for (int i = 0; i < transitionPatterns.Count; i++)
            {
                if (currentPattern != null && transitionPatterns[i].Equals(currentPattern))
                    return;

                if (transitionPatterns[i].CheckExecuteCondition() == false)
                    return;

                currentPattern = transitionPatterns[i];
                RunPatternSafe(transitionPatterns[i], monsterContext.CancellationToken.Token).Forget();
            }
        }

        async UniTaskVoid RunPatternSafe(MonsterPattern pat, CancellationToken ct)
        {
            await pat.Execute(ct);
            currentPattern = null;
        }

        public void SetUp(MonsterContext monsterContext)
        {
            this.monsterContext = monsterContext;
            var patternArr = monsterContext.Config.PatternSO;
            for (int i = 0; i < monsterContext.Config.PatternSO.Length; i++)
            {
                var pattern = PatternFactory.Create(patternArr[i]);
                pattern.Init(monsterContext, patternArr[i]);

                // 그룹 연결
                int groupId = patternArr[i].CooldownGroupId;
                if (groupId > 0) // 공유 그룹
                {
                    if (!cooldownGroups.ContainsKey(groupId))
                        cooldownGroups[groupId] = new CooldownGroup(patternArr[i].CoolDown);

                    pattern.SetCooldownGroup(cooldownGroups[groupId], groupId);
                }
                else // 독립 그룹
                {
                    pattern.SetCooldownGroup(new CooldownGroup(patternArr[i].CoolDown), 0);
                }

                // 스킬쿨 보유를 나눠두자
                if (patternArr[i].hasCoolDown)
                    skillCoolPatterns.Add(pattern);
                else
                    transitionPatterns.Add(pattern);
            }

            BuildFromConfig();
        }

        void BuildFromConfig()
        {
            var set = monsterContext?.Config.PatternSO;

            if (monsterContext.Config.PatternSO.Length == 0 ) 
            {
                total = 0;
                return;
            }
            
            total = monsterContext.Config.PatternSO.Sum(index => Mathf.Max(0.001f, index.Weight));
        }

        public async UniTask ExecuteNext(CancellationToken ct)
        {
            if (currentPattern != null)
                return;

            usablePatterns.Clear();
            for (int i = 0; i < skillCoolPatterns.Count; i++)
            {
                // 첫번째 쿨 확인
                if (skillCoolPatterns[i].IsReadyToExecute(Time.time))
                {
                    usablePatterns.Add(skillCoolPatterns[i]);
                }
            }

            var pick = Pick();
            if (pick == null)
                return;
            currentPattern = pick;

            // 성공적으로 리턴을 했을 때에만 스킬 쿨 초기화
            if (await pick.Execute(ct))
            {
                currentPattern = null;
                pick.ResetCooldown(Time.time); // 스킬 쿨초

                foreach (var p in skillCoolPatterns)
                {
                    if (p == pick)
                        continue;

                    if (p.coolDownGroupId == pick.coolDownGroupId)
                        p.ResetConsecutiveChain(); // 봉인 해제
                }
            }
        }

        MonsterPattern Pick()
        {
            float r = UnityEngine.Random.value * total; 
            float acc = 0f; 
            foreach (var w in usablePatterns)
            { 
                acc += Mathf.Max(0.0001f, w.weight); 
                if (r <= acc) 
                    return w; 
            }

            return null;
        }

    }
}
