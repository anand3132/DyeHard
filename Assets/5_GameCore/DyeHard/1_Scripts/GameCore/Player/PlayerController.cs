using UnityEngine;
using UnityEngine.InputSystem;

namespace RedGaint.Games.DyeHard
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : BaseCharacterController
    {
        public override bool LogThisClass { get; set; } = false;

        [Header("Movement Settings")]
        public MovementInputSettings movementSettings;

        [HideInInspector] public bool isGodMode;
        public bool dontTurnAround;

        private CharacterController controller;
        private Camera cam;

        private PlayerInput playerInput;
        private PlayerInputHandler inputHandler;
        private PlayerMovementHandler movementHandler;
        private PowerUpHandler powerUpHandler;

        private void Awake()
        {
            playerInput = GetComponent<PlayerInput>();
            controller = GetComponent<CharacterController>();
            inputHandler = new PlayerInputHandler(playerInput);
        }


        protected override void Initialize()
        {
            base.Initialize();
            
            characterID = "Player";
            movementHandler = new PlayerMovementHandler(controller, characterAnimator, transform, movementSettings);
            movementHandler.SetSpeed(movementSettings.movementSpeed);
            movementHandler.DontTurnAround = dontTurnAround;

            powerUpHandler = new PowerUpHandler(
                movementSettings.movementSpeed,
                SetCurrentSpeed,
                GetComponentInChildren<PowerUpBasket>()
            );
            
            cam = Camera.main;
            if (cam == null)
                BugsBunny.LogYellow("No Main Camera found in the scene.", this);

            SetPlayerTeam(TeamManager.Instance.GetBalancedRandomTeam());
        }


        public override bool ActivateTheActor()
        {
            Activate();
            UnfreezeInput();
            return true;
        }
        

        protected override float GetInitialSpeed() => movementSettings.movementSpeed;

        private void OnEnable()
        {
            inputHandler.Enable();

            inputHandler.OnMovePerformed += HandleMove;
            inputHandler.OnMoveCanceled += HandleMoveEnd;
            inputHandler.OnRotatePerformed += HandleRotate;
            inputHandler.OnRotateCanceled += HandleRotateEnd;
            inputHandler.OnPowerUpPressed += HandlePowerUp;

            SetCurrentSpeed(movementSettings.movementSpeed);
            // ResetCharacter();
        }


        private void OnDisable()
        {
            inputHandler.OnMovePerformed -= HandleMove;
            inputHandler.OnMoveCanceled -= HandleMoveEnd;
            inputHandler.OnRotatePerformed -= HandleRotate;
            inputHandler.OnRotateCanceled -= HandleRotateEnd;
            inputHandler.OnPowerUpPressed -= HandlePowerUp;

            inputHandler.Disable();
        }

        private void Update() => movementHandler?.Tick();

        #region Input Handlers

        private void HandleMove(Vector2 input)
        {
            movementHandler?.SetMoveInput(input);
        }

        private void HandleMoveEnd()
        {
            movementHandler?.SetMoveInput(Vector2.zero);
        }

        private void HandleRotate(Vector2 input)
        {
            GunState(true);
            // gunHoister.gameObject.SetActive(true);

            characterAnimator.SetBool("OnShooting", true);
            movementHandler?.SetRotateInput(input);
        }

        private void HandleRotateEnd()
        {
            GunState(false);
            // gunHoister.gameObject.SetActive(false);

            characterAnimator.SetBool("OnShooting", false);
            movementHandler?.SetRotateInput(Vector2.zero);
        }

        private void HandlePowerUp()
        {
            powerUpHandler?.TriggerPowerUp();
        }

        #endregion

        #region Power-Up & Speed

        public override void OnPowerUpTriggered(GlobalEnums.PowerUpType triggeredPowerUp, float duration, float speedOffset)
        {
            if (triggeredPowerUp == GlobalEnums.PowerUpType.Sprint)
            {
                powerUpHandler?.TriggerSprint(speedOffset, duration);
            }
        }

        public override void SetCurrentSpeed(float speed)
        {
            base.SetCurrentSpeed(speed);
            movementHandler?.SetSpeed(speed);
        }

        #endregion

        #region Lifecycle & State

        public override bool KillTheActor()
        {
            if (isGodMode)
                return false;

            currentMovementSpeed = movementSettings.movementSpeed;
            FreezeInput();
            Deactivate();
            Kill(1f, () => GamePlayManager.Instance.OnPlayerDeadth());

            return true;
        }

        public void FreezeInput() => movementHandler?.Freeze();
        public void UnfreezeInput() => movementHandler?.Unfreeze();

        #endregion
        
    }
}
