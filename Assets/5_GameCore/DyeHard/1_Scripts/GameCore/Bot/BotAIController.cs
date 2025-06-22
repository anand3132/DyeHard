using System;
using System.Collections;
using UnityEngine;

namespace RedGaint.Games.DyeHard
{
    public class BotAIController
    {
        private readonly BotController bot;
        private BTNode behaviorTree;
        private Coroutine behaviorTreeCoroutine;
        private float behaviorTickInterval = 0.2f;
        private bool shouldRunAway = false;

        private MonoBehaviour runner => bot; // used to run coroutines

        public BotAIController(BotController bot)
        {
            this.bot = bot;
            BuildBehaviorTree();
        }

        private void BuildBehaviorTree()
        {
            var idle = new IdleNode(bot, 5f);
            var patrol = new PatrolNode(bot);
            var detectEnemy = new DetectEnemyNode(bot, 30f, 360f);
            var followPlayer = new FollowPlayerNode(bot, bot.attackFromDistance);
            var runAway = new RunAwayNode(bot);
            var resumeCombat = new ResumeCombatNode(bot); // <-- new node
            var attack = new AttackNode(bot);

            var attackWithCounter = new BTConditionalCounterNode(attack, 5);
            attackWithCounter.nodeName = "AttackCounter";
            attackWithCounter.OnThresholdReached += () => shouldRunAway = true;

            var firstSequence = new BTParallel("1. Patrol & Detect ->", patrol, detectEnemy);
            var secondSequence = new BTSequence("2. Follow & attack ->", followPlayer, attackWithCounter);
    
            var thirdSequence = new BTSequence("3. Runnaway back to patrol →", runAway, resumeCombat, patrol);
            var runawayWithTimeout = new BTTimeout(thirdSequence, 15f);

            var combatSelector = new BTSelector(
                new BTSequence("EngageEnemySequence →", detectEnemy, secondSequence),
                firstSequence
            );
          //a  GameSequence
             behaviorTree = new BTSelector(
                 new BTCondition(() => shouldRunAway, runawayWithTimeout),
                 combatSelector
             );
            

        }


        public void StartAI()
        {
            if (behaviorTreeCoroutine == null)
            {
                bot.GunState(true);
                behaviorTreeCoroutine = runner.StartCoroutine(RunBehaviorTree());
            }
        }

        public void StopAI()
        {
            if (behaviorTreeCoroutine != null)
            {
                bot.GunState(false);
                runner.StopCoroutine(behaviorTreeCoroutine);
                behaviorTreeCoroutine = null;
            }
        }
        private IEnumerator RunBehaviorTree()
        {
            while (bot.IsModelActive)
            {
                behaviorTree?.Execute();
                yield return new WaitForSeconds(behaviorTickInterval);
            }
        }
    }
}
