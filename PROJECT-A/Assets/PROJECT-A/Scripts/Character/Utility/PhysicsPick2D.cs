using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Character
{
    public class PhysicsPick2D
    {   
        static readonly Collider2D[] _pointBuf = new Collider2D[8]; // 점 피킹용 작은 버퍼
        static readonly Collider2D[] _areaBuf = new Collider2D[256]; // 영역 피킹용 큰 버퍼

        public static ISelectable PickSelectableAt(Vector2 worldPoint, LayerMask characterLayer)
        {
            int n = Physics2D.OverlapPointNonAlloc(worldPoint, _pointBuf, characterLayer);

            for (int i = 0;  i < n; i++)
            {
                var col = _pointBuf[i];
                if (!col)
                    continue;
                var sel = col.GetComponentInParent<ISelectable>();
                if (sel != null)
                    return sel;
            }
            return null;
        }

        public static ICharacter PickCharacterAt(Vector2 worldPoint, LayerMask characterLayer)
        {
            int n = Physics2D.OverlapPointNonAlloc(worldPoint, _pointBuf, characterLayer);
            for (int i = 0; i < n; i++)
            {
                var col = _pointBuf[i];
                if (!col)
                    continue;
                var ch = col.GetComponentInParent<ICharacter>();
                if (ch != null)
                    return ch;
            }
            return null;
        }

        public static void BoxSelect(Vector2 worldMin, Vector2 worldMax, LayerMask layer, List<ISelectable> outList)
        {
            outList.Clear();
            int n = Physics2D.OverlapAreaNonAlloc(worldMin, worldMax, _areaBuf, layer);
            for (int i =0; i < n; i++)
            {
                var col = _areaBuf[i];
                if (!col)
                    continue;
                var sel = col.GetComponentInParent<ISelectable>();
                if (sel != null)
                    outList.Add(sel);
            }
        }

        public static bool IsGround(Vector2 worldPoint, LayerMask groundLayer)
        => Physics2D.OverlapPointNonAlloc(worldPoint, _pointBuf, groundLayer) > 0;
    }
}