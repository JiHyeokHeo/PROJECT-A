using Character;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;

public static class FormationUtility
{
    public static List<Vector2> BuildGridSlots(
        Vector2 origin,
        Vector2 forwardHint,
        int count,
        float spacing)
    {
        int cols = Mathf.CeilToInt(Mathf.Sqrt(count));
        int rows = Mathf.CeilToInt(count / (float)cols);

        var localSlots = new List<Vector2>(count);
        for (int i =0; i< count; i++)
        {
            int r = i / cols;
            int c = i % cols;

            float x = (c - (cols - 1) * 0.5f) * spacing;
            float y = (r - (rows - 1) * 0.5f) * spacing;
            localSlots.Add(new Vector2(x, y));
        }
        float angle = Mathf.Atan2(forwardHint.y, forwardHint.x);
        float cos = Mathf.Cos(angle);
        float sin = Mathf.Sin(angle);

        for (int i = 0; i < localSlots.Count; i++)
        {
            var p = localSlots[i];
            var rot = new Vector2(p.x * cos - p.y * sin, p.x * sin + p.y * cos);
            localSlots[i] = origin + rot;
        }
        return localSlots;
    }
    public static List<ICharacter> StableOrder(IEnumerable<ICharacter> chars)
    {
        return chars
            .OrderBy(c => c.Transform.position.y)
            .ThenBy(c => c.Transform.position.x)
            .ToList();
    }
}