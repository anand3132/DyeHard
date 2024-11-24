using System.Collections.Generic;
using UnityEngine;

public class PowerUpController : MonoBehaviour
{
    private Dictionary<string, PowerUpBase> collectedPowerUps = new Dictionary<string, PowerUpBase>();

    public void CollectPowerUp(PowerUpBase powerUp)
    {
        if (!collectedPowerUps.ContainsKey(powerUp.GetType().Name))
        {
            collectedPowerUps.Add(powerUp.GetType().Name, powerUp);
        }
    }

    public void ActivatePowerUp(string powerUpName)
    {
        if (collectedPowerUps.TryGetValue(powerUpName, out PowerUpBase powerUp))
        {
            powerUp.Activate();
        }
    }
}
