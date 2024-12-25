using UnityEngine;
using UnityEngine.UIElements;

namespace RedGaint
{
    public class InputHandler : Singleton<InputHandler>, IBugsBunny
    {
        public bool LogThisClass { get; } = false;
        public Sprite defaultSprite;
        public Color  powerUpBtnDefaultColor;
        public GameObject powerUpButtonObject;
        public Slider GameProgressBar;
    }
}//RedGaint
