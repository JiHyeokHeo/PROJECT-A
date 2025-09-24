using System.Collections.Generic;
using UnityEngine;

namespace Character
{
    public class ProjectileComponent : MonoBehaviour
    {
        ICharacter _caster;
        SkillDefinition _def;
        SkillCastContext _ctx;
        System.Action<ICharacter, SkillCastContext, IReadOnlyList<ICharacter>> _onHit;

        Vector2 _dir;
        float _speed;
        bool _pierce;

        public void Run(ICharacter caster, SkillDefinition def, SkillCastContext ctx,
            System.Action<ICharacter, SkillCastContext, IReadOnlyList<ICharacter>> onHit)
        {
            _caster = caster;
            _def = def;
            _ctx = ctx;
            _onHit = onHit;

            _speed = def.Projectile.Speed;
            _pierce = def.Projectile.Pierce;
            _dir = (ctx.Point - (Vector2)caster.Transform.position).normalized;
        }

        private void Update()
        {
            transform.position += (Vector3)(_dir * (_speed * Time.deltaTime));
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var target = other.GetComponentInParent<ICharacter>();
            if (target != null && target != _caster)
            {
                var list = new List<ICharacter> { target };
                _onHit?.Invoke(_caster, _ctx, list);

                if (!_pierce)
                    Destroy(gameObject);
            }
        }
    }
}