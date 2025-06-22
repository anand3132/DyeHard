using UnityEngine;

namespace RedGaint.Games.DyeHard
{
    public class PlayerMovementHandler
    {
        private readonly CharacterController controller;
        private readonly Animator animator;
        private readonly Transform characterTransform;
        private readonly MovementInputSettings movementSettings;
        private readonly Camera mainCamera;

        private Vector2 moveInput;
        private Vector2 rotateInput;
        private float currentSpeed;
        private float verticalVelocity;
        private Vector3 desiredDirection;

        private bool isFrozen;

        public bool DontTurnAround { get; set; }

        public PlayerMovementHandler(CharacterController controller, Animator animator, Transform characterTransform, MovementInputSettings movementSettings)
        {
            this.controller = controller;
            this.animator = animator;
            this.characterTransform = characterTransform;
            this.movementSettings = movementSettings;
            this.mainCamera = Camera.main;
        }

        public void SetSpeed(float speed)
        {
            currentSpeed = speed;
        }

        public void SetMoveInput(Vector2 input)
        {
            if (isFrozen) return;
            moveInput = input;
        }

        public void SetRotateInput(Vector2 input)
        {
            if (isFrozen) return;
            rotateInput = input;
        }

        public void Tick()
        {
            if (isFrozen) return;

            ApplyGravity();
            HandleRotation();
            HandleMovement();
        }

        public void Freeze()
        {
            isFrozen = true;
            moveInput = Vector2.zero;
            rotateInput = Vector2.zero;

            animator.SetFloat("Blend", 0);
            animator.SetFloat("X", 0);
            animator.SetFloat("Y", 0);
        }

        public void Unfreeze()
        {
            isFrozen = false;
        }

        private void ApplyGravity()
        {
            if (controller.isGrounded)
                verticalVelocity = 0f;
            else
                verticalVelocity -= 1f;

            controller.Move(new Vector3(0, verticalVelocity * 0.2f * Time.deltaTime, 0));
        }

        private void HandleRotation()
        {
            if (rotateInput.sqrMagnitude > 0.01f)
            {
                Vector3 direction = new Vector3(rotateInput.x, 0, rotateInput.y).normalized;
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                characterTransform.rotation = Quaternion.Slerp(characterTransform.rotation, targetRotation,
                    movementSettings.desiredRotationSpeed * 45f * Time.deltaTime);
            }
        }

        private void HandleMovement()
        {
            float inputMag = moveInput.sqrMagnitude;

            if (inputMag > movementSettings.allowPlayerRotation)
            {
                animator.SetFloat("Blend", inputMag, movementSettings.startAnimTime, Time.deltaTime);
                animator.SetFloat("X", moveInput.x, movementSettings.startAnimTime / 3, Time.deltaTime);
                animator.SetFloat("Y", moveInput.y, movementSettings.startAnimTime / 3, Time.deltaTime);
                MovePlayer();
            }
            else
            {
                animator.SetFloat("Blend", inputMag, movementSettings.stopAnimTime, Time.deltaTime);
                animator.SetFloat("X", moveInput.x, movementSettings.stopAnimTime / 3, Time.deltaTime);
                animator.SetFloat("Y", moveInput.y, movementSettings.stopAnimTime / 3, Time.deltaTime);
            }
        }

        private void MovePlayer()
        {
            
            Vector3 forward = mainCamera.transform.forward;
            Vector3 right = mainCamera.transform.right;
            forward.y = 0;
            right.y = 0;
            forward.Normalize();
            right.Normalize();

            desiredDirection = (forward * moveInput.y + right * moveInput.x).normalized;

            Vector3 finalMove;

            if (DontTurnAround)
            {
                Quaternion targetRot = Quaternion.LookRotation(desiredDirection);
                characterTransform.rotation = Quaternion.Slerp(characterTransform.rotation, targetRot, movementSettings.desiredRotationSpeed * Time.deltaTime);
                finalMove = desiredDirection * currentSpeed;
            }
            else
            {
                Vector3 localDir = (characterTransform.forward * moveInput.y + characterTransform.right * moveInput.x).normalized;
                finalMove = localDir * currentSpeed;
            }

            // Apply vertical gravity
            finalMove.y = verticalVelocity * 0.2f;

            // Apply movement
            controller.Move(finalMove * Time.deltaTime);

            // Debug velocity check (optional)
            Debug.Log("Player Speed: " + (finalMove.magnitude));
        }

    }
}
