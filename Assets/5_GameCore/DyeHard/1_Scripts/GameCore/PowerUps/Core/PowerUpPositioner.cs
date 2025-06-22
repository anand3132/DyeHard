// using System;
// using System.Collections.Generic;
// using UnityEngine;
//
// namespace RedGaint
// {
//     public class PowerUpPositioner
//     {
//         private GlobalEnums.Mode spawnMode;
//         private List<Transform> spawnPositions = new List<Transform>();
//         private List<int> availableIndices = new List<int>();
//         private int currentIndex = -1;
//
//         public PowerUpPositioner(GlobalEnums.Mode mode)
//         {
//             spawnMode = mode;
//         }
//
//         public void RegisterSpawnPoints(List<Transform> points)
//         {
//             spawnPositions = points ?? new List<Transform>();
//             ResetAvailablePositions();
//         }
//
//         public void ResetAvailablePositions()
//         {
//             availableIndices.Clear();
//             for (int i = 0; i < spawnPositions.Count; i++)
//                 availableIndices.Add(i);
//
//             if (spawnMode == GlobalEnums.Mode.Shuffle)
//                 ShuffleAvailableIndices();
//
//             currentIndex = -1;
//         }
//         
//         public Transform GetSpawnPointByIndex(int index)
//         {
//             return (index >= 0 && index < spawnPositions.Count) ? spawnPositions[index] : null;
//         }
//
//         /// <summary>
//         /// Returns the next spawn index and marks it as used.
//         /// If no positions are available, returns -1.
//         /// </summary>
//         public int GetNextIndex()
//         {
//             if (availableIndices.Count == 0)
//                 return -1;
//
//             int chosenIndex;
//
//             switch (spawnMode)
//             {
//                 case GlobalEnums.Mode.Random:
//                     chosenIndex = UnityEngine.Random.Range(0, availableIndices.Count);
//                     break;
//
//                 case GlobalEnums.Mode.Sequence:
//                 case GlobalEnums.Mode.RoundRobin:
//                     currentIndex = (currentIndex + 1) % availableIndices.Count;
//                     chosenIndex = currentIndex;
//                     break;
//
//                 case GlobalEnums.Mode.ReverseSequence:
//                     currentIndex = (currentIndex - 1 + availableIndices.Count) % availableIndices.Count;
//                     chosenIndex = currentIndex;
//                     break;
//
//                 case GlobalEnums.Mode.Shuffle:
//                     chosenIndex = 0;
//                     break;
//
//                 default:
//                     Debug.LogWarning($"Unsupported spawn mode: {spawnMode}. Defaulting to Random.");
//                     chosenIndex = UnityEngine.Random.Range(0, availableIndices.Count);
//                     break;
//             }
//
//             int indexToReturn = availableIndices[chosenIndex];
//             availableIndices.RemoveAt(chosenIndex);
//             return indexToReturn;
//         }
//
//         /// <summary>
//         /// Returns the next spawn Transform and marks it as used.
//         /// If no positions are available, returns null.
//         /// </summary>
//         public Transform GetNextSpawnPoint()
//         {
//             int index = GetNextIndex();
//             if (index >= 0 && index < spawnPositions.Count)
//                 return spawnPositions[index];
//             else
//                 return null;
//         }
//
//         public void MarkAvailable(int index)
//         {
//             if (!availableIndices.Contains(index) && index >= 0 && index < spawnPositions.Count)
//                 availableIndices.Add(index);
//         }
//
//         private void ShuffleAvailableIndices()
//         {
//             for (int i = 0; i < availableIndices.Count; i++)
//             {
//                 int rand = UnityEngine.Random.Range(i, availableIndices.Count);
//                 (availableIndices[i], availableIndices[rand]) = (availableIndices[rand], availableIndices[i]);
//             }
//         }
//
//         public bool HasAvailablePositions()
//         {
//             return availableIndices.Count > 0;
//         }
//
//         // New property to get count of available positions
//         public int AvailablePositionsCount
//         {
//             get { return availableIndices.Count; }
//         }
//
//         // New method to check if a specific index is available
//         public bool IsIndexAvailable(int index)
//         {
//             return availableIndices.Contains(index);
//         }
//
//         // New method to get all available indices (read-only)
//         public IReadOnlyList<int> GetAllAvailableIndices()
//         {
//             return availableIndices.AsReadOnly();
//         }
//     }
// }