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
        public bool isBotActive = false;
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

                patrolPoints=GetWayPointPositions(checkpointHandler.GetWayPointList());
                StartCoroutine(RunBotEngine());
                return true;


            }
            return false;
        }
        private IEnumerator RunBotEngine()
        {
            while (true)
            {
                // Move to the next patrol point
                Vector3 targetPosition = patrolPoints[currentTargetIndex];
                currentBotAgent.SetDestination(targetPosition);

                // Wait until the bot reaches the current destination
                yield return new WaitUntil(() =>
                    !currentBotAgent.pathPending && currentBotAgent.remainingDistance <= currentBotAgent.stoppingDistance);

                // Rotate to face the next patrol point
                int nextIndex = (currentTargetIndex + 1) % patrolPoints.Count;
                Vector3 directionToNextPoint = patrolPoints[nextIndex] - transform.position;
                Quaternion targetRotation = Quaternion.LookRotation(directionToNextPoint);

                float maxRotationTime = 2f; // Maximum time allowed for rotation
                float elapsedTime = 0f;

                while (Quaternion.Angle(transform.rotation, targetRotation) > 1f)
                {
                    // Smoothly rotate towards the target
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, currentBotAgent.angularSpeed * Time.deltaTime);

                    // Increment elapsed time
                    elapsedTime += Time.deltaTime;

                    // Break if rotation takes too long
                    if (elapsedTime > maxRotationTime)
                    {
                        Debug.LogWarning("Rotation taking too long. Snapping to target rotation.");
                        transform.rotation = targetRotation; // Snap to target rotation
                        break;
                    }

                    yield return null; // Wait until the next frame
                }

                // Ensure the bot faces the target exactly after rotation
                transform.rotation = targetRotation;

                // Update the target index for the next patrol point
                currentTargetIndex = nextIndex;
            }
        }


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