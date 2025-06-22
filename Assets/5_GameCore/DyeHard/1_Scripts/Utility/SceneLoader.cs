using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RedGaint.Games.DyeHard
{
    public class SceneLoader : Singleton<SceneLoader>, IBugsBunny
    {
        /// <summary>
        /// Loads a scene asynchronously.
        /// </summary>
        public void LoadScene(string sceneName, bool additive = false)
        {
            StartCoroutine(LoadSceneAsync(sceneName, additive));
        }

        private IEnumerator LoadSceneAsync(string sceneName, bool additive)
        {
            var loadMode = additive ? LoadSceneMode.Additive : LoadSceneMode.Single;

            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName, loadMode);
            while (!operation.isDone)
            {
                yield return null;
            }
        }

        /// <summary>
        /// Unloads a scene asynchronously.
        /// </summary>
        public void UnloadScene(string sceneName)
        {
            StartCoroutine(UnloadSceneAsync(sceneName));
        }

        private IEnumerator UnloadSceneAsync(string sceneName)
        {
            if (!SceneManager.GetSceneByName(sceneName).isLoaded)
            {
                Debug.LogWarning($"Scene '{sceneName}' is not loaded.");
                yield break;
            }

            AsyncOperation operation = SceneManager.UnloadSceneAsync(sceneName);
            while (!operation.isDone)
            {
                yield return null;
            }
        }

        /// <summary>
        /// Reloads the current active scene.
        /// </summary>
        public void ReloadActiveScene()
        {
            string currentSceneName = SceneManager.GetActiveScene().name;
            LoadScene(currentSceneName);
        }

        public bool LogThisClass { get; } = false;
    }

}