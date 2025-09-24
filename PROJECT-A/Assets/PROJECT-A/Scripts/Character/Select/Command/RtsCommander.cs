using System.Collections.Generic;
using TST;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Character
{
    public class RtsCommander : MonoBehaviour
    {
        [SerializeField]
        private Camera worldCam;
        [SerializeField]
        private SelectionBoxView boxView;

        [SerializeField]
        private LayerMask characterLayer;
        [SerializeField]
        private LayerMask groundLayer;
        [SerializeField]
        private LayerMask enemyLayer;

        [SerializeField]
        private float clickThresholdSqr = 16f;
        [SerializeField]
        private float spacing = 1.2f;
        
        private readonly List<ICharacter> _ordered = new(16);
        private readonly SelectionModel _selection = new();
        private readonly List<ISelectable> _tmpSelectList = new(16);
        private readonly List<ICharacter> _tmpChars = new(16);
        private readonly List<UnitCommandQueue> _queues = new(16);
        private readonly InputModeFSM _fsm = new();

        bool _pressing;    
        bool _dragging;    
        Vector2 _pressScreen;     
        Vector2 _pressWorld;      
        int _lastIssueFrame = -1;
        void OnEnable()
        {
            InputSystem.OnLeftDown += OnLeftDown;
            InputSystem.OnLeftUp += OnLeftUp;
            InputSystem.OnRightDown += OnRightDown;
            InputSystem.OnAttackMovePrime += OnAttackMovePrime;
            InputSystem.OnCast += TryCast;
        }
        void OnDisable()
        {
            InputSystem.OnLeftDown -= OnLeftDown;
            InputSystem.OnLeftUp -= OnLeftUp;
            InputSystem.OnRightDown -= OnRightDown;
            InputSystem.OnAttackMovePrime -= OnAttackMovePrime;
            InputSystem.OnCast -= TryCast;
        }
        void OnAttackMovePrime() => _fsm.ToggleAttackPrimed();
        void Update()
        {
  
            if (EventSystem.current is not null && EventSystem.current.IsPointerOverGameObject()) 
                return;
            if (_pressing && !_fsm.IsPrimed)
            {
                var cur = (Vector2)Input.mousePosition;
                var delta = cur - _pressScreen;

                if (!_dragging && delta.sqrMagnitude >= clickThresholdSqr)
                {
                    _dragging = true;
                    _fsm.BeginBoxSelect();
                    boxView?.Begin(_pressScreen, worldCam);
                }

                if (_dragging)
                    boxView?.UpdateRect(_pressScreen, cur, worldCam);
            }
        }

        void OnRightDown(Vector2 screenPos)
        {
            if (_lastIssueFrame == Time.frameCount) return;
            _lastIssueFrame = Time.frameCount;

            _selection.ToCharacters(_tmpChars);
            if (_tmpChars.Count == 0) return;

            var wp = (Vector2)worldCam.ScreenToWorldPoint(screenPos);
            if (!PhysicsPick2D.IsGround(wp, groundLayer)) 
                return;

            FormationUtility.StableOrderNonLinq(_tmpChars, _ordered);


            float sumX = 0f, sumY = 0f;
            for (int i = 0; i < _ordered.Count; i++)
            {
                var p = _ordered[i].Transform.position;
                sumX += p.x;
                sumY += p.y;
            }

            var center = new Vector2(sumX / _ordered.Count, sumY / _ordered.Count);

            var forward = (wp - center);
            if (forward.sqrMagnitude < 0.0001f) forward = Vector2.right;

            var slots = FormationUtility.BuildGridSlots(wp, forward.normalized, _ordered.Count, spacing);

            MapToQueues(_ordered, _queues);
            for (int i = 0; i < _ordered.Count; i++)
            {
                var unit = _ordered[i];
                var lockr = unit.GetCapability<ActionLock>();
                if (lockr is not null && lockr.IsLocked) continue;

                _queues[i]?.Enqueue(new MoveToPointCommand(unit, slots[i]), clearExisting: true);
            }
        }
        void OnLeftDown(Vector2 screenPos)
        {
            if (EventSystem.current is not null && EventSystem.current.IsPointerOverGameObject()) 
                return;

            _pressing = true;
            _dragging = false; 
            _pressScreen = screenPos;
            _pressWorld = worldCam.ScreenToWorldPoint(screenPos);


            if (_fsm.IsPrimed) 
                boxView?.End();
        }

        void OnLeftUp(Vector2 screenPos)
        {
            if (!_pressing) 
                return;
            _pressing = false;

            // 박스 뷰 닫기(켜져 있었으면)
            if (_dragging) { boxView?.End(); _fsm.EndBoxSelect(); }

            var mouse = screenPos;
            var wp = (Vector2)worldCam.ScreenToWorldPoint(mouse);
            bool isClick = !_dragging; // 임계를 넘지 않았으면 클릭

            // A-프라임: 무조건 클릭 취급 → 공격이동/타깃
            if (_fsm.IsPrimed)
            {
                var enemy = PhysicsPick2D.PickCharacterAt(wp, enemyLayer);
                IssueAttackMove(wp, enemy);
                _fsm.ToggleAttackPrimed(); // 소모
                return;
            }

            // Normal 모드
            if (isClick)
            {
                var sel = PhysicsPick2D.PickSelectableAt(wp, characterLayer);
                if (sel != null) _selection.Replace(sel);
                else _selection.Clear();
            }
            else
            {
                var min = Vector2.Min(_pressWorld, wp);
                var max = Vector2.Max(_pressWorld, wp);
                PhysicsPick2D.BoxSelect(min, max, characterLayer, _tmpSelectList);
                _selection.Clear();
                _selection.AddRangeNoAlloc(_tmpSelectList);
            }

            // 최신 선택 → 캐릭터 캐시
            _selection.ToCharacters(_tmpChars);
        }

        private void TryCast(KeyCode key)
        {
            if (_tmpChars.Count == 0) 
                return;
            Vector2 mouseWorld = worldCam.ScreenToWorldPoint(Input.mousePosition);
            if (key == KeyCode.LeftShift)
            {

                foreach (var caster in _tmpChars) // 확장에 맞춰 사용
                {
                    if (caster == null) continue;

                    var tr = caster.Transform;
                    var dir = ((Vector2)mouseWorld - (Vector2)tr.position).normalized;

                    var lockCg = caster.GetCapability<ActionLock>();
                    if ((lockCg is not null && lockCg.IsLocked)) 
                        continue;

                    // 롤 능력(인터페이스 우선, 구체 폴백)
                    var rollIf = caster.GetCapability<RollAbility>();
                    if (rollIf is not null)
                        rollIf.Roll(dir);
                }
                return;
            }

            foreach (var caster in _tmpChars)
            {
                if (caster == null) 
                    continue;

                // 락 체크
                var lockCg = caster.GetCapability<ActionLock>();
                if (lockCg is not null && lockCg.IsLocked)
                    continue;

                var skillSet = caster.GetCapability<ISkillSet>();
                if (skillSet == null) 
                    continue;

                // 단축키 매칭 + 캐스트 가능
                ISkill matched = null;
                var skills = skillSet.Skills;
                foreach (var s in skillSet.Skills)
                {
                    if (s.HotKey == key && s.CanCast(caster))
                    {
                        matched = s;
                        break;
                    }
                }
                if (matched == null) 
                    continue;

                var flipCg = caster.GetCapability<SpineSideFlip2D>();



                switch (matched.Type)
                {
                    case SkillTargetType.None:
                        {
                            matched.Cast(caster, Vector2.zero, null);
                            break;
                        }

                    case SkillTargetType.Point:
                        {
                            flipCg?.FaceByPoint(mouseWorld);
                            matched.Cast(caster, mouseWorld, null);
                            break;
                        }

                    case SkillTargetType.AlliedForces:
                    case SkillTargetType.EnemyForces:
                        {
                            var ch = PhysicsPick2D.PickCharacterAt(mouseWorld, enemyLayer);
                            if (ch == null || ch.Health == null || ch.Health.IsDead) 
                                break;
                            var targetSel = (ch as Component)?.GetComponentInParent<ISelectable>();
                            if (targetSel == null)
                                break;
                            var tpos = (Vector2)ch.Transform.position;
                            flipCg?.FaceByPoint(tpos);
                            matched.Cast(caster, tpos, targetSel);
                            break;
                        }
                }
            }
        }
        private void IssueAttackMove(Vector2 worldPoint, ICharacter explicitTarget)
        {
            _selection.ToCharacters(_tmpChars);
            if (_tmpChars.Count == 0)
                return;
            FormationUtility.StableOrderNonLinq(_tmpChars, _ordered);
            MapToQueues(_ordered, _queues);

            for (int i = 0; i < _ordered.Count; i++)
            {
                var character = _ordered[i];
                var lockr = character.GetCapability<ActionLock>();
                if (lockr != null && lockr.IsLocked)
                    continue;

                var q = _queues[i];
                if (q == null)
                    continue;

                if (explicitTarget != null)
                    q.Enqueue(new AttackTargetCommand(character, explicitTarget), clearExisting: true);
                else
                    q.Enqueue(new AttackMoveCommand(character, worldPoint), clearExisting: true);
            }
        }
        
        private void MapToQueues(IReadOnlyList<ICharacter> chars, List<UnitCommandQueue> outQueues)
        {
            outQueues.Clear();
            for (int i = 0; i < chars.Count; i++)
            {
                var q = chars[i].GetCapability<UnitCommandQueue>();
                outQueues.Add(q);
            }
        }
    }
}
