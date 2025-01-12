using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace RedGaint
{
   //This class which deliver the power to the one who hold it ..!!both powerups and players hold this class 
   public class PowerUpController : MonoBehaviour,IBugsBunny
   {
      public virtual bool LogThisClass { get; } = false;

      public GlobalEnums.PowerUpType powerUpType;
      public Sprite powerUpLogo;
      public UnityEvent powerUpTriggerEvents;
      public MovementInputSettings powerUpSettings;
      public MovementInputSettings currentSettings;
      public GameObject powerUpHolder;
      private IBugsBunny bugsBunnyImplementation;

      public void TriggerPowerUp()
      {
         StartCoroutine(OnPowerUp(10f));
      }

      public void ResetPowerUpController()
      {
         
      }
      private IEnumerator OnPowerUp(float delay)
      {
         powerUpTriggerEvents?.Invoke();
         // if (powerUpType == GlobalEnums.PowerUpType.Sprint)
         // {
         //    
         // }
         // currentSettings = powerUpHolder.GetComponent<PlayerController>().movementSettings;
         // if (currentSettings!=null)
         // {
         //    powerUpHolder.GetComponent<PlayerController>().movementSettings = powerUpSettings;
         // }
         BugsBunny.Log($"PowerUp {powerUpType} activated. Destroying in 5 seconds...",this);
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