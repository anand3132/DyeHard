using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Unity.Cinemachine;

namespace RedGaint
{
    public class HealthHandler : MonoBehaviour
    {
         private Camera activeCamera;
         private Slider healthBarSlider;
         private Image healthBarFill;

        [Header("Health Settings")] public float maxHealth =GlobalStaticVariables.PlayerMaxHealth;
        private float currentHealth;

        [Header("Blink Settings")] public Color fullHealthColor = Color.green;
        public Color halfHealthColor = Color.yellow;
        public Color lowHealthColor = Color.red;
        [Space]
        [Range(.5f,15f)]
        public float blinkSpeed = 5f; 
        
        private  Transform healthBar;
        private bool canBlink = false;
        private const string HEALTHBAR = "RF_HealthBar";
        private bool canTakeDamage = true;
        public  void InitializeHealthSystem()
        {
            // Initialize health
            currentHealth = maxHealth;
            
            healthBar=Helper.FindDeepChild(transform, HEALTHBAR);
            // Find the slider and fill image components
            healthBarSlider = GetComponentInChildren<Slider>();
            if (healthBarSlider == null)
            {
                BugsBunny.LogError("No Slider component found in children of the health bar object.");
                return;
            }

            healthBarFill = healthBarSlider.fillRect.GetComponent<Image>();
            if (healthBarFill == null)
            {
                BugsBunny.LogError("No Image component found in the slider's fill area.");
                return;
            }

            // Set the initial health bar value and color
            UpdateHealthBar();

            // Find the active Cinemachine camera
            var cinemachineBrain = Camera.main.GetComponent<CinemachineBrain>();
            if (cinemachineBrain != null)
            {
                activeCamera = cinemachineBrain.OutputCamera;
            }
            else
            {
                BugsBunny.LogYellow("No CinemachineBrain found on the main camera.");
            }
        }

        public void ResetHealth()
        {
            currentHealth=maxHealth;
            canTakeDamage = true;
            UpdateHealthBar();
        }

        void Update()
        {
            if (activeCamera != null)
            {
                // Rotate the health bar to look at the active camera
                Vector3 direction = activeCamera.transform.position - healthBar.position;
                direction.y = 0; // Keep the health bar upright
                healthBar.rotation = Quaternion.LookRotation(-direction);
            }

            if (canBlink)
            {
                // Blink the health bar fill color
                float alpha = Mathf.Abs(Mathf.Sin(Time.time * blinkSpeed)); // Calculate alpha using sine wave
                Color blinkColor = healthBarFill.color;
                blinkColor.a = alpha; // Set alpha for the blink effect
                healthBarFill.color = blinkColor; // Apply the color to the health bar fill
            }
        }


        /// <summary>
        /// Updates the health bar slider value and color based on current health.
        /// </summary>
        private void UpdateHealthBar()
        {
            if (healthBarSlider != null)
            {
                healthBarSlider.value = currentHealth / maxHealth;
            }
            UpdateHealthBarColor();
        }

        /// <summary>
        /// Updates the health bar fill color based on the current health percentage.
        /// </summary>
        private void UpdateHealthBarColor()
        {
            canBlink = false;
            if (healthBarFill != null)
            {
                if (currentHealth / maxHealth > 0.9f)
                {
                    healthBarFill.color = fullHealthColor;
                }
                else if (currentHealth / maxHealth > 0.8f)
                {
                    healthBarFill.color = halfHealthColor;
                }
                else if (currentHealth / maxHealth > 0.6f)
                {
                    healthBarFill.color = lowHealthColor;
                }
                else if (currentHealth / maxHealth < 0.4f)
                {
                    healthBarFill.color = lowHealthColor;
                    canBlink = true;
                }
            }
        }


        /// <summary>
        /// Reduces the health by a given amount and updates the health bar.
        /// </summary>
        /// <param name="damageAmount">Amount of damage taken.</param>
        public void TakeDamage(float damageAmount)
        {
            if(!canTakeDamage)
                return;
            currentHealth -= damageAmount;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
            UpdateHealthBar();

            if (currentHealth <=.1)
            {
                OnDeath();
            }
        }

        /// <summary>
        /// Increases the health by a given amount and updates the health bar.
        /// </summary>
        /// <param name="healAmount">Amount of health restored.</param>
        public void Heal(float healAmount)
        {
            currentHealth += healAmount;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
            UpdateHealthBar();
        }

        /// <summary>
        /// Handles logic when the character dies.
        /// </summary>
        private void OnDeath()
        {
            UpdateHealthBar();
            GetComponent<BaseCharacterController>().KillTheActor();
            // Add death handling logic here (e.g., disable the health bar, trigger animations, etc.)
        }

        public void OnPowerUpImpact(GlobalEnums.PowerUpType powerUpType,float duration=0f)
        {
            if (powerUpType==GlobalEnums.PowerUpType.Shield)
            {
                Heal(GlobalStaticVariables.PlayerMaxHealth);
                canTakeDamage = false;
                StartCoroutine(Freeeze(duration));
            }
        }

        private float powerOffsettime = 2f;
        IEnumerator Freeeze(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            canTakeDamage = true;
        }
        

        /// <summary>
        /// Updates the active camera when switching Cinemachine cameras.
        /// </summary>
        /// <param name="newCamera">The new active camera.</param>
        public void UpdateActiveCamera(Camera newCamera)
        {
            activeCamera = newCamera;
        }
    }
}