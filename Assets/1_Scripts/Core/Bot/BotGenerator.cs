using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

namespace RedGaint {
    public class BotGenerator : Singleton<BotGenerator>, IBugsBunny
    {
        public bool LogThisClass { get; } = true;
        private CheckPointHandler checkpointHandler;
        private List<Vector3> allSpawnPositions = new List<Vector3>();
        private bool IsGeneratorActive = false;
        public int createBot = 1;
        public List<GameObject> BotList=new List<GameObject>();
        public List<BotController> ReSpawnList=new List<BotController>();
        public List<GameObject> BotPrefab;
        public GlobalEnums.Mode botSpawnMode;
        public GlobalEnums.Mode botPatrollingMode;
        public bool Debug_pauseBot = false;
        private void Start()
        {
            InitializeGenerator();
        }
        
        private void InitializeGenerator()
        {
            checkpointHandler = transform.root.GetComponentInChildren<CheckPointHandler>();
            if (checkpointHandler == null)
            {
                BugsBunny.LogRed("BotGenerator: Can't fetch Core Component -> CheckPointHandler",this);
                return; 
            }
            else
            {
                UpdateAllSpawnPoints(botSpawnMode);
                IsGeneratorActive = true;
            }

            if (BotPrefab.Count == 0)
            {
                BugsBunny.LogRed("BotGenerator: There is no bot prefab for Bot Generation",this);
                return;
            }
            for (int i = 0; i < createBot; i++)
            {
                Vector3 spawnPosition = allSpawnPositions[i % allSpawnPositions.Count];
             //-------------------------   
                //todo:need to remove later 
                temporyDataHolder = spawnPosition;
                
                var team = GetRandomTeam();
                // var team = GlobalEnums.GameTeam.TeamGreen;
                GenerateNewBot(GetTeamPositions(team),team, out GameObject bot);

               //----------------Hack--------- 
                BotList.Add(bot);
            }
        }

        private string GetNewBotID()
        {
            return "bot_"+System.DateTime.Now.ToString("HHmmss");
        }
        private void UpdateAllSpawnPoints(GlobalEnums.Mode mode )
        {
            allSpawnPositions.Clear();
            List<Vector3> itemList = checkpointHandler.GetSpawnPositions();
            allSpawnPositions=GetModifiedPath(mode, itemList);
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
        }

        public void AddToReSpawnList(BotController currentBot)
        {
            if(currentBot == null)
                return;
            foreach (var bot in ReSpawnList)
            {
                if(bot == currentBot)
                    return;
            }
            ReSpawnList.Add(currentBot);
        }
        
        private List<Vector3> GetModifiedPath(GlobalEnums.Mode mode, List<Vector3> modifiedList)
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
                    modifiedList.Reverse();
                    break;

                case GlobalEnums.Mode.Shuffle:
                    for (int i = 0; i < modifiedList.Count; i++)
                    {
                        int randomIndex = Random.Range(0, modifiedList.Count);
                        (modifiedList[i], modifiedList[randomIndex]) = (modifiedList[randomIndex], modifiedList[i]);
                    }
                    break;
                case GlobalEnums.Mode.RoundRobin:
                    break;
                
                case GlobalEnums.Mode.ReverseSequence:
                    modifiedList.Reverse(); 
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
        public static GlobalEnums.GameTeam GetRandomTeam()
        {
            Array teams = Enum.GetValues(typeof(GlobalEnums.GameTeam));
            return (GlobalEnums.GameTeam)teams.GetValue(UnityEngine.Random.Range(1, teams.Length));
        }

        //has to replace later just a place holder
        private Vector3 temporyDataHolder;
        private Vector3 GetTeamPositions(GlobalEnums.GameTeam teams)
        {
            return temporyDataHolder;
        }
        private bool GenerateNewBot(Vector3 position,GlobalEnums.GameTeam team, out GameObject bot)
        {
            bot = GameObject.Instantiate(BotPrefab[Random.Range(0,BotPrefab.Count)], transform);
            if (bot == null)
                return false;
            
            bot.SetActive(true);
            var currentBotcontroller = bot.GetComponent<BotController>();
            // Rigidbody rb = bot.GetComponent<Rigidbody>();
            List<Vector3> patrollingPath = new List<Vector3> { position };
            List<Vector3> tmpPath= GetModifiedPath(GlobalEnums.Mode.Random,checkpointHandler.GetWayPointPositions());
            
            patrollingPath.AddRange(tmpPath);
            currentBotcontroller.InitialiseBot(patrollingPath,GetNewBotID()).ActivateBot(team,Debug_pauseBot);
            return true;
        }

    }

}