using System.Collections.Generic;
using UnityEngine;
namespace RedGaint
{
// Level class to hold level-specific information
    public class Level : MonoBehaviour {
        public int LevelId { get; private set; }
        // [SerializeField]private List<GameObject> paintableObjects;
        //As the level is loaded dynamically for debug purpose we are keeping the level in the hierarchy 
        // [Header("For Debug Purpose : keep the level ")]
        // [Tooltip("Default is false")]
        // public bool setDebugLevel = false;
        // private void Awake() {
        //     // if(!setDebugLevel)
        //     //     BugsBunny.Log("There is a debug level attached on the scene please remove");
        //     // gameObject.SetActive(setDebugLevel);
        //     
        //   //  paintableObjects = new List<GameObject>();
        // }

        // Method to find and cache paintable objects in the level
        // public void InitializeLevelPaintables() {
        //     if (paintableObjects == null) {
        //         Debug.LogError("Paintable objects list is not initialized!");
        //         return;
        //     }
        //
        //     paintableObjects.Clear();
        //     Paintable[] paintables = Object.FindObjectsOfType<Paintable>();
        //     foreach (var paintable in paintables) {
        //         paintableObjects.Add(paintable.gameObject);
        //     }
        // }

        // Get the list of paintable objects
        // public List<GameObject> GetPaintableObjects() {
        //     return paintableObjects;
        // }
    }
}