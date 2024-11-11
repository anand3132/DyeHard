using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CheckPointType
{
    None,
    WayPoint,
    DefendPoint,
    Destination,
    SpawnPoint
}

public class CheckPointHandler : MonoBehaviour
{
    public List<Transform> wayPoints = new List<Transform>();
    public List<Transform> defendPoints = new List<Transform>();
    public List<Transform> destinationPoints = new List<Transform>();
    public List<Transform> spawnPoints = new List<Transform>();

    private void Start()
    {
        InitializeCheckPoints();
    }

    private void InitializeCheckPoints()
    {
        // Iterate over each child of the CheckPointHandler
        foreach (Transform child in transform)
        {
            // Get the CheckPoint component on each child
            CheckPoint checkPoint = child.GetComponent<CheckPoint>();

            // Ensure the CheckPoint component exists
            if (checkPoint != null)
            {
                // Add the transform to the appropriate list based on CheckPointType
                switch (checkPoint.CheckPointType)
                {
                    case CheckPointType.WayPoint:
                        wayPoints.Add(child);
                        break;
                    case CheckPointType.DefendPoint:
                        defendPoints.Add(child);
                        break;
                    case CheckPointType.Destination:
                        destinationPoints.Add(child);
                        break;
                    case CheckPointType.SpawnPoint:
                        spawnPoints.Add(child);
                        break;
                }
            }
        }
    }
}
