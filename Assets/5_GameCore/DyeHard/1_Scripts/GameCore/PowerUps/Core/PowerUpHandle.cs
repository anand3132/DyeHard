using System;
using UnityEngine;
using UnityEngine.Events;

namespace RedGaint.Games.DyeHard
{
    /// <summary>
    /// This class holds the metadata and logic reference for a power-up prefab instance.
    /// It links the MonoBehaviour-based IPowerUp implementation to the game object
    /// and triggers its lifecycle.
    /// </summary>
    public class PowerUpHandle : MonoBehaviour
    {
        [Header("Power-Up Meta")]
        public GlobalEnums.PowerUpType powerUpType;
        public Sprite powerUpLogo;
        public UnityEvent powerUpTriggerEvents;

        private IPowerUp powerUpLogic;
        public bool IsActive { get; private set; }

        /// <summary>
        /// Initializes the power-up logic by injecting the owning character and preparing the logic layer.
        /// </summary>
        /// <param name="owner">The GameObject holding the PowerUpBasket (e.g. Player or Bot)</param>
        public void Initialize(GameObject owner)
        {
            var user = owner.GetComponent<ICharacterPowerUpUser>();
            if (user == null)
            {
                Debug.LogError($"PowerUpHandle could not find ICharacterPowerUpUser on {owner.name}");
                return;
            }

            powerUpLogic = GetComponent<IPowerUp>();
            if (powerUpLogic == null)
            {
                Debug.LogError($"PowerUpHandle requires an IPowerUp component on the same GameObject. Missing on: {gameObject.name}");
                return;
            }

            if (powerUpLogic is IInjectUser injectable)
            {
                injectable.SetUser(user);
            }

            powerUpLogic.Initialize();
        }

        /// <summary>
        /// Triggers the power-up logic.
        /// </summary>
        public void TriggerPowerUp()
        {
            if (IsActive || powerUpLogic == null)
                return;

            IsActive = true;
            powerUpTriggerEvents?.Invoke();
            powerUpLogic.Activate();
        }

        /// <summary>
        /// Cleanup logic when the power-up ends or is cancelled.
        /// </summary>
        public void Cleanup()
        {
            if (!IsActive || powerUpLogic == null)
                return;

            powerUpLogic.Cleanup();
            IsActive = false;
        }
    }
}
