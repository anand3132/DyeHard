using System;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

namespace RedGaint
{
    public class PlayerProfileUI : MonoBehaviour
    {
        [SerializeField] private Button previousButton;
        [SerializeField] private Button nextButton;
        [SerializeField] private Button backButton;
        [SerializeField] private Button StartTheGameButton;
        [SerializeField] private TextMeshProUGUI PlayerNameText;
        [SerializeField] private GameObject StartScreen;
        [SerializeField] private ModelTurntable ModelTurntable;
        void Start()
        {
            
            previousButton.onClick.AddListener(PreviousClicked);
            nextButton.onClick.AddListener(NextClicked);
            backButton.onClick.AddListener(BackClicked);
            StartTheGameButton.onClick.AddListener(StartGamerClicked);
            PlayerNameText.text = "";
        }

        private void OnEnable()
        {
            ModelTurntable.gameObject.SetActive(true);
        }

        private void OnDisable()
        {
            ModelTurntable.gameObject.SetActive(false);
        }

        private void StartGamerClicked()
        {
            SceneLoader.Instance.LoadSceneAsync("DyeHard_Multiplayer");
        }

        private void PreviousClicked()
        {
            ModelTurntable.ShowPreviousModel();
        }

        private void NextClicked()
        {
            ModelTurntable.ShowNextModel();
        }

        private void BackClicked()
        {
            gameObject.SetActive(false);
            StartScreen.SetActive(true);
        }

    }
}