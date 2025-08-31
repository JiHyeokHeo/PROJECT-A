using A;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

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
        private LayerMask characterLayer;
        [SerializeField]
        private LayerMask groundLayer;
        [SerializeField]
        private RectTransform _rectTransform;

        [SerializeField]
        private float spacing = 1.2f;
        int _lastIssueFrame = -1;
        [SerializeField]
        private readonly List<ISelectable> selected = new();

        private Vector2 dragStartScreen;
        private Vector2 dragStartUIScreen;
        private bool dragging = false;
        

        void Update()
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;
           
        }

        private void OnEnable()
        {
            TST.InputSystem.onDrag += DragLogic;
            TST.InputSystem.onDragEnd += DragEndLogic;
            TST.InputSystem.onMove += IssueMove;
            TST.InputSystem.onCast += TryCast;
        }
        private void OnDisable()
        {
            TST.InputSystem.onDrag -= DragLogic;
            TST.InputSystem.onDragEnd -= DragEndLogic;
            TST.InputSystem.onMove -= IssueMove;
            TST.InputSystem.onCast -= TryCast;
        }
        private void DragLogic()
        {
            if (dragging == false)
            {
                dragging = true;
                dragStartScreen = Input.mousePosition;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(_rectTransform, Input.mousePosition, worldCam, out dragStartUIScreen);
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

        //void UpdateDrag()
        //{
        //    Vector2 cur;
        //    RectTransformUtility.ScreenPointToLocalPointInRectangle(_rectTransform, Input.mousePosition, worldCam, out dragStartScreen);
        //    Vector2 min = Vector2.Min(dragStartScreen, cur);
        //    Vector2 size = Vector2.Max(dragStartScreen, cur) - min;

        //    selectionBox.anchoredPosition = min;
        //    selectionBox.sizeDelta = size;
        //}

        void UpdateDrag()
        {
            Vector2 cur;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_rectTransform, Input.mousePosition, worldCam, out cur);
            Vector2 min = Vector2.Min(dragStartUIScreen, cur);
            Vector2 size = Vector2.Max(dragStartUIScreen, cur) - min;

            selectionBox.anchoredPosition = min;
            selectionBox.sizeDelta = size;
        }

        void EndDrag()
        {
            if (((Vector2)Input.mousePosition - dragStartScreen).sqrMagnitude < 16f)
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

        //추후에 groundlayer 뿐만 아니라 몬스터(보스)가 있으면 
        // 공격 사거리 범위까지 들어가서 공격하는 것도 추가(롤처럼)
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
                ordered[i].Movable?.MoveTo(slots[i]);
            }
        }

        void TryCast(KeyCode key)
        {
            if (selected.Count == 0)
                return;

            foreach (var caster in selected
            .Select(s => (s as Component)?.GetComponent<ICharacter>())
            .Where(c => c != null))
            {
                var SkillSet = caster.SkillSet;
                if (SkillSet == null) 
                    continue;

                var skill = SkillSet.Skills.FirstOrDefault(s => s.HotKey == key && s.CanCast(caster));
                if (skill == null) continue;

                switch (skill.Type)
                {
                    case SkillTargetType.None:
                        skill.Cast(caster, Vector2.zero, null);
                        break;

                    case SkillTargetType.Point:
                        Vector2 point = worldCam.ScreenToWorldPoint(Input.mousePosition);
                        skill.Cast(caster, point, null);
                        break;

                    case SkillTargetType.AlliedForces:
                    case SkillTargetType.EnemyForces:
                        Vector2 wp = worldCam.ScreenToWorldPoint(Input.mousePosition);
                        var hit = Physics2D.OverlapPoint(wp, characterLayer);
                        var target = hit ? hit.GetComponentInParent<ISelectable>() : null;
                        if (target != null)
                            skill.Cast(caster, ((Component)target).transform.position, target);
                        break;
                }
            }
        }
    }
}