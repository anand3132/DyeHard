using RedGaint;
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

        //Position Parameter
        private int currentDestinationIndex = 0;
        private Vector3 NextDestination;

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
        public bool isBotActive=false;
        private Animator currentAnimtor;

        [SerializeField] private ParticleSystem inkParticle;

        public void InitialiseBot(Vector3 position,CheckPointHandler checkPointHandler)
        {
            if (BotRoot == null)
                BotRoot = gameObject.transform;
            BotRoot.transform.position = position;
            currentBotAgent = GetComponent<NavMeshAgent>();
            currentAnimtor = GetComponent<Animator>();
            currentBotAgent.speed = botSettings.movementSpeed;
            checkpointHandler= checkPointHandler;
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
  
        public bool ActivateBot()
        {
            if (isBotActive) { return false; } 
            
            if (checkpointHandler != null)// && checkpointHandler.destinationPoints.Count > 0)
            {
                MoveToNextCheckpoint();
                isBotActive = true;
                return true;
            }
            return false;
        }


        void Update()
        {
            if (!isBotActive)
                return;
            RunBotEngine();
        }

        private void RunBotEngine()
        {
            MoveBotTo(currentBotAgent.destination);

            // Move to the next checkpoint if close enough to the current one
            //if (isMoving && currentBotAgent.remainingDistance < currentBotAgent.stoppingDistance)
            //{
            //    MoveToNextCheckpoint();
            //}

            UpdateAnimationParameters();
        }

        private bool IsPathComplete()
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

        private void MoveToNextCheckpoint()
        {
            // Check if there are any checkpoints in the handler
            if (checkpointHandler.GetWayPointList().Count == 0)
            {
                Debug.LogWarning("No Way points Avilable.");
                return;
            }

            if(!isBotActive)
            {
                currentDestinationIndex = 0;
                currentBotAgent = GetComponent<NavMeshAgent>();
                var item = checkpointHandler.GetWayPointList();
                currentBotAgent.destination = item[currentDestinationIndex].position;
                MoveBotTo(currentBotAgent.destination);
                isMoving = true;
                return;
            }

            // Set the bot's destination if it has reached the previous checkpoint
            if (IsPathComplete())
            {
                Debug.Log("Bot reached destination.");
                currentDestinationIndex = (currentDestinationIndex + 1) % checkpointHandler.GetWayPointList().Count;
                var item = checkpointHandler.GetWayPointList();
                currentBotAgent.destination = item[currentDestinationIndex].position;
                MoveBotTo(currentBotAgent.destination);
                //isMoving = false;
                return;
            }

            // Check if bot has arrived at the current destination
            if (HasReachedDestination()|| !isMoving)
            {
                isMoving = false;
                currentBotAgent.isStopped = true;
                currentAnimtor.SetFloat("Blend", 0); // Set animation to idle
                Debug.Log("Bot has reached the final destination and stopped.");
            }

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

            // Update NavMeshAgent to move based on direction
            currentBotAgent.Move(moveDirection.normalized * botSettings.movementSpeed * Time.deltaTime);
        }
        private void UpdateAnimationParameters()
        {
            float speed = new Vector2(inputX, inputZ).sqrMagnitude;
            GunState(true);
            if (speed > 0.1f)
            {
                currentAnimtor.SetFloat("Blend", speed, startAnimTime, Time.deltaTime);
                currentAnimtor.SetFloat("X", inputX, startAnimTime / 3, Time.deltaTime);
                currentAnimtor.SetFloat("Y", inputZ, startAnimTime / 3, Time.deltaTime);
            }
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
            if(other.GetComponent<CheckPoint>() != null) {
                CheckPoint currentWayPoint = other.GetComponent<CheckPoint>();
               if(currentWayPoint.CheckPointType == GlobalEnums.CheckPointType.WayPoint)
                {
                    currentBotAgent.isStopped = true;
                }

            }

            Debug.Log("triggered with : "+other.gameObject.name);
        }
        public void OnCollisionEnter(Collision collision)
        {
            Debug.Log("on collition with : "+collision.gameObject.name);

        }

    }
}