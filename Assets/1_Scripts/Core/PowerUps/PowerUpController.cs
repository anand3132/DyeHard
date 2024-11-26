using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace RedGaint
{
   //This class which deliver the power to the one who hold it ..!!
   public class PowerUpController : MonoBehaviour
   {
      public GlobalEnums.PowerUpType powerUpType;
      public Sprite powerUpLogo;
      public UnityEvent powerUpTriggerEvents;
      public MovementInputSettings powerUpSettings;
      public MovementInputSettings currentSettings;
      public GameObject powerUpHolder;
      public void TriggerPowerUp()
      {
         StartCoroutine(OnPowerUp(10f));
      }

      private IEnumerator OnPowerUp(float delay)
      {
         powerUpTriggerEvents?.Invoke();
         // currentSettings = powerUpHolder.GetComponent<PlayerController>().movementSettings;
         // if (currentSettings!=null)
         // {
         //    powerUpHolder.GetComponent<PlayerController>().movementSettings = powerUpSettings;
         // }
         Debug.Log($"PowerUp {powerUpType} activated. Destroying in 5 seconds...");
         yield return new WaitForSeconds(delay);
         // if (currentSettings != null)
         // {
         //    powerUpHolder.GetComponent<PlayerController>().movementSettings = currentSettings;
         // }

         powerUpHolder = null;
         // Destroy this GameObject after the delay
         Destroy(gameObject);
      }
   }

}//ResGaint