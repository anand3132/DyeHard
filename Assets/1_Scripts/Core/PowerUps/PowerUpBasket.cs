using UnityEngine;
using UnityEngine.UI;
namespace RedGaint
{
    //Class Responsibility : To hold, activate and trigger powerUp...!! It's just a basket to hold power up!!
    public class PowerUpBasket : MonoBehaviour
    {
        [Header("Info---")]
        [SerializeField] private GameObject  powerUpObjectPrefab;
        [SerializeField] private GameObject currentPowerUp;
        [SerializeField] private bool isPowerUpAvilable = false;
        public bool ActivateCurrentPowerUp(GlobalEnums.PowerUpType powerUpType)
        {
            if (isPowerUpAvilable)
                return false;

            powerUpObjectPrefab = PowerUpManager.instance.GetPowerUpPrefab(powerUpType);
            if (powerUpObjectPrefab != null && powerUpObjectPrefab.GetComponent<PowerUpController>().powerUpType == powerUpType)
            {
                // Instantiate the power-up
                currentPowerUp = Instantiate(powerUpObjectPrefab, transform.position, Quaternion.identity);
                currentPowerUp.transform.parent = transform;

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

                currentPowerUp.GetComponent<PowerUpController>().powerUpHolder = gameObject;

                // Update UI if the player holds this power-up
                if (playerController != null)
                {
                    InputHandler.instance.powerUpButtonObject.GetComponent<Image>().color = Color.white;
                    InputHandler.instance.powerUpButtonObject.GetComponent<Image>().sprite =
                        currentPowerUp.GetComponent<PowerUpController>().powerUpLogo;
                }

                currentPowerUp.SetActive(false);
                isPowerUpAvilable = true;
                return true;
            }
            return false;
        }

        public bool IsPowerUpAvilable()=>isPowerUpAvilable;
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