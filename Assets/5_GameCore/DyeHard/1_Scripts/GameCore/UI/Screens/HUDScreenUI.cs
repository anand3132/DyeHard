using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace RedGaint.Games.DyeHard.UI
{
    public class HUDScreenUI : MonoBehaviour, IUIScreen
    {
        [Header("Main Panel")]
        public GameObject mainPanel;


        [Header("Power-Up Button")]
        public GameObject powerUpButtonObject;
        public Color powerUpBtnDefaultColor = Color.gray;

        private Sprite _defaultPowerUpSprite;
        private Sprite _currentPowerUpSprite;

        void Awake()
        {
            if (mainPanel != null)
                mainPanel.SetActive(false);

            if (powerUpButtonObject != null && powerUpButtonObject.TryGetComponent(out Image img))
            {
                _defaultPowerUpSprite = img.sprite;
            }
        }

        public void ShowScreen(UIScreenContext context = null)
        {
            if (mainPanel != null)
                mainPanel.SetActive(true);

            if (context is HUDScreenContext hudContext)
            {
                if (hudContext.powerUpIcon != null)
                    SetPowerUpIcon(hudContext.powerUpIcon);
                else
                    SetPowerUpIcon(); // fallback
            }
        }

        public void HideScreen()
        {
            if (mainPanel != null)
                mainPanel.SetActive(false);

            SetPowerUpIcon(); // reset on hide
        }


        public void SetPowerUpIcon(Sprite icon = null)
        {
            if (powerUpButtonObject != null && powerUpButtonObject.TryGetComponent(out Image img))
            {
                if (icon != null)
                {
                    img.color = Color.white;
                    img.sprite = icon;
                    _currentPowerUpSprite = icon;
                }
                else
                {
                    img.sprite = _defaultPowerUpSprite;
                    img.color = powerUpBtnDefaultColor;
                    _currentPowerUpSprite = _defaultPowerUpSprite;
                }
            }
        }
    }
}
