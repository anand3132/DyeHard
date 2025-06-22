using UnityEngine;
using UnityEngine.InputSystem;
using System;

namespace RedGaint.Games.DyeHard
{
    public class PlayerInputHandler
    {
        private readonly PlayerInput playerInput;

        private InputAction moveAction;
        private InputAction rotateAction;
        private InputAction powerUpAction;

        public event Action<Vector2> OnMovePerformed;
        public event Action OnMoveCanceled;
        public event Action<Vector2> OnRotatePerformed;
        public event Action OnRotateCanceled;
        public event Action OnPowerUpPressed;

        // Keep references to handlers so we can unbind them
        private Action<InputAction.CallbackContext> movePerformedHandler;
        private Action<InputAction.CallbackContext> moveCanceledHandler;
        private Action<InputAction.CallbackContext> rotatePerformedHandler;
        private Action<InputAction.CallbackContext> rotateCanceledHandler;
        private Action<InputAction.CallbackContext> powerUpStartedHandler;

        public PlayerInputHandler(PlayerInput playerInput)
        {
            this.playerInput = playerInput;

            moveAction = playerInput.actions["Move"];
            rotateAction = playerInput.actions["Rotate"];
            powerUpAction = playerInput.actions["PowerUp"];

            movePerformedHandler = ctx => OnMovePerformed?.Invoke(ctx.ReadValue<Vector2>());
            moveCanceledHandler = ctx => OnMoveCanceled?.Invoke();
            rotatePerformedHandler = ctx => OnRotatePerformed?.Invoke(ctx.ReadValue<Vector2>());
            rotateCanceledHandler = ctx => OnRotateCanceled?.Invoke();
            powerUpStartedHandler = ctx => OnPowerUpPressed?.Invoke();
        }

        public void Enable()
        {
            moveAction.Enable();
            rotateAction.Enable();
            powerUpAction.Enable();

            moveAction.performed += movePerformedHandler;
            moveAction.canceled += moveCanceledHandler;

            rotateAction.performed += rotatePerformedHandler;
            rotateAction.canceled += rotateCanceledHandler;

            powerUpAction.started += powerUpStartedHandler;
        }

        public void Disable()
        {
            moveAction.performed -= movePerformedHandler;
            moveAction.canceled -= moveCanceledHandler;

            rotateAction.performed -= rotatePerformedHandler;
            rotateAction.canceled -= rotateCanceledHandler;

            powerUpAction.started -= powerUpStartedHandler;

            moveAction.Disable();
            rotateAction.Disable();
            powerUpAction.Disable();
        }
    }
}
