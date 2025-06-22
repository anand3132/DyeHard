using System.Collections;
using UnityEngine;

namespace RedGaint.Games.DyeHard
{
    public class BotLifeCycleController
    {
        private readonly BotController botController;
        private readonly GameObject deathEffect;
        private readonly GameObject spawnEffect;
        private readonly MonoBehaviour coroutineRunner;

        public BotLifeCycleController(BotController controller, Transform effectRoot, MonoBehaviour coroutineHost)
        {
            botController = controller;
            coroutineRunner = coroutineHost;

            // Locate effects
            deathEffect = Helper.FindDeepChild(effectRoot, "RF_DeathEffect")?.gameObject;
            spawnEffect = Helper.FindDeepChild(effectRoot, "RF_ReSpawrnEffect")?.gameObject;

            deathEffect?.SetActive(false);
            spawnEffect?.SetActive(false);
        }

        public void KillBot()
        {
            botController.StopAI();        
            ResetBotAgent();
            botController.Deactivate();   
            
            // Start reset after visual delay if needed
           coroutineRunner.StartCoroutine(WaitAndResetBot(0.1f));
        }

        private IEnumerator WaitAndResetBot(float delay)
        {
            yield return new WaitForSeconds(delay);
            BotGenerator.Instance?.ReturnToPool(botController);
        }

        public void ResetBotAgent()
        {
            // Reset AI and path
            botController.detectedPlayer = null;
            if (botController.currentBotAgent != null)
            {
                botController.currentBotAgent.enabled = false;
                botController.transform.position = botController.respawnPosition;
                botController.currentBotAgent.enabled = true;
            }
        }
    }
}
