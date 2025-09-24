using Character;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Character
{
    public interface ICharacter
    {
        string Id { get; }
        Faction Faction { get; }
        Transform Transform { get; }

        IStats Stats { get; }
        IHealth Health { get; }
        IStateMachine StateMachine { get; }
        T GetCapability<T>() where T : class;
    }

    public enum Faction
    {
        Player = 0,
        Enemy = 1,
        Neutral = 2,
    }
}
