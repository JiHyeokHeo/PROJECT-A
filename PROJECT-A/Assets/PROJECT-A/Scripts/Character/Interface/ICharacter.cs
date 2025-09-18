using Character;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Character
{
    public interface ICharacter
    {
        string Id { get; }
        Faction faction { get; }
        Transform Transform { get; }

        IStats Stats { get; }
        IHealth Health { get; }
        IStateMachine StateMachine { get; }
        T GetCapability<T>() where T : class;
    }

    public enum Faction
    {
        Player = 0,
        Enmey = 1,
        Neutral = 2,
    }
}
