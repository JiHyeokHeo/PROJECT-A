using System.Collections.Generic;

namespace Character
{
    public static class EffectExecutor
    {
        public static void Apply(EffectDefinition e, ICharacter caster, IReadOnlyList<ICharacter> targets,
            SkillCastContext ctx, SkillDefinition def)
        {
            switch (e.Kind)
            {
                case EffectKind.Damage:
                    foreach (var t in targets) 
                        DealDamage(caster, t, e, def);
                    break;
                case EffectKind.Heal:
                    foreach (var t in targets) 
                        Heal(t, e);
                    break;
                case EffectKind.Buff:
                    foreach (var t in targets) 
                        BuffSystem.Apply(t, e.Buff);
                    break;
                case EffectKind.Debuff:
                    foreach (var t in targets) 
                        BuffSystem.Apply(t, e.Buff); // 부정 태그
                    break;
                case EffectKind.Shield:
                    foreach (var t in targets) 
                        ShieldSystem.Apply(t, e.Value, e.Duration);
                    break;
                case EffectKind.CC:
                    foreach (var t in targets) 
                        CcSystem.Apply(t, e.Cc);
                    break;
                case EffectKind.Threat:
                    foreach (var t in targets) 
                        ThreatSystem.AddThreat(t, caster, e.Value);
                    break;
            }
        }

        static void DealDamage(ICharacter caster, ICharacter target, EffectDefinition e, SkillDefinition def)
        {
            // 무기 태그에 따라 물리/마법 배율 선택(기획 2번 규칙)
            var stats = caster.Stats;
            float atk = WeaponScaling.Select(stats, def.Tags); // 물리/마법
            float dmg = atk * e.Value;
            target.Health?.DealDamage(dmg, e.Tags);
        }

        static void Heal(ICharacter target, EffectDefinition e)
        {
            target.Health?.Heal(e.Value);
        }
    }
}