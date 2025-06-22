using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RedGaint.Games.DyeHard
{
    public class PowerUpGenerator : MonoBehaviour
    {
        [Header("Spawn Settings")]
        [Tooltip("How many powerups will be generated initially")]
        public int powerUpBatchCount = 5;

        [Tooltip("How the placement of each powerup will be chosen")]
        public GlobalEnums.Mode positionSelectionMode = GlobalEnums.Mode.Random;

        [Header("Type Settings")]
        [Tooltip("Which order the power-ups will be spawned in")]
        public GlobalEnums.PowerUpSelectionMode spawningMode = GlobalEnums.PowerUpSelectionMode.Random;

        [Tooltip("If Specific mode selected, specify the type of power-up to spawn")]
        public GlobalEnums.PowerUpType specificType = GlobalEnums.PowerUpType.None;

        [Tooltip("If Limited mode selected, specify the max count of power-ups to spawn")]
        public int totalPowerupCount = 1;

        [Tooltip("How many times to retry spawning if no valid point is found")]
        public int maxSpawnRetryPerPowerUp = 3;

        [Tooltip("All the available power-up prefabs")]
        public List<PowerUp> powerUpPrefabs;

        // Internal
        private PowerUpSelector selector;
        private List<PowerUp> activePowerUps = new List<PowerUp>();
        private Transform lastSpawnPoint;
        private int spawnedCount = 0;

        private void Start()
        {
            if (powerUpPrefabs == null || powerUpPrefabs.Count == 0)
            {
                Debug.LogError("No power-up prefabs assigned!");
                return;
            }

            InitializeGenerator();
        }

        private void InitializeGenerator()
        {
            if (PositionHandler.Instance == null)
            {
                Debug.LogError("PositionHandler instance not found!");
                return;
            }

            // Mark all children as power-up checkpoints
            CheckPoint[] childCheckPoints = GetComponentsInChildren<CheckPoint>(true);
            foreach (var child in childCheckPoints)
            {
                var checkPoint = child.GetComponent<CheckPoint>() ?? child.gameObject.AddComponent<CheckPoint>();
                if (!checkPoint.HasType(GlobalEnums.CheckPoints.PowerUPPoint))
                {
                    checkPoint.AddType(new CheckPointType(GlobalEnums.CheckPoints.PowerUPPoint));
                }
            }

            PositionHandler.Instance.Initialize();

            int availableCount = PositionHandler.Instance.AvailablePositionsCount(GlobalEnums.CheckPoints.PowerUPPoint);
            powerUpBatchCount = Mathf.Min(powerUpBatchCount, Mathf.Max(1, availableCount - 1));

            selector = new PowerUpSelector(powerUpPrefabs, spawningMode, specificType);

            SpawnPowerUps(powerUpBatchCount);
        }

        private void SpawnPowerUps(int count)
        {
            int attempts = 0;
            int successfullySpawned = 0;

            while (successfullySpawned < count && attempts < count * maxSpawnRetryPerPowerUp)
            {
                if (spawningMode == GlobalEnums.PowerUpSelectionMode.Limited && spawnedCount >= totalPowerupCount)
                    break;

                Transform spawnPoint = GetValidSpawnPoint();

                if (spawnPoint != null)
                {
                    SpawnSinglePowerUp(spawnPoint);
                    spawnedCount++;
                    successfullySpawned++;
                }
                else
                {
                    Debug.LogWarning("Failed to find valid spawn point. Retrying...");
                }

                attempts++;
            }

            if (successfullySpawned < count)
            {
                Debug.LogWarning($"Only spawned {successfullySpawned}/{count} power-ups after {attempts} attempts.");
            }
        }
        private int lastUsedSpawnIndex = -1;

        private Transform GetValidSpawnPoint()
        {
            if (!PositionHandler.Instance.HasAvailablePositions(GlobalEnums.CheckPoints.PowerUPPoint))
                return null;

            int attempts = 0;
            const int maxAttempts = 10;

            while (attempts < maxAttempts)
            {
                var spawnPoint = PositionHandler.Instance.GetNextPosition(
                    GlobalEnums.CheckPoints.PowerUPPoint,
                    positionSelectionMode
                );

                if (spawnPoint == null)
                {
                    attempts++;
                    continue;
                }

                // Check if the selected point was the last used
                var allPoints = PositionHandler.Instance.GetAllPositions(GlobalEnums.CheckPoints.PowerUPPoint);
                int index = allPoints.IndexOf(spawnPoint);

                if (index == lastUsedSpawnIndex)
                {
                    // Re-add to available list and try again
                    PositionHandler.Instance.MarkPositionAvailable(GlobalEnums.CheckPoints.PowerUPPoint, index);
                    attempts++;
                    continue;
                }

                return spawnPoint;
            }

            return null;
        }


        private void SpawnSinglePowerUp(Transform spawnPoint)
        {
            PowerUp prefab = selector.GetNext();
            if (prefab == null)
            {
                Debug.LogWarning("PowerUp prefab is null or exhausted by selector.");
                return;
            }

            var allPoints = PositionHandler.Instance.GetAllPositions(GlobalEnums.CheckPoints.PowerUPPoint);
            if (allPoints.Count < powerUpBatchCount)
            {
                Debug.LogWarning("Not enough power-up spawn points!");
            }

            int spawnIndex = allPoints.IndexOf(spawnPoint);

            GameObject instance = Instantiate(prefab.gameObject, spawnPoint.position, Quaternion.identity, transform);
            PowerUp powerUp = instance.GetComponent<PowerUp>();
            powerUp.Initialize(spawnIndex);
            powerUp.OnPowerUpConsumed += OnPowerUpConsumed;

            activePowerUps.Add(powerUp);
            lastSpawnPoint = spawnPoint;
        }

        private void OnPowerUpConsumed(int index)
        {
            PositionHandler.Instance.MarkPositionAvailable(GlobalEnums.CheckPoints.PowerUPPoint, index);

            if (spawningMode == GlobalEnums.PowerUpSelectionMode.Limited)
                spawnedCount--; // allow re-spawning within the limit

            StartCoroutine(DelayedRespawn(1f));
        }

        private IEnumerator DelayedRespawn(float delay)
        {
            yield return new WaitForSeconds(delay);

            if (this != null)
            {
                if (spawningMode != GlobalEnums.PowerUpSelectionMode.Limited || spawnedCount < totalPowerupCount)
                {
                    SpawnPowerUps(1);
                }
            }
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
            foreach (var powerUp in activePowerUps)
            {
                if (powerUp != null)
                {
                    powerUp.OnPowerUpConsumed -= OnPowerUpConsumed;
                }
            }
            activePowerUps.Clear();
        }
    }
}
