using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System;
using RedGaint;

public class SceneLoader : Singleton<SceneLoader>, IBugsBunny
{

    // Event for UI to listen to loading progress (0f to 1f)
    public event Action<float> OnLoadingProgress;

    // Event when loading is complete
    public event Action OnLoadingComplete;


    public void LoadSceneAsync(string sceneName)
    {
        StartCoroutine(LoadSceneCoroutine(sceneName));
    }

    private IEnumerator LoadSceneCoroutine(string sceneName)
    {
        Debug.Log("Loading scene: " + sceneName);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;

        while (!asyncLoad.isDone)
        {
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);

            // Notify listeners (e.g., UI progress bar)
            OnLoadingProgress?.Invoke(progress);

            if (asyncLoad.progress >= 0.9f)
            {
                // Optional: wait here until the UI is done with any transition
                OnLoadingProgress?.Invoke(1f); // Full progress
                yield return new WaitForSeconds(0.5f); // Fake delay for smooth transition

                asyncLoad.allowSceneActivation = true;
            }

            yield return null;
        }

        OnLoadingComplete?.Invoke();
    }

    public bool LogThisClass { get; } = false;
}