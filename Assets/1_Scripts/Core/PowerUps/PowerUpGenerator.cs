using System;
using System.Collections.Generic;
using UnityEngine;

namespace RedGaint
{
    public class PowerUpGenerator : MonoBehaviour
    {
        public GlobalEnums.Mode spawnMode;
        public GlobalEnums.PowerUpType selectedPowerUpType = GlobalEnums.PowerUpType.None;
        [Space]
        public List<Material> powerUpMaterials;
        public GameObject powerUpPrefab;

        private List<Transform> spawnPositions = new List<Transform>();
        private List<int> availablePositions = new List<int>();
        private int currentIndex = 0;

        // Dictionary to map each mode to its corresponding spawn method
        private Dictionary<GlobalEnums.Mode, Action> spawnActions;

        private void Start()
        {
            InitializeSpawnPositions();
            InitializeSpawnActions();

            // Execute the spawn action corresponding to the selected spawn mode
            if (spawnActions.TryGetValue(spawnMode, out Action spawnAction))
            {
                spawnAction.Invoke();
            }
            else
            {
                Debug.LogWarning($"Spawn mode {spawnMode} not supported.");
            }
        }

        private void InitializeSpawnPositions()
        {
            foreach (Transform child in transform)
            {
                spawnPositions.Add(child);
            }

            for (int i = 0; i < spawnPositions.Count; i++)
            {
                availablePositions.Add(i);
            }
        }

        private void InitializeSpawnActions()
        {
            spawnActions = new Dictionary<GlobalEnums.Mode, Action>
            {
                { GlobalEnums.Mode.Random, SpawnRandom },
                { GlobalEnums.Mode.Sequence, SpawnSequence },
                { GlobalEnums.Mode.ReverseSequence, SpawnReverseSequence },
                { GlobalEnums.Mode.RoundRobin, SpawnRoundRobin },
                { GlobalEnums.Mode.Shuffle, SpawnShuffle },
                { GlobalEnums.Mode.Stack, SpawnStack },
                { GlobalEnums.Mode.Cluster, SpawnCluster },
                { GlobalEnums.Mode.SingleShot, SpawnSingleShot },
                { GlobalEnums.Mode.DoubleShot, SpawnDoubleShot }
            };
        }

        private void SpawnRandom()
        {
            SpawnAtRandomPosition();
        }

        private void SpawnSequence()
        {
            for (int i = 0; i < spawnPositions.Count; i++)
            {
                SpawnNextPowerUp(currentIndex % availablePositions.Count);
                currentIndex++;
            }
        }

        private void SpawnReverseSequence()
        {
            for (int i = spawnPositions.Count - 1; i >= 0; i--)
            {
                SpawnNextPowerUp(i % availablePositions.Count);
            }
        }

        private void SpawnRoundRobin()
        {
            for (int i = 0; i < spawnPositions.Count; i++)
            {
                SpawnNextPowerUp(i % availablePositions.Count);
            }
        }

        private void SpawnShuffle()
        {
            ShuffleAvailablePositions();
            foreach (int posIndex in availablePositions)
            {
                SpawnNextPowerUp(posIndex);
            }
        }

        private void SpawnStack()
        {
            while (availablePositions.Count > 0)
            {
                int stackIndex = availablePositions[availablePositions.Count - 1];
                availablePositions.RemoveAt(availablePositions.Count - 1);
                SpawnNextPowerUp(stackIndex);
            }
        }

        private void SpawnCluster()
        {
            foreach (Transform spawnPoint in spawnPositions)
            {
                SpawnNextPowerUp(0); // Or use another fixed index if needed
            }
        }

        private void SpawnSingleShot()
        {
            if (spawnPositions.Count > 0)
            {
                SpawnNextPowerUp(0, selectedPowerUpType); // Pass the selected type
            }
            else
            {
                Debug.LogWarning("No spawn positions available for SingleShot mode.");
            }
        }


        private void SpawnDoubleShot()
        {
            SpawnNextPowerUp(0);
            if (spawnPositions.Count > 1)
            {
                SpawnNextPowerUp(1);
            }
        }

        private void SpawnAtRandomPosition()
        {
            int randomIndex = availablePositions[UnityEngine.Random.Range(0, availablePositions.Count)];
            SpawnNextPowerUp(randomIndex);
        }

        private void SpawnNextPowerUp(int spawnIndex)
        {
            if (spawnIndex < 0 || spawnIndex >= spawnPositions.Count)
                return;
        
            Transform spawnPoint = spawnPositions[spawnIndex];
            GameObject powerUpInstance = Instantiate(powerUpPrefab, transform);
            powerUpInstance.transform.position = spawnPoint.position;
        
            // Determine the power-up type dynamically
            GlobalEnums.PowerUpType powerUpType = GetPowerUpType(spawnIndex);
        
            PowerUp powerUpComponent = powerUpInstance.GetComponent<PowerUp>();
            powerUpComponent.Initialize(powerUpMaterials, spawnIndex, powerUpType);
            powerUpComponent.OnPowerUpConsumed += HandlePowerUpConsumed;
        
            // Remove this position from available positions
            availablePositions.Remove(spawnIndex);
        }

        private void SpawnNextPowerUp(int spawnIndex, GlobalEnums.PowerUpType specificType)
        {
            if (spawnIndex < 0 || spawnIndex >= spawnPositions.Count)
                return;

            Transform spawnPoint = spawnPositions[spawnIndex];
            GameObject powerUpInstance = Instantiate(powerUpPrefab, transform);
            powerUpInstance.transform.position = spawnPoint.position;

            PowerUp powerUpComponent = powerUpInstance.GetComponent<PowerUp>();
            if (powerUpComponent != null)
            {
                powerUpComponent.Initialize(powerUpMaterials, spawnIndex, specificType); // Use specific type
                powerUpComponent.OnPowerUpConsumed += HandlePowerUpConsumed;

                // Remove this position from available positions
                availablePositions.Remove(spawnIndex);
            }
            else
            {
                Debug.LogError("PowerUp prefab is missing the PowerUp component.");
            }
        }


        private GlobalEnums.PowerUpType GetPowerUpType(int spawnIndex)
        {
            // Example logic to determine power-up type based on spawnIndex or spawn mode
            switch (spawnMode)
            {
                case GlobalEnums.Mode.Random:
                    return (GlobalEnums.PowerUpType)UnityEngine.Random.Range(0,
                        Enum.GetValues(typeof(GlobalEnums.PowerUpType)).Length);
                case GlobalEnums.Mode.Sequence:
                    return (GlobalEnums.PowerUpType)
                        (spawnIndex % Enum.GetValues(typeof(GlobalEnums.PowerUpType)).Length);
                case GlobalEnums.Mode.RoundRobin:
                    currentIndex = (currentIndex + 1) % Enum.GetValues(typeof(GlobalEnums.PowerUpType)).Length;
                    return (GlobalEnums.PowerUpType)currentIndex;
                default:
                    // Fallback to Random type if mode is not explicitly handled
                    return GlobalEnums.PowerUpType.Sprint;
            }
        }


        private void HandlePowerUpConsumed(int positionIndex)
        {
            // Re-add the consumed power-up's position index to availablePositions
            if (!availablePositions.Contains(positionIndex))
            {
                availablePositions.Add(positionIndex);
            }

            // Check if there are still available positions
            if (availablePositions.Count > 0)
            {
                int nextIndex = GetSpawnPosition();

                // If in SingleShot mode, ensure the selected power-up type is spawned
                if (spawnMode == GlobalEnums.Mode.SingleShot)
                {
                    SpawnNextPowerUp(nextIndex, selectedPowerUpType);
                }
                else
                {
                    // Default behavior for other modes
                    SpawnNextPowerUp(nextIndex);
                }
            }
        }



        private int GetSpawnPosition()
        {
            if (availablePositions.Count == 0)
            {
                Debug.LogWarning("No available positions to spawn power-ups.");
                return -1; // No valid positions
            }

            switch (spawnMode)
            {
                case GlobalEnums.Mode.Random:
                    return availablePositions[UnityEngine.Random.Range(0, availablePositions.Count)];

                case GlobalEnums.Mode.Sequence:
                    currentIndex = (currentIndex + 1) % availablePositions.Count;
                    return availablePositions[currentIndex];

                case GlobalEnums.Mode.ReverseSequence:
                    currentIndex = (currentIndex - 1 + availablePositions.Count) % availablePositions.Count;
                    return availablePositions[currentIndex];

                case GlobalEnums.Mode.Shuffle:
                    ShuffleAvailablePositions();
                    return availablePositions[0]; // Pick the first after shuffle

                case GlobalEnums.Mode.RoundRobin:
                    currentIndex = (currentIndex + 1) % availablePositions.Count;
                    return availablePositions[currentIndex];

                // case GlobalEnums.Mode.Cluster:
                //     // Example logic for cluster: Pick a nearby index
                //     int clusterBase = availablePositions[UnityEngine.Random.Range(0, availablePositions.Count)];
                //     return availablePositions.Find(pos => Mathf.Abs(pos - clusterBase) <= 1) ?? clusterBase;

                case GlobalEnums.Mode.SingleShot:
                    return availablePositions[0]; // Always return the first available position

                case GlobalEnums.Mode.DoubleShot:
                    // Example for DoubleShot: Pick two positions if available
                    return availablePositions.Count >= 2 ? availablePositions[1] : availablePositions[0];

                default:
                    Debug.LogWarning($"Unknown spawn mode: {spawnMode}. Defaulting to Random.");
                    return availablePositions[UnityEngine.Random.Range(0, availablePositions.Count)];
            }
        }

        private void ShuffleAvailablePositions()
        {
            for (int i = 0; i < availablePositions.Count; i++)
            {
                int randomIndex = UnityEngine.Random.Range(0, availablePositions.Count);
                (availablePositions[i], availablePositions[randomIndex]) = (availablePositions[randomIndex], availablePositions[i]);
            }
        }
    }//PowerUpGenerator
}//RedGaint
