using UnityEngine;

namespace RedGaint.Games.DyeHard
{
    public class IdleNode : BTNode
    {
        private BotController bot;
        private float duration;
        private float startTime;
        private bool isInitialized;

        public IdleNode(BotController bot, float duration)
        {
            this.bot = bot;
            this.duration = duration;
        }

        protected override BTStatus ExecuteNode()
        {
            if (!isInitialized)
            {
                startTime = Time.time;
                bot.currentBotAgent.isStopped = true;

                // Stop animation movement
                bot.SetIdleAnimation();

                isInitialized = true;
            }

            float elapsed = Time.time - startTime;

            if (elapsed >= duration)
            {
                bot.currentBotAgent.isStopped = false;
                return BTStatus.Success;
            }

            return BTStatus.Running;
        }
    }
}