using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
namespace RedGaint
{
    public class BotController : MonoBehaviour
    {
 #region MemberVariables
        //public GlobalEnums.BotType botType = GlobalEnums.BotType.Runner;
        // Movement parameters
        private bool isMoving = true;
        private float inputX;
        private float inputZ;

        //Animation Parameter
        //[Header("Animation Smoothing")]
        private float horizontalAnimSmoothTime;
        private float verticalAnimSmoothTime;
        private float startAnimTime;
        private float stopAnimTime;
        public float animationspeedOffset; 
        //References
        public Transform BotRoot;
        private CheckPointHandler checkpointHandler;
        private Transform currentPlayerTransform;
        private Animator currentAnimtor;
        public ParticleSystem inkParticle;
        private NavMeshAgent currentBotAgent;
        //Data List
        private List<Vector3> botCurrentPathNodes;
        //Switches
        private bool isBotActive = false;
        private bool isFollowingPlayer = false;
        //Iteratores
        private int currentTargetIndex = 0;

        //Controlls
        public BotSettings botSettings;
        private int currentDestinationIndex = 0;

        public float sightRange;
        public float fovAngle;
        public float attackRange;
        public float moveSpeed;
        public int maxFollowRange;
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
        public void InitialiseBot(Vector3 _position, CheckPointHandler checkPointHandler)
        {
            if (botSettings == null)
            {
                Debug.LogError("Please set bot settings");
                return;
            }
            checkpointHandler = checkPointHandler;
            InitialiseBotSettings(botSettings,_position);
            InitializeAnimationSettings(botSettings);
        }
        private void InitialiseBotSettings(BotSettings settings,Vector3 _position)
        {
            if (BotRoot == null)
                BotRoot = gameObject.transform;
            BotRoot.transform.position = _position;
            
            currentBotAgent = GetComponent<NavMeshAgent>();
            currentBotAgent.speed = settings.movementSpeed;
            sightRange= settings.sightRange;
            fovAngle= settings.fovAngle;
            attackRange = settings.attackRange;
            moveSpeed=settings.movementSpeed;
            maxFollowRange = settings.maxFollowRange;
            minRotationAngle=settings.minRotationAngle;
            maxRotationAngle=settings.maxRotationAngle;
            rotationDuration=settings.rotationDuration;
        }

        private void InitializeAnimationSettings(BotSettings settings)
        {
            currentAnimtor = GetComponent<Animator>();
            horizontalAnimSmoothTime = settings.horizontalAnimSmoothTime;
            verticalAnimSmoothTime = settings.verticalAnimSmoothTime;
            startAnimTime = settings.startAnimTime;
            stopAnimTime = settings.stopAnimTime;
            animationspeedOffset = settings.animationSpeedOffset;
        }

        public List<Vector3> GetWayPointPositions(List<Transform> wayPointTransforms)
        {
            List<Vector3> positions = new List<Vector3>();
            foreach (Transform waypoint in wayPointTransforms)
            {
                positions.Add(waypoint.position);
            }
            return positions;
        }
        public bool ActivateBot()
        {
            if (isBotActive) { return false; }
            currentTargetIndex = 0;
            if (currentBotAgent == null)
            {
                Debug.LogError("Cant able to get the Bot Agent!!");
                return false;
            }
            if (checkpointHandler != null)
            {
                isBotActive = true;
                botCurrentPathNodes =GetWayPointPositions(checkpointHandler.GetWayPointList());
                StartCoroutine(WanderCoroutine());
                return true;
            }
            return false;
        }

        void Update()
        {
            if (isBotActive)
            {
                Transform detectedPlayer = DetectPlayer(sightRange, fovAngle);
                if (detectedPlayer != null)
                {
                    Debug.Log("<color=red>------Played detected : -------------</color>");
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
                        Debug.Log("<color=green>------returning patrol : -------------</color>");
                    }
                }
                // Check for bot's speed and update animation parameters
                float speed = currentBotAgent.velocity.magnitude;
                UpdateAnimationParameters(speed);
                // Check if the bot is stuck (via the failsafe)
                CheckIfBotIsStuck(speed);
            }
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
            Debug.Log("<color=yellow>------Bot is stuck, handling it.-------------</color>");
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
                Debug.Log("Bot reached destination : "+currentDestinationIndex);
                Debug.Log("-------------------------------------------------");

               yield return new WaitUntil(() => !currentBotAgent.pathPending && currentBotAgent.remainingDistance <= currentBotAgent.stoppingDistance);

                // Look for player after reaching each way point
                // :delay to simulate "looking around"
                
                yield return new WaitForSeconds(1f); 
                
            }
        }
        private IEnumerator BotGunMovement(GlobalEnums.RotationMode rotationMode)
        {
            yield return new WaitForSeconds(startBotSurvilanceAfter);

            // Set the flag to indicate that the bot is rotating
            isRotating = true;

            // Store the initial rotation (before applying any mode)
            Quaternion initialRotation = inkParticle.gameObject.transform.rotation;

            // Perform rotation based on the selected mode
            float rotationAngle = 0f;

            switch (rotationMode)
            {
                case GlobalEnums.RotationMode.RandomMode:
                    rotationAngle = Random.Range(minRotationAngle, maxRotationAngle);
                    break;

                case GlobalEnums.RotationMode.SineWaveMode:
                    rotationAngle = Mathf.Sin(Time.time) * maxRotationAngle;
                    break;
            }

            // Calculate the target rotation based on the current forward direction and the calculated angle
            Quaternion targetRotation = inkParticle.gameObject.transform.rotation * Quaternion.Euler(0, rotationAngle, 0);

            // Smoothly rotate the bot to the target rotation over the specified duration
            float elapsedTime = 0f;
            while (elapsedTime < rotationDuration)
            {
                inkParticle.gameObject.transform.rotation = Quaternion.Slerp(initialRotation, targetRotation, elapsedTime / rotationDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Finalize the rotation to ensure it reaches the exact target rotation
            inkParticle.gameObject.transform.rotation = targetRotation;

            // Wait for the specified duration before rotating back to the original orientation
            yield return new WaitForSeconds(EndBotSurvilanceBefore);

            // Rotate back to the initial rotation
            elapsedTime = 0f;
            while (elapsedTime < rotationDuration)
            {
                inkParticle.gameObject.transform.rotation = Quaternion.Slerp(targetRotation, initialRotation, elapsedTime / rotationDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Finalize the return rotation to ensure it reaches the exact initial rotation
            inkParticle.gameObject.transform.rotation = initialRotation;

            // Reset the rotating flag after the action is complete
            isRotating = false;
        }
        private Vector3 GetNextPatrolPoint()
        {
            // Check if the patrolPoints list is empty
            if (botCurrentPathNodes.Count == 0)
            {
                Debug.LogWarning("No path node points are set!");
                return BotRoot.position; // Return the bot's current position if no points are set
            }

            // Get the current pathnode point from the list
            Vector3 nextPathNodePoint = botCurrentPathNodes[currentDestinationIndex];

            // Update the index for the next pathnode point (loop back to 0 when we reach the end)
            currentDestinationIndex = (currentDestinationIndex + 1) % botCurrentPathNodes.Count;
            Debug.Log("given position : " + nextPathNodePoint);
            return nextPathNodePoint;
        }

        private IEnumerator FollowPlayerCoroutine()
        {
            while (isFollowingPlayer)
            {
                if (currentPlayerTransform != null)
                {
                    currentBotAgent.SetDestination(currentPlayerTransform.position);
                    Debug.Log("started Following ...");
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
                    // Check if the hit object has a PlayerController component
                    if (h.collider.GetComponent<PlayerController>() != null)
                    {
                        // Return the player's transform if detected
                        return h.transform;
                    }
                }
            }

            return null;
        }
 #region forDebug

        // Utility to draw a filled cone for the field of view
        private void DrawCone(Vector3 origin, Vector3 leftDirection, Vector3 rightDirection, float length)
        {
            int steps = 10;
            Vector3 previousPoint = origin + leftDirection * length;
            for (int i = 1; i <= steps; i++)
            {
                float t = i / (float)steps;
                Vector3 point = Vector3.Lerp(previousPoint, origin + rightDirection * length, t);
                Gizmos.DrawLine(previousPoint, point);
                previousPoint = point;
            }
        }

#endregion
        private void AttackPlayer()
        {
            Debug.Log("Bot is attacking the player!");
            //BotAttack();
        }
        private void MoveBotTo(Vector4 direction)
        {
            Vector3 moveDirection = Vector3.zero;

            if (direction.x > 0) moveDirection += transform.right;         // Right
            if (direction.x < 0) moveDirection -= transform.right;         // Left
            if (direction.z > 0) moveDirection += transform.forward;       // Forward
            if (direction.z < 0) moveDirection -= transform.forward;       // Backward
            inputX = direction.x;
            inputZ = direction.z;
            currentBotAgent.Move(moveDirection.normalized * botSettings.movementSpeed * Time.deltaTime);
        }

        private void UpdateAnimationParameters(float speed)
        {
            // Apply the speed offset to reduce the speed
            float adjustedSpeed = speed * animationspeedOffset;

            // Run
            if (adjustedSpeed > 0.1f)
            {
                currentAnimtor.SetFloat("Blend", adjustedSpeed, startAnimTime, Time.deltaTime);
                currentAnimtor.SetFloat("X", inputX, startAnimTime / 3, Time.deltaTime);
                currentAnimtor.SetFloat("Y", inputZ, startAnimTime / 3, Time.deltaTime);
            }
            // Idle
            else
            {
                currentAnimtor.SetFloat("Blend", adjustedSpeed, stopAnimTime, Time.deltaTime);
                currentAnimtor.SetFloat("X", inputX, stopAnimTime / 3, Time.deltaTime);
                currentAnimtor.SetFloat("Y", inputZ, stopAnimTime / 3, Time.deltaTime);
            }
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
            Debug.Log("Bot is attacking in direction: " + attackDirection);
        }

        public void GunState(bool status)
        {
            if (status)
            {
                inkParticle.Play();
            }
            else
            {
                inkParticle.Stop();
            }
        }
        
    }//BotController
}//RedGaint