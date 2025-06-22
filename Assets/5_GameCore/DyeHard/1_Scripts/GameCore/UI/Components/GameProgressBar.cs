using System;
using System.Collections;
using System.Collections.Generic;
using CW.Common;
using PaintCore;
using UnityEngine;
using UnityEngine.UI;

namespace RedGaint.Games.DyeHard
{
    public class GameProgressBar : SingletonSimple<GameProgressBar>, IBugsBunny
    {
        public GameObject fillArea;
        public GameObject fill;
        // public int FillAmount;
        private GameObject currentSpacer;
        public GameObject spacer;
        // public GameObject colorParent;

        // [SerializeField] private List<Color> playerColors; // List of player colors to track
        private CwColorCounter cwColorCounter;
        // private List<RenderTexture> cachedRenderTextures = new List<RenderTexture>();

        // Use LinkedList to track the color fill amounts
        private LinkedList<ColorObject> trackingColorLists = new LinkedList<ColorObject>();
        [SerializeField] private List<CwColorCounter> counters;
        private Action gamebarInitCallback;
        private void  InitializeGameProgressBar()
        {
          //  yield return new WaitForSeconds(.1f);
            TeamManager.Instance.RegisterTeamNotification(gamebarInitCallback);
            OnGameBarInit();
            InvokeRepeating(nameof(UpdateFillAmount), 0f, 0.5f);
        }

        
        private void Start()
        {
            gamebarInitCallback += OnGameBarInit;
            InitializeGameProgressBar();
          //  StartCoroutine(InitializeGameProgressBar());
        }

        private void OnDestroy()
        {
            // Stop the repeating invocation when the object is destroyed
            gamebarInitCallback -= OnGameBarInit;
            CancelInvoke(nameof(UpdateFillAmount));
        }
        public  void ClearGameProgressBar()
        {
            if (fillArea != null)
            {
                // Iterate through all children of FillArea
                for (int i = fillArea.transform.childCount - 1; i >= 0; i--)
                {
                    // Get the child object
                    Transform child = fillArea.transform.GetChild(i);

                    // Destroy the child GameObject
                    BugsBunny.Log("Destroying : "+child.name);
                    Destroy(child.gameObject);


                }
                BugsBunny.Log("All children under FillArea have been removed.");
            }
            else
            {
                BugsBunny.LogYellow("FillArea reference is not assigned.");
            }
            trackingColorLists.Clear();
            counters.Clear();
        }
        public CircularGameProgressBar circularBar;

        
        private void OnGameBarInit()
        {
            ClearGameProgressBar();

            Dictionary<GlobalEnums.GameTeam, TeamData> teamData = TeamManager.Instance.GetAllTeamData();

            List<CwColor> teamColors = new List<CwColor>();
            foreach (var data in teamData)
            {
                teamColors.Add(data.Value.TeamColorComponent);
            }

            circularBar.Initialize(teamColors);

            trackingColorLists.Clear();
            foreach (var team in teamColors)
            {
                trackingColorLists.AddLast(new ColorObject(team));
            }
        }

        
        
        private void UpdateFillAmount()
        {
            Dictionary<CwColor, float> fillMap = new Dictionary<CwColor, float>();
            List<CwColorCounter> finalCounters = counters.Count > 0 ? counters : null;

            long total = CwColorCounter.GetTotal(finalCounters);

            foreach (var colorObject in trackingColorLists)
            {
                long count = CwColorCounter.GetCount(colorObject.PlayerColor, finalCounters);
                float percent = CwCommon.RatioToPercentage(CwHelper.Divide(count, total), 0);
                fillMap[colorObject.PlayerColor] = percent / 100f; // 0 to 1
            }

            circularBar.UpdateFillAmounts(fillMap);
        }

        
        
        
        
        
        private void OnGameBarInit1()
        {
            ClearGameProgressBar();
            Dictionary<GlobalEnums.GameTeam,TeamData> currentTeamData = TeamManager.Instance.GetAllTeamData();
            foreach (KeyValuePair<GlobalEnums.GameTeam, TeamData> teamData in currentTeamData)
            {
                CwColor item = teamData.Value.TeamColorComponent;
                trackingColorLists.AddLast(new ColorObject(item));
            }
            // Instantiate Fill objects for each player color
            foreach (ColorObject colorObject in trackingColorLists)
            {
                GameObject currentfill = Instantiate(fill, fillArea.transform);
                currentfill.gameObject.name = colorObject.PlayerColor.ToString();

                Image fillImage = currentfill.GetComponent<Image>();
                if (fillImage != null)
                {
                    fillImage.color = colorObject.PlayerColor.Color;
                }

                colorObject.setFillObject(currentfill.GetComponent<LayoutElement>());
            }
            currentSpacer = Instantiate(spacer, fillArea.transform);
            currentSpacer.SetActive(true);

        }

        private void UpdateFillAmount1()
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
        
        
        public string GetCurrentHighestColorName()
        {
            ColorObject winningColor = null;
            int maxFill = -1;

            foreach (var colorObj in trackingColorLists)
            {
                if (colorObj.FillAmount > maxFill)
                {
                    maxFill = colorObj.FillAmount;
                    winningColor = colorObj;
                }
            }

            return winningColor?.PlayerColor?.name ?? "Unknown";
        }

        

        private string ColorToString(Color color)
        {
            if (color == Color.red) return "Red";
            if (color == Color.blue) return "Blue";
            if (color == Color.green) return "Green";
            if (color == Color.yellow) return "Yellow";
            if (color == Color.cyan) return "Cyan";
            if (color == Color.magenta) return "Magenta";
            if (color == Color.black) return "Black";
            if (color == Color.white) return "White";

            // fallback to RGB
            return $"R:{(int)(color.r * 255)} G:{(int)(color.g * 255)} B:{(int)(color.b * 255)}";
        }


        public bool LogThisClass { get; } = false;
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
