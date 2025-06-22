using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace RedGaint.Games.DyeHard
{
    public class Bootstrap : MonoBehaviour
    {
        [Header("UI Elements")]
        public Button startGameButton;
        public string sceneToLoad = "DyeHard_New"; 

        private void Start()
        {
            if (startGameButton != null)
            {
                startGameButton.onClick.AddListener(OnStartGameClicked);
            }
            else
            {
                Debug.LogError("Start Game Button not assigned!");
            }
        }

        private void OnStartGameClicked()
        {
            // Optionally add a transition/fade here
            SceneManager.LoadScene(sceneToLoad);
        }
    }

}