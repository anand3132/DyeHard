using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
namespace RedGaint
{
    public class DyeHardDebugWindow : MonoBehaviour
    {
        void Start()
        {
            DebugMenu.instance.AddButton("Spectate", SpectateNextTarget);
            DebugMenu.instance.AddButton("Toggle Invincibility", ToggleInvincibility);
            // DebugMenu.instance.AddButton("Spawn Enemy", SpawnEnemy);
        }
        
        #region Spectate
        public CinemachineCamera cinemachineFreeLook;
        public BotGenerator botGenerator;
        public PlayerController playerController;
        private int currentIndex = -1;
        private List<Transform> spectatableTargets = new List<Transform>();
        private void InitializeSpectatableTargets()
        {
            spectatableTargets.Clear();

            // Add player as the first target
            if (playerController != null)
            {
                spectatableTargets.Add(playerController.transform);
            }

            // Add all bots to the list
            foreach (GameObject bot in botGenerator.BotList)
            {
                if (bot != null)
                {
                    spectatableTargets.Add(bot.transform);
                }
            }
        }

        private void SpectateNextTarget()
        {
            RefreshSpectatableTargets();
            if (spectatableTargets.Count == 0)
            {
                BugsBunny.LogYellow("No targets available to spectate!");
                return;
            }

            // Move to the next target
            currentIndex = (currentIndex + 1) % spectatableTargets.Count;
            Transform targetToSpectate = spectatableTargets[currentIndex];

            if (targetToSpectate != null)
            {
                cinemachineFreeLook.LookAt = targetToSpectate;
                cinemachineFreeLook.Follow = targetToSpectate;
                BugsBunny.Log3($"Now spectating: {targetToSpectate.name}");
            }
        }

        // Call this method whenever the bot list changes or new bots are added
        public void RefreshSpectatableTargets()
        {
            InitializeSpectatableTargets();
        }
        #endregion
        private void ToggleInvincibility()
        {
            BugsBunny.Log3("Toggling invincibility...");
            // Add your invincibility toggle logic here
        }
        private void SpawnEnemy()
        {
            BugsBunny.Log3("Spawning an enemy...");
            // Add your enemy spawn logic here
        }
    }
}