using System;
using RedGaint;
using UnityEngine;

namespace RedGaint
{
    public class PowerUpBasket : MonoBehaviour
    {
        [SerializeField] GameObject  powerUpObjectPrefab;
        [SerializeField] GameObject currentPowerUp;
        [SerializeField] private bool isPowerUpAvilable = false;
        public bool ActivateCurrentPowerUp(GlobalEnums.PowerUpType powerUpType)
        {
            if (isPowerUpAvilable)
                return false;
            powerUpObjectPrefab= PowerUpManager.instance.GetPowerUp(powerUpType);
            if (powerUpObjectPrefab != null && powerUpObjectPrefab.GetComponent<PowerUpController>().powerUpType == powerUpType)
            {
                currentPowerUp=Instantiate(powerUpObjectPrefab,transform.position,Quaternion.identity);
                currentPowerUp.transform.parent = transform;
                currentPowerUp.SetActive(false);
                isPowerUpAvilable = true;
                return true;
            }
            return false;
        }

        public bool TriggerPowerUp()
        {
            if (isPowerUpAvilable)
            {
                currentPowerUp.SetActive(true);
                currentPowerUp.GetComponent<PowerUpController>().TriggerPowerUp();
                //powerUp destruction is handled by powerUp controller
                
                currentPowerUp = null;
                powerUpObjectPrefab = null;
                isPowerUpAvilable = false;
                return true;
            }
            return false;
        }

    }
}