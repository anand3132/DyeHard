using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace RedGaint {
    public class BotGenerator : MonoBehaviour
    {
        private CheckPointHandler checkpointHandler;
        private GlobalEnums.Mode spawnMode = GlobalEnums.Mode.Sequence;
        private List<Vector3> allSpawnPositions = new List<Vector3>();
        private int sequenceIndex = 0;
        private List<Vector3> shuffleList;
        private int roundRobinIndex = 0;
        private int reverseIndex;
        private bool IsGeneratorActive = false;
        public int createBot = 1;
        private List<GameObject> BotList=new List<GameObject>();
        public List<GameObject> BotPrefab;
        private void Start()
        {
            checkpointHandler = transform.root.GetComponentInChildren<CheckPointHandler>();
            if (checkpointHandler == null)
            {
                BugsBunny.LogError("BotGenerator: Cant able to fetch Core Component -> CheckPointHandler");
            }
            else
            {
                UpdateAllSpawnPoints();
                InitializeMode();
                IsGeneratorActive = true;
            }
            for (int i = 0; i < createBot; i++)
            {
                if (GetNewSpawnPosition(GlobalEnums.Mode.Random,out Vector3 posiion))
                {
                    GenerateNewBot(posiion, out GameObject bot);
                    BotList.Add(bot);
                }
            }
        }

        private void UpdateAllSpawnPoints()
        {
            allSpawnPositions.Clear();
            List<Transform> itemList = checkpointHandler.GetSpawnList();
            foreach (Transform t in itemList)
            {
                allSpawnPositions.Add(t.position);
            }
        }

        public void ResetGenerator()
        {
            allSpawnPositions.Clear();
        }

        private void InitializeMode()
        {
            switch (spawnMode)
            {
                case GlobalEnums.Mode.Shuffle:
                    shuffleList = new List<Vector3>(allSpawnPositions);
                    ShuffleList(shuffleList);
                    sequenceIndex = 0;
                    break;

                case GlobalEnums.Mode.Sequence:
                    sequenceIndex = 0;
                    break;

                case GlobalEnums.Mode.ReverseSequence:
                    reverseIndex = allSpawnPositions.Count - 1;
                    break;

                case GlobalEnums.Mode.RoundRobin:
                    roundRobinIndex = 0;
                    break;
            }
        }

        public void SwitchSpawnMode()
        {
            // Move to the next mode in the enum
            spawnMode = (GlobalEnums.Mode)(((int)spawnMode + 1) % Enum.GetValues(typeof(GlobalEnums.Mode)).Length);

            // Reinitialize mode-specific settings if necessary
            InitializeMode();

            BugsBunny.Log("Spawn mode switched to: " + spawnMode);
        }

        public void SwitchSpawnMode(GlobalEnums.Mode mode)
        {
            spawnMode = mode;
            InitializeMode();
            BugsBunny.Log("Spawn mode switched to: " + spawnMode);
        }
        public List<Vector3> GetModifiedPath(GlobalEnums.Mode mode, List<Vector3> modifiedList)
        {
            switch (mode)
            {
                case GlobalEnums.Mode.Random:
                    modifiedList.Sort((a, b) => Random.Range(-1, 2)); // Shuffle using random sorting
                    break;

                case GlobalEnums.Mode.Sequence:
                    // Already in sequence by default
                    break;

                case GlobalEnums.Mode.Stack:
                    modifiedList.Reverse(); // Reverse list for stack ordering
                    break;

                case GlobalEnums.Mode.Shuffle:
                    for (int i = 0; i < modifiedList.Count; i++)
                    {
                        int randomIndex = Random.Range(0, modifiedList.Count);
                        (modifiedList[i], modifiedList[randomIndex]) = (modifiedList[randomIndex], modifiedList[i]);
                    }
                    break;

                // case GlobalEnums.Mode.RoundRobin:
                //     modifiedList = new List<Vector3>(botCurrentPathNodes); // Repeat list indefinitely (sample case)
                //     break;

                case GlobalEnums.Mode.ReverseSequence:
                    modifiedList.Reverse(); // Reverse order
                    break;

                case GlobalEnums.Mode.Cluster:
                    int clusterSize = Mathf.Max(1, modifiedList.Count / 3); // Cluster items by reducing size
                    modifiedList = modifiedList.GetRange(0, clusterSize);
                    break;

                case GlobalEnums.Mode.SingleShot:
                    // No modification needed
                    break;

                case GlobalEnums.Mode.DoubleShot:
                    List<Vector3> doubleShotList = new List<Vector3>(modifiedList);
                    doubleShotList.AddRange(modifiedList); // Duplicate sequence
                    modifiedList = doubleShotList;
                    break;
            }

            return modifiedList;
        }
        
        public bool GetNewSpawnPosition(GlobalEnums.Mode pathMode,out Vector3 position)
        {
            if (allSpawnPositions.Count < 1)
            {
                position = Vector3.zero;
                return false;
            }

            switch (pathMode)
            {
                case GlobalEnums.Mode.Random:
                    position = allSpawnPositions[UnityEngine.Random.Range(0, allSpawnPositions.Count)];
                    break;

                case GlobalEnums.Mode.Sequence:
                    position = allSpawnPositions[sequenceIndex];
                    sequenceIndex = (sequenceIndex + 1) % allSpawnPositions.Count;
                    break;

                case GlobalEnums.Mode.Stack:
                    position = allSpawnPositions[allSpawnPositions.Count - 1];
                    allSpawnPositions.RemoveAt(allSpawnPositions.Count - 1);
                    break;

                case GlobalEnums.Mode.Shuffle:
                    if (sequenceIndex >= shuffleList.Count) ShuffleList(shuffleList);
                    position = shuffleList[sequenceIndex];
                    sequenceIndex = (sequenceIndex + 1) % shuffleList.Count;
                    break;

                case GlobalEnums.Mode.RoundRobin:
                    position = allSpawnPositions[roundRobinIndex];
                    roundRobinIndex = (roundRobinIndex + 1) % allSpawnPositions.Count;
                    break;

                case GlobalEnums.Mode.ReverseSequence:
                    position = allSpawnPositions[reverseIndex];
                    reverseIndex = reverseIndex > 0 ? reverseIndex - 1 : allSpawnPositions.Count - 1;
                    break;

                default:
                    position = Vector3.zero;
                    return false;
            }
            return true;
        }

        private void ShuffleList(List<Vector3> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                int randomIndex = UnityEngine.Random.Range(i, list.Count);
                (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
            }
        }

        private bool GenerateNewBot(Vector3 position, out GameObject bot)
        {
            bot = GameObject.Instantiate(BotPrefab[0], transform);
            bot.SetActive(true);
            if (bot == null)
                return false;
            var currentBotcontroller = bot.GetComponent<BotController>();
            List<Vector3> patrollingPath = new List<Vector3> { position };
            List<Vector3> tmpPath= GetModifiedPath(GlobalEnums.Mode.Random,checkpointHandler.GetWayPointPositions());
            patrollingPath.AddRange(tmpPath);
            currentBotcontroller.InitialiseBot(patrollingPath).ActivateBot();
            return true;
        }
    }

}