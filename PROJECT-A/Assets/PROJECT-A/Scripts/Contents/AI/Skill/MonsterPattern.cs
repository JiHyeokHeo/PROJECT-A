using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace A
{
    enum EWarningSign
    {
        Rush,
        Inner,
    }

    [Serializable]
    public abstract class MonsterPattern
    {
        // TODO : �������̽��� ���� CoolDown Casting �� �и� ����, ���� �ʿ��ϴٸ� �и�
        protected MonsterContext context;
        protected MonsterPatternSetSO patternSO;
        protected float castingTime;
        protected float usingTime;
        protected float attackRange;
        public float weight;

        public int coolDownGroupId;
        protected CooldownGroup cooldownGroup;

        private int consecutiveUses = 0;
        private bool isAvailableToLock = false;
        private bool isLocked = false;
        private int maxConsecutiveUses = 2;

        public virtual bool CheckExecuteCondition()
        {
            return false;
        }

        public void SetCooldownGroup(CooldownGroup group, int groupId = 0) // 0���� �׷� ���� ����
        {
            cooldownGroup = group;
            coolDownGroupId = groupId;
        }

        public bool IsReadyToExecute(float now)
        {
            if (isLocked && isAvailableToLock)
                return false;
        ;
            return cooldownGroup?.IsReady(now) ?? true; // �� ���� ������
        }

        public void ResetCooldown(float now)
        {
            cooldownGroup?.Trigger(now);

            consecutiveUses++;

            if (consecutiveUses >= maxConsecutiveUses)
            {
                isLocked = true;
                consecutiveUses = 0; // �ʱ�ȭ
            }
        }

        // �ٸ� ��ų ��� �� ȣ��
        public void ResetConsecutiveChain()
        {
            consecutiveUses = 0;
            isLocked = false; // ���� ����
        }

        //pattern.Init(context, definition.CoolDown, definition.Weight);
        public virtual void Init(MonsterContext context, MonsterPatternSetSO data)
        {
            patternSO = data;
            this.context = context;
            weight = data.Weight;
            attackRange = data.AttackRange;

            if (data.CooldownGroupId > 0)
                isAvailableToLock = true;
        }

        public abstract UniTask<bool> Execute(CancellationToken ct);
    }
}
