using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartScreenUI : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
   [SerializeField]
   private Button singlePlayerButton;
   [SerializeField]
   private Button multiPlayerButton;
   [SerializeField]
   private GameObject playerProfileScreen;
    void Start()
    {
        playerProfileScreen.SetActive(false);
        singlePlayerButton.onClick.AddListener(onSinglePlayerClicked);
        multiPlayerButton.onClick.AddListener(onMultiPlayerClicked);
    }

    private void onSinglePlayerClicked()
    {
       SceneLoader.Instance.LoadSceneAsync("DyeHard_Singleplayer");
    }
    
    private void onMultiPlayerClicked()
    {
        gameObject.SetActive(false);
        playerProfileScreen.SetActive(true);
        //AsyncSceneLoader.Instance.LoadSceneAsync("DyeHard_Singleplayer");
    }
}
