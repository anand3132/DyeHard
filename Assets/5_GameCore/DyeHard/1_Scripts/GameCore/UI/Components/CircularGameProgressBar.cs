using System.Collections.Generic;
using PaintCore;
using UnityEngine;
using UnityEngine.UI;

namespace RedGaint.Games.DyeHard
{
    public class CircularGameProgressBar : MonoBehaviour
    {
        [Header("References")]
        public Transform segmentContainer; // The parent GameObject where segments are instantiated
        public GameObject radialSegmentPrefab; // Prefab with Image Type = Filled, FillMethod = Radial360

        private class SegmentData
        {
            public CwColor Color;
            public Image SegmentImage;
            public float FillPercent; // From 0 to 1
        }

        private List<SegmentData> segments = new List<SegmentData>();

        public void Initialize(List<CwColor> teamColors)
        {
            ClearSegments();

            int totalTeams = teamColors.Count;
            if (totalTeams == 0) return;

            float equalFill = 1f / totalTeams; // Equal fill (e.g., 0.25 for 4 teams)
            Debug.Log("totalTeams:" + totalTeams+", equalFill:" + equalFill);
            float cumulativeAngle = 0f;

            foreach (var teamColor in teamColors)
            {
                GameObject segmentGO = Instantiate(radialSegmentPrefab, segmentContainer);
                Image segmentImage = segmentGO.GetComponent<Image>();

                if (segmentImage != null)
                {
                    segmentImage.color = teamColor.Color;
                    segmentImage.fillAmount = equalFill;
                    segmentImage.fillClockwise = true;
                    segmentImage.fillOrigin = 0;
                    segmentImage.type = Image.Type.Filled;
                    segmentImage.fillMethod = Image.FillMethod.Radial360;

                    segmentImage.rectTransform.localRotation = Quaternion.Euler(0, 0, -cumulativeAngle);
                }

                segments.Add(new SegmentData
                {
                    Color = teamColor,
                    SegmentImage = segmentImage,
                    FillPercent = equalFill
                });

                cumulativeAngle += 360f * equalFill;
            }
        }

        public void UpdateFillAmounts(Dictionary<CwColor, float> teamFillPercentages)
        {
            // Debug.Log("teamFillPercentages:" + teamFillPercentages);

            const float minVisualPercent = 0.1f; // 10% minimum per team
            int totalTeams = segments.Count;
            float reservedFill = totalTeams * minVisualPercent;
            float remainingFill = Mathf.Clamp01(1f - reservedFill); // Ensure we never go over 1

            // Step 1: Compute total raw fill across all teams
            float totalRawFill = 0f;
            foreach (var segment in segments)
            {
                if (teamFillPercentages.TryGetValue(segment.Color, out float rawFill))
                {
                    totalRawFill += rawFill;
                }
            }

            // Prevent division by zero
            if (totalRawFill <= 0.0001f)
            {
                totalRawFill = 1f; // To evenly distribute remaining fill
            }

            float cumulativeAngle = 0f;

            foreach (var segment in segments)
            {
                float rawFill = teamFillPercentages.TryGetValue(segment.Color, out float value) ? value : 0f;
                float normalized = rawFill / totalRawFill;

                // Every team gets minimum + proportional share of remaining
                float visualFill = minVisualPercent + normalized * remainingFill;
                segment.FillPercent = visualFill;

                if (segment.SegmentImage != null)
                {
                    segment.SegmentImage.fillAmount = visualFill;
                    segment.SegmentImage.rectTransform.localRotation = Quaternion.Euler(0, 0, -cumulativeAngle);
                }

                cumulativeAngle += 360f * visualFill;
            }
        }



        public void ClearSegments()
        {
            foreach (Transform child in segmentContainer)
            {
                Destroy(child.gameObject);
            }
            segments.Clear();
        }
    }
}
