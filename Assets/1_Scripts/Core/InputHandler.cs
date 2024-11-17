using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RedGaint
{
    public class InputHandler : Singleton<InputHandler>
    {
        [SerializeField] private VariableJoystick rightJoystick;
        [SerializeField] private VariableJoystick leftJoystick;

        private bool isEditor = false;

        // Callbacks for when joystick is pressed or released
        public event Action<Vector2> HandOnRightJoystick;   // Called when user touches the right joystick
        public event Action<Vector2> HandOnLeftJoystick;    // Called when user touches the left joystick
        public event Action ReleaseRightJoystick;           // Called when user releases the right joystick
        public event Action ReleaseLeftJoystick;            // Called when user releases the left joystick

        // Flags to track if the joystick is being held
        private bool isRightJoystickActive = false;
        private bool isLeftJoystickActive = false;

        private void Start()
        {
#if UNITY_EDITOR && !ONTEST_INPUT
            isEditor = true;
#endif
        }

        // Merged function to handle both PointerDown and PointerUp
        private void HandleJoystickInput(PointerEventData eventData)
        {
            // Check if right joystick was interacted with
            if (eventData.pointerEnter == rightJoystick.gameObject)
            {
                if (!isRightJoystickActive)  // Start handling input when pointer down
                {
                    isRightJoystickActive = true;
                    HandOnRightJoystick?.Invoke(GetRightJoystickDirection()); // Trigger event when joystick is touched
                }
                else  // Continuously call the callback until the pointer is up
                {
                    HandOnRightJoystick?.Invoke(GetRightJoystickDirection());
                }
            }
            // Check if left joystick was interacted with
            else if (eventData.pointerEnter == leftJoystick.gameObject)
            {
                if (!isLeftJoystickActive)  // Start handling input when pointer down
                {
                    isLeftJoystickActive = true;
                    HandOnLeftJoystick?.Invoke(GetLeftJoystickDirection()); // Trigger event when joystick is touched
                }
                else  // Continuously call the callback until the pointer is up
                {
                    HandOnLeftJoystick?.Invoke(GetLeftJoystickDirection());
                }
            }
        }

        // Called when pointer is pressed (when the user touches the joystick)
        private void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.pointerEnter == rightJoystick.gameObject)
            {
                isRightJoystickActive = true;
                HandOnRightJoystick?.Invoke(GetRightJoystickDirection()); // Trigger the event when joystick is touched
            }
            else if (eventData.pointerEnter == leftJoystick.gameObject)
            {
                isLeftJoystickActive = true;
                HandOnLeftJoystick?.Invoke(GetLeftJoystickDirection()); // Trigger the event when joystick is touched
            }
        }

        // Called when pointer is released (when the user lifts their finger from the joystick)
        private void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.pointerEnter == rightJoystick.gameObject)
            {
                ReleaseRightJoystick?.Invoke(); // Trigger release event
                isRightJoystickActive = false;  // Stop calling the callback when the user releases
            }
            else if (eventData.pointerEnter == leftJoystick.gameObject)
            {
                ReleaseLeftJoystick?.Invoke();  // Trigger release event
                isLeftJoystickActive = false;  // Stop calling the callback when the user releases
            }
        }

        public Vector2 GetRightJoystickDirection()
        {
            if (isEditor)
            {
                return new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            }
            return rightJoystick?.Direction ?? Vector2.zero;
        }

        public Vector2 GetLeftJoystickDirection()
        {
            if (isEditor)
            {
                if (Input.GetMouseButton(0))
                {
                    return new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
                }
                else
                {
                    return Vector2.zero;
                }
            }
            return leftJoystick?.Direction ?? Vector2.zero;
        }

        public Dictionary<string, Vector2> GetAllInputs() => new Dictionary<string, Vector2>
        {
            { "RightJoystick", GetRightJoystickDirection() },
            { "LeftJoystick", GetLeftJoystickDirection() }
        };
    }
}
