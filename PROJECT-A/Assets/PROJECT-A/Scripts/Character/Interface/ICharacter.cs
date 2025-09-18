using Character;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Character
{
    public interface ICharacter
    {
        public Transform Transform { get; }
        public IStats Stats { get; }
        public IHealth Health { get; }
        public IMovable Movable { get; }
        public IStateMachine StateMachine { get;}
        public ISkillSet SkillSet { get; }

        public RollAbility RollAbility { get; }
        public ActionLock Lock { get; }

        public CharacterAnimatorDriver Driver { get; }
        public CharacterCombat CharacterCombat { get; }

        public SpineSideFlip2D SpineSideFlip { get; }
    }
}
