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
        private InputAction shootAction;
        private InputAction powerUpAction;
        private Vector2 movementInput;
        private const string RESPAERNEFFECT = "RF_ReSpawrnEffect";
        private const string DEADTHEFFECT = "RF_DeathEffect";
        private void OnEnable()
        {
            if (playerInput == null)
                playerInput = GetComponent<PlayerInput>();
            
            moveAction.Enable();
            shootAction.Enable();
            powerUpAction.Enable();
            moveAction.performed += OnMove;
            moveAction.canceled += OnMove;
            shootAction.started += OnShootStarted;
            shootAction.performed += OnShoot;
            shootAction.canceled += OnShootEnd;
            powerUpAction.started += OnPowerUp;

        }
        private void OnDisable()
        {
            moveAction.performed -= OnMove;
            moveAction.canceled -= OnMove;
            shootAction.started -= OnShootStarted;
            shootAction.performed -= OnShoot;
            shootAction.canceled -= OnShootEnd;
            powerUpAction.started -= OnPowerUp;
            powerUpAction.Disable();
            moveAction.Disable();
            shootAction.Disable();
        }


        private void Awake()
        {
            //Binding all inputs
            if (playerInput == null)
                playerInput = GetComponent<PlayerInput>();
            moveAction = playerInput.actions["Move"];
            shootAction = playerInput.actions["Shoot"];
            powerUpAction = playerInput.actions["PowerUp"];
            characternID = "Player";
            anim = GetComponent<Animator>();
            controller = GetComponent<CharacterController>();
            gunHoister = GetComponentInChildren<GunHoister>();
            
        }
        
#region UICalls
        //Input Events- UI --------------------------------------------------
        private void OnMove(InputAction.CallbackContext context)
        {
            movementInput = context.ReadValue<Vector2>();
        }

        private void OnShootStarted(InputAction.CallbackContext obj)
        {
            GunState(true);
        }

        private void OnShoot(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                anim.SetBool("OnShooting", true);
                // shootingSystem.Shoot(true);
            }
        }
        private void OnShootEnd(InputAction.CallbackContext obj)
        {
            anim.SetBool("OnShooting", false);
            GunState(false);
        }

        private void OnPowerUp(InputAction.CallbackContext context)
        {
            if(!GetComponent<PowerUpBasket>().IsPowerUpAvilable())
                return;
            GetComponent<PowerUpBasket>().TriggerPowerUp();
            InputHandler.Instance.powerUpButtonObject.GetComponent<Image>().color =
                InputHandler.Instance.powerUpBtnDefaultColor;
            InputHandler.Instance.powerUpButtonObject.GetComponent<Image>().sprite =
                InputHandler.Instance.defaultSprite;
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
        
        private void InitialisePlayerController()
        {
            deadthEffect= Helper.FindDeepChild(transform,DEADTHEFFECT).gameObject;
            spawnEffect= Helper.FindDeepChild(transform, RESPAERNEFFECT).gameObject;
            if(deadthEffect)
                deadthEffect.SetActive(false);
            if(spawnEffect)
                spawnEffect.SetActive(false);
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

        private bool isPlayerHealthy()
        {
            return false;
        }

        public override bool KillTheActor()
        {
            if (deadthEffect != null)
                deadthEffect.SetActive(true);
            StartCoroutine(WaitForDeadthEffect(.1f));
            return true;
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
                controller.Move(desiredMoveDirection * Time.deltaTime * movementSettings.movementSpeed);
            }
            else
            {
                controller.Move((transform.forward * movementInput.y + transform.right * movementInput.x) * Time.deltaTime * movementSettings.movementSpeed);
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
}//RedGaint