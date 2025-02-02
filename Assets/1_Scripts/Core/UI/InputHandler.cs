using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
namespace RedGaint
{
    public class InputHandler : Singleton<InputHandler>, IBugsBunny
    {
        public bool LogThisClass { get; } = false;
        public Sprite defaultSprite;
        public Color  powerUpBtnDefaultColor;
         public GameObject powerUpButtonObject;
        public GameObject GameProgressBar;
        private Sprite _defaultSprite = null;

        private void Awake()
        {
            _defaultSprite=powerUpButtonObject.GetComponent<Image>().sprite;
        }

        public void SetPowerUpIcon( Sprite icon=null)
        {
            if (icon != null)
            {
                powerUpButtonObject.GetComponent<Image>().color = Color.white;
                powerUpButtonObject.GetComponent<Image>().sprite = icon;
            }
            else
            {
                powerUpButtonObject.GetComponent<Image>().sprite = _defaultSprite;
                powerUpButtonObject.GetComponent<Image>().color = powerUpBtnDefaultColor;

            }
        }
    }
}//RedGaint
