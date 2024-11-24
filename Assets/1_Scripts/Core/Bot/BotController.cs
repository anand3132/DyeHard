using RedGaint;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static Cinemachine.CinemachineTargetGroup;

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

        //References
        public Transform BotRoot;
        private CheckPointHandler checkpointHandler;
        private CapsuleCollider capsuleCollider;
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

        [Range(20, 80)]
        public float sightRange = 20f; // How far the bot can see
        [Range(15, 180)]
        public float fovAngle = 45f;  // Field of view angle
        //public LayerMask playerLayer;  // Layer containing the player
        [Range(3, 10)]
        public float attackRange = 3f; // Range at which the bot can attack
        [Range(3, 5)]
        public float moveSpeed = 3f;  // Speed at which the bot moves
        [Range(3, 20)]
        public int maxFollowRange = 3;


        private int currentDestinationIndex = 0;
        [SerializeField] private float minRotationAngle = -15f;  // Minimum random angle for left rotation
        [SerializeField] private float maxRotationAngle = 15f;   // Maximum random angle for right rotation
        [SerializeField] private float rotationDuration = 2f;     // Time to rotate to the new random angle

        //[SerializeField] private GlobalEnums.RotationMode rotationMode = GlobalEnums.RotationMode.SineWaveMode;


        [Range(0, 5)]
        [SerializeField] private float startBotSurvilanceAfter = 1f;         // Time to wait before rotating back to the original angle

        [Range(0, 5)]
        [SerializeField] private float EndBotSurvilanceBefore = 1f;         // Time to wait before rotating back to the original angle

        private bool isRotating = false;  // Flag to prevent multiple rotations happening at once
        private Quaternion initialRotation; // Store the initial rotation to return to after the random rotation


        //ID's
        private string currentDestinationID;

#endregion//=======================================================================================

        public void InitialiseBot(Vector3 position, CheckPointHandler checkPointHandler)
        {
            if (BotRoot == null)
                BotRoot = gameObject.transform;
            BotRoot.transform.position = position;
            currentBotAgent = GetComponent<NavMeshAgent>();
            currentAnimtor = GetComponent<Animator>();
            UpdateBotSettings(botSettings);

            checkpointHandler = checkPointHandler;
            InitializeAnimationSmoothing(botSettings);

        }
        private void UpdateBotSettings(BotSettings _botSettings)
        {
            currentBotAgent.speed = _botSettings.movementSpeed;

        }

        private void InitializeAnimationSmoothing(BotSettings settings)
        {
            // Assign settings values for animation smoothing
            horizontalAnimSmoothTime = settings.horizontalAnimSmoothTime;
            verticalAnimSmoothTime = settings.verticalAnimSmoothTime;
            startAnimTime = settings.startAnimTime;
            stopAnimTime = settings.stopAnimTime;

        }

        public List<Vector3> GetWayPointPositions(List<Transform> wayPointTransforms)
        {

            // Create a new list to store the positions
            List<Vector3> positions = new List<Vector3>();

            // Extract positions from the Transform list
            foreach (Transform waypoint in wayPointTransforms)
            {
                positions.Add(waypoint.position);
            }

            return positions;
        }
        public List<Transform> debugPathList;
        public bool ActivateBot()
        {
            if (isBotActive) { return false; }
            currentTargetIndex = 0;
            if (checkpointHandler != null)
            {
                //   SetNextDestination();
                isBotActive = true;

                debugPathList = checkpointHandler.GetWayPointList();

                botCurrentPathNodes =GetWayPointPositions(debugPathList);
                capsuleCollider = GetComponent<CapsuleCollider>();
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

                        //StartCoroutine(PatrolRoutine());

                    }
                }
               // MoveBotTo(currentBotAgent.destination);
                UpdateAnimationParameters(currentBotAgent.velocity.magnitude);
            }
        }

  

        private IEnumerator WanderCoroutine()
        {
            while (!isFollowingPlayer)
            {
                GunState(true);
                //if (!isRotating)
                //{
                //    StartCoroutine(BotGunMovement(GlobalEnums.RotationMode.SineWaveMode));
                //}
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
        //private string GetWaypointID(currentDestinationIndex)
        //{
        //    checkpointHandler.GetWayPointList()[currentDestinationIndex].GetComponent<way>
        //}
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

        // Logic to follow the player
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
                    if (h.collider.GetComponent<MovementInput>() != null)
                    {
                        // Return the player's transform if detected
                        return h.transform;
                    }
                }
            }

            return null;
        }

        private Vector3 GetPlayerPosition()
        {
            // Replace this with actual logic to get the player's position (e.g., reference to player object)
            return transform.root.GetComponent<GameCoreElements>().GetPlayer().transform.position;
        }

        //-----------------------------------------------------------------------------------------------
        //OLD LOGIC 
        private IEnumerator PatrolRoutine()
        {
            while (true)
            {
                // Check if the bot is following the player
                if (isFollowingPlayer)
                {
                    // Stop wandering and follow the player logic 
                    //Todo:need to check
                    yield return StartCoroutine(FollowPlayerRoutine());
                }
                else
                {
                    // Move to the next position
                    Vector3 targetPosition = botCurrentPathNodes[currentDestinationIndex];
                    currentBotAgent.SetDestination(targetPosition);


                    // Wait until the bot reaches the current destination
                    yield return new WaitUntil(() =>
                        !currentBotAgent.pathPending && currentBotAgent.remainingDistance <= currentBotAgent.stoppingDistance);

                    // Rotate to face the next point
                    int nextIndex = (currentDestinationIndex + 1) % botCurrentPathNodes.Count;
                    Vector3 directionToNextPoint = botCurrentPathNodes[nextIndex] - transform.position;
                    Quaternion targetRotation = Quaternion.LookRotation(directionToNextPoint);

                    // Smoothly rotate towards the next point
                    while (Quaternion.Angle(transform.rotation, targetRotation) > 1f)
                    {
                        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, currentBotAgent.angularSpeed * Time.deltaTime);
                        yield return null; // Wait until the next frame to continue rotating
                    }

                    Debug.Log("============================================");
                    Debug.Log("Bot is moving to the next patrol point: " + currentDestinationIndex + " -> " + nextIndex);
                    // Update the target index to the next patrol point
                    currentDestinationIndex = nextIndex;


                }

                // Add a small delay before starting the next patrol point
                yield return null;
            }
        }

        private IEnumerator FollowPlayerRoutine()
        {
            // This is where bot detect and move towards the player

            Debug.Log("Bot detected the player and is following.");

            while (isFollowingPlayer)
            {
                Vector3 playerPosition = GetPlayerPosition();

                // Wait for bot to reach the player or the target
                yield return new WaitUntil(() =>
                    !currentBotAgent.pathPending && currentBotAgent.remainingDistance <= currentBotAgent.stoppingDistance);

                // Call the attack function when close enough to the player
                if (Vector3.Distance(transform.position, playerPosition) <= attackRange)
                {
                    AttackPlayer();
                    isFollowingPlayer = false;
                }

                // If the player runs away, stop following
                if (Vector3.Distance(transform.position, playerPosition) > maxFollowRange)
                {
                    isFollowingPlayer = false;
                    Debug.Log("<color=blue> Player is out of range, returning to patrol.</color>");
                }

                yield return null;
            }
        }

//------------------------------------------------------------------------------------------------

        
 #region forDebug

        private void OnDrawGizmos()
        {
            if (capsuleCollider != null)
            {
                Gizmos.color = Color.red;
                Vector3 rayOrigin = capsuleCollider.bounds.center;

                // Visualize the cone of rays (for debugging)
                float angleStep = fovAngle / 5;
                for (int i = 0; i < 5; i++)
                {
                    float angle = (-fovAngle / 2) + (i * angleStep);
                    Vector3 direction = Quaternion.Euler(0, angle, 0) * transform.forward;

                    // Draw the cone's rays
                    Gizmos.DrawRay(rayOrigin, direction * sightRange);
                }

                // Draw the field of view as a cone shape
                Gizmos.color = new Color(1f, 0f, 0f, 0.2f); // Semi-transparent red for the cone area
                Vector3 left = Quaternion.Euler(0, -fovAngle / 2, 0) * transform.forward;
                Vector3 right = Quaternion.Euler(0, fovAngle / 2, 0) * transform.forward;
                Gizmos.DrawRay(rayOrigin, left * sightRange);
                Gizmos.DrawRay(rayOrigin, right * sightRange);

                // Optionally, you can fill the area with a semi-transparent cone shape:
                DrawCone(rayOrigin, left, right, sightRange);
            }
        }

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
            //float speed = new Vector2(inputX, inputZ).sqrMagnitude;
            //Run
            if (speed > 0.1f)
            {
                currentAnimtor.SetFloat("Blend", speed, startAnimTime, Time.deltaTime);
                currentAnimtor.SetFloat("X", inputX, startAnimTime / 3, Time.deltaTime);
                currentAnimtor.SetFloat("Y", inputZ, startAnimTime / 3, Time.deltaTime);
            }
            //Idle
            else
            {
                currentAnimtor.SetFloat("Blend", speed, stopAnimTime, Time.deltaTime);
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

        public void OnTriggerEnter(Collider other)
        {
            //if (other.GetComponent<CheckPoint>() != null)
            //{
            //    CheckPoint currentWayPoint = other.GetComponent<CheckPoint>();
            //    if (currentWayPoint.CheckPointType == GlobalEnums.CheckPointType.WayPoint
            //        && string.Equals(currentDestinationID, currentWayPoint.CheckPointID))
            //    {
            //        currentBotAgent.isStopped = true;
            //        isMoving = false;
            //        GunState(false);
            //        Debug.Log("Reached Destination : " + currentDestinationID);
            //        Debug.Log("------------------------------------------------");
            //        if (SetNextDestination())
            //        {
            //            isMoving = true;
            //            GunState(true);
            //        }

            //    }

            //    Debug.Log("triggered with : " + other.gameObject.name);
            //}
        }

        public void OnCollisionEnter(Collision collision)
        {
            Debug.Log("on collition with : " + collision.gameObject.name);

        }//OnCollisionEnter
    }//BotController
}//RedGaint