
using UnityEngine;

namespace RedGaint.Games.DyeHard
{
    public class PatrolNode : BTNode
    {
        private BotController bot;
        private int currentWaypointIndex = 0;
        private bool initialized;

        public PatrolNode(BotController bot)
        {
            this.bot = bot;
        }

        protected override BTStatus ExecuteNode()
        {
            if (!initialized)
            {
                InitializePatrol();
                return BTStatus.Running;
            }

            if (bot.botPatrollingPath.Count == 0)
                return BTStatus.Failure;

            bot.currentBotAgent.SetDestination(bot.botPatrollingPath[currentWaypointIndex]);

            if (!bot.currentBotAgent.pathPending &&
                bot.currentBotAgent.remainingDistance <= bot.currentBotAgent.stoppingDistance)
            {
                currentWaypointIndex = (currentWaypointIndex + 1) % bot.botPatrollingPath.Count;
            }

            return BTStatus.Running;
        }

        private void InitializePatrol()
        {
            bot.GunState(true); 
            currentWaypointIndex = 0;
            initialized = true;

            if (bot.botPatrollingPath.Count > 0)
            {
                bot.currentBotAgent.SetDestination(bot.botPatrollingPath[0]);
            }

        }

    }
}//RedGaint