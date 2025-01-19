using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace RedGaint
{
   //This class which deliver the power to the one who hold it ..!!both powerups and players hold this class 
   public class PowerUpHandle : MonoBehaviour
   {
      public GlobalEnums.PowerUpType powerUpType;
      public Sprite powerUpLogo;
      public UnityEvent powerUpTriggerEvents;
      public void TriggerPowerUp()
      {
         StartCoroutine(OnPowerUp(10f));
      }
      private IEnumerator OnPowerUp(float delay)
      {
         powerUpTriggerEvents?.Invoke();
         yield return new WaitForSeconds(delay);
         Destroy(gameObject);
      }
   }

}//ResGaint