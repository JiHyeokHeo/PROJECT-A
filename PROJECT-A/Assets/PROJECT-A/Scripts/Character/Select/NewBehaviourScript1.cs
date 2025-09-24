//using Character;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using TST;
//using UnityEngine;
//using UnityEngine.EventSystems;

//namespace Character
//{
//    public class AttackMoveCommand : IUnitCommand
//    {
//        public ICharacter Character { get; }
//        public bool IsBlocking => false;
//        public bool IsFinished { get; private set; }

//        private Vector2 _dest;
//        private CharacterCombat _combat;

//        public AttackMoveCommand(ICharacter character, Vector2 dest)
//        {
//            Character = character;
//            _dest = dest;
//            _combat = character?.GetCapability<CharacterCombat>();
//        }

//        public void Execute()
//        {
//            _combat?.CancelAll();
//            _combat?.IssueAttackMove(_dest);
//            IsFinished = true;
//        }

//        public void Cancel() => _combat?.CancelAll();
//        public void Undo() { }
//        public bool TryMerge(IUnitCommand newer) => false;
//    }
//}
//namespace Character
//{
//    public class AttackTargetCommand : IUnitCommand
//    {
//        public ICharacter Character { get; }
//        public bool IsBlocking => true;
//        public bool IsFinished
//        {
//            get
//            {
//                return _target == null
//                    || _target.Health == null
//                    || _target.Health.IsDead;
//            }
//        }

//        private ICharacter _target;
//        private CharacterCombat _combat;

//        public AttackTargetCommand(ICharacter character, ICharacter target)
//        {
//            Character = character;
//            _target = target;
//            _combat = character.GetCapability<CharacterCombat>();
//        }

//        public void Execute()
//        {
//            _combat?.CancelAll();
//            if (_target != null)
//                _combat?.IssueAttackTarget(_target);
//        }

//        public void Cancel() => _combat?.CancelAll();
//        public void Undo() { }
//        public bool TryMerge(IUnitCommand newer) => false;
//    }
//}
//using System.Collections;
//using UnityEngine;

//namespace Character
//{
//    public enum InputMode
//    {
//        Normal,
//        AttackPrimed,
//        BoxSelecting,
//        Casting
//    }
//    public sealed class InputModeFSM
//    {
//        public InputMode Mode { get; private set; } = InputMode.Normal;
//        public bool IsPrimed => Mode == InputMode.AttackPrimed;

//        public void ToggleAttackPrimed()
//        {
//            Mode = (Mode == InputMode.AttackPrimed) ? InputMode.Normal : InputMode.AttackPrimed;
//        }

//        public void BeginBoxSelect()
//        {
//            Mode = InputMode.BoxSelecting;
//        }

//        public void EndBoxSelect()
//        {
//            if (Mode == InputMode.BoxSelecting)
//                Mode = InputMode.Normal;
//        }

//        public void BeginCasting()
//        {
//            Mode = InputMode.Casting;
//        }

//        public void EndCasting()
//        {
//            Mode = InputMode.Normal;
//        }

//        public override string ToString()
//        {
//            return Mode.ToString();
//        }
//    }
//}
//using System.Collections;
//using UnityEngine;


//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//namespace Character
//{
//    public class PhysicsPick2D
//    {
//        static readonly Collider2D[] _pointBuf = new Collider2D[8]; // 점 피킹용 작은 버퍼
//        static readonly Collider2D[] _areaBuf = new Collider2D[256]; // 영역 피킹용 큰 버퍼

//        public static ISelectable PickSelectableAt(Vector2 worldPoint, LayerMask characterLayer)
//        {
//            int n = Physics2D.OverlapPointNonAlloc(worldPoint, _pointBuf, characterLayer);

//            for (int i = 0; i < n; i++)
//            {
//                var col = _pointBuf[i];
//                if (!col)
//                    continue;
//                var sel = col.GetComponentInParent<ISelectable>();
//                if (sel != null)
//                    return sel;
//            }
//            return null;
//        }

//        public static ICharacter PickCharacterAt(Vector2 worldPoint, LayerMask characterLayer)
//        {
//            int n = Physics2D.OverlapPointNonAlloc(worldPoint, _pointBuf, characterLayer);
//            for (int i = 0; i < n; i++)
//            {
//                var col = _pointBuf[i];
//                if (!col)
//                    continue;
//                var ch = col.GetComponentInParent<ICharacter>();
//                if (ch != null)
//                    return ch;
//            }
//            return null;
//        }

//        public static void BoxSelect(Vector2 worldMin, Vector2 worldMax, LayerMask layer, List<ISelectable> outList)
//        {
//            outList.Clear();
//            int n = Physics2D.OverlapAreaNonAlloc(worldMin, worldMax, _areaBuf, layer);
//            for (int i = 0; i < n; i++)
//            {
//                var col = _areaBuf[i];
//                if (!col)
//                    continue;
//                var sel = col.GetComponentInParent<ISelectable>();
//                if (sel != null)
//                    outList.Add(sel);
//            }
//        }

//        public static bool IsGround(Vector2 worldPoint, LayerMask groundLayer)
//        => Physics2D.OverlapPointNonAlloc(worldPoint, _pointBuf, groundLayer) > 0;
//    }
//}
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using TST;
//using UnityEngine;
//using UnityEngine.EventSystems;

//namespace Character
//{
//    public class RtsCommander : MonoBehaviour
//    {
//        [SerializeField]
//        private Camera worldCam;
//        [SerializeField]
//        private SelectionBoxView boxView;

//        [SerializeField]
//        private LayerMask characterLayer;
//        [SerializeField]
//        private LayerMask groundLayer;
//        [SerializeField]
//        private LayerMask enemyLayer;

//        [SerializeField]
//        private float clickThresholdSqr = 16f;
//        [SerializeField]
//        private float spacing = 1.2f;

//        private readonly SelectionModel _selection = new();
//        private readonly List<ISelectable> _tmpSelectList = new(16);
//        private readonly List<ICharacter> _tmpChars = new(16);
//        private readonly List<UnitCommandQueue> _queues = new(16);
//        private readonly InputModeFSM _fsm = new();

//        bool _pressing;
//        bool _dragging;
//        Vector2 _pressScreen;
//        Vector2 _pressWorld;
//        int _lastIssueFrame = -1;
//        void OnEnable()
//        {
//            InputSystem.OnLeftDown += OnLeftDown;
//            InputSystem.OnLeftUp += OnLeftUp;
//            InputSystem.OnRightDown += OnRightDown;
//            InputSystem.OnAttackMovePrime += OnAttackMovePrime;
//            InputSystem.OnCast += TryCast;
//        }
//        void OnDisable()
//        {
//            InputSystem.OnLeftDown -= OnLeftDown;
//            InputSystem.OnLeftUp -= OnLeftUp;
//            InputSystem.OnRightDown -= OnRightDown;
//            InputSystem.OnAttackMovePrime -= OnAttackMovePrime;
//            InputSystem.OnCast -= TryCast;
//        }
//        void OnAttackMovePrime() => _fsm.ToggleAttackPrimed();
//        void Update()
//        {

//            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
//                return;

//            if (_pressing && !_fsm.IsPrimed)
//            {
//                var cur = (Vector2)Input.mousePosition;
//                var delta = cur - _pressScreen;

//                if (!_dragging && delta.sqrMagnitude >= clickThresholdSqr)
//                {
//                    _dragging = true;
//                    _fsm.BeginBoxSelect();
//                    boxView?.Begin(_pressScreen, worldCam);
//                }

//                if (_dragging)
//                {
//                    boxView?.UpdateRect(_pressScreen, cur, worldCam);
//                }
//            }
//        }

//        void OnRightDown(Vector2 screenPos)
//        {
//            if (_lastIssueFrame == Time.frameCount) return;
//            _lastIssueFrame = Time.frameCount;

//            _selection.ToCharacters(_tmpChars);
//            if (_tmpChars.Count == 0) return;

//            var wp = (Vector2)worldCam.ScreenToWorldPoint(screenPos);
//            if (!PhysicsPick2D.IsGround(wp, groundLayer)) return;

//            var ordered = FormationUtility.StableOrder(_tmpChars);
//            var center = new Vector2(ordered.Average(c => c.Transform.position.x),
//                                     ordered.Average(c => c.Transform.position.y));
//            var forward = (wp - center);
//            if (forward.sqrMagnitude < 0.0001f) forward = Vector2.right;
//            var slots = FormationUtility.BuildGridSlots(wp, forward.normalized, ordered.Count, spacing);

//            MapToQueues(ordered, _queues);
//            for (int i = 0; i < ordered.Count; i++)
//            {
//                var unit = ordered[i];
//                var lockr = unit.GetCapability<ActionLock>();
//                if (lockr != null && lockr.IsLocked) continue;

//                _queues[i]?.Enqueue(new MoveToPointCommand(unit, slots[i]), clearExisting: true);
//            }
//        }
//        void OnLeftDown(Vector2 screenPos)
//        {
//            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

//            _pressing = true;
//            _dragging = false; // 아직 클릭/드래그 미정
//            _pressScreen = screenPos;
//            _pressWorld = worldCam.ScreenToWorldPoint(screenPos);

//            // A-프라임 모드면 박스는 애초에 사용 안 함
//            if (_fsm.IsPrimed) boxView?.End();
//        }

//        void OnLeftUp(Vector2 screenPos)
//        {
//            if (!_pressing) return;
//            _pressing = false;

//            // 박스 뷰 닫기(켜져 있었으면)
//            if (_dragging) { boxView?.End(); _fsm.EndBoxSelect(); }

//            var mouse = screenPos;
//            var wp = (Vector2)worldCam.ScreenToWorldPoint(mouse);
//            bool isClick = !_dragging; // 임계를 넘지 않았으면 클릭

//            // A-프라임: 무조건 클릭 취급 → 공격이동/타깃
//            if (_fsm.IsPrimed)
//            {
//                var enemy = PhysicsPick2D.PickCharacterAt(wp, enemyLayer);
//                IssueAttackMove(wp, enemy);
//                _fsm.ToggleAttackPrimed(); // 소모
//                return;
//            }

//            // Normal 모드
//            if (isClick)
//            {
//                var sel = PhysicsPick2D.PickSelectableAt(wp, characterLayer);
//                if (sel != null) _selection.Replace(sel);
//                else _selection.Clear();
//            }
//            else
//            {
//                var min = Vector2.Min(_pressWorld, wp);
//                var max = Vector2.Max(_pressWorld, wp);
//                PhysicsPick2D.BoxSelect(min, max, characterLayer, _tmpSelectList);
//                _selection.Clear();
//                _selection.AddRangeNoAlloc(_tmpSelectList);
//            }

//            // 최신 선택 → 캐릭터 캐시
//            _selection.ToCharacters(_tmpChars);
//        }

//        private void TryCast(KeyCode key)
//        {
//            if (_tmpChars.Count == 0)
//                return;
//            Vector2 mouseWorld = worldCam.ScreenToWorldPoint(Input.mousePosition);
//            if (key == KeyCode.LeftShift)
//            {

//                foreach (var caster in _tmpChars) // 확장에 맞춰 사용
//                {
//                    if (caster == null) continue;

//                    var tr = caster.Transform;
//                    var dir = ((Vector2)mouseWorld - (Vector2)tr.position).normalized;

//                    var lockCg = caster.GetCapability<ActionLock>();
//                    if ((lockCg != null && lockCg.IsLocked))
//                        continue;

//                    // 롤 능력(인터페이스 우선, 구체 폴백)
//                    var rollIf = caster.GetCapability<RollAbility>();
//                    if (rollIf != null)
//                        rollIf.Roll(dir);
//                }
//                return;
//            }

//            foreach (var caster in _tmpChars)
//            {
//                if (caster == null)
//                    continue;

//                // 락 체크
//                var lockCg = caster.GetCapability<ActionLock>();
//                if (lockCg != null && lockCg.IsLocked)
//                    continue;

//                var skillSet = caster.GetCapability<ISkillSet>();
//                if (skillSet == null)
//                    continue;

//                // 단축키 매칭 + 캐스트 가능
//                var skill = skillSet.Skills.FirstOrDefault(s => s.HotKey == key && s.CanCast(caster));
//                if (skill == null) continue;

//                var flipCg = caster.GetCapability<SpineSideFlip2D>();



//                switch (skill.Type)
//                {
//                    case SkillTargetType.None:
//                        {
//                            skill.Cast(caster, Vector2.zero, null);
//                            break;
//                        }

//                    case SkillTargetType.Point:
//                        {
//                            flipCg?.FaceByPoint(mouseWorld);
//                            skill.Cast(caster, mouseWorld, null);
//                            break;
//                        }

//                    case SkillTargetType.AlliedForces:
//                    case SkillTargetType.EnemyForces:
//                        {
//                            var ch = PhysicsPick2D.PickCharacterAt(mouseWorld, enemyLayer);
//                            if (ch == null || ch.Health == null || ch.Health.IsDead)
//                                break;
//                            var targetSel = (ch as Component)?.GetComponentInParent<ISelectable>();
//                            if (targetSel == null)
//                                break;
//                            var tpos = (Vector2)ch.Transform.position;
//                            flipCg?.FaceByPoint(tpos);
//                            skill.Cast(caster, tpos, targetSel);
//                            break;
//                        }
//                }
//            }
//        }
//        private void IssueAttackMove(Vector2 worldPoint, ICharacter explicitTarget)
//        {
//            _selection.ToCharacters(_tmpChars);
//            if (_tmpChars.Count == 0)
//                return;
//            var ordered = FormationUtility.StableOrder(_tmpChars);
//            MapToQueues(ordered, _queues);

//            for (int i = 0; i < ordered.Count; i++)
//            {
//                var character = ordered[i];
//                var lockr = character.GetCapability<ActionLock>();
//                if (lockr != null && lockr.IsLocked)
//                    continue;

//                var q = _queues[i];
//                if (q == null)
//                    continue;

//                if (explicitTarget != null)
//                    q.Enqueue(new AttackTargetCommand(character, explicitTarget), clearExisting: true);
//                else
//                    q.Enqueue(new AttackMoveCommand(character, worldPoint), clearExisting: true);
//            }

//        }

//        private void MapToQueues(IReadOnlyList<ICharacter> chars, List<UnitCommandQueue> outQueues)
//        {
//            outQueues.Clear();
//            for (int i = 0; i < chars.Count; i++)
//            {
//                var q = chars[i].GetCapability<UnitCommandQueue>();
//                outQueues.Add(q);
//            }
//        }
//    }
//}
//using UnityEngine;

//namespace Character
//{
//    public class SelectableCharacter : MonoBehaviour, ISelectable
//    {

//        public GameObject selectionIndicator;
//        public bool IsSelected { get; private set; }
//        public void SetSelected(bool on)
//        {
//            IsSelected = on;
//            if (selectionIndicator)
//                selectionIndicator.SetActive(on);
//        }
//    }
//}
//using System.Collections;
//using UnityEngine;

//namespace Character
//{
//    public class SelectionBoxView : MonoBehaviour
//    {
//        [SerializeField]
//        RectTransform box;
//        [SerializeField]
//        RectTransform canvasRoot;

//        public bool IsActive => box.gameObject.activeSelf;

//        public void Begin(Vector2 screenPos, Camera uiCam)
//        {
//            if (!box)
//                return;
//            box.gameObject.SetActive(true);
//            UpdateRect(screenPos, screenPos, uiCam);
//        }

//        public void UpdateRect(Vector2 startScreen, Vector2 curScreen, Camera uiCam)
//        {
//            if (!box || !canvasRoot)
//                return;
//            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRoot, startScreen, uiCam, out var a);
//            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRoot, curScreen, uiCam, out var b);
//            var min = Vector2.Min(a, b);
//            var size = Vector2.Max(a, b) - min;
//            box.anchoredPosition = min;
//            box.sizeDelta = size;
//        }

//        public void End()
//        {
//            if (!box)
//                return;
//            box.gameObject.SetActive(false);
//        }
//    }
//}
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//namespace Character
//{
//    public class SelectionModel
//    {
//        readonly List<ISelectable> _list = new(32);

//        public IReadOnlyList<ISelectable> Items => _list;

//        public void Clear()
//        {
//            for (int i = 0; i < _list.Count; i++)
//                _list[i]?.SetSelected(false);
//            _list.Clear();
//        }

//        public void Add(ISelectable sel)
//        {
//            if (sel == null)
//                return;
//            if (_list.Contains(sel))
//                return;
//            _list.Add(sel);
//            sel.SetSelected(true);
//        }

//        public void Replace(ISelectable sel)
//        {
//            Clear();
//            Add(sel);
//        }

//        public void AddRangeNoAlloc(List<ISelectable> src)
//        {
//            for (int i = 0; i < src.Count; i++)
//                Add(src[i]);
//        }

//        public void ToCharacters(List<ICharacter> outList)
//        {
//            outList.Clear();
//            for (int i = 0; i < _list.Count; i++)
//            {
//                if (_list[i] is Component c)
//                {
//                    var ch = c.GetComponent<ICharacter>();
//                    if (ch != null)
//                        outList.Add(ch);
//                }
//            }
//        }

//    }
//}
//using System.Collections.Generic;
//using UnityEngine;

//namespace Character
//{
//    [DefaultExecutionOrder(-10)]
//    public sealed class UnitCommandQueue : MonoBehaviour, ICapability, ITickable
//    {
//        private ICharacter _character;
//        private readonly Queue<IUnitCommand> _queue = new();
//        private IUnitCommand _current;

//        private void Awake()
//        {
//            _character = GetComponentInParent<ICharacter>(true);

//            if (_character == null)
//            {
//                Debug.LogError("Not Character scripts");
//            }
//        }

//        public void Clear()
//        {
//            _current?.Cancel();
//            _current = null;
//            while (_queue.Count > 0)
//                _queue.Dequeue().Cancel();
//        }

//        public void Enqueue(IUnitCommand cmd, bool clearExisting = false)
//        {
//            if (cmd == null)
//                return;
//            if (clearExisting)
//                Clear();

//            if (_current != null && _current.TryMerge(cmd))
//                return;
//            if (_queue.Count > 0 && _queue.Peek().TryMerge(cmd))
//                return;
//            _queue.Enqueue(cmd);
//        }

//        public void Tick(float dt)
//        {
//            if (_current == null)
//            {
//                if (_queue.Count == 0)
//                    return;
//                _current = _queue.Dequeue();
//                _current.Execute();
//            }

//            if (_current.IsFinished || !_current.IsBlocking)
//            {
//                _current = null;
//            }
//        }
//    }
//}
//using System;
//using UnityEngine;

//namespace Character
//{
//    public class MoveToPointCommand : IUnitCommand
//    {
//        public ICharacter Character { get; }
//        public bool IsBlocking => true;
//        public bool IsFinished { get; private set; }

//        readonly Vector2 _dest;
//        readonly float _arriveDist;

//        IMovable _move;
//        IMoveNotifier _notifier; // 이벤트 지원 시 사용
//        bool _subscribed;

//        public MoveToPointCommand(ICharacter unit, Vector2 dest, float arriveDist = 0.12f)
//        {
//            Character = unit;
//            _dest = dest;
//            _arriveDist = arriveDist;

//            _move = unit.GetCapability<IMovable>();
//            _notifier = _move as IMoveNotifier    // Moveable이면 캐스팅 성공
//                        ?? (unit as MonoBehaviour)?.GetComponentInChildren<IMoveNotifier>(true);
//        }

//        public void Execute()
//        {
//            IsFinished = false;

//            // 이벤트 기반: 이동 완료/취소 시점에 종료
//            if (_notifier != null && !_subscribed)
//            {
//                _notifier.OnArrived += OnArrived;
//                _subscribed = true;
//            }

//            _move?.MoveTo(_dest);

//            // 이벤트가 아예 없는 구현체일 경우(레거시), 폴백:
//            if (_notifier == null)
//            {
//                // 폴백은 “즉시 완료” or “폴링” 중 선택해야 함.
//                // 간단히 즉시 완료로 두되, 필요하면 커맨드 틱/폴링 구조로 확장 가능.
//                IsFinished = true;
//            }
//        }

//        void OnArrived(Vector2 dest, MoveArriveReason reason)
//        {
//            // 목적지 일치(혹은 충분히 가까움)만 인정
//            if ((dest - _dest).sqrMagnitude > _arriveDist * _arriveDist)
//                return;

//            CleanupSub();
//            IsFinished = true;
//        }

//        public void Cancel()
//        {
//            CleanupSub();
//            _move?.Stop();
//            IsFinished = true; // 취소된 것으로 간주
//        }

//        public void Undo() { }
//        public bool TryMerge(IUnitCommand newer) => false;

//        void CleanupSub()
//        {
//            if (_subscribed && _notifier != null)
//            {
//                _notifier.OnArrived -= OnArrived;
//                _subscribed = false;
//            }
//        }
//    }
//}
//using Character;
//using System;
//using System.Collections.Generic;
//using UnityEngine;

//[RequireComponent(typeof(Rigidbody2D))]
//public class Moveable : MonoBehaviour, IMovable, IMoveNotifier
//{
//    [Header("Speed & Smoothing")]
//    [SerializeField] float defaultMoveSpeed = 5f;
//    [SerializeField] float accel = 15f;

//    [Header("Arrival")]
//    [SerializeField] float arriveDist = 0.1f;         // 최종 목표 도달 판정
//    [SerializeField] float waypointArriveDist = 0.12f;// 웨이포인트 도달 판정

//    [Header("Separation")]
//    [SerializeField] int maxAllyScan = 32;
//    [SerializeField] float separationRadius = 0.1f;
//    [SerializeField] float separationStrength = 1.0f;
//    [SerializeField] LayerMask allyMask;

//    [Header("Pathfinding")]
//    [SerializeField] bool usePathfinder = true;
//    [Tooltip("가능하면 인스펙터로 할당, 없으면 Awake에서 FindObjectOfType로 폴백")]
//    [SerializeField] MonoBehaviour pathfinderBehaviour; // IPathfinder를 구현한 컴포넌트
//    IPathfinder pathfinder;

//    [Header("Presentation (optional)")]
//    [SerializeField] SpineSideFlip2D spineSideFlip;

//    // 물리
//    Rigidbody2D rb;

//    // 경로 상태
//    readonly Queue<Vector2> waypoints = new();
//    bool hasPath;
//    Vector2 finalGoal;

//    // 새 명령 연타 떨림 방지
//    [SerializeField] float separationPauseOnNewOrder = 0.08f;
//    float _separationResumeTime;
//    Vector2 _lastIssuedTarget;

//    IStats stats;

//    // 이벤트(도착/취소 알림)
//    public event Action<Vector2, MoveArriveReason> OnArrived;

//    // 외부에서 참고할 수 있게 공개 프로퍼티
//    public bool HasDestination => hasPath;
//    public Vector2 Destination => finalGoal;

//    public bool CanMove => true;

//    void Awake()
//    {
//        rb = GetComponent<Rigidbody2D>();
//        rb.gravityScale = 0f;
//        rb.freezeRotation = true;

//        _allyBuffer = new Collider2D[maxAllyScan];
//        stats = GetComponent<IStats>();

//        if (pathfinderBehaviour != null)
//            pathfinder = pathfinderBehaviour as IPathfinder;
//        if (usePathfinder && pathfinder == null)
//            pathfinder = FindObjectOfType<MonoBehaviour>() as IPathfinder; // 폴백(가능하면 인스펙터 할당 추천)
//    }

//    public void Stop()
//    {
//        bool wasMoving = hasPath;
//        hasPath = false;
//        waypoints.Clear();
//        rb.velocity = Vector2.zero;

//        // 취소 이벤트(현재 목적지 기준)
//        if (wasMoving)
//            SafeInvokeArrived(finalGoal, MoveArriveReason.Cancelled);

//        finalGoal = rb.position;
//    }

//    public void MoveTo(Vector2 worldPos)
//    {
//        // 같은 지점 연타 방지
//        if ((_lastIssuedTarget - worldPos).sqrMagnitude < 0.04f) return;
//        _lastIssuedTarget = worldPos;

//        finalGoal = worldPos;
//        waypoints.Clear();
//        rb.velocity = Vector2.zero;
//        _separationResumeTime = Time.time + separationPauseOnNewOrder;

//        if (usePathfinder && pathfinder != null)
//        {
//            var start = rb.position;
//            var path = pathfinder.FindPath(start, worldPos);
//            if (path != null && path.Count > 0)
//            {
//                const float minStep = 0.12f;
//                Vector2 prev = start;
//                foreach (var p in path)
//                {
//                    if ((p - prev).sqrMagnitude >= minStep * minStep)
//                    {
//                        waypoints.Enqueue(p);
//                        prev = p;
//                    }
//                }
//                finalGoal = prev;
//                hasPath = true;
//                return;
//            }
//        }

//        waypoints.Enqueue(worldPos);
//        hasPath = true;
//    }

//    void FixedUpdate()
//    {
//        if (!hasPath)
//        {
//            rb.velocity = Vector2.MoveTowards(rb.velocity, Vector2.zero, accel * Time.fixedDeltaTime);
//            return;
//        }

//        // 웨이포인트/최종 도착 체크
//        if (waypoints.Count == 0)
//        {
//            if (Vector2.Distance(rb.position, finalGoal) <= arriveDist)
//            {
//                hasPath = false;
//                rb.velocity = Vector2.MoveTowards(rb.velocity, Vector2.zero, accel * Time.fixedDeltaTime);

//                // 도착 이벤트
//                SafeInvokeArrived(finalGoal, MoveArriveReason.Reached);
//                return;
//            }
//            waypoints.Enqueue(finalGoal);
//        }

//        // 현재 목표/방향
//        var target = waypoints.Peek();
//        Vector2 to = target - rb.position;
//        float dist = to.magnitude;

//        // 웨이포인트 도착 판정
//        if (dist <= waypointArriveDist)
//        {
//            waypoints.Dequeue();
//            if (waypoints.Count > 0)
//            {
//                target = waypoints.Peek();
//                to = target - rb.position;
//            }
//        }

//        // 속도 목표
//        float moveSpeed = (stats != null ? stats.Speed : defaultMoveSpeed);

//        // 급선회 감속
//        if (rb.velocity.sqrMagnitude > 0.0001f)
//        {
//            float dot = Vector2.Dot(rb.velocity.normalized, to.normalized);
//            if (dot < -0.2f) rb.velocity *= 0.85f;
//        }

//        Vector2 desired = (to.sqrMagnitude > 0.0001f) ? to.normalized * moveSpeed : Vector2.zero;

//        // 분리
//        Vector2 sep = Vector2.zero;
//        if (Time.time >= _separationResumeTime && separationRadius > 0.01f)
//        {
//            int count = Physics2D.OverlapCircleNonAlloc(rb.position, separationRadius, _allyBuffer, allyMask);
//            for (int i = 0; i < count; i++)
//            {
//                var h = _allyBuffer[i];
//                if (!h || h.attachedRigidbody == rb) continue;

//                Vector2 away = (Vector2)rb.position - (Vector2)h.transform.position;
//                float d = Mathf.Max(away.magnitude, 0.2f);
//                sep += away / d;
//            }
//            sep *= separationStrength * moveSpeed * 0.5f;
//            sep = Vector2.ClampMagnitude(sep, moveSpeed * 0.6f);
//        }

//        // 가감속
//        Vector2 steer = (desired - rb.velocity) + sep;
//        Vector2 newVel = rb.velocity + steer * Time.fixedDeltaTime * accel;
//        rb.velocity = Vector2.ClampMagnitude(newVel, moveSpeed);

//        if (spineSideFlip != null) spineSideFlip.FaceByVelocity(rb.velocity);
//    }

//    // ---------- internals ----------

//    Collider2D[] _allyBuffer;

//    void SafeInvokeArrived(in Vector2 dest, MoveArriveReason reason)
//    {
//        try { OnArrived?.Invoke(dest, reason); }
//        catch (Exception e) { Debug.LogException(e, this); }
//    }

//    // 주입용(테스트/런타임 교체)
//    public void SetPathfinder(IPathfinder pf) => pathfinder = pf;
//}