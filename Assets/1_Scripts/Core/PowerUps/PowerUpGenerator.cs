using System.Collections.Generic;
using RedGaint;
using UnityEngine;

namespace RedGaint
{
    public class PowerUpGenerator : MonoBehaviour
    {
        [SerializeField] private GlobalEnums.Mode spawnMode;
        [SerializeField] private GameObject[] spawnPositions; // Array of spawn position objects
        [SerializeField] private GameObject powerUpPrefab;    // Single prefab for all power-ups

        private List<int> availablePositions = new List<int>();
        private int currentIndex = 0;

        private void Start()
        {
            // Populate availablePositions with all spawn position indices
            for (int i = 0; i < spawnPositions.Length; i++)
            {
                availablePositions.Add(i);
            }
        
            SpawnNextPowerUp();
        }

        private void SpawnNextPowerUp()
        {
            if (availablePositions.Count == 0)
                return;

            int spawnIndex = GetSpawnPosition();
            GameObject spawnPoint = spawnPositions[spawnIndex];

            // Instantiate the single power-up prefab at the chosen spawn position
            GameObject powerUpInstance = Instantiate(powerUpPrefab, spawnPoint.transform.position, Quaternion.identity);

            // Initialize the PowerUp component and subscribe to its OnPowerUpConsumed event
            PowerUp powerUpComponent = powerUpInstance.GetComponent<PowerUp>();
            powerUpComponent.Initialize(spawnIndex);
            powerUpComponent.OnPowerUpConsumed += HandlePowerUpConsumed;

            availablePositions.Remove(spawnIndex);
        }

        private int GetSpawnPosition()
        {
            // Choose a spawn position based on the specified mode (e.g., Random)
            return availablePositions[Random.Range(0, availablePositions.Count)];
        }

        private void HandlePowerUpConsumed(int positionIndex)
        {
            // Re-add the consumed power-up’s position index to availablePositions
            availablePositions.Add(positionIndex);
        
            // Spawn the next power-up if needed
            SpawnNextPowerUp();
        }
    }
}
