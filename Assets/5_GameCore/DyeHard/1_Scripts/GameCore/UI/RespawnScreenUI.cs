using System.Collections;
using UnityEngine;
using TMPro;

namespace RedGaint.Games.DyeHard.UI
{
    public class RespawnScreenUI : MonoBehaviour, IUIScreen
    {
        public GameObject mainPanel; // this is your actual UI panel
        public TextMeshProUGUI _respawnText;
        private Coroutine _respawnRoutine;

        void Awake()
        {
            if (mainPanel != null)
                mainPanel.SetActive(false);
        }

        public void ShowScreen(UIScreenContext context = null)
        {
            if (mainPanel != null)
                mainPanel.SetActive(true);

            if (context is RespawnScreenContext respawnContext)
            {
                _respawnRoutine = StartCoroutine(WaitForRespawn(respawnContext));
            }
            else
            {
                Debug.LogWarning("RespawnScreenUI: Invalid context.");
            }
        }

        public void HideScreen()
        {
            if (_respawnRoutine != null)
                StopCoroutine(_respawnRoutine);

            if (mainPanel != null)
                mainPanel.SetActive(false);
        }

        private IEnumerator WaitForRespawn(RespawnScreenContext context)
        {
            float timeRemaining = context.respawnTime;

            while (timeRemaining > 0f)
            {
                _respawnText.text = $"{Mathf.CeilToInt(timeRemaining)}";
                yield return null;
                timeRemaining -= Time.deltaTime;
            }

            if (mainPanel != null)
                mainPanel.SetActive(false);

            context.onRespawnComplete?.Invoke();
        }
    }
}