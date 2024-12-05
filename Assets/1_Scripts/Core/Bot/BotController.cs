using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace RedGaint
{
    public class BotController : BaseCharacterController
    {
 #region MemberVariables
        //public        
        //Controlls
        public BotSettings botSettings;
        
        //Animation Parameter
        //[Header("Animation Smoothing")]
        private float startAnimTime;
        private float stopAnimTime;
        public float animationspeedOffset; 
        
        //References
        public Transform botRoot;
        private Transform currentPlayerTransform;
        private Animator currentAnimtor;
        private NavMeshAgent currentBotAgent;
       
        //Data List
        private List<Vector3> botPatrollingPath;
        //Switches
        private bool isBotActive = false;
        private bool isInitialized = false; 
        private bool isFollowingPlayer = false;
        private bool isMoving = true;

        //iterators
        private int currentTargetIndex = 0;
        private int currentDestinationIndex = 0;

        public float sightRange;
        public float fovAngle;
        public float attackRange;
        private float minRotationAngle;
        private float maxRotationAngle;
        private float rotationDuration;

        //[SerializeField] private GlobalEnums.RotationMode rotationMode = GlobalEnums.RotationMode.SineWaveMode;
        public float stuckThreshold = 0.1f;  // Threshold for considering the bot stuck (low velocity)
        private float timeStuck = 0f;
        public float stuckTimeLimit = 2f; 
        [Range(0, 5)] [SerializeField] private float startBotSurvilanceAfter = 1f;         // Time to wait before rotating back to the original angle
        [Range(0, 5)][SerializeField] private float EndBotSurvilanceBefore = 1f;         // Time to wait before rotating back to the original angle

        private bool isRotating = false;  // Flag to prevent multiple rotations happening at once
        private Quaternion initialRotation; // Store the initial rotation to return to after the random rotation
        //ID's
        private string currentDestinationID;

#endregion//=======================================================================================
        private void InitialiseBotSettings(BotSettings settings,Vector3 spawnOrigin)
        {
            if (botRoot == null)
                botRoot = gameObject.transform;
            
            botRoot.transform.position = spawnOrigin;
            
            currentBotAgent = GetComponent<NavMeshAgent>();
            currentBotAgent.speed = settings.movementSpeed;
            sightRange= settings.sightRange;
            fovAngle= settings.fovAngle;
            attackRange = settings.attackRange;
            minRotationAngle=settings.minRotationAngle;
            maxRotationAngle=settings.maxRotationAngle;
            rotationDuration=settings.rotationDuration;
        }

        private void InitializeAnimationSettings(BotSettings settings)
        {
            currentAnimtor = GetComponent<Animator>();
            startAnimTime = settings.startAnimTime;
            stopAnimTime = settings.stopAnimTime;
            animationspeedOffset = settings.animationSpeedOffset;
        }
        
        public BotController InitialiseBot(List<Vector3> patrollingPath,string botID)
        {
            if (botSettings == null)
            {
                BugsBunny.LogError("Please set bot settings");
                return null; 
            }

            characternID = botID;
            gunHoister = GetComponentInChildren<GunHoister>();
            botPatrollingPath = patrollingPath;
            // Initialize bot settings
            InitialiseBotSettings(botSettings, patrollingPath[0]);
            
            // Initialize animation settings
            InitializeAnimationSettings(botSettings);
            isInitialized = true;
            return this;
        }

        public BotController ActivateBot(GlobalEnums.GameTeam team)
        {
            if (!isInitialized)
            {
                BugsBunny.LogError("Cannot activate bot: Bot has not been initialized.");
                return this;
            }

            if (isBotActive)
            {
                BugsBunny.LogYellow("Bot is already active.");
                return this; 
            }

            currentTargetIndex = 0;

            if (currentBotAgent == null)
            {
                BugsBunny.LogError("Bot agent is missing.");
                return this; 
            }

            if (botPatrollingPath == null || botPatrollingPath.Count < 1)
            {
                BugsBunny.LogError("No valid path nodes available.");
                return this; 
            }
            // Activate the bot
            isBotActive = true;
            SetPlayerTeam(team);
            StartCoroutine(WanderCoroutine());
            BugsBunny.Log3("Bot activated successfully.");
                
            return this;
        }
        void Update()
        {
            if (isBotActive)
            {
                Transform detectedPlayer = DetectPlayer(sightRange, fovAngle);
                if (detectedPlayer != null)
                {
                    BugsBunny.Log3("<color=red>------Played detected : -------------</color>");
                    if (!isFollowingPlayer)
                    {
                        StopCoroutine(WanderCoroutine());
                        isFollowingPlayer = true;
                        currentPlayerTransform = detectedPlayer;
                        StartCoroutine(FollowPlayerCoroutine());
                    }
                }
                else
                {
                    if (isFollowingPlayer)
                    {
                        isFollowingPlayer = false;
                        currentPlayerTransform = null;
                        StartCoroutine(WanderCoroutine());
                        BugsBunny.Log3("<color=green>------returning patrol : -------------</color>");
                    }
                }
                // Check for bot's speed and update animation parameters
                float speed = currentBotAgent.velocity.magnitude;
                UpdateAnimationParameters(speed);
                // Check if the bot is stuck (via the failsafe)
                CheckIfBotIsStuck(speed);
                CheckBotHealth();
            }
        }

        private float botHealth = 10f;
        private void CheckBotHealth()
        {
            if (botHealth < 1)
                KillTheActor();
        }

        // How long to wait before considering the bot stuck
        private void CheckIfBotIsStuck(float speed)
        {
            if (speed < stuckThreshold)
            {
                timeStuck += Time.deltaTime;

                // If the bot has been stuck for more than the time limit, handle it
                if (timeStuck >= stuckTimeLimit)
                {
                    HandleBotStuck();
                }
            }
            else
            {
                // Reset stuck time if the bot is moving
                timeStuck = 0f;
            }
        }
        
        private void HandleBotStuck()
        {
            // If the bot is stuck, stop the animation and reset the path
            BugsBunny.Log3("<color=yellow>------Bot is stuck, handling it.-------------</color>");
            currentAnimtor.SetFloat("Blend", 0f);  // Stop the walking animation

            // Optionally reset the NavMeshAgent's path
            currentBotAgent.ResetPath();

            // Optionally, attempt to replan the path or take other actions
            // currentBotAgent.SetDestination(newDestination); // Recalculate path
        }
        
        private IEnumerator WanderCoroutine()
        {
            while (!isFollowingPlayer)
            {
                GunState(true);
                // if (!isRotating)
                // {
                //     StartCoroutine(BotGunMovement(GlobalEnums.RotationMode.SineWaveMode));
                // }
                Vector3 targetPosition = GetNextPatrolPoint();
                currentBotAgent.SetDestination(targetPosition);
                BugsBunny.Log3("Bot reached destination : "+currentDestinationIndex);
                BugsBunny.Log3("-------------------------------------------------");

               yield return new WaitUntil(() => !currentBotAgent.pathPending && currentBotAgent.remainingDistance <= currentBotAgent.stoppingDistance);

                // Look for player after reaching each way point
                // :delay to simulate "looking around"
                
                yield return new WaitForSeconds(1f); 
            }
        }
        private Vector3 GetNextPatrolPoint()
        {
            // Check if the patrolPoints list is empty
            if (botPatrollingPath.Count == 0)
            {
                BugsBunny.Log3("No path node points are set!");
                return botRoot.position; // Return the bot's current position if no points are set
            }

            // Get the current pathnode point from the list
            Vector3 nextPathNodePoint = botPatrollingPath[currentDestinationIndex];

            // Update the index for the next pathnode point (loop back to 0 when we reach the end)
            currentDestinationIndex = (currentDestinationIndex + 1) % botPatrollingPath.Count;
            BugsBunny.Log3("given position : " + nextPathNodePoint);
            return nextPathNodePoint;
        }

        private IEnumerator FollowPlayerCoroutine()
        {
            while (isFollowingPlayer)
            {
                if (currentPlayerTransform != null)
                {
                    currentBotAgent.SetDestination(currentPlayerTransform.position);
                    BugsBunny.Log3("started Following ...");
                    // Attack if within attack range
                    if (Vector3.Distance(transform.position, currentPlayerTransform.position) <= attackRange)
                    {
                        AttackPlayer();

                       // yield return new WaitForSeconds(2f); // Delay between attacks, adjust as needed
                    }
                }

                yield return null;
            }
        }

        private Transform DetectPlayer(float range, float fovAngle)
        {
            RaycastHit hit;

            // Get the CapsuleCollider component
            CapsuleCollider capsuleCollider = GetComponent<CapsuleCollider>();

            // Use the center of the CapsuleCollider for raycast origin
            Vector3 rayOrigin = capsuleCollider != null ? capsuleCollider.bounds.center : transform.position;

            // Get the forward direction of the bot
            Vector3 forward = transform.TransformDirection(Vector3.forward);

            // Number of rays to cast (more rays = wider coverage)
            int numberOfRays = 5;
            float angleStep = fovAngle / numberOfRays;

            // Cast multiple rays to cover a cone of vision
            for (int i = 0; i < numberOfRays; i++)
            {
                // Calculate the direction of each ray
                float angle = (-fovAngle / 2) + (i * angleStep);
                Vector3 direction = Quaternion.Euler(0, angle, 0) * forward;

                // Perform the raycast
                RaycastHit[] hits = Physics.RaycastAll(rayOrigin, direction, range);

                foreach (var h in hits)
                {
                    
                    PlayerController player = h.collider.gameObject.GetComponent<PlayerController>();
                    if (player != null&& player.CurrentTeam!=currentTeam )
                    {
                        return h.transform;
                    }
                    
                    BotController OtherBot = h.collider.gameObject.GetComponent<BotController>();
                    if (OtherBot != null&& OtherBot.currentTeam!=currentTeam )
                    {
                        return h.transform;
                    }
                }
            }

            return null;
        }
        private void AttackPlayer()
        {
            BugsBunny.Log3("Bot is attacking the player!");
            //BotAttack();
        }
        private void UpdateAnimationParameters(float speed)
        {
            float adjustedSpeed = speed * animationspeedOffset;
            if (adjustedSpeed > 0.1f)
                currentAnimtor.SetFloat("Blend", adjustedSpeed, startAnimTime, Time.deltaTime);//Run
            else
                currentAnimtor.SetFloat("Blend", adjustedSpeed, stopAnimTime, Time.deltaTime);//Idle
        }
        public void BotAttack(Vector4 direction)
        {
            Vector3 attackDirection = new Vector3(direction.x, 0, direction.z).normalized;

            if (attackDirection != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(attackDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5);
            }

            // Simulate an attack action
            currentAnimtor.SetTrigger("Attack");
            BugsBunny.Log3("Bot is attacking in direction: " + attackDirection);
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<PowerUp>())
            {
                if (gameObject.GetComponent<PowerUpBasket>().IsPowerUpAvilable())
                {
                    gameObject.GetComponent<PowerUpBasket>().TriggerPowerUp();
                    BugsBunny.Log3("<color=red>Bot is attacking in powerup! please wait...</color>");
                }
            }
        }
    }//BotController
}//RedGaint