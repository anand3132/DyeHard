using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BotController : MonoBehaviour
{
    private NavMeshAgent botAgent;
    public CheckPointHandler checkPointHandler;
    private Animator anim;
    private int currentDestinationIndex = 0;

    // Movement parameters
    public float attackRange = 2.0f;
    public float movementSpeed = 3.5f;
    private bool isMoving = true;
    private float inputX;
    private float inputZ;

    [Header("Animation Smoothing")]
    [Range(0, 1f)] public float horizontalAnimSmoothTime = 0.2f;
    [Range(0, 1f)] public float verticalAnimTime = 0.2f;
    [Range(0, 1f)] public float startAnimTime = 0.3f;
    [Range(0, 1f)] public float stopAnimTime = 0.15f;

    void Start()
    {
        botAgent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        botAgent.speed = movementSpeed;

        if (checkPointHandler != null && checkPointHandler.destinationPoints.Count > 0)
        {
            MoveToNextCheckpoint();
        }
    }

    void Update()
    {
        // Move to the next checkpoint if close enough to the current one
        if (isMoving && botAgent.remainingDistance < botAgent.stoppingDistance)
        {
            MoveToNextCheckpoint();
        }

        UpdateAnimationParameters();
    }
    protected bool isPathComplete()
    {
        if (Vector3.Distance(botAgent.destination, botAgent.transform.position) <= botAgent.stoppingDistance)
        {
            if (!botAgent.hasPath || botAgent.velocity.sqrMagnitude == 0f)
            {
                return true;
            }
        }

        return false;
    }

    private void MoveToNextCheckpoint()
    {
        // Check if there are any checkpoints in the handler
        if (checkPointHandler.destinationPoints.Count == 0)
        {
            Debug.LogWarning("No destination points set in CheckPointHandler.");
            return;
        }

        // Set the bot's destination if it has reached the previous checkpoint
        if (isPathComplete2())
        {
            Debug.Log("Bot reached destination.");
            currentDestinationIndex = (currentDestinationIndex + 1) % checkPointHandler.destinationPoints.Count;
            botAgent.destination = checkPointHandler.destinationPoints[currentDestinationIndex].position;
            BotMove(botAgent.destination);
            isMoving = true;
        }

        // Check if bot has arrived at the current destination
        if (HasReachedDestination())
        {
            isMoving = false;
            botAgent.isStopped = true;
            anim.SetFloat("Blend", 0); // Set animation to idle
            Debug.Log("Bot has reached the final destination and stopped.");
        }
    }

    // Method to check if bot has reached the current destination
    private bool HasReachedDestination()
    {
        if (!botAgent.pathPending && botAgent.remainingDistance <= botAgent.stoppingDistance)
        {
            return !botAgent.hasPath || botAgent.velocity.sqrMagnitude == 0f;
        }
        return false;
    }

    // Checks if the current path is complete (i.e., at the last destination)
    private bool isPathComplete2()
    {
        return HasReachedDestination() && currentDestinationIndex == checkPointHandler.destinationPoints.Count - 1;
    }


    /// <summary>
    /// Moves the bot based on the provided direction vector (left, right, forward, backward).
    /// </summary>
    /// <param name="direction">A Vector4 containing directions for left, right, forward, and backward movements.</param>
    public void BotMove(Vector4 direction)
    {
        Vector3 moveDirection = Vector3.zero;

        if (direction.x > 0) moveDirection += transform.right;         // Right
        if (direction.x < 0) moveDirection -= transform.right;         // Left
        if (direction.z > 0) moveDirection += transform.forward;       // Forward
        if (direction.z < 0) moveDirection -= transform.forward;       // Backward

        inputX = direction.x;
        inputZ = direction.z;

        // Update NavMeshAgent to move based on direction
        botAgent.Move(moveDirection.normalized * movementSpeed * Time.deltaTime);
    }

    /// <summary>
    /// Simulates an attack in the given direction.
    /// </summary>
    /// <param name="direction">A Vector4 indicating the direction of the attack.</param>
    public void BotAttack(Vector4 direction)
    {
        Vector3 attackDirection = new Vector3(direction.x, 0, direction.z).normalized;

        if (attackDirection != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(attackDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5);
        }

        // Simulate an attack action
        anim.SetTrigger("Attack");
        Debug.Log("Bot is attacking in direction: " + attackDirection);
    }

    /// <summary>
    /// Updates the animation parameters for movement.
    /// </summary>
    [SerializeField] ParticleSystem inkParticle;

    public void GunState(bool status) {
        if (status)
        {
            inkParticle.Play();
        }
        else {
            inkParticle.Stop();
        }
    }
    private void UpdateAnimationParameters()
    {
        float speed = new Vector2(inputX, inputZ).sqrMagnitude;
        GunState(true);
        if (speed > 0.1f)
        {
            anim.SetFloat("Blend", speed, startAnimTime, Time.deltaTime);
            anim.SetFloat("X", inputX, startAnimTime / 3, Time.deltaTime);
            anim.SetFloat("Y", inputZ, startAnimTime / 3, Time.deltaTime);
        }
        else
        {
            anim.SetFloat("Blend", speed, stopAnimTime, Time.deltaTime);
            anim.SetFloat("X", inputX, stopAnimTime / 3, Time.deltaTime);
            anim.SetFloat("Y", inputZ, stopAnimTime / 3, Time.deltaTime);
        }
    }
}

