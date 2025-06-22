using System;
using JetBrains.Annotations;
using UnityEngine;

namespace RedGaint.Games.DyeHard
{
    public class PowerUpManager : SingletonSimple<PowerUpManager>, IBugsBunny
    {
        public bool LogThisClass { get; } = true;
        public GameObject[] powerUps;
        [CanBeNull]
        public GameObject GetPowerUpPrefab(GlobalEnums.PowerUpType _powerUpType)
        {
            foreach (var item in powerUps)
            {
                if (item.GetComponent<PowerUpHandle>().powerUpType == _powerUpType)
                {
                    return item;
                }
            }
            string typeName = Enum.IsDefined(typeof(GlobalEnums.PowerUpType), _powerUpType)
                ? _powerUpType.ToString()
                : $"Unknown({_powerUpType})";

            Debug.LogError($"Can't get the specified PowerUp prefab of type: {typeName}", this);
            return null;
        }
        
    }
}//RedGaint