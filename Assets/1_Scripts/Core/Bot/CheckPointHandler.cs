using System.Collections.Generic;
using UnityEngine;

namespace RedGaint
{
    public class CheckPointHandler : MonoBehaviour
    {
        private List<Transform> wayPoints = new List<Transform>();
        private List<Transform> defendPoints = new List<Transform>();
        private List<Transform> destinationPoints = new List<Transform>();
        private List<Transform> spawnPoints = new List<Transform>();
        bool isHandlerInitialised = false;

        private void Start()
        {
            Reset();
            InitializeCheckPoints();
        }

        public List<Transform> GetSpawnList()
        {
            if (!isHandlerInitialised) InitializeCheckPoints();
            return spawnPoints;
        }
        
        public List<Vector3> GetSpawnPositions()
        {
            if (!isHandlerInitialised) InitializeCheckPoints();
            return ConvertToPositions(spawnPoints);
        }

        public List<Transform> GetdefendPointsList()
        {
            if (!isHandlerInitialised) InitializeCheckPoints();
            return defendPoints;
        }

        public List<Transform> GetWayPointTransform()
        {
            if (!isHandlerInitialised) InitializeCheckPoints();
            return wayPoints;
        }
        public List<Vector3> GetWayPointPositions()
        {
            if (!isHandlerInitialised) InitializeCheckPoints();
            return ConvertToPositions(wayPoints);
        }
        public List<Transform> GetDestinationPointsList()
        {
            if (!isHandlerInitialised) InitializeCheckPoints();
            return destinationPoints;
        }

        private void InitializeCheckPoints()
        {
            InitializeCheckPointsRecursive(transform);
        }
        public static List<Vector3> ConvertToPositions(List<Transform> transforms)
        {
            if (transforms == null || transforms.Count == 0)
            {
                BugsBunny.LogYellow("Transform list is null or empty.");
                return new List<Vector3>();
            }

            List<Vector3> positions = new List<Vector3>();

            foreach (Transform transform in transforms)
            {
                if (transform != null)
                {
                    positions.Add(transform.position);
                }
                else
                {
                    BugsBunny.LogYellow("Null transform found in the list, skipping.");
                }
            }

            return positions;
        }
        private void InitializeCheckPointsRecursive(Transform parentTransform)
        {
            // Iterate over each child of the given parent transform
            foreach (Transform child in parentTransform)
            {
                // Get the CheckPoint component on each child
                CheckPoint checkPoint = child.GetComponent<CheckPoint>();

                // Ensure the CheckPoint component exists
                if (checkPoint != null)
                {
                    // Add the transform to the appropriate list based on CheckPointType
                    switch (checkPoint.CheckPointType)
                    {
                        case GlobalEnums.CheckPointType.WayPoint:
                            checkPoint.CheckPointID = "WAY_" + wayPoints.Count;
                            wayPoints.Add(child);
                            break;
                        case GlobalEnums.CheckPointType.DefendPoint:
                            checkPoint.CheckPointID = "DEF_" + defendPoints.Count;
                            defendPoints.Add(child);
                            break;
                        case GlobalEnums.CheckPointType.Destination:
                            checkPoint.CheckPointID = "DES_" + destinationPoints.Count;
                            destinationPoints.Add(child);
                            break;
                        case GlobalEnums.CheckPointType.SpawnPoint:
                            checkPoint.CheckPointID = "SPW_" + spawnPoints.Count;
                            spawnPoints.Add(child);
                            break;
                    }
                }

                // Recursively call this method to go through all nested children
                InitializeCheckPointsRecursive(child);
            }
        }

        private void Reset()
        {
            wayPoints.Clear();
            defendPoints.Clear();
            destinationPoints.Clear();
            spawnPoints.Clear();
        }
    }
}