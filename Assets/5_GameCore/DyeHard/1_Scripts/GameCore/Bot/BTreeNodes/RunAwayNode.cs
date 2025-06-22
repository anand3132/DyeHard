using UnityEngine;

namespace RedGaint.Games.DyeHard
{
    public class RunAwayNode : BTNode
    {
        private BotController bot;
        private Vector3 runDirection;
        private float runDistance = 10f;

        public RunAwayNode(BotController bot)
        {
            this.bot = bot;
        }

        protected override BTStatus ExecuteNode()
        {
            if (bot.detectedPlayer == null) return BTStatus.Failure;

            if (runDirection == Vector3.zero)
            {
                runDirection = (bot.transform.position - bot.detectedPlayer.position).normalized;
                bot.DropBomb();
            }

            var targetPos = bot.transform.position + runDirection * runDistance;
            bot.currentBotAgent.SetDestination(targetPos);

            if (!bot.currentBotAgent.pathPending &&
                bot.currentBotAgent.remainingDistance <= bot.currentBotAgent.stoppingDistance)
            {
                runDirection = Vector3.zero;
                return BTStatus.Success;
            }

            return BTStatus.Running;
        }

    }

}//RedGaint