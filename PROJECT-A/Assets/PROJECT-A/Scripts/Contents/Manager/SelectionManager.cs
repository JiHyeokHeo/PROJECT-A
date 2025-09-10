//using System.Collections.Generic;
//using TST;
//using UnityEngine;

//namespace A
//{
//    public class SelectionManager : SingletonBase<SelectionManager>
//    {
//        public readonly List<ISelectable> selectables = new List<ISelectable>(); // ��ü �ĺ� ���
//        public readonly List<ISelectable> current = new List<ISelectable>();     // ���� ���õ� ģ����

//        public void Register(ISelectable selectable) => selectables.Add(selectable);
//        public void UnRegister(ISelectable selectable) => selectables.Remove(selectable);

//        public void CollectSelectableObjects(ISelectable obj, Rect screenRect)
//        {
//            current.Clear();
//            for (int i = 0; i < selectables.Count; i++) 
//            {
//                if (SelectCheck(selectables[i], screenRect))
//                    current.Add(selectables[i]);
//            }

//            Debug.Log($"���� ���õ� ģ���� {current.Count}�� �Դϴ�.");
//        }

//        bool SelectCheck(ISelectable obj, Rect screenRect)
//        {
//            Vector2 screenPoint = Camera.main.WorldToScreenPoint(obj.SelectionPivot.position);

//            return screenRect.Contains(screenPoint);
//        }

//        private float deltaTime = 0f;
//        private void Update()
//        {
//            deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
//        }

//        float edgeSize = 10f; // �����ڸ� ���� ���� px
//        float moveSpeed = 10f;
//        private void LateUpdate()
//        {
//            //Vector3 move = Vector3.zero;
//            //if (Input.mousePosition.x <= edgeSize) move.x -= 1;
//            //if (Input.mousePosition.x >= Screen.width - edgeSize) move.x += 1;
//            //if (Input.mousePosition.y <= edgeSize) move.y -= 1;
//            //if (Input.mousePosition.y >= Screen.height - edgeSize) move.y += 1;

//            //if (Input.GetKeyDown(KeyCode.Space) && current.Count > 0)
//            //{
//            //    Vector3 newPos = new Vector3(current[0].CurrentPos.x , current[0].CurrentPos.y, -10);
//            //    Camera.main.transform.position = newPos;
//            //}

//            //Camera.main.transform.position += move * moveSpeed * Time.deltaTime;
//        }

//        //[SerializeField] private int size = 7;
//        [SerializeField] private Color color = Color.red;

//        private void OnGUI()
//        {
//            //GUIStyle style = new GUIStyle();

//            //Rect rect = new Rect(10, 10, Screen.width, Screen.height);
//            //Rect rect2 = new Rect(10, 40, Screen.width, Screen.height);
//            //style.alignment = TextAnchor.UpperLeft;
//            //style.fontSize = size;
//            //style.normal.textColor = color;

//            //float ms = deltaTime * 1000f;
//            //float fps = 1.0f / deltaTime;
//            //string text = string.Format("{0:0.} FPS ({1:0.0} ms)", fps, ms);
//            //string text2 = string.Format("Selected Count ({0:0.0} ��)", current.Count);

//            //GUI.Label(rect, text, style);
//            //GUI.Label(rect2, text2, style);
//        }
//    }
//}
