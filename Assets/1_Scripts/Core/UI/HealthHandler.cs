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
        public float currentHealth;

        [Header("Blink Settings")] public Color fullHealthColor = Color.green;
        public Color halfHealthColor = Color.yellow;
        public Color lowHealthColor = Color.red;
        public float blinkSpeed = 2f; 
        public Transform healthBar;
        private bool isBlinking = false;
        private const string HEALTHBAR = "RF_HealthBar";

        void Start()
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

        void Update()
        {
            if (activeCamera != null)
            {
                // Rotate the health bar to look at the active camera
                Vector3 direction = activeCamera.transform.position - healthBar.position;
                direction.y = 0; // Keep the health bar upright
                healthBar.rotation = Quaternion.LookRotation(-direction);
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
            if (healthBarFill != null)
            {
                if (currentHealth / maxHealth > 0.8f)
                {
                    healthBarFill.color = fullHealthColor;
                    StopBlinking();
                }
                else if (currentHealth / maxHealth > 0.7f)
                {
                    healthBarFill.color = halfHealthColor;
                    StopBlinking();
                }
                else if (currentHealth / maxHealth > 0.4f)
                {
                    healthBarFill.color = lowHealthColor;
                    StopBlinking();
                }
                else if (currentHealth / maxHealth < 0.3f)
                {
                    healthBarFill.color = lowHealthColor;
                    StartBlinking();
                }
            }
        }

        /// <summary>
        /// Reduces the health by a given amount and updates the health bar.
        /// </summary>
        /// <param name="damageAmount">Amount of damage taken.</param>
        public void TakeDamage(float damageAmount)
        {
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
            StopBlinking();
            GetComponent<BaseCharacterController>().KillTheActor();
            // Add death handling logic here (e.g., disable the health bar, trigger animations, etc.)
        }

        /// <summary>
        /// Starts the blinking effect on the health bar when health is critically low.
        /// </summary>
        private void StartBlinking()
        {
            if (!isBlinking)
            {
                isBlinking = true;
                StartCoroutine(BlinkHealthBar());
            }
        }

        /// <summary>
        /// Stops the blinking effect on the health bar.
        /// </summary>
        private void StopBlinking()
        {
            isBlinking = false;
            StopAllCoroutines();
            if (healthBarFill != null)
            {
                healthBarFill.color = lowHealthColor;
            }
        }

        /// <summary>
        /// Coroutine to blink the health bar fill color.
        /// </summary>
        private System.Collections.IEnumerator BlinkHealthBar()
        {
            while (isBlinking)
            {
                healthBarFill.color = Color.clear; // Transparent
                yield return new WaitForSeconds(1f / blinkSpeed);
                healthBarFill.color = lowHealthColor; // Low health color
                yield return new WaitForSeconds(1f / blinkSpeed);
            }
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