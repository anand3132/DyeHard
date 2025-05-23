using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace RedGaint
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : BaseCharacterController
    {
        public override bool LogThisClass { get; set; } = true; 
        [Header("Movement Settings")]
      public MovementInputSettings movementSettings;
        
        private Animator anim;
        private Camera cam;
        private CharacterController controller;
        private bool isGrounded;
        private Vector3 desiredMoveDirection;
        private float verticalVel;
        
        private PlayerInput playerInput;
        private InputAction moveAction;
        private InputAction powerUpAction;
        private InputAction rotateAction;
        private Vector2 movementInput;
        private Vector2 rotateInput;
        public bool dontTurnAround ;

        private const string RESPAERNEFFECT = "RF_ReSpawrnEffect";
        private const string DEADTHEFFECT = "RF_DeathEffect";
        private void OnEnable()
        {
            if (playerInput == null)
                playerInput = GetComponent<PlayerInput>();
            
            moveAction.Enable();
            powerUpAction.Enable();
            rotateAction.Enable();
            moveAction.performed += OnMove;
            moveAction.canceled += OnMoveEnd;
            powerUpAction.started += OnPowerUp;
            rotateAction.started += OnRotate;
            rotateAction.performed += OnRotate;
            rotateAction.canceled += OnRotateEnd;
        }
        private void OnDisable()
        {
            moveAction.performed -= OnMove;
            moveAction.canceled -= OnMoveEnd;
            powerUpAction.started -= OnPowerUp;
            rotateAction.started -= OnRotate;
            rotateAction.performed -= OnRotate;
            rotateAction.canceled -= OnRotateEnd;
            powerUpAction.Disable(); 
            moveAction.Disable();
            rotateAction.Disable();
        }


        private void Awake()
        {
            //Binding all inputs
            if (playerInput == null)
                playerInput = GetComponent<PlayerInput>();
            moveAction = playerInput.actions["Move"];
            powerUpAction = playerInput.actions["PowerUp"];
            rotateAction = playerInput.actions["Rotate"];
            characternID = "Player";
            anim = GetComponent<Animator>();
            controller = GetComponent<CharacterController>();
            gunHoister = GetComponentInChildren<GunHoister>();
        }
        
#region UICalls
        //Input Events- UI --------------------------------------------------
        private void OnMove(InputAction.CallbackContext context)
        {
            // canMove = true;
            movementInput = context.ReadValue<Vector2>();
        }
        private void OnMoveEnd(InputAction.CallbackContext context)
        {
            // canMove = false;
            movementInput = context.ReadValue<Vector2>();
        }

        private void OnRotate(InputAction.CallbackContext context)
        {
            GunState(true);
            anim.SetBool("OnShooting", true);
            // canRotate = true;
            rotateInput = context.ReadValue<Vector2>();
        }

        private void OnRotateEnd(InputAction.CallbackContext context)
        {
            GunState(false);
            anim.SetBool("OnShooting", false);
            // canRotate = false;
            rotateInput = context.ReadValue<Vector2>();        }

        // private void OnShootStarted(InputAction.CallbackContext context)
        // {
        //     GunState(true);
        // }
        
//      OnPowerupButton pressed On inputhandler
        private void OnPowerUp(InputAction.CallbackContext context)
        {
            if(!GetComponent<PowerUpBasket>().IsPowerUpAvilable())
                return;
            GetComponent<PowerUpBasket>().TriggerPowerUp();
            InputHandler.Instance.SetPowerUpIcon();
        }
#endregion
//----------------------------------------------------------------------------------------------------------------------
        protected override void Start()
        {            
            BugsBunny.LogYellow("--- PlayerController ---",this);
            base.Start();
            InitialisePlayerController();
            cam = Camera.main;
            if (cam == null)
                BugsBunny.LogYellow("No Main Camera found in the scene.",this);
            SetPlayerTeam(GlobalEnums.GameTeam.TeamBlue);
            spawnEffect.SetActive(true);
        }

        public override void OnPowerUpTriggered(GlobalEnums.PowerUpType triggeredPowerUp, float duration, float speedOffset)
        {
            if (triggeredPowerUp == GlobalEnums.PowerUpType.Sprint)
            {
                currentMovementSpeed = speedOffset;
                StartCoroutine(sprint(duration, speedOffset));
            }
        }

        
        IEnumerator sprint(float duration, float speed)
        {
            yield return new WaitForSeconds(duration);
            currentMovementSpeed = movementSettings.movementSpeed;
        }

        private void InitialisePlayerController()
        {
            deadthEffect= Helper.FindDeepChild(transform,DEADTHEFFECT).gameObject;
            spawnEffect= Helper.FindDeepChild(transform, RESPAERNEFFECT).gameObject;
            if(deadthEffect)
                deadthEffect.SetActive(false);
            if(spawnEffect)
                spawnEffect.SetActive(false);
            currentMovementSpeed = movementSettings.movementSpeed;
        }
        void Update()
        {
            InputMagnitude();
            RotatePlayer();
            isGrounded = controller.isGrounded;
            if (isGrounded)
                verticalVel -= 0;
            else
                verticalVel -= 1;
            controller.Move(new Vector3(0, verticalVel * .2f * Time.deltaTime, 0));
        }
        public override bool KillTheActor()
        {
            if (deadthEffect != null)
                deadthEffect.SetActive(true);
            
            currentMovementSpeed = movementSettings.movementSpeed;
            StartCoroutine(WaitForDeadthEffect(.1f));
            GamePlayManager.Instance.OnPlayerDeadth();
            return true;
        }

        #region PlayerMovement
        
        private float currentMovementSpeed = 0;
        void PlayerMoveAndRotation()
        {
            var forward = cam.transform.forward;
            var right = cam.transform.right;

            forward.y = 0f;
            right.y = 0f;

            forward.Normalize();
            right.Normalize();

            desiredMoveDirection = forward * movementInput.y + right * movementInput.x;
            
            if (dontTurnAround)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(desiredMoveDirection), movementSettings.desiredRotationSpeed);
                controller.Move(desiredMoveDirection *(Time.deltaTime * currentMovementSpeed));
            }
            else
            {
                controller.Move((transform.forward * movementInput.y + transform.right * movementInput.x) *( Time.deltaTime * currentMovementSpeed));
            }
        }
        void RotatePlayer()
        {
            float rotationSpeed =45f;
            if (rotateInput.sqrMagnitude > 0.01f)
            {
                Vector3 direction = new Vector3(rotateInput.x, 0, rotateInput.y).normalized;
                Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);

                // Adjust the multiplier and speed to make rotation more responsive
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, (movementSettings.desiredRotationSpeed * rotationSpeed) * Time.deltaTime);
            }
        }
        void InputMagnitude()
        {
            // if(!canMove)
            //     return;
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
        
        #endregion
    }
}//RedGaint