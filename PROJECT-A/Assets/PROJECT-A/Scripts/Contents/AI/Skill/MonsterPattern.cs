using Cysharp.Threading.Tasks;
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

    public abstract class MonsterPattern
    {
        // TODO : �������̽��� ���� CoolDown Casting �� �и� ����, ���� �ʿ��ϴٸ� �и�
        protected MonsterContext context;
        protected float castingTime;
        protected float attackRange;
        public float weight;

        protected CooldownGroup cooldownGroup;

        private int consecutiveUses = 0;
        private bool isLocked = false;
        private int maxConsecutiveUses = 2;

        public void SetCooldownGroup(CooldownGroup group)
        {
            cooldownGroup = group;
        }

        public bool IsReadyToExecute(float now)
        {
            if (isLocked)
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
        public abstract void Init(MonsterContext context, MonsterPatternSetSO data);

        public abstract UniTask<bool> Execute(CancellationToken ct);
    }
}
