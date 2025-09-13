using Character;
using System;
using System.Collections;
using UnityEngine;

public static class CombatSearch 
{
    // 반경 내에서 필터를 만족하는 ICharcater 중 "가장 가까운 대상"을 반환
    public static ICharacter NearestCharacter(
        Vector2 origin,
        float radius,
        LayerMask mask,
        Collider2D[] buffer,
        Func<ICharacter, bool> filter = null)
    {
        int n = Physics2D.OverlapCircleNonAlloc(origin, radius, buffer, mask);

        ICharacter best = null;
        float bestDsq = float.PositiveInfinity;

        for (int i = 0; i< n; i++)
        {
            var col = buffer[i];
            if (!col)
                continue;

            var ch = col.GetComponent<ICharacter>();
            if (ch == null)
                continue;

            if (filter != null && !filter(ch))
                continue;

            float dsq = ((Vector2)ch.Transform.position - origin).sqrMagnitude;
            if (dsq < bestDsq)
            {
                bestDsq = dsq;
                best = ch;
            }
        }
        return best;
    }

    //public static ICharacter HightestHP(Vector2 origin, float radius, LayerMask mask, Collider2D[] buffer, Func<ICharacter, bool> filter = null)
    //{
    //    int n = Physics2D.OverlapCircleNonAlloc(origin, radius, buffer, mask);
    //    ICharacter best = null;
    //    float bestHp = float.NegativeInfinity;

    //    for (int i =0; i < n; i++)
    //    {
    //        var col = buffer[i];
    //        if (!col)
    //            continue;
    //        var ch = col.GetComponent<ICharacter>();
    //        if (filter != null && !filter(ch))
    //            continue;

    //        // 여기에 HP가 많은 친구를 찾아 공격 (지혁님하고 동기화)
    //        //float hp = ch.Health

    //    }
    //}
    

    public static bool IsAlive(ICharacter ch)
        => ch != null && ch.Health != null && !ch.Health.IsDead;
    public static Func<ICharacter, bool> Alive()
    => ch => IsAlive(ch);

    public static Func<ICharacter, bool> AliveExclude(ICharacter exclude)
        => ch => IsAlive(ch) && !ReferenceEquals(ch, exclude);

    public static bool IsDead(ICharacter ch)
    => ch != null && ch.Health != null && ch.Health.IsDead;

    public static Func<ICharacter, bool> Dead()
        => ch => IsDead(ch);
}
