using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace RedGaint.Games.DyeHard
{
    public class BotMovementController
    {
        private readonly NavMeshAgent agent;
        private readonly Transform transform;

        private List<Vector3> patrollingPath;
        private int currentDestinationIndex = 0;
        private bool isPaused = false;

        public BotMovementController(NavMeshAgent agent, Transform transform)
        {
            this.agent = agent;
            this.transform = transform;
        }

        public void SetMovementSpeed(float speed)
        {
            if (agent != null)
                agent.speed = speed;
        }

        public void SetPath(List<Vector3> path)
        {
            patrollingPath = path;
            currentDestinationIndex = 0;
        }

        public void Resume()
        {
            isPaused = false;
            if (agent != null) agent.isStopped = false;
        }

        public void Pause()
        {
            isPaused = true;
            if (agent != null) agent.isStopped = true;
        }

        public void Stop()
        {
            if (agent != null) agent.isStopped = true;
        }

        public void MoveTo(Vector3 destination)
        {
            if (agent != null && !isPaused)
            {
                agent.SetDestination(destination);
                agent.isStopped = false;
            }
        }

        public void MoveToNextPatrolPoint()
        {
            if (patrollingPath == null || patrollingPath.Count == 0) return;

            MoveTo(patrollingPath[currentDestinationIndex]);

            currentDestinationIndex = (currentDestinationIndex + 1) % patrollingPath.Count;
        }

        public bool HasReachedDestination(float threshold = 0.2f)
        {
            if (agent == null || !agent.hasPath || agent.pathPending) return false;

            return agent.remainingDistance <= threshold;
        }

        public Vector3 GetCurrentPosition()
        {
            return transform.position;
        }

        public float GetCurrentSpeed()
        {
            return agent != null ? agent.velocity.magnitude : 0f;
        }

        public void Tick()
        {
            if (agent != null && agent.velocity.sqrMagnitude > 0.01f)
            {
              //  Debug.Log("Bot Velocity: " + agent.velocity.magnitude.ToString("F2"));
            }
        }

    }
}
