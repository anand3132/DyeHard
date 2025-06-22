using System;
using RedGaint.Games.DyeHard.UI;
using UnityEngine;

namespace RedGaint.Games.DyeHard
{
    /// <summary>
    /// Class Responsibility: To hold, activate and trigger power-ups.
    /// It simply acts as a basket to attach and manage one power-up at a time.
    /// </summary>
    public class PowerUpBasket : MonoBehaviour
    {
        [Header("Power-Up State")]
        private GameObject powerUpPrefab;
        private GameObject currentPowerUp;
        private bool isPowerUpAvailable = false;

        [SerializeField] private GlobalEnums.PowerUpType currentPowerType;
        public GlobalEnums.PowerUpType CurrentPowerUpType => currentPowerType;

        private static readonly string POWERUP_HOOK_NAME = "RF_PowerUps";
        private Transform powerUpHook;

        private void Awake()
        {
            powerUpHook = Helper.FindDeepChild<Transform>(transform, POWERUP_HOOK_NAME);
        }

        /// <summary>
        /// Attach a new power-up to the player or bot.
        /// </summary>
        public bool ActivateCurrentPowerUp(GlobalEnums.PowerUpType powerUpType)
        {
            if (isPowerUpAvailable)
                return false;

            powerUpPrefab = PowerUpManager.Instance.GetPowerUpPrefab(powerUpType);
            if (powerUpPrefab == null || powerUpPrefab.GetComponent<PowerUpHandle>()?.powerUpType != powerUpType)
                return false;

            currentPowerUp = Instantiate(powerUpPrefab, powerUpHook.position, Quaternion.identity, powerUpHook);
            currentPowerUp.transform.localRotation = Quaternion.identity;

            currentPowerType = powerUpType;

            // Set forward rotation if it's a player
            if (TryGetComponent<PlayerController>(out var player))
            {
                currentPowerUp.transform.rotation = Quaternion.LookRotation(player.transform.forward);
            }

            // Initialize power-up handle with owner context
            var handle = currentPowerUp.GetComponent<PowerUpHandle>();
            handle.Initialize(gameObject);

            // Special-case: set bomb color
            if (powerUpType == GlobalEnums.PowerUpType.Bomb)
            {
                var bomb = currentPowerUp.GetComponent<BombPowerUp>();
                bomb?.SetBombFor(GetComponent<BaseCharacterController>().CurrentTeam);
            }

            // UI hook for players
            if (player != null &&
                UXController.Instance.GetCurrentScreen() == UIScreen.HUD &&
                UXController.Instance.TryGetScreen<HUDScreenUI>(UIScreen.HUD, out var hudUI))
            {
                hudUI.SetPowerUpIcon(handle.powerUpLogo);
            }

            currentPowerUp.SetActive(false);
            isPowerUpAvailable = true;
            return true;
        }

        /// <summary>
        /// Triggers and activates the currently held power-up.
        /// </summary>
        public bool TriggerPowerUp()
        {
            if (!isPowerUpAvailable || currentPowerUp == null)
                return false;

            currentPowerUp.SetActive(true);
            var handle = currentPowerUp.GetComponent<PowerUpHandle>();
            handle?.TriggerPowerUp();

            currentPowerUp = null;
            powerUpPrefab = null;
            isPowerUpAvailable = false;

            return true;
        }

        /// <summary>
        /// Resets and clears the held power-up.
        /// </summary>
        public void ResetPowerUp()
        {
            if (powerUpHook != null)
            {
                foreach (Transform child in powerUpHook)
                {
                    Destroy(child.gameObject);
                }
            }

            powerUpPrefab = null;
            currentPowerUp = null;
            isPowerUpAvailable = false;
        }

        public bool IsPowerUpAvilable() => isPowerUpAvailable;

        private void OnDisable()
        {
            ResetPowerUp();
        }
    }
}
