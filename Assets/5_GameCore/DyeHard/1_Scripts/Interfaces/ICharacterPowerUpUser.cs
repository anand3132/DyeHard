using UnityEngine;

namespace RedGaint.Games.DyeHard
{
    public interface ICharacterPowerUpUser
    {
        float moveSpeed { get; set; }

        /// <summary>
        /// Called when a power-up starts affecting the character.
        /// </summary>
        void OnPowerUpTriggered(GlobalEnums.PowerUpType type, float duration, float multiplier);

        /// <summary>
        /// Called when a power-up finishes its effect.
        /// </summary>
        void OnPowerUpEnded(GlobalEnums.PowerUpType type);

        /// <summary>
        /// The team this character belongs to.
        /// </summary>
        GlobalEnums.GameTeam Team { get; }
    }
}