using System.Collections.Generic;
using CW.Common;
using PaintCore;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace RedGaint
{
    public class GameProgressBar : MonoBehaviour
    {
        public GameObject FillArea;
        public GameObject Fill;
        public int FillAmount;

        public GameObject colorParent;

        // [SerializeField] private List<Color> playerColors; // List of player colors to track
        private CwColorCounter cwColorCounter;
        // private List<RenderTexture> cachedRenderTextures = new List<RenderTexture>();

        // Use LinkedList to track the color fill amounts
        private LinkedList<ColorObject> trackingColorLists = new LinkedList<ColorObject>();
        [SerializeField] private List<CwColorCounter> counters;

        void Start()
        {
            InitialiseGameBar();
            InvokeRepeating(nameof(UpdateFillAmount), 0f, 0.5f);
        }

        private void OnDestroy()
        {
            // Stop the repeating invocation when the object is destroyed
            CancelInvoke(nameof(UpdateFillAmount));
        }

        private void InitialiseGameBar()
        {
            foreach (CwColor colorcomponent in colorParent.GetComponentsInChildren<CwColor>())
            {
                trackingColorLists.AddLast(new ColorObject(colorcomponent));
            }

            // Instantiate Fill objects for each player color
            foreach (ColorObject colorObject in trackingColorLists)
            {

                GameObject fill = Instantiate(Fill, FillArea.transform);
                fill.gameObject.name = colorObject.PlayerColor.ToString();

                Image fillImage = fill.GetComponent<Image>();
                if (fillImage != null)
                {
                    fillImage.color = colorObject.PlayerColor.Color;
                }

                colorObject.setFillObject(fill.GetComponent<LayoutElement>());
            }
        }

        private void UpdateFillAmount()
        {
            // Reset all fill amounts before recalculating
            foreach (var colorFillAmount in trackingColorLists)
            {
                colorFillAmount.FillAmount = 0;
            }

            // Calculate fill amounts for each color (without modifying the collection during enumeration)
            // foreach (RenderTexture item in cachedRenderTextures) {
            // Call CalculateLeadingColor with the LinkedList and get the area for each color

            // Update the fill amounts for each color
            foreach (var colorFillAmount in trackingColorLists)
            {

                List<CwColorCounter> finalCounters = counters.Count > 0 ? counters : null;
                long total = CwColorCounter.GetTotal(finalCounters);
                long count = CwColorCounter.GetCount(colorFillAmount.PlayerColor, finalCounters);

                // if (inverse == true)
                // {
                //     count = total - count;
                // }

                float percent = CwCommon.RatioToPercentage(CwHelper.Divide(count, total), 0);

                //PaintManager.PaintAnalyzer.CalculateLeadingColor(item, colorFillAmounts);
                // Assuming CalculateLeadingColor returns area filled per color, you can update the colorFillAmount here
                colorFillAmount.FillAmount = (int)percent;
            }
            // }

            // Update the UI fill amount for each player color
            // int i = 0;
            foreach (var colorFillAmount in trackingColorLists)
            {

                // if (i < FillArea.transform.childCount) {
                //     GameObject fill = FillArea.transform.GetChild(i).gameObject;
                //     LayoutElement layoutElement = fill.GetComponent<LayoutElement>();
                if (colorFillAmount.Fill != null)
                {
                    colorFillAmount.Fill.flexibleWidth = colorFillAmount.FillAmount;
                   //BugsBunny.LogRed($"FillAmount for {colorFillAmount.PlayerColor}: {colorFillAmount.FillAmount}");
                }
                // i++;
                // }
            }
        }
    }//GameProgressBar

    public class ColorObject
    {
        
        public CwColor PlayerColor { get; set; }
        public int FillAmount { get; set; }

        public LayoutElement Fill { get; set; }

        public ColorObject(CwColor color)
        {
            PlayerColor = color;
            FillAmount = 0;
        }

        public void setFillObject(LayoutElement fill)
        {
            Fill = fill;
        }
    }//ColorObject
} //RedGaint
