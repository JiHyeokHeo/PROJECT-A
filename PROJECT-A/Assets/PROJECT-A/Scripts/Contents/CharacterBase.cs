using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

namespace A
{
    // 추후 사이즈가 커지면 Controller로 분리
    public class CharaterBase : MonoBehaviour, Tory.ISelectable
    {
        [Title("화면 판정 기준 설정 가능", titleAlignment: TitleAlignments.Centered)]
        [SerializeField] private GameObject selectionVisual;
        SpriteRenderer spriteRenderer;

        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                if (IsSelected == value)
                    return;

                IsSelected = value;
            }
        }
        private bool isSelected;

        public Transform SelectionPivot => transform;

        public Vector2 CurrentPos => rb2d.position;

        void OnEnable()
        {
            //SelectionManager.Singleton.Register(Torythis); // 추후 구조가 바뀔수도?
        }

        void OnDisable()
        {
            //SelectionManager.Singleton.UnRegister(this);
        }

        void Start()
        {
            rb2d = GetComponent<Rigidbody2D>();
  
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        // UI
        public Canvas overlayCanvas;
        public RectTransform selectionBox;
        private bool isDragging = false;
        private Vector2 dragStartScreen;
        private Vector2 targetRightClickScreen;
        //private bool isClicked = false;
        public float moveSpeed = 30f;
        public Rigidbody2D rb2d;
        public bool isMoveing = false;
        void Update()
        {
            // 샘플 구현
            // UI Block
            if (EventSystem.current && EventSystem.current.IsPointerOverGameObject())
                return;

            if (Input.GetMouseButtonDown(0))
            {
                isDragging = true;
                dragStartScreen = Input.mousePosition;
                if (selectionBox)
                    selectionBox.gameObject.SetActive(true);
                UpdateSelectionBox(dragStartScreen, dragStartScreen);

                Debug.Log("마우스 버튼 클릭");
            }
            else if (isDragging && Input.GetMouseButton(0))
            {
                UpdateSelectionBox(dragStartScreen, Input.mousePosition);
                Debug.Log("마우스 드래그");
            }
            else if (isDragging && Input.GetMouseButtonUp(0))
            {
                isDragging = false;

                var screenRect = GetScreenRect(dragStartScreen, (Vector2)Input.mousePosition);
                // 셀렉션 내용 들어가야함
                ApplySelection(screenRect);

                if (selectionBox)
                    selectionBox.gameObject.SetActive(false);
            }

            if (Input.GetMouseButtonDown(1))
            {
                //if (SelectionManager.Singleton.current.Contains(this))
                //{
                //    isMoveing = true;
                //    Vector3 worldClick = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                //    targetRightClickScreen = new Vector2(worldClick.x, worldClick.y);
                //}
            }

            Vector2 pos = rb2d.position;
            Vector2 dir = targetRightClickScreen - pos;
            Vector2 move = dir.normalized * moveSpeed * Time.deltaTime;

            if (dir.magnitude > 1.0f)
                rb2d.MovePosition(pos + move);
            else
                isMoveing = false;

            bool isRight = (targetRightClickScreen.x - rb2d.position.x) > 0f;
            spriteRenderer.flipX = !isRight; // 기본이 오른쪽 바라보는 스프라이트라면
        }

        void ApplySelection(Rect screenRect)
        {
            //SelectionManager.Singleton.CollectSelectableObjects(this, screenRect);
        }

        void UpdateSelectionBox(Vector2 startScreen, Vector2 endScreen)
        {
            if (selectionBox == null || overlayCanvas == null)
                return;

            RectTransform canvasRect = overlayCanvas.transform as RectTransform; ;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, startScreen, null, out Vector2 startLocalPoint);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, endScreen, null, out Vector2 endLocalPoint);

            Vector2 min = Vector2.Min(startLocalPoint, endLocalPoint);
            Vector2 max = Vector2.Max(startLocalPoint, endLocalPoint);

            // 앵커드 포지션이랑 LocalPosition 둘다 동작 정상.. 이유 알아봐야할듯
            selectionBox.anchoredPosition = min;
            selectionBox.sizeDelta = max - min;
        }

        Rect GetScreenRect(Vector2 a, Vector2 b)
        {
            Vector2 min = Vector2.Min(a, b);
            Vector2 max = Vector2.Max(a, b);
            return Rect.MinMaxRect(min.x, min.y, max.x, max.y);
        }
    }
}