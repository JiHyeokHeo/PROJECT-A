using Character;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace A
{
    public interface ICharacter
    {
        public Transform Transform { get; }
        public IStats Stats { get; }
        public IHealth Health { get; }
        public IMovable Movable { get; }
        public IStateMachine StateMachine { get;}
        public ISkillSet SkillSet { get; }
    }
}
