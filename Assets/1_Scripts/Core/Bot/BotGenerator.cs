using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using Random = UnityEngine.Random;

namespace RedGaint {
    public class BotGenerator : MonoBehaviour
    {
        private CheckPointHandler checkpointHandler;
        private List<Vector3> allSpawnPositions = new List<Vector3>();
        private bool IsGeneratorActive = false;
        public int createBot = 1;
        private List<GameObject> BotList=new List<GameObject>();
        public List<GameObject> BotPrefab;
        public GlobalEnums.Mode botSpawnMode;
        public GlobalEnums.Mode botPatrollingMode;

        private void Start()
        {
            InitializeGenerator();
        }

        private void InitializeGenerator()
        {
            checkpointHandler = transform.root.GetComponentInChildren<CheckPointHandler>();
            if (checkpointHandler == null)
            {
                BugsBunny.LogRed("BotGenerator: Can't fetch Core Component -> CheckPointHandler");
                return; 
            }
            else
            {
                UpdateAllSpawnPoints(botSpawnMode);
                IsGeneratorActive = true;
            }

            if (BotPrefab.Count == 0)
            {
                BugsBunny.LogRed("BotGenerator: There is no bot prefab for Bot Generation");
                return;
            }
            for (int i = 0; i < createBot; i++)
            {
                Vector3 spawnPosition = allSpawnPositions[i % allSpawnPositions.Count];
             //-------------------------   
                //todo:need to remove later 
                temporyDataHolder = spawnPosition;
                
                var team = GetRandomTeam();
                GenerateNewBot(GetTeamPositions(team),team, out GameObject bot);
               //----------------Hack--------- 
                BotList.Add(bot);
            }
        }
        private void UpdateAllSpawnPoints(GlobalEnums.Mode mode )
        {
            allSpawnPositions.Clear();
            List<Vector3> itemList = checkpointHandler.GetSpawnPositions();
            allSpawnPositions=GetModifiedPath(mode, itemList);
        }

        public void ResetGenerator()
        {
            foreach (var item in BotList)
            {
                if (item != null && item.GetComponent<BotController>().KillBot())
                {
                    BugsBunny.Log1("ResetGenerator: on bot killing...");
                }
            }
            allSpawnPositions.Clear();
            IsGeneratorActive = false;
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
            return (GlobalEnums.GameTeam)teams.GetValue(UnityEngine.Random.Range(0, teams.Length));
        }

        //has to replace later just a place holder
        private Vector3 temporyDataHolder;
        private Vector3 GetTeamPositions(GlobalEnums.GameTeam teams)
        {
            return temporyDataHolder;
        }
        private bool GenerateNewBot(Vector3 position,GlobalEnums.GameTeam team, out GameObject bot)
        {
            bot = GameObject.Instantiate(BotPrefab[0], transform);
            bot.SetActive(true);
            if (bot == null)
                return false;
            var currentBotcontroller = bot.GetComponent<BotController>();
            
            List<Vector3> patrollingPath = new List<Vector3> { position };
            
            List<Vector3> tmpPath= GetModifiedPath(GlobalEnums.Mode.Random,checkpointHandler.GetWayPointPositions());
            
            patrollingPath.AddRange(tmpPath);
            currentBotcontroller.InitialiseBot(patrollingPath).ActivateBot(team);
            return true;
        }
    }

}