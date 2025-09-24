using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Character
{
    public static class ProjectileRunner
    {
        public static void SpawnAndRun(ICharacter caster, SkillDefinition def, SkillCastContext ctx,
            System.Action<ICharacter, SkillCastContext, IReadOnlyList<ICharacter>> onHit)
        {
            var prefab = def.Projectile?.Prefab;
            if (prefab == null) return;

            var projObj = Object.Instantiate(prefab, caster.Transform.position, Quaternion.identity);
            var proj = projObj.GetComponent<ProjectileComponent>();
            if (proj == null) return;

            proj.Run(caster, def, ctx, onHit);
        }
    }
}