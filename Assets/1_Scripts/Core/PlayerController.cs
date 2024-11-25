using System;
using UnityEngine;
using UnityEngine.InputSystem;
namespace RedGaint
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement Settings")]
        public MovementInputSettings movementSettings;
        public ParticleSystem gunSystem;

        private Animator anim;
        private Camera cam;
        private CharacterController controller;
        private bool isGrounded;
        private Vector3 desiredMoveDirection;
        private float verticalVel;
        // private Vector3 moveVector;
        
        private PlayerInput playerInput;
        private InputAction moveAction;
        private InputAction shootAction;
        private Vector2 movementInput;
        

        private void OnEnable()
        {
            if (playerInput == null)
                playerInput = GetComponent<PlayerInput>();
            
            moveAction.Enable();
            shootAction.Enable();
            moveAction.performed += OnMove;
            moveAction.canceled += OnMove;
            shootAction.started += OnShootStarted;
            shootAction.performed += OnShoot;
            shootAction.canceled += OnShootEnd;

        }
        private void OnDisable()
        {
            moveAction.performed -= OnMove;
            moveAction.canceled -= OnMove;
            shootAction.started -= OnShootStarted;
            shootAction.performed -= OnShoot;
            shootAction.canceled -= OnShootEnd;
            moveAction.Disable();
            shootAction.Disable();
        }

        private void OnShootStarted(InputAction.CallbackContext obj)
        {
            gunSystem.Play();
            // gunSystem.SetParticles();
        }

        private void Awake()
        {
            if (playerInput == null)
                playerInput = GetComponent<PlayerInput>();
            moveAction = playerInput.actions["Move"];
            shootAction = playerInput.actions["Shoot"];
            anim = GetComponent<Animator>();
            controller = GetComponent<CharacterController>();
            // cam = Camera.main;
        }
        

        private void OnMove(InputAction.CallbackContext context)
        {
            movementInput = context.ReadValue<Vector2>();
        }

        private void OnShoot(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                anim.SetBool("OnShooting", true);
              shootingSystem.Shoot(true);

            }
        }
        private void OnShootEnd(InputAction.CallbackContext obj)
        {
            anim.SetBool("OnShooting", false);
            gunSystem.Stop();
        }

        private ShootingSystem shootingSystem;
        private void Start()
        {
            cam = Camera.main;
            if (cam == null)
                Debug.LogWarning("No Main Camera found in the scene.");
            shootingSystem = GetComponent<ShootingSystem>();
        }

        void Update()
        {
            InputMagnitude();
            isGrounded = controller.isGrounded;
            if (isGrounded)
                verticalVel -= 0;
            else
                verticalVel -= 1;
            controller.Move(new Vector3(0, verticalVel * .2f * Time.deltaTime, 0));
        }

        void PlayerMoveAndRotation()
        {
            var forward = cam.transform.forward;
            var right = cam.transform.right;

            forward.y = 0f;
            right.y = 0f;

            forward.Normalize();
            right.Normalize();

            desiredMoveDirection = forward * movementInput.y + right * movementInput.x;

            if (!movementSettings.blockRotationPlayer)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(desiredMoveDirection), movementSettings.desiredRotationSpeed);
                controller.Move(desiredMoveDirection * Time.deltaTime * movementSettings.velocity);
            }
            else
            {
                controller.Move((transform.forward * movementInput.y + transform.right * movementInput.x) * Time.deltaTime * movementSettings.velocity);
            }
        }

        void InputMagnitude()
        {
            float speed = movementInput.sqrMagnitude;
            if (speed > movementSettings.allowPlayerRotation)
            {
                anim.SetFloat("Blend", speed, movementSettings.startAnimTime, Time.deltaTime);
                anim.SetFloat("X", movementInput.x, movementSettings.startAnimTime / 3, Time.deltaTime);
                anim.SetFloat("Y", movementInput.y, movementSettings.startAnimTime / 3, Time.deltaTime);
                PlayerMoveAndRotation();
            }
            else if (speed < movementSettings.allowPlayerRotation)
            {
                anim.SetFloat("Blend", speed, movementSettings.stopAnimTime, Time.deltaTime);
                anim.SetFloat("X", movementInput.x, movementSettings.stopAnimTime / 3, Time.deltaTime);
                anim.SetFloat("Y", movementInput.y, movementSettings.stopAnimTime / 3, Time.deltaTime);
            }
        }
    }
}