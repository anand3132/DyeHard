using RedGaint;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
namespace RedGaint
{
    public class BotController : MonoBehaviour
    {

        public BotSettings botSettings; // Reference to the ScriptableObject
        private NavMeshAgent currentBotAgent;
        public Transform BotRoot;
        private CheckPointHandler checkpointHandler;
        public GlobalEnums.BotType botType = GlobalEnums.BotType.Runner;
        //Position Parameter
        private int currentDestinationIndex = 0;
        //private Vector3 NextDestination;
        private string currentDestinationID;
        // Movement parameters
        private bool isMoving = true;
        private float inputX;
        private float inputZ;

        //Animation Parameter
        [Header("Animation Smoothing")]
        private float horizontalAnimSmoothTime;
        private float verticalAnimSmoothTime;
        private float startAnimTime;
        private float stopAnimTime;
        //public bool isBotActive = false;
        private Animator currentAnimtor;

        [SerializeField] private List<Vector3> patrolPoints;
        private int currentTargetIndex;

        [SerializeField] private ParticleSystem inkParticle;

        public void InitialiseBot(Vector3 position, CheckPointHandler checkPointHandler)
        {
            if (BotRoot == null)
                BotRoot = gameObject.transform;
            BotRoot.transform.position = position;
            currentBotAgent = GetComponent<NavMeshAgent>();
            currentAnimtor = GetComponent<Animator>();
            currentBotAgent.speed = botSettings.movementSpeed;
            checkpointHandler = checkPointHandler;
            InitializeAnimationSmoothing(botSettings);

        }
        private void InitializeAnimationSmoothing(BotSettings settings)
        {
            // Assign settings values for animation smoothing
            horizontalAnimSmoothTime = settings.horizontalAnimSmoothTime;
            verticalAnimSmoothTime = settings.verticalAnimSmoothTime;
            startAnimTime = settings.startAnimTime;
            stopAnimTime = settings.stopAnimTime;

        }
        //-------------------------------------------------------------------------------------------------
        public float sightRange = 20f; // How far the bot can see
        public float fovAngle = 45f;  // Field of view angle
        public LayerMask playerLayer;  // Layer containing the player
        public float attackRange = 3f; // Range at which the bot can attack
        public float moveSpeed = 3f;  // Speed at which the bot moves

        private bool isBotActive = false;
        private bool isFollowingPlayer = false;
        private Transform currentPlayerTransform;

        // Reference to the Bot's CapsuleCollider
        private CapsuleCollider capsuleCollider;

        public List<Vector3> GetWayPointPositions(List<Transform> wayPoints)
        {

            // Create a new list to store the positions
            List<Vector3> positions = new List<Vector3>();

            // Extract positions from the Transform list
            foreach (Transform waypoint in wayPoints)
            {
                positions.Add(waypoint.position);
            }

            return positions;
        }

        public bool ActivateBot()
        {
            if (isBotActive) { return false; }
            currentTargetIndex = 0;

            if (checkpointHandler != null)
            {
                //   SetNextDestination();
                isBotActive = true;

                patrolPoints =GetWayPointPositions(checkpointHandler.GetWayPointList());
                capsuleCollider = GetComponent<CapsuleCollider>();
               // StartCoroutine(WanderCoroutine());
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
                    }
                }
            }
        }

        private IEnumerator WanderCoroutine()
        {
            while (!isFollowingPlayer)
            {
                // Logic to wander around and patrol (previous logic)
                Vector3 targetPosition = GetNextPatrolPoint();
                currentBotAgent.SetDestination(targetPosition);

                yield return new WaitUntil(() => !currentBotAgent.pathPending && currentBotAgent.remainingDistance <= currentBotAgent.stoppingDistance);

                // Look for player after reaching each patrol point
                yield return new WaitForSeconds(1f); // Optional delay to simulate "looking around"
            }
        }

        private IEnumerator FollowPlayerCoroutine()
        {
            while (isFollowingPlayer)
            {
                if (currentPlayerTransform != null)
                {
                    currentBotAgent.SetDestination(currentPlayerTransform.position);

                    // Attack if within attack range
                    if (Vector3.Distance(transform.position, currentPlayerTransform.position) <= attackRange)
                    {
                        AttackPlayer();
                        yield return new WaitForSeconds(2f); // Delay between attacks, adjust as needed
                    }
                }

                yield return null;
            }
        }

        private Transform DetectPlayer(float range, float fovAngle)
        {
            RaycastHit hit;
            Vector3 rayOrigin = capsuleCollider != null ? capsuleCollider.bounds.center : transform.position;
            Vector3 forward = transform.TransformDirection(Vector3.forward);

            RaycastHit[] hits = Physics.RaycastAll(rayOrigin, forward, range);

            foreach (var h in hits)
            {
                // Check if the hit object has a PlayerController component
                if (h.collider.GetComponent<MovementInput>() != null)
                {
                    // Check if the player is within the field of view
                    Vector3 directionToPlayer = (h.transform.position - rayOrigin).normalized;
                    float angle = Vector3.Angle(transform.forward, directionToPlayer);

                    if (angle <= fovAngle * 0.5f)
                    {
                        return h.transform; // Return the player's transform if detected
                    }
                }
            }

            return null;
        }

        private void AttackPlayer()
        {
            // Here you would call your attack logic, for example:
            Debug.Log("Bot is attacking the player!");

            // Example: Call a method from the PlayerController if needed
            //if (currentPlayerTransform != null)
            //{
            //    PlayerController playerController = currentPlayerTransform.GetComponent<PlayerController>();
            //    playerController.TakeDamage(10); // Example of dealing damage
            //}
        }

        private Vector3 GetNextPatrolPoint()
        {
            // Logic to get the next patrol point, this can be a list of waypoints or random positions
            return new Vector3(Random.Range(-10f, 10f), 0f, Random.Range(-10f, 10f)); // Example random point
        }

        // To visualize the raycast (debugging purposes)
        private void OnDrawGizmos()
        {
            if (capsuleCollider != null)
            {
                Gizmos.color = Color.red;
                Vector3 rayOrigin = capsuleCollider.bounds.center;
                Gizmos.DrawLine(rayOrigin, rayOrigin + transform.TransformDirection(Vector3.forward) * sightRange);
            }
        }
    //}
    //// Raycast to detect the player by checking for PlayerController component
    //private Transform DetectPlayer(float range, float fovAngle)
    //{
    //    RaycastHit hit;

    //    // Get direction for the raycast (forward direction)
    //    Vector3 forward = transform.TransformDirection(Vector3.forward);
    //    Vector3 origin = transform.position;

    //    // Check if the player is within the field of view angle and range
    //    RaycastHit[] hits = Physics.RaycastAll(origin, forward, range, playerLayer);

    //    foreach (var h in hits)
    //    {
    //        // Calculate angle between forward direction and hit direction
    //        Vector3 directionToPlayer = (h.transform.position - origin).normalized;
    //        float angle = Vector3.Angle(transform.forward, directionToPlayer);

    //        // If player is within the field of view angle and hit object has PlayerController component
    //        if (angle <= fovAngle * 0.5f && h.collider.GetComponent<MovementInput>() != null)
    //        {
    //            return h.transform; // Return the player's transform if detected
    //        }
    //    }
    //    return null;
    //}

    //private void Attack(Transform player)
    //    {
    //        // Your attack logic goes here
    //        Debug.Log("Attacking player");
    //        // Example: Call player’s health system, play attack animations, etc.
    //    }

        //void Update()
        //{
        //    if (!isBotActive)
        //        return;
        //    RunBotEngine();
        //}

        //private void RunBotEngine()
        //{
        //    if (isMoving)
        //        MoveBotTo(currentBotAgent.destination);
        //    else
        //        MoveBotTo(Vector3.zero);

        //    UpdateAnimationParameters();
        //}

        private bool IsPathCompletel()
        {

            if (Vector3.Distance(currentBotAgent.destination, currentBotAgent.transform.position) <= currentBotAgent.stoppingDistance)
            {
                //if (!currentBotAgent.hasPath || currentBotAgent.velocity.sqrMagnitude == 0f)
                //{
                return true;
                //}
            }

            return false;
        }

        private bool SetNextDestination()
        {
            // Check if there are any checkpoints in the handler
            if (checkpointHandler.GetWayPointList().Count == 0)
            {
                Debug.LogWarning("No Way points Avilable.");
                return false;
            }

            if (!isBotActive)
            {
                currentDestinationIndex = 0;
                currentBotAgent = GetComponent<NavMeshAgent>();
                var item = checkpointHandler.GetWayPointList();
                currentBotAgent.destination = item[currentDestinationIndex].position;
                currentDestinationID = item[currentDestinationIndex].GetComponent<CheckPoint>().CheckPointID;
                Debug.Log("Starting ID : " + currentDestinationID);

                MoveBotTo(currentBotAgent.destination);
                GunState(true);
                isMoving = true;
                return true;
            }
            else
            {
                currentDestinationIndex = (currentDestinationIndex + 1) % checkpointHandler.GetWayPointList().Count;
                List<Transform> item = checkpointHandler.GetWayPointList();
                currentBotAgent.destination = item[currentDestinationIndex].position;
                BotRoot.transform.LookAt(currentBotAgent.destination);
                currentDestinationID = item[currentDestinationIndex].GetComponent<CheckPoint>().CheckPointID;
                Debug.Log("Destination ID : " + currentDestinationID);

                MoveBotTo(currentBotAgent.destination);
                return true;
            }

            return false;

        }

        private bool HasReachedDestination()
        {
            if (!currentBotAgent.pathPending && currentBotAgent.remainingDistance <= currentBotAgent.stoppingDistance)
            {
                return !currentBotAgent.hasPath || currentBotAgent.velocity.sqrMagnitude == 0f;
            }
            return false;
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

        private void UpdateAnimationParameters()
        {
            float speed = new Vector2(inputX, inputZ).sqrMagnitude;
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