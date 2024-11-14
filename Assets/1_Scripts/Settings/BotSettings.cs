using UnityEngine;
namespace RedGaint
{
    [CreateAssetMenu(fileName = "BotSettings", menuName = "GameSettings/BotSettings", order = 1)]
    public class BotSettings : ScriptableObject
    {
        [Header("Movement Settings")]
        public float attackRange = 2.0f;
        public float movementSpeed = 3.5f;

        [Header("Animation Smoothing")]
        [Range(0, 1f)] public float horizontalAnimSmoothTime = 0.2f;
        [Range(0, 1f)] public float verticalAnimSmoothTime = 0.2f;
        [Range(0, 1f)] public float startAnimTime = 0.3f;
        [Range(0, 1f)] public float stopAnimTime = 0.15f;
    }
}

