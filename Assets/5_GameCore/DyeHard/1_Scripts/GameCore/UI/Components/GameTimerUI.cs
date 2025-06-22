using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RedGaint.Games.DyeHard
{
    public class GameTimerUI : MonoBehaviour
    {
        [Header("Timer Settings")]
        public float gameDuration = 180f; // 3 minutes
        public bool isPaused = false;     // Debug toggle for pausing the timer

        [Header("UI")]
        public TextMeshProUGUI timerText; // Assign in inspector

        private float remainingTime;

        private void Start()
        {
            if (timerText == null)
            {
                Debug.LogError("Timer Text not assigned!");
                return;
            }

            timerText.color = Color.white;
            remainingTime = gameDuration;
            StartCoroutine(RunGameTimer());
        }

        private IEnumerator RunGameTimer()
        {
            while (remainingTime > 5f)
            {
                if (!isPaused)
                {
                    UpdateTimerUI(remainingTime);
                    remainingTime -= 1f;
                }

                yield return new WaitForSeconds(1f);
            }

            StartCoroutine(BlinkWhileCountingDown());

            while (remainingTime > 0f)
            {
                if (!isPaused)
                {
                    UpdateTimerUI(remainingTime);
                    remainingTime -= 1f;
                }

                yield return new WaitForSeconds(1f);
            }

            UpdateTimerUI(0);

            GamePlayManager.Instance.OnGameEndTimeUpTriggered();
        }

        private void UpdateTimerUI(float time)
        {
            int minutes = Mathf.FloorToInt(time / 60f);
            int seconds = Mathf.FloorToInt(time % 60f);
            timerText.text = $"{minutes:00}:{seconds:00}";
        }

        private IEnumerator BlinkWhileCountingDown()
        {
            timerText.color = Color.red;
            while (remainingTime > 0f)
            {
                timerText.enabled = !timerText.enabled;
                yield return new WaitForSeconds(0.3f);
            }

            timerText.enabled = true;
        }
    }
}
