using UnityEngine;

public class PowerUpBasket : MonoBehaviour
{
    public PowerUpBase[] availablePowerUps;

    [Header("Bounce Settings")]
    public float bounceHeight = 0.5f;       // The height of the bounce
    public float bounceSpeed = 2.0f;        // The speed of the bounce

    [Header("Rotation Settings")]
    public float rotationSpeed = 50f;       // Rotation speed in degrees per second

    private Vector3 startPosition;

    private void Start()
    {
        // Store the initial position of the basket
        startPosition = transform.position;
    }

    private void Update()
    {
        // Bouncing effect
        float newY = startPosition.y + Mathf.Sin(Time.time * bounceSpeed) * bounceHeight;
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);

        // Rotation effect
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Grant a random power-up to the player upon collision
            int randomIndex = Random.Range(0, availablePowerUps.Length);
            PowerUpBase collectedPowerUp = Instantiate(availablePowerUps[randomIndex]);
            FindObjectOfType<PowerUpController>().CollectPowerUp(collectedPowerUp);

            // Destroy the power-up basket after collection
            Destroy(gameObject);
        }
    }
}
