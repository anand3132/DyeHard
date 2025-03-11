using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
namespace RedGaint
{
    public class InputHandler : Singleton<InputHandler>, IBugsBunny
    {
        public bool LogThisClass { get; } = false;
        public Sprite defaultSprite=null;
        public Color  powerUpBtnDefaultColor;
         public GameObject powerUpButtonObject;
        public GameObject GameProgressBar;
        private Sprite _defaultSprite = null;
        private Sprite currentIcon=null;

        private void OnEnable()
        {
            if(_defaultSprite)
                SetPowerUpIcon(currentIcon);
            else
            {
                _defaultSprite=powerUpButtonObject.GetComponent<Image>().sprite;
            }
            
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
            currentIcon =  powerUpButtonObject.GetComponent<Image>().sprite;
        }

        private void OnDisable()
        {
            SetPowerUpIcon();
        }
    }
}//RedGaint
