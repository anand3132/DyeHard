using System.Collections.Generic;
using RedGaint.Games.DyeHard;
using UnityEngine;

namespace RedGaint.Games.DyeHard
{

    [System.Serializable]
    public class CheckPointType
    {
        public GlobalEnums.CheckPoints type;
        [HideInInspector] public string idSuffix; // For internal use

        // Type-specific properties (optional)
        [Range(0.1f, 2f)] public float weight = 1f;
        [Range(0f, 1f)] public float spawnChance = 1f;

        // Constructor for easy initialization
        public CheckPointType(GlobalEnums.CheckPoints type)
        {
            this.type = type;
            this.idSuffix = "";
        }
    }

    public class CheckPoint : MonoBehaviour
    {
        [SerializeField] private List<CheckPointType> checkPointTypes = new List<CheckPointType>();

        // Public property with custom editor to prevent direct modification
        public IReadOnlyList<CheckPointType> CheckPointTypes => checkPointTypes;

        // Cache for performance
        private Dictionary<GlobalEnums.CheckPoints, CheckPointType> typeLookup;
        private bool isCacheValid = false;

        private void OnValidate()
        {
            // Generate IDs when modified in editor
            foreach (var cpType in checkPointTypes)
            {
                cpType.idSuffix = $"{cpType.type.ToString().Substring(0, 3)}_{GetInstanceID()}";
            }

            isCacheValid = false;
        }

        private void BuildTypeCache()
        {
            if (isCacheValid) return;

            typeLookup = new Dictionary<GlobalEnums.CheckPoints, CheckPointType>();
            foreach (var cpType in checkPointTypes)
            {
                typeLookup[cpType.type] = cpType;
            }

            isCacheValid = true;
        }

        public bool HasType(GlobalEnums.CheckPoints type)
        {
            BuildTypeCache();
            return typeLookup.ContainsKey(type);
        }

        public CheckPointType GetTypeData(GlobalEnums.CheckPoints type)
        {
            BuildTypeCache();
            return typeLookup.TryGetValue(type, out var result) ? result : null;
        }

        public void AddType(GlobalEnums.CheckPoints type)
        {
            if (!HasType(type))
            {
                checkPointTypes.Add(new CheckPointType(type));
                isCacheValid = false;
            }
        }
        public void AddType(CheckPointType type)
        {
            if (!HasType(type.type))
            {
                checkPointTypes.Add(type);
                isCacheValid = false;
            }
        }

        public void RemoveType(GlobalEnums.CheckPoints type)
        {
            int removed = checkPointTypes.RemoveAll(t => t.type == type);
            if (removed > 0) isCacheValid = false;
        }

        // Optional: Get all positions for this checkpoint
        public Vector3 GetPosition()
        {
            return transform.position;
        }
    }
}