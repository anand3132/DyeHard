namespace RedGaint.Games.DyeHard
{
    public class FollowPlayerNode : BTNode
    {
        private BotController bot;
        private float stoppingDistance;

        public FollowPlayerNode(BotController bot, float stoppingDistance)
        {
            this.bot = bot;
            this.stoppingDistance = stoppingDistance;
        }

        protected override BTStatus ExecuteNode()
        {
            if (bot.detectedPlayer == null)
                return BTStatus.Failure;

            var agent = bot.currentBotAgent;

            // Set destination and stopping distance if needed
            if (agent.destination != bot.detectedPlayer.position)
                agent.SetDestination(bot.detectedPlayer.position);

            if (!agent.hasPath || agent.pathPending)
                return BTStatus.Running;

            // Ensure agent uses correct stopping distance
            if (agent.stoppingDistance != stoppingDistance)
                agent.stoppingDistance = stoppingDistance;

            // Wait until bot reaches within the stop distance
            if (agent.remainingDistance > agent.stoppingDistance)
                return BTStatus.Running;

            if (agent.velocity.sqrMagnitude > 0.1f)
                return BTStatus.Running;

            return BTStatus.Success;
        }
    }
}