using UnityEngine;

namespace RedGaint.Games.DyeHard
{
    [CreateAssetMenu(fileName = "GameData", menuName = "GameSettings/GameData", order = 1)]
    public class GolbalGameData : ScriptableObject
    {
        public Color teamRedColor = Color.red;
        public float gameSectionTime = 300f;
        public float botMaxHealth = 100f;
        public float playerMaxHealth = 100f;
        public float botMaxFollowTime = 1f;

    }
}