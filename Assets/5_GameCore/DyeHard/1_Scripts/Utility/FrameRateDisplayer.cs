using UnityEngine;

namespace RedGaint.Games.DyeHard
{
    public class FrameRateDisplayer : MonoBehaviour
    {
        private float deltaTime = 0.0f;
        [Header("Display Settings")]
        public Color textColor = Color.white;
        public int fontSize = 24;
        public Vector2 screenPosition = new Vector2(10, 10);

        private GUIStyle guiStyle;

        private void Start()
        {
            guiStyle = new GUIStyle();
            guiStyle.fontSize = fontSize;
            guiStyle.normal.textColor = textColor;
        }

        void Update()
        {
            // Smooth deltaTime for better readability
            deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        }

        void OnGUI()
        {
            float fps = 1.0f / deltaTime;
            string text = string.Format("{0:0.} FPS", fps);
            GUI.Label(new Rect(screenPosition.x, screenPosition.y, 200, 40), text, guiStyle);
        }
    }

}