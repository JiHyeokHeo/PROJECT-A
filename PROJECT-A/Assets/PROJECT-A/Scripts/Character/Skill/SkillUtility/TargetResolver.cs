namespace Character
{
    public static class TargetResolver
    {
        public static bool CheckContextValid(ICharacter caster, SkillDefinition def, SkillCastContext ctx)
        {
            return true;
        }

        public static void FillTargets(ICharacter caster, SkillDefinition def, SkillCastContext ctx)
        {
            switch (def.useDistance)
            {
                case UseDistance.Self:
                    ctx.AddTarget(caster);
                    break;
                case UseDistance.Melee:
                case UseDistance.Ranged:
                case UseDistance.Targeted:
                    break;
                case UseDistance.Aura:
                case UseDistance.AoE:
                    break;
            }
        }
    }
}