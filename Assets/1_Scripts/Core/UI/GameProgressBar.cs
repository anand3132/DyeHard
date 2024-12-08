using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace RedGaint {
    public class GameProgressBar : MonoBehaviour {
        public GameObject FillArea;
        public GameObject Fill;
        public int FillAmount;
        public List<Color> playerColors; // List of player colors to track
        private List<RenderTexture> cachedRenderTextures = new List<RenderTexture>();

        // Use LinkedList to track the color fill amounts
        private LinkedList<ColorFillAmount> colorFillAmounts = new LinkedList<ColorFillAmount>();

        IEnumerator WaitForLevelReadyAndCache() {
            while (GameLevelManager.Instance.GetCurrentLevelPaintables().Count == 0) {
                yield return new WaitForSeconds(3f); // Poll every 3 seconds
            }
            Debug.Log("Got textures---------------");
            CacheRenderTextures();
            InvokeRepeating(nameof(UpdateFillAmount), 0.1f, 0.5f); // Start updates after caching
        }

        void Start() {
            InitialiseGameBar();
            StartCoroutine(WaitForLevelReadyAndCache());
        }

        private void InitialiseGameBar() {
            // Instantiate Fill objects for each player color
            foreach (Color color in playerColors) {
                GameObject fill = Instantiate(Fill, FillArea.transform);
                Image fillImage = fill.GetComponent<Image>(); // Get the Image component
                if (fillImage != null) {
                    fillImage.color = color; // Set the Image color to match the player color
                }
                fill.name = "Fill_" + color.ToString(); // Optional: Name each fill object for easier debugging

                // Add a new ColorFillAmount for each color
                colorFillAmounts.AddLast(new ColorFillAmount(color));
            }
        }

        private void CacheRenderTextures() {
            cachedRenderTextures = GameLevelManager.Instance.GetAllRenderTextures();
        }
        private void UpdateFillAmount() {
            // Reset all fill amounts before recalculating
            foreach (var colorFillAmount in colorFillAmounts) {
                colorFillAmount.FillAmount = 0;
            }

            // Calculate fill amounts for each color (without modifying the collection during enumeration)
            foreach (RenderTexture item in cachedRenderTextures) {
                // Call CalculateLeadingColor with the LinkedList and get the area for each color
                int areaFilled = PaintManager.PaintAnalyzer.CalculateLeadingColor(item, colorFillAmounts);

                // Update the fill amounts for each color
                foreach (var colorFillAmount in colorFillAmounts) {
                    // Assuming CalculateLeadingColor returns area filled per color, you can update the colorFillAmount here
                    colorFillAmount.FillAmount += areaFilled;
                }
            }

            // Update the UI fill amount for each player color
            int i = 0;
            foreach (var colorFillAmount in colorFillAmounts) {
                if (i < FillArea.transform.childCount) {
                    GameObject fill = FillArea.transform.GetChild(i).gameObject;
                    LayoutElement layoutElement = fill.GetComponent<LayoutElement>();
                    if (layoutElement != null) {
                        layoutElement.flexibleWidth = colorFillAmount.FillAmount;
                        Debug.Log($"FillAmount for {colorFillAmount.PlayerColor}: {colorFillAmount.FillAmount}");
                    }
                    i++;
                }
            }
        }

    }//GameProgressBar
}//Redgaint
