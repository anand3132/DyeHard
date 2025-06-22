using System;
using System.Collections.Generic;
using UnityEngine;

namespace RedGaint.Games.DyeHard
{
    public class PositionHandler : SingletonSimple<PositionHandler>, IBugsBunny
    {
        [System.Serializable]
        public class PositionGroup
        {
            public GlobalEnums.CheckPoints type;
            public List<Transform> positions = new List<Transform>();
            [NonSerialized] public List<int> availableIndices = new List<int>();
            [NonSerialized] public int currentIndex = -1;
            
            public float totalWeight = 0f;
            public List<float> positionWeights = new List<float>();
        }

        [Header("Configuration")]
        public GlobalEnums.Mode defaultSpawnMode = GlobalEnums.Mode.Random;
        public bool autoInitialize = true;

        private Dictionary<GlobalEnums.CheckPoints, PositionGroup> typeToGroup = new Dictionary<GlobalEnums.CheckPoints, PositionGroup>();
        private bool isInitialized = false;
        private bool isInitializing = false;

        public bool LogThisClass { get; set; } = true;


        public void Initialize()
        {

            if (isInitialized || isInitializing) return;
            
            isInitializing = true;
            try
            {
                typeToGroup.Clear();
                CollectCheckPointsFromChildren();
                isInitialized = true;
                Debug.Log($"PositionHandler initialized with {typeToGroup.Count} position groups", this);
            }
            finally
            {
                isInitializing = false;
            }
        }

        private void CollectCheckPointsFromChildren()
        {
            CheckPoint[] allCheckPoints = GetComponentsInChildren<CheckPoint>(false);
            
            if (allCheckPoints.Length == 0)
            {
                BugsBunny.LogYellow("No CheckPoint components found in children!", this);
                return;
            }

            foreach (CheckPoint checkPoint in allCheckPoints)
            {
                if (checkPoint == null) continue;

                if (checkPoint.CheckPointTypes == null || checkPoint.CheckPointTypes.Count == 0)
                {
                    BugsBunny.LogYellow($"CheckPoint {checkPoint.name} has no types assigned!", this);
                    continue;
                }

                foreach (var cpType in checkPoint.CheckPointTypes)
                {
                    if (!typeToGroup.TryGetValue(cpType.type, out PositionGroup group))
                    {
                        group = new PositionGroup { type = cpType.type };
                        typeToGroup[cpType.type] = group;
                    }

                    group.positions.Add(checkPoint.transform);
                    group.positionWeights.Add(cpType.weight);
                    group.totalWeight += cpType.weight;
                }
            }

            // Initialize available indices directly without calling ResetAvailablePositions
            foreach (var group in typeToGroup.Values)
            {
                group.availableIndices.Clear();
                for (int i = 0; i < group.positions.Count; i++)
                {
                    group.availableIndices.Add(i);
                }

                if (defaultSpawnMode == GlobalEnums.Mode.Shuffle)
                {
                    ShuffleAvailableIndices(group);
                }
                group.currentIndex = -1;
            }
        }

        private void EnsureInitialized()
        {
            if (!isInitialized && !isInitializing && autoInitialize)
            {
                Initialize();
            }
        }

        #region Public API

        public void ResetAvailablePositions(GlobalEnums.CheckPoints type)
        {
            EnsureInitialized();
            if (!typeToGroup.TryGetValue(type, out var group)) 
            {
                BugsBunny.LogYellow($"No positions found for type {type}", this);
                return;
            }

            group.availableIndices.Clear();
            for (int i = 0; i < group.positions.Count; i++)
            {
                group.availableIndices.Add(i);
            }

            if (defaultSpawnMode == GlobalEnums.Mode.Shuffle)
            {
                ShuffleAvailableIndices(group);
            }
            group.currentIndex = -1;
        }

        public Transform GetNextPosition(GlobalEnums.CheckPoints type, GlobalEnums.Mode mode = GlobalEnums.Mode.Default)
        {
            EnsureInitialized();
            if (!typeToGroup.TryGetValue(type, out var group))
            {
                Debug.Log($"No positions registered for type {type}", this);
                return null;
            }

            int index = GetNextIndex(group, mode == GlobalEnums.Mode.Default ? defaultSpawnMode : mode);
            return index >= 0 ? group.positions[index] : null;
        }

        public Vector3 GetNextPositionVector(GlobalEnums.CheckPoints type, GlobalEnums.Mode mode = GlobalEnums.Mode.Default)
        {
            EnsureInitialized();
            var transform = GetNextPosition(type, mode);
            return transform != null ? transform.position : Vector3.zero;
        }

        public void MarkPositionAvailable(GlobalEnums.CheckPoints type, int index)
        {
            if (!typeToGroup.TryGetValue(type, out var group)) return;

            if (!group.availableIndices.Contains(index) && index >= 0 && index < group.positions.Count)
            {
                group.availableIndices.Add(index);
            }
        }

        public bool HasAvailablePositions(GlobalEnums.CheckPoints type)
        {
            return typeToGroup.TryGetValue(type, out var group) && group.availableIndices.Count > 0;
        }

        public int AvailablePositionsCount(GlobalEnums.CheckPoints type)
        {
            return typeToGroup.TryGetValue(type, out var group) ? group.availableIndices.Count : 0;
        }

        public List<Transform> GetAllPositions(GlobalEnums.CheckPoints type)
        {
            return typeToGroup.TryGetValue(type, out var group) ? group.positions : new List<Transform>();
        }

        public List<Vector3> GetAllPositionVectors(GlobalEnums.CheckPoints type)
        {
            var positions = new List<Vector3>();
            if (typeToGroup.TryGetValue(type, out var group))
            {
                foreach (var transform in group.positions)
                {
                    if (transform != null) positions.Add(transform.position);
                }
            }
            return positions;
        }

        #endregion

        #region Helper Methods

        private int GetNextIndex(PositionGroup group, GlobalEnums.Mode mode)
        {
            if (group.availableIndices.Count == 0)
            {
                BugsBunny.LogYellow($"No available positions for type {group.type}", this);
                return -1;
            }

            int chosenIndex;

            switch (mode)
            {
                case GlobalEnums.Mode.Random:
                    if (group.positionWeights.Count > 0 && group.totalWeight > 0)
                    {
                        float randomValue = UnityEngine.Random.Range(0f, group.totalWeight);
                        float cumulativeWeight = 0f;
                        for (int i = 0; i < group.availableIndices.Count; i++)
                        {
                            int actualIndex = group.availableIndices[i];
                            cumulativeWeight += group.positionWeights[actualIndex];
                            if (randomValue <= cumulativeWeight)
                            {
                                chosenIndex = i;
                                break;
                            }
                        }
                        chosenIndex = group.availableIndices.Count - 1;
                    }
                    else
                    {
                        chosenIndex = UnityEngine.Random.Range(0, group.availableIndices.Count);
                    }
                    break;

                case GlobalEnums.Mode.Sequence:
                case GlobalEnums.Mode.RoundRobin:
                    group.currentIndex = (group.currentIndex + 1) % group.availableIndices.Count;
                    chosenIndex = group.currentIndex;
                    break;

                case GlobalEnums.Mode.ReverseSequence:
                    group.currentIndex = (group.currentIndex - 1 + group.availableIndices.Count) % group.availableIndices.Count;
                    chosenIndex = group.currentIndex;
                    break;

                case GlobalEnums.Mode.Shuffle:
                    chosenIndex = 0;
                    break;

                default:
                    BugsBunny.LogYellow($"Unsupported spawn mode: {mode}. Defaulting to Random.", this);
                    chosenIndex = UnityEngine.Random.Range(0, group.availableIndices.Count);
                    break;
            }

            int indexToReturn = group.availableIndices[chosenIndex];
            group.availableIndices.RemoveAt(chosenIndex);
            return indexToReturn;
        }

        private void ShuffleAvailableIndices(PositionGroup group)
        {
            for (int i = 0; i < group.availableIndices.Count; i++)
            {
                int rand = UnityEngine.Random.Range(i, group.availableIndices.Count);
                (group.availableIndices[i], group.availableIndices[rand]) = (group.availableIndices[rand], group.availableIndices[i]);
            }
        }

        #endregion
    }
}