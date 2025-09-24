using System.Collections;
using UnityEngine;

namespace Character
{
    public class SelectionBoxView : MonoBehaviour
    {
        [SerializeField]
        RectTransform box;
        [SerializeField]
        RectTransform canvasRoot;

        public bool IsActive => box.gameObject.activeSelf;

        public void Begin(Vector2 screenPos, Camera uiCam)
        {
            if (!box)
                return;
            box.gameObject.SetActive(true);
            UpdateRect(screenPos, screenPos, uiCam);
        }

        public void UpdateRect(Vector2 startScreen, Vector2 curScreen, Camera uiCam)
        {
            if (!box || !canvasRoot)
                return;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRoot, startScreen, uiCam, out var a);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRoot, curScreen, uiCam, out var b);
            var min = Vector2.Min(a, b);
            var size = Vector2.Max(a, b) - min;
            box.anchoredPosition = min;
            box.sizeDelta = size;
        }

        public void End()
        {
            if (!box)
                return;
            box.gameObject.SetActive(false);
        }
    }
}