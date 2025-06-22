using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace RedGaint.Games.DyeHard
{
    public class BotController : BaseCharacterController
    {
        public override bool LogThisClass { get; set; } = true;

        #region Public Fields

        public BotSettings botSettings;
        internal Vector3 respawnPosition;
        private Transform botRoot;
        internal NavMeshAgent currentBotAgent;
        public List<Vector3> botPatrollingPath;

        internal float sightRange;
        internal float fovAngle;
        internal float attackFromDistance;
        private bool pauseMovement = false;

        public Transform detectedPlayer;

        #endregion

        #region Private Fields

        private BotAIController botAI;
        private BotAnimationController animationController;
        private BotMovementController movementController;
        private BotLifeCycleController lifeCycleController;

        #endregion

        #region Unity Events

        private void FixedUpdate()
        {
            if (!IsModelActive) return;
            UpdateMovementAnimation(movementController.GetCurrentSpeed());
            movementController.Tick();

        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out PowerUp powerUp) &&
                TryGetComponent(out PowerUpBasket basket) &&
                basket.IsPowerUpAvilable())
            {
                basket.TriggerPowerUp();
            }
        }

        #endregion

        #region Initialization

        public BotController Initialize(List<Vector3> patrollingPath, string botID, GlobalEnums.GameTeam team)
        {
            base.Initialize();

            characterID = botID;
            botRoot ??= transform;
            botPatrollingPath = patrollingPath;
            respawnPosition = patrollingPath[0];

            currentBotAgent = GetComponent<NavMeshAgent>();
            movementController = new BotMovementController(currentBotAgent, transform);
            Animator animator = GetComponent<Animator>();
            animationController = new BotAnimationController(animator);

            botAI = new BotAIController(this);
            lifeCycleController = new BotLifeCycleController(this, transform, this);

            InitializeBotComponents();

            SetPlayerTeam(team);
            return this;
        }

        private void InitializeBotComponents()
        {
            // Set position and rotation
            botRoot.position = respawnPosition;
            transform.rotation = Quaternion.identity;

            // Setup NavMeshAgent
            if (currentBotAgent != null)
            {
                currentBotAgent.enabled = false;
                currentBotAgent.speed = botSettings.movementSpeed;
                currentBotAgent.enabled = true;
            }

            // Bot settings
            sightRange = botSettings.sightRange;
            fovAngle = botSettings.fovAngle;
            attackFromDistance = botSettings.attackFromDistance;

            // Movement and animation setup
            movementController.SetMovementSpeed(botSettings.movementSpeed);
            movementController.SetPath(botPatrollingPath);
            animationController.Initialize(botSettings.startAnimTime, botSettings.stopAnimTime, botSettings.animationSpeedOffset);

            // Reset state
            detectedPlayer = null;
            SetIdleAnimation();
            botAI?.StopAI();
        }

        #endregion

        #region Activation

        public override bool ActivateTheActor()
        {
            Activate(0.1f, () =>
            {
                if (!pauseMovement)
                {
                    botAI?.StartAI();
                }
            });

            return true;
        }

        #endregion

        #region Power-Ups

        public override void OnPowerUpTriggered(GlobalEnums.PowerUpType triggeredPowerUp, float duration, float speedOffset)
        {
            // TODO: Implement power-up effects
            throw new System.NotImplementedException();
        }

        public void DropBomb()
        {
            if (TryGetComponent(out PowerUpBasket basket) &&
                basket.IsPowerUpAvilable() &&
                basket.CurrentPowerUpType == GlobalEnums.PowerUpType.Bomb)
            {
                basket.TriggerPowerUp();
            }
        }

        #endregion

        #region Death and Animation

        //called first
        public override bool KillTheActor()
        {
            Kill(1f, () =>
            {
                lifeCycleController.KillBot();
                
            });

            return true;
        }

        public void StopAI()
        {
            movementController.Stop();
            SetIdleAnimation();
            botAI?.StopAI();
        }

        public void SetIdleAnimation() => animationController?.UpdateMovementAnimation(0f);
        private void UpdateMovementAnimation(float speed) => animationController?.UpdateMovementAnimation(speed);

        #endregion
    }
}
