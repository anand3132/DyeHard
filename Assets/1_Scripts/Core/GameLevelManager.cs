using System.Collections.Generic;
using UnityEngine;
namespace RedGaint
{
public class GameLevelManager : MonoBehaviour {
    public static GameLevelManager Instance { get; private set; }

    private Dictionary<int, Level> levels;
    private Level currentLevel;
    
    private void Awake() {
        if (Instance == null) {
            Instance = this;
            levels = new Dictionary<int, Level>();
        } else {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        LoadLevel("Level/Level1");
    }

    // Load a level by LevelID (prefab name)
    public void LoadLevel(string levelId) {
        if (currentLevel != null) {
            UnloadCurrentLevel();
        }

        // Load the prefab from the Resources folder
        GameObject levelPrefab = Resources.Load<GameObject>(levelId);
        if (levelPrefab == null) {
            Debug.LogError($"Level prefab with ID {levelId} not found in Resources.");
            return;
        }

        // Instantiate the level prefab under the levelLoader GameObject
        GameObject levelInstance = Instantiate(levelPrefab, transform);
        Level levelComponent = levelInstance.GetComponent<Level>();

        if (levelComponent == null) {
            Debug.LogError($"Level prefab {levelId} does not contain a Level component.");
            Destroy(levelInstance);
            return;
        }

        if (!levels.ContainsKey(levelComponent.LevelId)) {
            levels[levelComponent.LevelId] = levelComponent;
        }

        currentLevel = levelComponent;
        Debug.Log($"Loading Level {levelId}");

        // Initialize paintable objects in the level
        // currentLevel.InitializeLevelPaintables();
    }

    // Unload the current level
    public void UnloadCurrentLevel() {
        if (currentLevel == null) {
            Debug.LogWarning("No level is currently loaded to unload.");
            return;
        }

        Debug.Log($"Unloading Level {currentLevel.LevelId}");

        // Destroy the current level instance
        Destroy(currentLevel.gameObject);
        currentLevel = null;
    }

    // Get the list of paintable objects in the current level
    public List<GameObject> GetCurrentLevelPaintables() {
        if (currentLevel == null) {
            Debug.LogWarning("No level is currently loaded.");
            return new List<GameObject>();
        }

        return currentLevel.GetPaintableObjects();
    }
    // Get all RenderTextures from paintable objects in the current level
    // public List<RenderTexture> GetAllRenderTextures() {
    //     if (currentLevel == null) {
    //         Debug.LogWarning("No level is currently loaded.");
    //         return new List<RenderTexture>();
    //     }
    //
    //     List<RenderTexture> renderTextures = new List<RenderTexture>();
    //     foreach (var paintableObject in currentLevel.GetPaintableObjects()) {
    //         Paintable paintable = paintableObject.GetComponent<Paintable>();
    //         if (paintable != null) {
    //             renderTextures.Add(paintable.getMask());
    //             renderTextures.Add(paintable.getUVIslands());
    //             renderTextures.Add(paintable.getExtend());
    //             renderTextures.Add(paintable.getSupport());
    //         }
    //     }
    //     return renderTextures;
    // }
}

}