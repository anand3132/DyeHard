using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace RedGaint.Games.DyeHard
{
    public class BotGenerator : SingletonSimple<BotGenerator>, IBugsBunny
    {
        public bool LogThisClass { get; } = true;

        [Header("Settings")]
        public int createBot = 1;
        public List<GameObject> BotPrefab;
        public GlobalEnums.Mode botSpawnMode;
        public GlobalEnums.Mode botPatrollingMode;
        public bool Debug_pauseBot = false;
        public bool respawnBots = true;

        [Header("Debug")]
        public static string RF_CHECKHANDLER = "RF_CheckHandler";

        private List<Vector3> allSpawnPositions = new List<Vector3>();
        private bool IsGeneratorActive = false;

        private readonly Queue<BotController> ReSpawnQueue = new Queue<BotController>();
        private bool botsOnRespawn = false;

        public List<GameObject> BotList = new List<GameObject>();

        private void Start()
        {
            InitializeGenerator();
        }

        private void InitializeGenerator()
        {
            if (BotPrefab.Count == 0)
            {
                BugsBunny.LogRed("BotGenerator: There is no bot prefab for Bot Generation", this);
                return;
            }

            UpdateAllSpawnPoints(botSpawnMode);
    
            // Check if we have any spawn positions
            if (allSpawnPositions.Count == 0)
            {
                BugsBunny.LogRed("BotGenerator: No spawn positions available", this);
                return;
            }

            if (allSpawnPositions.Count < createBot)
            {
                Debug.Log("BotGenerator: Not Enough Spwan Positions Available");
            }
            
            IsGeneratorActive = true;

            // Ensure we don't try to create more bots than available positions
            int botsToCreate = Mathf.Min(createBot, allSpawnPositions.Count);
    
            for (int i = 0; i < botsToCreate; i++)
            {
                Vector3 spawnPosition = allSpawnPositions[i % allSpawnPositions.Count];
                temporyDataHolder = spawnPosition;

                var team = TeamManager.Instance.GetBalancedRandomTeam();
                if (!GenerateNewBot(GetTeamPositions(team), team, out GameObject bot))
                {
                    Debug.LogError("Generation Failed!");
                    return;
                }

                BotList.Add(bot);
            }
        }

        private void UpdateAllSpawnPoints(GlobalEnums.Mode mode)
        {
            allSpawnPositions.Clear();
            var itemList = PositionHandler.Instance.GetAllPositionVectors(GlobalEnums.CheckPoints.SpawnPoint);
    
            // Add null check before processing
            if (itemList != null && itemList.Count > 0)
            {
                allSpawnPositions = GetModifiedPath(mode, itemList);
            }
            else
            {
                BugsBunny.LogYellow("BotGenerator: No spawn points found in PositionHandler");
            }
        }
        public void ResetGenerator()
        {
            BugsBunny.Log("ResetGenerator: ResetGenerator...");

            foreach (var item in BotList)
            {
                if (item != null && item.GetComponent<BotController>().KillTheActor())
                {
                    BugsBunny.Log("ResetGenerator: on bot killing...");
                }
            }

            allSpawnPositions.Clear();
            IsGeneratorActive = false;
            ReSpawnQueue.Clear();
        }

        public void ReturnToPool(BotController bot)
        {
            AddToReSpawnQueue(bot);
        }

        public void AddToReSpawnQueue(BotController currentBot)
        {
            if (currentBot == null || ReSpawnQueue.Contains(currentBot)) return;

            ReSpawnQueue.Enqueue(currentBot);

            if (respawnBots && !botsOnRespawn)
                StartCoroutine(RespawnBots(1f));
        }

        private IEnumerator RespawnBots(float seconds)
        {
            botsOnRespawn = true;

            while (ReSpawnQueue.Count > 0)
            {
                var bot = ReSpawnQueue.Dequeue();
                yield return new WaitForSeconds(seconds);

                if (bot == null || bot.GetComponent<BaseCharacterController>().IsModelActive) continue;

                var path = new List<Vector3> { bot.respawnPosition };
                path.AddRange(PositionHandler.Instance.GetAllPositionVectors(GlobalEnums.CheckPoints.WayPoint));
                bot.ActivateTheActor();
            }

            botsOnRespawn = false;
            BugsBunny.Log("RespawnBots: Bots respawned and reinitialized.");
        }

        private bool GenerateNewBot(Vector3 position, GlobalEnums.GameTeam team, out GameObject bot)
        {
            bot = Instantiate(BotPrefab[Random.Range(0, BotPrefab.Count)], transform);
            if (bot == null)
                return false;

            bot.SetActive(true);

            var currentBotController = bot.GetComponent<BotController>();
            List<Vector3> patrollingPath = new List<Vector3> { position };
            List<Vector3> tmpPath = GetModifiedPath(GlobalEnums.Mode.Random, 
                PositionHandler.Instance.GetAllPositionVectors(GlobalEnums.CheckPoints.WayPoint));

            patrollingPath.AddRange(tmpPath);
            currentBotController.Initialize(patrollingPath, GetNewBotID(), team).ActivateTheActor();
            return true;
        }

        private string GetNewBotID()
        {
            return "bot_" + DateTime.Now.ToString("HHmmss");
        }

        private Vector3 temporyDataHolder;

        private Vector3 GetTeamPositions(GlobalEnums.GameTeam team)
        {
            return temporyDataHolder; // Placeholder - implement team-specific positioning if needed
        }

        private List<Vector3> GetModifiedPath(GlobalEnums.Mode mode, List<Vector3> modifiedList)
        {
            if (modifiedList == null || modifiedList.Count == 0)
                return modifiedList;

            switch (mode)
            {
                case GlobalEnums.Mode.Random:
                    modifiedList.Sort((a, b) => Random.Range(-1, 2));
                    break;

                case GlobalEnums.Mode.Stack:
                case GlobalEnums.Mode.ReverseSequence:
                    modifiedList.Reverse();
                    break;

                case GlobalEnums.Mode.Shuffle:
                    for (int i = 0; i < modifiedList.Count; i++)
                    {
                        int randIdx = Random.Range(0, modifiedList.Count);
                        (modifiedList[i], modifiedList[randIdx]) = (modifiedList[randIdx], modifiedList[i]);
                    }
                    break;

                case GlobalEnums.Mode.Cluster:
                    int clusterSize = Mathf.Max(1, modifiedList.Count / 3);
                    modifiedList = modifiedList.GetRange(0, clusterSize);
                    break;

                case GlobalEnums.Mode.DoubleShot:
                    var doubleShot = new List<Vector3>(modifiedList);
                    doubleShot.AddRange(modifiedList);
                    modifiedList = doubleShot;
                    break;

                case GlobalEnums.Mode.Sequence:
                case GlobalEnums.Mode.SingleShot:
                case GlobalEnums.Mode.RoundRobin:
                default:
                    break;
            }

            return modifiedList;
        }

        // Helper method to get next spawn position from PositionHandler
        private Vector3 GetNextSpawnPosition()
        {
            return PositionHandler.Instance.GetNextPositionVector(
                GlobalEnums.CheckPoints.SpawnPoint, 
                botSpawnMode
            );
        }
    }
}