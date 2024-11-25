using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace RedGaint
{
   public class PowerUpController : MonoBehaviour
   {
      public GlobalEnums.PowerUpType powerUpType;

      public void TriggerPowerUp()
      {
         StartCoroutine(OnPowerUp(5f));
      }

      private IEnumerator OnPowerUp(float delay)
      {
         Debug.Log($"PowerUp {powerUpType} activated. Destroying in 5 seconds...");
         yield return new WaitForSeconds(delay);
         Destroy(gameObject); // Destroy this GameObject after the delay
      }
   }
}//ResGaint