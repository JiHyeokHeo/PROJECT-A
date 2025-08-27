using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Character
{
    public interface IStats
    {
        float MaxHP { get; }
        float HP { get; }
        float Atk { get; }
        float M_Atk { get; }
        float Heal { get; }
        float Def { get; }
        float MDef { get; }
        float IceDef { get; }
        float FireDef { get; }
        float EletricDef { get; }
        float LightDef { get; }
        float DarkDef { get; }
        float SlashDef { get; }
        float BluntDef { get; }
        float Acc { get; }
        float Eva { get; }
        float CoolRed { get; }
        float CastSpeed { get; }
        float AtkSpeed { get; }
        float Duration { get; }
        float CritRate { get; }
        float CritDamage { get; }
        float StatusResist { get; }
        float Speed { get; }
        float GetExp { get; }
        float GetNyang { get; }
        float GetDrop { get; }
    }
}
