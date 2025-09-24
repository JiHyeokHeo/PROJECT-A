using Character;
using System.Collections.Generic;
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
    private sealed class YXComparer : IComparer<ICharacter>
    {
        public static readonly YXComparer Instance = new();
        public int Compare(ICharacter a, ICharacter b)
        {
            // y 우선
            float ay = a.Transform.position.y;
            float by = b.Transform.position.y;
            if (ay < by) return -1;
            if (ay > by) return 1;

            // x 다음
            float ax = a.Transform.position.x;
            float bx = b.Transform.position.x;
            if (ax < bx) return -1;
            if (ax > bx) return 1;
            int ai = a.Transform.GetInstanceID();
            int bi = b.Transform.GetInstanceID();
            return ai.CompareTo(bi);
        }
    }

    public static void StableOrderNonLinq(IEnumerable<ICharacter> chars, List<ICharacter> dst)
    {
        dst.Clear();
        dst.AddRange(chars);
        dst.Sort(YXComparer.Instance);
    }
}