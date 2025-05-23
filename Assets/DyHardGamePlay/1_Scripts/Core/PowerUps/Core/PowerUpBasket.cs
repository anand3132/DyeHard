using System;
using UnityEngine;
using UnityEngine.UI;
namespace RedGaint
{
    //Class Responsibility : To hold, activate and trigger powerUp...!! It's just a basket to hold power up!!
    public class PowerUpBasket : MonoBehaviour
    {
        [Header("Info---")]
         private GameObject  powerUpObjectPrefab;
         private GameObject currentPowerUp;
         private bool isPowerUpAvilable = false;
        [SerializeField] private GlobalEnums.PowerUpType currentpowerType;
        public GlobalEnums.PowerUpType CurrentPowerUpType=>currentpowerType;
        private static string POWERUPHOOK = "RF_PowerUps";
        public  Transform powerUphook;
        private void Awake()
        {
            powerUphook = Helper.FindDeepChild<Transform>(transform, POWERUPHOOK);
        }

        public bool ActivateCurrentPowerUp(GlobalEnums.PowerUpType powerUpType)
        {
            if (isPowerUpAvilable)
                return false;

            powerUpObjectPrefab = PowerUpManager.Instance.GetPowerUpPrefab(powerUpType);
            if (powerUpObjectPrefab != null && powerUpObjectPrefab.GetComponent<PowerUpHandle>().powerUpType == powerUpType)
            {
                // Instantiate the power-up
                currentPowerUp = Instantiate(powerUpObjectPrefab, powerUphook.position, Quaternion.identity,powerUphook);
                currentPowerUp.transform.parent = powerUphook;
                currentpowerType = powerUpType;
                // Rotate the power-up towards the player's forward direction
                var playerController = GetComponent<PlayerController>();
                if (playerController != null)
                {
                    Vector3 playerForward = playerController.transform.forward;
                    currentPowerUp.transform.rotation = Quaternion.LookRotation(playerForward);
                }
                else
                {
                    currentPowerUp.transform.rotation = Quaternion.identity; // Default rotation
                }

                if (powerUpType == GlobalEnums.PowerUpType.Bomb)
                {
                    currentPowerUp.GetComponent<BombTrigger>()
                        .SetBombFor(GetComponent<BaseCharacterController>().CurrentTeam);
                }
                // currentPowerUp.GetComponent<PowerUpHandle>().powerUpHolder = gameObject;

                // Update UI if the player holds this power-up
                if (playerController != null)
                {
                    InputHandler.Instance.SetPowerUpIcon(currentPowerUp.GetComponent<PowerUpHandle>().powerUpLogo);
                }

                currentPowerUp.SetActive(false);
                isPowerUpAvilable = true;
                return true;
            }
            return false;
        }

        public void ResetPowerUp()
        {
            // if(GetComponent<PlayerController>())
            //     InputHandler.Instance.SetPowerUpIcon();
            if (powerUphook != null)
            {
                foreach (Transform child in powerUphook)
                {
                    Destroy(child.gameObject);
                }
            }
            powerUpObjectPrefab = null;
            currentPowerUp=null;
            isPowerUpAvilable = false;
            
        }

        public bool IsPowerUpAvilable()=>isPowerUpAvilable;
        public bool TriggerPowerUp()
        {
            if (isPowerUpAvilable)
            {
                currentPowerUp.SetActive(true);
                currentPowerUp.GetComponent<PowerUpHandle>().TriggerPowerUp();
                //powerUp destruction is handled by powerUpHandle itself
                currentPowerUp = null;
                powerUpObjectPrefab = null;
                isPowerUpAvilable = false;
                return true;
            }
            return false;
        }
        private void OnDisable()
        {
            ResetPowerUp();
        }
    }
}