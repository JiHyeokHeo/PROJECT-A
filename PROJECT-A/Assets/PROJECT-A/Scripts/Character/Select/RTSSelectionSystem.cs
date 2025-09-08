using A;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEditor.PlayerSettings;

namespace Character
{
    public class RTSSelectionSystem : MonoBehaviour
    {

        [Header("Refs")]
        [SerializeField]
        private Camera worldCam;

        [SerializeField]
        private RectTransform selectionBox;

        [SerializeField]
        private RectTransform selectionBoxCanvas;

        [SerializeField]
        private LayerMask characterLayer;

        [SerializeField]
        private LayerMask groundLayer;

        [SerializeField]
        private LayerMask enemyLayer;

        // 클릭인지 드래그인지 판단하는 임계값
        [SerializeField]
        private float ClickThresholdSqr = 16f;

        // 포메이션 슬롯 간격
        [SerializeField]
        private float spacing = 1.2f;

        //같은 프레임에 중복 이동 명령이 여러 번 나가는 것을 방지
        int _lastIssueFrame = -1;

        //현재 선택된 유닛
        [SerializeField]
        private readonly List<ISelectable> selected = new();

        //드래그 시작점(스크린 & UI)
        private Vector2 dragStartScreen;
        private Vector2 dragStartUIScreen;

        // 현재 드래그 중인지
        private bool dragging = false;
        
        //A 키 무브 상태
        private bool attackMovePrimed = false;


        void Update()
        {
            // UI 무시
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;
        }

        private void OnEnable()
        {
            TST.InputSystem.onDrag += DragLogic;
            TST.InputSystem.onDragEnd += DragEndLogic;
            TST.InputSystem.onMove += IssueMove;
            TST.InputSystem.onAttackMovePrime += onAttackMovePrime;
            TST.InputSystem.onCast += TryCast;
        }
        private void OnDisable()
        {
            TST.InputSystem.onDrag -= DragLogic;
            TST.InputSystem.onDragEnd -= DragEndLogic;
            TST.InputSystem.onMove -= IssueMove;
            TST.InputSystem.onAttackMovePrime -= onAttackMovePrime;
            TST.InputSystem.onCast -= TryCast;
        }
        
        // A 무브 토글
        private void onAttackMovePrime()
        {
            attackMovePrimed = !attackMovePrimed;
            
        }
      

        private void DragLogic()
        {
            if (dragging == false)
            {
                dragging = true;
                dragStartScreen = Input.mousePosition;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(selectionBoxCanvas, Input.mousePosition, worldCam, out dragStartUIScreen);
                selectionBox.gameObject.SetActive(true);
            }
            UpdateDrag();
        }
        private void DragEndLogic()
        {
            if (dragging == true)
            {
                dragging = false;
                selectionBox.gameObject.SetActive(false);
                EndDrag();
            }
        }

        void UpdateDrag()
        {
            Vector2 cur;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(selectionBoxCanvas, Input.mousePosition, worldCam, out cur);
            Vector2 min = Vector2.Min(dragStartUIScreen, cur);
            Vector2 size = Vector2.Max(dragStartUIScreen, cur) - min;

            selectionBox.anchoredPosition = min;
            selectionBox.sizeDelta = size;
        }

        void EndDrag()
        {
            bool isClick = ((Vector2)Input.mousePosition - dragStartScreen).sqrMagnitude < ClickThresholdSqr;
            
            if (isClick && attackMovePrimed)
            {
                Vector2 wp = worldCam.ScreenToWorldPoint(Input.mousePosition);

                var enemyHit = Physics2D.OverlapPoint(wp, enemyLayer);
                if (enemyHit)
                {
                    var targetSel = enemyHit.GetComponent<ICharacter>();
                    IssueCommandAttackMove(wp, targetSel);
                    attackMovePrimed = false;
                    return;
                }

                if (Physics2D.OverlapPoint(wp, groundLayer))
                {
                    IssueCommandAttackMove(wp, null);
                    attackMovePrimed = false;
                    return;
                }

                attackMovePrimed = false;
                return;
            }
            
            if (isClick)
            {
                Vector2 wp = worldCam.ScreenToWorldPoint(Input.mousePosition);
                var hit = Physics2D.OverlapPoint(wp, characterLayer);
                if (hit != null)
                {
                    var sel = hit.GetComponentInParent<ISelectable>();
                    ReplaceSelection(sel);
                }
                else
                {
                    ClearSelection();
                }
            }
            else
            {
                Vector2 startPoint = worldCam.ScreenToWorldPoint(dragStartScreen);
                Vector2 endPoint = worldCam.ScreenToWorldPoint(Input.mousePosition);
                Vector2 min = Vector2.Min(startPoint, endPoint);
                Vector2 max = Vector2.Max(startPoint, endPoint);

                var hits = Physics2D.OverlapAreaAll(min, max, characterLayer);
                ClearSelection();

                foreach (var hit in hits)
                {
                    var sel = hit.GetComponentInParent<ISelectable>();
                    if (sel != null)
                        AddSelection(sel);
                }

            }
        }

        void ReplaceSelection(ISelectable select)
        {
            ClearSelection();
            AddSelection(select);
        }

        void AddSelection(ISelectable select)
        {
            if (select == null || selected.Contains(select))
                return;
            selected.Add(select);
            select.SetSelected(true);
        }

        void ClearSelection()
        {
            foreach (var s in selected)
                s?.SetSelected(false);
            selected.Clear();
        }

        void IssueCommandAttackMove(Vector2 worldPoint, ICharacter explicitTarget)
        {
            // 선택된 유닛 -> ICharacter 목록으로
            var chars = selected
                .Select(s => (s as Component)?.GetComponent<ICharacter>())
                .Where(c => c != null)
                .ToList();
            if (chars.Count == 0) return;

            // 포메이션 슬롯 계산
            var ordered = FormationUtility.StableOrder(chars);
            var center = new Vector2(
                ordered.Average(c => c.Transform.position.x),
                ordered.Average(c => c.Transform.position.y)
            );
            var forward = (worldPoint - center);
            if (forward.sqrMagnitude < 0.0001f) forward = Vector2.right;

            var slots = FormationUtility.BuildGridSlots(worldPoint, forward.normalized, ordered.Count, spacing);

            // 유닛별 명령 내리기
            for (int i = 0; i < ordered.Count; i++)
            {
                var unit = ordered[i];
                if (unit.Lock.IsLocked)
                    continue;
                if (unit.CharacterCombat != null)
                {
                    // 명시 타깃 공격: 타깃이 사라지면 해당 슬롯 위치로 이동하게 폴백 포인트 전달
                    if (explicitTarget != null)
                    {
                        unit.CharacterCombat.CancelAll();
                        unit.CharacterCombat.IssueAttackTarget(explicitTarget, unit);
                    }
                    else
                    {
                        unit.CharacterCombat.CancelAll();
                        unit.CharacterCombat.IssueAttackMove(worldPoint, unit);
                    }
                }
            }
        }

        void IssueMove()
        {
            if (_lastIssueFrame == Time.frameCount) return;
            _lastIssueFrame = Time.frameCount;

            var chars = selected
            .Select(s => (s as Component)?.GetComponent<ICharacter>())
            .Where(c => c != null)
            .ToList();
            if (chars.Count == 0)
            {
                return;
            }
            
            Vector2 wp = worldCam.ScreenToWorldPoint(Input.mousePosition);
            if (!Physics2D.OverlapPoint(wp, groundLayer))
                return;
            Vector2 origin = wp;
   
            var ordered = FormationUtility.StableOrder(chars);
            var center = new Vector2(
                ordered.Average(c => c.Transform.position.x),
                ordered.Average(c => c.Transform.position.y));
  
            var forward = (origin - center);
            if (forward.sqrMagnitude < 0.0001f)
            {
                forward = Vector2.right;

            }
            var slots = FormationUtility.BuildGridSlots(origin, forward.normalized, ordered.Count(), spacing);
            for (int i = 0; i < ordered.Count(); i++)
            {
                if (ordered[i].Lock != null && ordered[i].Lock.IsLocked)
                    continue;
                ordered[i].CharacterCombat?.CancelAll();
                ordered[i].Movable?.MoveTo(slots[i]);
            }
        }

        void TryCast(KeyCode key)
        {
            if (selected.Count == 0)
                return;
            Vector2 mouseWorld = worldCam.ScreenToWorldPoint(Input.mousePosition);
            if (key == KeyCode.LeftShift)
            {
                
                foreach (var caster in selected
                    .Select(s => (s as Component)?.GetComponent<ICharacter>())
                    .Where(c => c != null))
                {
                    var tr = caster.Transform;
                    var dir = ((Vector2)mouseWorld - (Vector2)tr.position).normalized;
                    var Roll = caster.RollAbility;

                    if (Roll == null)
                        continue;

                    var lockComp = caster.Lock;
                    if (lockComp != null && lockComp.IsLocked) continue;

                    Roll.Roll(dir);
                }
                return;
            }
            foreach (var caster in selected
            .Select(s => (s as Component)?.GetComponent<ICharacter>())
            .Where(c => c != null))
            {
                var lockComp = caster.Lock;
                if (lockComp != null && lockComp.IsLocked) continue;
                var SkillSet = caster.SkillSet;
                if (SkillSet == null) 
                    continue;

                var skill = SkillSet.Skills.FirstOrDefault(s => s.HotKey == key && s.CanCast(caster));
                if (skill == null) 
                    continue;
                var flipper = caster.SpineSideFlip;
                Vector2 p = worldCam.ScreenToWorldPoint(Input.mousePosition);
                
                switch (skill.Type)
                {
                    case SkillTargetType.None:
                        skill.Cast(caster, Vector2.zero, null);
                        break;

                    case SkillTargetType.Point:
                        Vector2 point = worldCam.ScreenToWorldPoint(Input.mousePosition);
                        flipper?.FaceByPoint(p);
                        skill.Cast(caster, point, null);
                        break;

                    case SkillTargetType.AlliedForces:
                    case SkillTargetType.EnemyForces:
                        Vector2 wp = worldCam.ScreenToWorldPoint(Input.mousePosition);
                        var hit = Physics2D.OverlapPoint(wp, characterLayer);
                        var target = hit ? hit.GetComponentInParent<ISelectable>() : null;
                        if (target != null)
                        {
                            var tpos = ((Component)target).transform.position;
                            flipper?.FaceByPoint(p);
                            skill.Cast(caster, ((Component)target).transform.position, target);
                        }
                        break;
                }
            }
        }
    }
}