using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TST
{
    public class InputSystem : SingletonBase<InputSystem>
    {
        public bool IsActiveCursorVisible => Cursor.visible;

        public static Action onDrag;
        public static Action onDragEnd;
        public static Action onMove;
        public static Action<KeyCode> onCast;
        private void Start()
        {
            SetCursorVisible(false);
        }

        private static void SetCursorVisible(bool isVisible)
        {
            if (isVisible)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        private void Update()
        {

            if (Input.GetMouseButton(0))
            {
                onDrag?.Invoke();
            }
            
            if (Input.GetMouseButtonUp(0))
            {
                onDragEnd?.Invoke();
            }
            if (Input.GetMouseButtonDown(1))
                onMove?.Invoke();
            if (Input.GetKeyDown(KeyCode.Q))
            {
                onCast?.Invoke(KeyCode.Q);
            }
            if (Input.GetKeyDown(KeyCode.W))
            {
                onCast?.Invoke(KeyCode.W);
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                onCast?.Invoke(KeyCode.E);
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                onCast?.Invoke(KeyCode.R);
            }
        }

        public void ChangeCursorVisibility(bool isVisible)
        {
            if (isVisible == false)
            {
                if (UIManager.Singleton.ActiveCursorVisibleUIsCount <= 0)
                {
                    SetCursorVisible(false);
                }
            }
            else
            {
                SetCursorVisible(true);
            }

        }
    }
}
