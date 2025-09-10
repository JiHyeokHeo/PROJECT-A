using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Character
{
    public class NavPathfinder : MonoBehaviour, IPathfinder
    {
        //최대 스냅 거리
        [SerializeField]
        private float sampleMaxDistance = 3.0f;
        // 허용 영역
        [SerializeField]
        private int areaMask = NavMesh.AllAreas;

        [SerializeField]
        private float cornerMergeDistance = 0.08f;

        // NevMesh 좌표 2D 변환
        Vector3 ToNav3(Vector2 v) => (new Vector3(v.x, v.y, 0f));
        Vector2 ToWorld2(Vector3 v) => (new Vector2(v.x, v.y));

        public List<Vector2> FindPath(Vector2 start, Vector2 goal)
        {
            var s3 = ToNav3(start);
            var g3 = ToNav3(goal);
            // NavMesh 위의 가장 가까운 유효 포인트로 스냅.
            // 시작 / 목표 둘 중 하나라도 NavMesh에서 너무 멀면 경로 없음(null).
            if (!NavMesh.SamplePosition(s3, out var sHit, sampleMaxDistance, areaMask))
                return null;
            if (!NavMesh.SamplePosition(g3, out var gHit, sampleMaxDistance, areaMask))
                return null;

            var navPath = new NavMeshPath();
            if (!NavMesh.CalculatePath(sHit.position, gHit.position, areaMask, navPath))
                return null;
            if (navPath.status != NavMeshPathStatus.PathComplete)
                return null;

            var corners = navPath.corners;
            var list = new List<Vector2>(corners.Length + 1);
            // Corner 포인트들이 너무 촘촘하면 병합해서 떨림 감소.
            Vector3 prev = corners.Length > 0 ? corners[0] : sHit.position;
            foreach (var corner in corners )
            {
                var v2 = ToWorld2(corner);
                if ((v2 - ToWorld2(prev)).sqrMagnitude >= cornerMergeDistance *  cornerMergeDistance)
                {
                    list.Add(v2);
                    prev = corner;
                }
            }
            var goal2 = ToWorld2(gHit.position);
            if (list.Count == 0 || (list[^1] - goal2).sqrMagnitude > 0.0001f)
                list.Add(goal2);
            return list;
        }

    }
}
