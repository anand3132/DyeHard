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
            InitializeCheckPoints();
        }

        public List<Transform> GetSpawnList()
        {
            if (!isHandlerInitialised) InitializeCheckPoints();
            return spawnPoints;
        }

        public List<Transform> GetdefendPointsList()
        {
            if (!isHandlerInitialised) InitializeCheckPoints();
            return defendPoints;
        }

        public List<Transform> GetWayPointList()
        {
            if (!isHandlerInitialised) InitializeCheckPoints();
            return wayPoints;
        }

        public List<Transform> GetDestinationPointsList()
        {
            if (!isHandlerInitialised) InitializeCheckPoints();
            return destinationPoints;
        }

        private void InitializeCheckPoints()
        {
            // Iterate over each child of the CheckPointHandler
            foreach (Transform child in transform)
            {
                // Get the CheckPoint component on each child
                CheckPoint checkPoint = child.GetComponent<CheckPoint>();

                // Ensure the CheckPoint component exists
                if (checkPoint == null)
                {
                    return;
                }
                // Add the transform to the appropriate list based on CheckPointType
                switch (checkPoint.CheckPointType)
                {
                    case GlobalEnums.CheckPointType.WayPoint:
                        wayPoints.Add(child);
                        break;
                    case GlobalEnums.CheckPointType.DefendPoint:
                        defendPoints.Add(child);
                        break;
                    case GlobalEnums.CheckPointType.Destination:
                        destinationPoints.Add(child);
                        break;
                    case GlobalEnums.CheckPointType.SpawnPoint:
                        spawnPoints.Add(child);
                        break;
                }

            }
            isHandlerInitialised = true;

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