using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

namespace Character
{
    public sealed class SkillRuntime : MonoBehaviour, ISkill
    {
        [SerializeField] 
        private SkillDefinition def;
        [SerializeField] 
        private KeyCode hotkey = KeyCode.None;

        private ICharacter _owner;
        private float _cd;
        private bool _on;

        public KeyCode HotKey => hotkey;
        public SkillKind Kind => def.kind;
        public SkillTargetType TargetType => def.targetType;
        public bool IsOn => _on;
        public bool IsCoolingDown => _cd > 0f;
        public float RemainingCooldown => _cd;

        public void Initialize(ICharacter owner)
        {
            _owner = owner;
        }

        public void Tick(ICharacter caster, float dt)
        {
            if (_cd > 0)
                _cd -= dt;

            // 메크로: 쿨끝 + 조건충족 + (필요시 애니 대기 해제) ⇒ 자동 Cast
            if (def.kind == SkillKind.Macro && _cd <= 0f &&
                MacroReady(caster) && !AnimationGateActive())
            {
                var ctx = SkillCastContext.Auto(caster, def);
                if (CanCast(caster, ctx))
                    Cast(caster, ctx);
            }

            // 토글 유지형: 켜져 있으면 자원 소모/조건 불만족 시 자동 Off
            if (def.kind == SkillKind.ActiveToggle && _on)
                MaintainToggleOrTurnOff(caster, dt);
        }

        public bool CanCast(ICharacter caster, SkillCastContext ctx)
        {
            if (caster == null)
                return false;
            if (def.kind != SkillKind.ActiveToggle && IsCoolingDown)
                return false;
            if (AnimationGateActive() && def.kind == SkillKind.Macro)
                return false;

            if (def.targetingPolicy == TargetingPolicy.Required && !ctx.HasAnyTarget)
                return false;

            return TargetResolver.CheckContextValid(caster, def, ctx);
        }

        public void Cast(ICharacter caster, SkillCastContext ctx)
        {
            //토글 스킬이면 On/Off
            if (def.kind == SkillKind.ActiveToggle)
            {
                if (!_on)
                    TurnOn(caster, ctx);
                else
                    TurnOff(caster);
                return;
            }
            
            //토글이 아닐 떄 실행
            Execute(caster, ctx);
            
            StartCooldown();
        }

        // === 이하 내부: 실행 파이프라인 ===
        void Execute(ICharacter caster, SkillCastContext ctx)
        {
            //투사체 기반이면 Runner에 위임
            if (def.useProjectile)
                ProjectileRunner.SpawnAndRun(caster, def, ctx, ApplyEffects);
            // 아니면 즉시형
            else
                ApplyEffects(caster, ctx, ctx.Targets);
        }

        //스킬에 묶인 모든 효과(데미지/힐/버프/CC/어그로 등)을 대상 집합에 순차 적용
        void ApplyEffects(ICharacter caster, SkillCastContext ctx, IReadOnlyList<ICharacter> targets)
        {
            foreach (var eff in def.effects)
                EffectExecutor.Apply(eff, caster, targets, ctx, def);
        }
        
        // 성공 시전에만 쿨다운 타이머 시작.
        void StartCooldown()
        {
            if (def.cooldown > 0) 
                _cd = def.cooldown;
        }
        
        //매크로는 기본 공격/다른 액티브 애니가 끝날 떄까지 대기
        //ActionLock이 걸려 있으면 매크로 자동시전 보류
        bool AnimationGateActive()
        {
            if (!def.WaitForAnimation) 
                return false;
            var lockr = _owner.GetCapability<ActionLock>();
            return lockr != null && lockr.IsLocked; // 기본공격/다른 액티브 애니 중
        }
        
        //토글형 스킬 켤 때 1회성 초기 효과(버프 부여/즉시 데미지 등)을 실행
        //끌 때는 유지효과 해제
        void TurnOn(ICharacter caster, SkillCastContext ctx)
        {
            _on = true;
            ApplyEffects(caster, ctx, ctx.Targets);
        }

        void TurnOff(ICharacter caster)
        {
            _on = false; /* 지속효과 해제 등 */
        }

        void MaintainToggleOrTurnOff(ICharacter caster, float dt)
        {
            /* 자원 소모/조건 검사 */
        }

        bool MacroReady(ICharacter caster)
        {
            /* 범위 내 대상 존재? 타겟 정책 준수? */
            return true;
        }
    }
}