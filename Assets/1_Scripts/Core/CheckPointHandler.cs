using System.Collections.Generic;
using UnityEngine;

namespace RedGaint
{
    public class CheckPointHandler : MonoBehaviour
    {
        private List<Transform> wayPoints = new List<Transform>();
        private List<Transform> defendPoints = new List<Transform>();
        public List<Transform> destinationPoints = new List<Transform>();
        private List<Transform> spawnPoints = new List<Transform>();

        private void Start()
        {
            InitializeCheckPoints();
        }

        public List<Transform> GetSpawnList()
        {
            return spawnPoints;
        }
        public List<Transform> GetWayPointList()
        {
            return wayPoints;
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
        }
    }
}