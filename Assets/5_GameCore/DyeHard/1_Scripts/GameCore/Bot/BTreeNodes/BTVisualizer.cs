// using UnityEngine;
// using UnityEngine.AI;
//
// namespace RedGaint
// {
//     [ExecuteInEditMode]
//     public class BTVisualizer : MonoBehaviour
//     {
//         [Header("Visualization Settings")]
//         [SerializeField] private bool showVisualization = true;
//         [SerializeField] private bool showOnlyWhenSelected = false;
//         
//         [Header("Detection Settings")]
//         [SerializeField] private Color detectionRangeColor = new Color(1, 1, 0, 0.1f);
//         [SerializeField] private Color fovConeColor = Color.yellow;
//         [SerializeField] private Color detectedPlayerColor = Color.red;
//         
//         [Header("Path Settings")]
//         [SerializeField] private Color pathColor = Color.blue;
//         [SerializeField] private float waypointSize = 0.5f;
//         
//         private BotController botController;
//         private NavMeshAgent agent;
//
//         private void Awake()
//         {
//             botController = GetComponent<BotController>();
//             agent = GetComponent<NavMeshAgent>();
//         }
//
//         private void OnDrawGizmos()
//         {
//             if (!showVisualization || showOnlyWhenSelected) 
//                 return;
//             
//             DrawVisualization();
//         }
//
//         private void OnDrawGizmosSelected()
//         {
//             if (!showVisualization || !showOnlyWhenSelected) 
//                 return;
//             
//             DrawVisualization();
//         }
//
//         private void DrawVisualization()
//         {
//             if (botController == null || agent == null)
//                 return;
//
//             // Draw detection range
//             Gizmos.color = detectionRangeColor;
//             Gizmos.DrawSphere(transform.position, botController.sightRange);
//
//             // Draw FOV cone
//             DrawFOVCone();
//
//             // Draw detected player
//             if (botController.detectedPlayer != null)
//             {
//                 Gizmos.color = detectedPlayerColor;
//                 Gizmos.DrawLine(transform.position, botController.detectedPlayer.position);
//             }
//
//             // Draw path
//             DrawPath();
//         }
//
//         private void DrawFOVCone()
//         {
//             Gizmos.color = fovConeColor;
//             Vector3 forward = transform.forward;
//             Vector3 origin = transform.position;
//             int rays = 5;
//
//             for (int i = 0; i <= rays; i++)
//             {
//                 float angle = (-botController.fovAngle/2) + (i * (botController.fovAngle/rays));
//                 Vector3 direction = Quaternion.Euler(0, angle, 0) * forward;
//                 Gizmos.DrawLine(origin, origin + direction * botController.sightRange);
//             }
//         }
//
//         private void DrawPath()
//         {
//             if (agent == null || agent.path == null)
//                 return;
//
//             Gizmos.color = pathColor;
//             var path = agent.path;
//
//             if (path.corners.Length < 2)
//                 return;
//
//             for (int i = 0; i < path.corners.Length - 1; i++)
//             {
//                 Gizmos.DrawLine(path.corners[i], path.corners[i + 1]);
//             }
//
//             // Draw waypoints if patrolling
//             if (botController.botPatrollingPath != null && botController.botPatrollingPath.Count > 0)
//             {
//                 foreach (var point in botController.botPatrollingPath)
//                 {
//                     Gizmos.DrawSphere(point, waypointSize);
//                 }
//             }
//         }
//
//         // Toggle visibility at runtime
//         public void ToggleVisualization(bool visible)
//         {
//             showVisualization = visible;
//         }
//     }
// }//RedGaint