using JetBrains.Annotations;
using UnityEngine;

namespace RedGaint
{
    public class PowerUpManager : Singleton<PowerUpManager>
    {
        public GameObject[] powerUps;
        [CanBeNull]
        public GameObject GetPowerUpPrefab(GlobalEnums.PowerUpType _powerUpType)
        {
            foreach (var item in powerUps)
            {
                if (item.GetComponent<PowerUpController>().powerUpType == _powerUpType)
                {
                    return item;
                }
            }
            BugsBunny.LogError("Cant able to get the specified powerUp prefab of type :" +_powerUpType.ToString());
            return null;
        }
    }
}//RedGaint