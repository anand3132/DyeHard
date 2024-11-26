using UnityEngine;
using System;
using System.Collections.Generic;


namespace RedGaint
{
    public class PowerUp : MonoBehaviour
    {
        public event Action<int> OnPowerUpConsumed;
        public PowerUpBase[] availablePowerUps;

        [Header("Bounce Settings")]
        public float bounceHeight = 0.5f;       // The height of the bounce
        public float bounceSpeed = 2.0f;        // The speed of the bounce

        [Header("Rotation Settings")]
        public float rotationSpeed = 50f;       // Rotation speed in degrees per second

        private Vector3 startPosition;
        int positionIndex;
        public GlobalEnums.PowerUpType powerUpType;
        public bool isActive=false;
        private List<Material> powerUpMaterials;

        public void Initialize( List<Material> _powerUpMaterials, int _positionIndex,GlobalEnums.PowerUpType  _powerUpType)
        {   
            positionIndex=_positionIndex;
            powerUpType = _powerUpType;
            powerUpMaterials = _powerUpMaterials;
            startPosition = transform.position;
            SetPowerUpType(_powerUpType);
            isActive = true;
            Debug.Log("PowerUp Initialized");
        }

        private void SetPowerUpType(GlobalEnums.PowerUpType powerUpType1)
        {
            // Ensure the provided power-up type has a corresponding material in the list
            if ((int)powerUpType < powerUpMaterials.Count)
            {
                // Get the Renderer component of the power-up game object
                Renderer renderer = GetComponent<Renderer>();

                if (renderer != null)
                {
                    // Set the material based on the power-up type
                    renderer.material = powerUpMaterials[(int)powerUpType];
                }
                else
                {
                    Debug.LogWarning("Renderer component not found on power-up object.");
                }
            }
            else
            {
                Debug.LogWarning("No material available for the specified power-up type.");
            }
        }

        public GlobalEnums.PowerUpType PowerUpType { get { return powerUpType; } private set { powerUpType = value; } }
        private void Update()
        {
            if(!isActive)
                return;
            // Bouncing effect
            float newY = startPosition.y + Mathf.Sin(Time.time * bounceSpeed) * bounceHeight;
            transform.position = new Vector3(startPosition.x, newY, startPosition.z);
            
            // Rotation effect
            transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.World);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<PlayerController>()||other.GetComponentInParent<BotController>())
            {
                if(!other.GetComponent<PowerUpBasket>().ActivateCurrentPowerUp(powerUpType))
                    return;
                // Trigger the event to notify the generator
                OnPowerUpConsumed?.Invoke(positionIndex);
                // Destroy the power-up after collection
                Destroy(gameObject);
            }
        }
    }//PowerUp
}//RedGaint
