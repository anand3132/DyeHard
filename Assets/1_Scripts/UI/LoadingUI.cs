using UnityEngine;
using UnityEngine.UI;

public class LoadingUI : MonoBehaviour
{
    public Slider progressBar; 

    private void OnEnable()
    {
        if (SceneLoader.Instance != null)
        {
            SceneLoader.Instance.OnLoadingProgress += UpdateProgress;
            SceneLoader.Instance.OnLoadingComplete += HideUI;
        }
    }

    private void OnDisable()
    {
        if (SceneLoader.Instance != null)
        {
            SceneLoader.Instance.OnLoadingProgress -= UpdateProgress;
            SceneLoader.Instance.OnLoadingComplete -= HideUI;
        }
    }

    private void UpdateProgress(float value)
    {
        progressBar.value = value;
    }

    private void HideUI()
    {
        gameObject.SetActive(false);
    }
}