using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace RedGaint.Games.DyeHard.UI
{
    public class GameEndScreenUI : MonoBehaviour, IUIScreen
    {
        [Header("UI Elements")]
        public GameObject screenRoot;
        public TextMeshProUGUI gameEndMessageText;
        public Button exitButton;

        private GameEndScreenContext currentContext;

        void Awake()
        {
            if (screenRoot != null)
                screenRoot.SetActive(false);

            if (exitButton != null)
                exitButton.onClick.AddListener(OnExitButtonClicked);
        }

        public void ShowScreen(UIScreenContext context = null)
        {
            if (context is GameEndScreenContext endContext)
            {
                currentContext = endContext;

                if (gameEndMessageText != null)
                    gameEndMessageText.text = endContext.winningTeamMessage;

                if (screenRoot != null)
                    screenRoot.SetActive(true);
            }
            else
            {
                Debug.LogWarning("GameEndScreenUI: Invalid context passed.");
            }
        }

        public void HideScreen()
        {
            if (screenRoot != null)
                screenRoot.SetActive(false);

            currentContext = null;
        }

        private void OnExitButtonClicked()
        {
            currentContext?.onExitClicked?.Invoke();
            HideScreen();
        }
    }
}