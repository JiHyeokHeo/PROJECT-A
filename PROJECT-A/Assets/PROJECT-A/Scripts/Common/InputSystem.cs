using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TST
{
    public class InputSystem : SingletonBase<InputSystem>
    {
        public bool IsActiveCursorVisible => Cursor.visible;

        public static Action<Vector2> OnLeftDown;
        public static Action<Vector2> OnLeftUp;
        public static Action<Vector2> OnRightDown;
        public static Action<KeyCode> OnCast;
        public static Action OnAttackMovePrime;

        private void Start()
        {
            SetCursorVisible(true);
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
                var m = (Vector2)Input.mousePosition;
                OnLeftDown?.Invoke(m);
            }

            if (Input.GetMouseButtonUp(0))
            {
                var m = (Vector2)Input.mousePosition;
                OnLeftUp?.Invoke(m);
            }

            if (Input.GetMouseButtonDown(1))
            {
                var m = (Vector2)Input.mousePosition;
                OnRightDown?.Invoke(m);
            }


            if (Input.GetKeyUp(KeyCode.Q))
                OnCast?.Invoke(KeyCode.Q);
            
            if (Input.GetKeyUp(KeyCode.W))
                OnCast?.Invoke(KeyCode.W);

            if (Input.GetKeyUp(KeyCode.E))
                OnCast?.Invoke(KeyCode.E);

            if (Input.GetKeyUp(KeyCode.R))
                OnCast?.Invoke(KeyCode.R);
            if (Input.GetKeyDown(KeyCode.LeftShift))
                OnCast?.Invoke(KeyCode.LeftShift);
            if (Input.GetKeyDown(KeyCode.A))
                OnAttackMovePrime?.Invoke();

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
