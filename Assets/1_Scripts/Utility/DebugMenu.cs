using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace RedGaint
{
    public class DebugMenu : Singleton<DebugMenu>, IBugsBunny
    {
        public bool LogThisClass { get; } = false;
        private Canvas debugCanvas;
        private GameObject panel;
        private GameObject toggleButton;
        private List<Button> buttons = new List<Button>();
        private List<Text> texts = new List<Text>();
        private bool isMenuVisible = false;

        private void Awake()
        {
            // Ensure the initialization happens early
            CreateDebugCanvas();
            CreateToggleButton();
            CreateDebugMenu();
        }

        private void CreateDebugCanvas()
        {
            debugCanvas = new GameObject("DebugCanvas").AddComponent<Canvas>();
            debugCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            debugCanvas.gameObject.AddComponent<CanvasScaler>().uiScaleMode =
                CanvasScaler.ScaleMode.ScaleWithScreenSize;
            debugCanvas.gameObject.AddComponent<GraphicRaycaster>();
        }

        private void CreateDebugMenu()
        {
            panel = new GameObject("Panel");
            panel.transform.SetParent(debugCanvas.transform);
            RectTransform panelRect = panel.AddComponent<RectTransform>();
            panelRect.sizeDelta = new Vector2(400, 600);
            panelRect.anchoredPosition = new Vector2(-200, -100);
            panel.AddComponent<Image>().color = new Color(0, 0, 0, 0.5f);
            panel.SetActive(false);
        }

        private void CreateToggleButton()
        {
            toggleButton = new GameObject("ToggleButton");
            toggleButton.transform.SetParent(debugCanvas.transform);

            RectTransform buttonRect = toggleButton.AddComponent<RectTransform>();
            buttonRect.sizeDelta = new Vector2(100, 50);
            buttonRect.anchorMin = new Vector2(1, 1);
            buttonRect.anchorMax = new Vector2(1, 1);
            buttonRect.pivot = new Vector2(1, 1);
            buttonRect.anchoredPosition = new Vector2(-10, -10);

            Button button = toggleButton.AddComponent<Button>();
            toggleButton.AddComponent<Image>().color = new Color(0.2f, 0.8f, 0.2f, 1f);

            GameObject textObject = new GameObject("Text");
            textObject.transform.SetParent(toggleButton.transform);
            Text buttonText = textObject.AddComponent<Text>();
            buttonText.text = "Debug";
            buttonText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"); // Updated font
            buttonText.alignment = TextAnchor.MiddleCenter;
            buttonText.color = Color.white;

            RectTransform textRect = textObject.GetComponent<RectTransform>();
            textRect.sizeDelta = new Vector2(100, 50);
            textRect.anchoredPosition = Vector2.zero;

            button.onClick.AddListener(ToggleMenuVisibility);
        }

        public void AddButton(string buttonText, UnityEngine.Events.UnityAction onClickAction)
        {
            GameObject buttonObject = new GameObject($"Button_{buttonText}");
            buttonObject.transform.SetParent(panel.transform);

            RectTransform buttonRect = buttonObject.AddComponent<RectTransform>();
            buttonRect.sizeDelta = new Vector2(200, 50);
            buttonRect.anchoredPosition = new Vector2(0, -50 * buttons.Count);

            Button button = buttonObject.AddComponent<Button>();
            buttonObject.AddComponent<Image>().color = new Color(0.2f, 0.2f, 0.8f, 1f);

            GameObject textObject = new GameObject("Text");
            textObject.transform.SetParent(buttonObject.transform);
            Text buttonTextComponent = textObject.AddComponent<Text>();
            buttonTextComponent.text = buttonText;
            buttonTextComponent.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            buttonTextComponent.alignment = TextAnchor.MiddleCenter;
            buttonTextComponent.color = Color.white;

            RectTransform textRect = textObject.GetComponent<RectTransform>();
            textRect.sizeDelta = new Vector2(200, 50);
            textRect.anchoredPosition = Vector2.zero;

            button.onClick.AddListener(onClickAction);
            buttons.Add(button);
        }

        private void ToggleMenuVisibility()
        {
            isMenuVisible = !isMenuVisible;
            panel.SetActive(isMenuVisible);
        }

    }
}
