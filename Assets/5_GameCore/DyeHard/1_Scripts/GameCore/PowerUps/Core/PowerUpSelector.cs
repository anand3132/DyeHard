using System.Collections.Generic;
using UnityEngine;

namespace RedGaint.Games.DyeHard
{
    public class PowerUpSelector
    {
        private readonly List<PowerUp> powerUps;
        private readonly GlobalEnums.PowerUpSelectionMode selectionMode;
        private readonly GlobalEnums.PowerUpType specificType;
        private int currentIndex = 0;

        public PowerUpSelector(List<PowerUp> available, GlobalEnums.PowerUpSelectionMode mode, GlobalEnums.PowerUpType specific = GlobalEnums.PowerUpType.None)
        {
            powerUps = available;
            selectionMode = mode;
            specificType = specific;
        }

        public PowerUp GetNext()
        {
            switch (selectionMode)
            {
                case GlobalEnums.PowerUpSelectionMode.Random:
                    return powerUps[Random.Range(0, powerUps.Count)];

                case GlobalEnums.PowerUpSelectionMode.Sequence:
                    var powerUp = powerUps[currentIndex % powerUps.Count];
                    currentIndex++;
                    return powerUp;

                case GlobalEnums.PowerUpSelectionMode.Specific:
                    return powerUps.Find(p => p.powerUpType == specificType);

                default:
                    Debug.LogWarning("Unknown PowerUpSelectionMode.");
                    return null;
            }
        }
    }
}
