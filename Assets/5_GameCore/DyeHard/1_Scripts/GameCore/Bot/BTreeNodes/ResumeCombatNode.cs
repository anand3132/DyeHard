using UnityEngine;

namespace RedGaint.Games.DyeHard
{
    public class ResumeCombatNode : BTNode
    {
        private readonly BotController bot;
        private bool hasResumed = false;

        public ResumeCombatNode(BotController bot)
        {
            this.bot = bot;
        }

        protected override BTStatus ExecuteNode()
        {
            if (hasResumed) return BTStatus.Success;

            bot.GunState(true);
            Debug.Log("Resumed combat by re-enabling gun state", bot);
            hasResumed = true;

            return BTStatus.Success;
        }

        public override void Reset()
        {
            hasResumed = false;
        }
    }

}