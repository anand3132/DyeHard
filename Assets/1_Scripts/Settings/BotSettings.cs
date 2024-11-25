using UnityEngine;

namespace RedGaint
{
    [CreateAssetMenu(fileName = "BotSettings", menuName = "GameSettings/BotSettings", order = 1)]
    public class BotSettings : ScriptableObject
    {
        [Header("Movement Settings")]//-----------------------------------------------------
        [Tooltip("The range at which the bot can attack.")]
        [Range(3, 10)]
        public float attackRange = 3f;

        [Tooltip("The speed at which the bot moves.")]
        [Range(3, 5)]
        public float movementSpeed = 3f;

        [Tooltip("The maximum distance the bot will follow a target.")]
        [Range(3, 20)]
        public int maxFollowRange = 3;

        [Header("Animation Smoothing")]//---------------------------------------------------------
        [Tooltip("Smoothing time for horizontal animation.")]
        [Range(0, 1f)] public float horizontalAnimSmoothTime = 0.2f;

        [Tooltip("Smoothing time for vertical animation.")]
        [Range(0, 1f)] public float verticalAnimSmoothTime = 0.2f;

        [Tooltip("Time delay to start the walk animation.")]
        [Range(0, 1f)] public float startAnimTime = 0.3f;

        [Tooltip("Time delay to stop the walk animation.")]
        [Range(0, 1f)] public float stopAnimTime = 0.15f;

        [Range(.01f, 1.0f)] public float animationSpeedOffset = 0.5f; 

        [Header("Detection Settings")]//---------------------------------------------------------
        [Tooltip("How far the bot can see.")]
        [Range(20, 80)]
        public float sightRange = 20f;

        [Tooltip("Field of view angle for the bot.")]
        [Range(15, 180)]
        public float fovAngle = 45f;

        [Header("Rotation Settings")]//-----------------------------------------------------
        [Tooltip("Minimum random angle for left rotation.")]
        [SerializeField] public float minRotationAngle = -15f;

        [Tooltip("Maximum random angle for right rotation.")]
        [SerializeField] public float maxRotationAngle = 15f;

        [Tooltip("Duration to complete the rotation.")]
        [SerializeField] public float rotationDuration = 2f;
        
        
    }
}
