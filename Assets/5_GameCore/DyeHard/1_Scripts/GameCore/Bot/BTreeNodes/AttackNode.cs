using UnityEngine;

namespace RedGaint.Games.DyeHard
{
    public class AttackNode : BTNode
    {
        private BotController bot;
        private float attackCooldown;
        private float lastAttackTime;
        private bool attackTriggered;
    
        public AttackNode(BotController bot, float attackCooldown = 1.5f)
        {
            this.bot = bot;
            this.attackCooldown = attackCooldown;
        }
    
        protected override BTStatus ExecuteNode()        {
            
            // Validate target
            if (bot.detectedPlayer == null)
            {
                Debug.Log("No player found");
                return BTStatus.Failure;
            }
    
            // Face target
            Vector3 direction = (bot.detectedPlayer.position - bot.transform.position).normalized;
            if (direction != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                bot.transform.rotation = Quaternion.Slerp(bot.transform.rotation, lookRotation, Time.deltaTime * 10f);
            }
    
            // Handle attack cooldown
            if (Time.time < lastAttackTime + attackCooldown)
            {
                Debug.Log("attack trigger (Time.time < lastAttackTime + attackCooldown)");
                return attackTriggered ? BTStatus.Running : BTStatus.Failure;
            }
    
            // Execute attack
            if (!attackTriggered)
            {
                StartAttack();
                Debug.Log("attackTriggered----");
                return BTStatus.Running;
            }
    
            // Complete attack
            if (Time.time >= lastAttackTime + 0.5f) // Attack duration
            {
                CompleteAttack();
                Debug.Log("Complete attack----");

                return BTStatus.Success;
            }
    
            Debug.Log("final...");
            return BTStatus.Running;
        }
    
        private void StartAttack()
        {
            lastAttackTime = Time.time;
            attackTriggered = true;
            
            // Animation
     //       bot.currentAnimtor.SetTrigger("Attack");
            
            // Effects
            bot.DropBomb();
            
            // Shooting
            bot.GunState(true);
        }
    
        private void CompleteAttack()
        {
            attackTriggered = false;
            bot.GunState(false);
            Debug.Log("<Color=red>-------------------Done Attacking-------------------</Color>");
        }
    }
}