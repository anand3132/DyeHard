using System.Collections.Generic;
using UnityEngine;

namespace RedGaint.Games.DyeHard.UI
{
    public enum UIScreen
    {
        None,
        MainMenu,
        HUD,
        PauseMenu,
        GameOver,
        Respawn
    }

    public class UXController : SingletonSimple<UXController>, IBugsBunny
    {
        [System.Serializable]
        public class UIScreenEntry
        {
            public UIScreen screenType;
            public GameObject screenObject;
        }

        public List<UIScreenEntry> screenEntries;

        private Dictionary<UIScreen, IUIScreen> _screenMap = new();
        private UIScreen _currentScreen = UIScreen.None;

        void Awake()
        {
            foreach (var entry in screenEntries)
            {
                if (entry.screenObject.TryGetComponent(out IUIScreen uiScreen))
                {
                    _screenMap.Add(entry.screenType, uiScreen);
                }
                else
                {
                    Debug.LogWarning($"Screen {entry.screenType} does not implement IUIScreen.");
                }
            }
        }

        public void ShowScreen(UIScreen screenType, UIScreenContext context = null)
        {
            if (_currentScreen != UIScreen.None && _screenMap.TryGetValue(_currentScreen, out var current))
            {
                current.HideScreen();
            }

            if (_screenMap.TryGetValue(screenType, out var nextScreen))
            {
                nextScreen.ShowScreen(context);
                _currentScreen = screenType;
            }
            else
            {
                Debug.LogWarning($"UXController: Screen {screenType} not found.");
            }
        }

        public void HideCurrentScreen()
        {
            if (_currentScreen != UIScreen.None && _screenMap.TryGetValue(_currentScreen, out var screen))
            {
                screen.HideScreen();
                _currentScreen = UIScreen.None;
            }
        }

        public UIScreen GetCurrentScreen() => _currentScreen;

        public bool TryGetScreen<T>(UIScreen screenType, out T screen) where T : class, IUIScreen
        {
            if (_screenMap.TryGetValue(screenType, out var baseScreen) && baseScreen is T typedScreen)
            {
                screen = typedScreen;
                return true;
            }

            screen = null;
            return false;
        }

        public bool LogThisClass { get; } = false;
    }
}
